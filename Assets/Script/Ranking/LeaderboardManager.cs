using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField]
    private string leaderboardId = "high-score";

    [SerializeField]
    private string environmentName = "production";

    private Task initializationTask;
    private ILeaderboardsService leaderboardsService;

    public bool IsReady { get; private set; }

    private void Awake()
    {
        initializationTask = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        if (IsReady)
        {
            return;
        }

        try
        {
            var options = new InitializationOptions();

            if (!string.IsNullOrWhiteSpace(environmentName))
            {
                options.SetEnvironmentName(environmentName);
            }

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync(options);
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance
                    .SignInAnonymouslyAsync();
            }

            // 静的InstanceではなくCoreのレジストリから取得する
            leaderboardsService =
                UnityServices.Instance.GetLeaderboardsService();

            if (leaderboardsService == null)
            {
                throw new InvalidOperationException(
                    "Leaderboards service could not be obtained.");
            }

            IsReady = true;

            Debug.Log(
                $"UGS initialized. " +
                $"State: {UnityServices.State}, " +
                $"Player ID: {AuthenticationService.Instance.PlayerId}, " +
                $"Leaderboard ID: {leaderboardId}, " +
                $"Environment: {environmentName}");
        }
        catch (Exception exception)
        {
            IsReady = false;
            leaderboardsService = null;

            Debug.LogError("UGS initialization failed.");
            Debug.LogException(exception);
            throw;
        }
    }

    private async Task<bool> EnsureInitializedAsync()
    {
        Task currentInitializationTask =
            initializationTask ??= InitializeAsync();

        try
        {
            await currentInitializationTask;

            return IsReady && leaderboardsService != null;
        }
        catch (Exception)
        {
            if (ReferenceEquals(initializationTask, currentInitializationTask))
            {
                initializationTask = null;
            }

            return false;
        }
    }

    public async Task SubmitScoreAsync(double score)
    {
        if (!await EnsureInitializedAsync())
        {
            Debug.LogError(
                "スコアを送信できません。UGSの初期化に失敗しています。");
            return;
        }

        try
        {
            LeaderboardEntry result =
                await leaderboardsService.AddPlayerScoreAsync(
                    leaderboardId,
                    score);

            Debug.Log(
                $"Score submitted: {result.Score}, " +
                $"Rank: {result.Rank + 1}");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Score submission failed. Leaderboard ID: {leaderboardId}");
            Debug.LogException(exception);
        }
    }

    public async Task GetTopScoresAsync(int count = 10)
    {
        if (count <= 0)
        {
            Debug.LogError("取得件数は1以上を指定してください。");
            return;
        }

        if (!await EnsureInitializedAsync())
        {
            Debug.LogError(
                "ランキングを取得できません。UGSの初期化に失敗しています。");
            return;
        }

        try
        {
            LeaderboardScoresPage page =
                await leaderboardsService.GetScoresAsync(
                    leaderboardId,
                    new GetScoresOptions
                    {
                        Offset = 0,
                        Limit = count
                    });

            Debug.Log("===== Global Ranking =====");

            foreach (LeaderboardEntry entry in page.Results)
            {
                Debug.Log(
                    $"{entry.Rank + 1}位 | " +
                    $"{entry.PlayerName ?? entry.PlayerId} | " +
                    $"{entry.Score}");
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Ranking retrieval failed. Leaderboard ID: {leaderboardId}");
            Debug.LogException(exception);
        }
    }

    public async Task GetMyScoreAsync()
    {
        if (!await EnsureInitializedAsync())
        {
            Debug.LogError(
                "自分の順位を取得できません。UGSの初期化に失敗しています。");
            return;
        }

        try
        {
            LeaderboardEntry entry =
                await leaderboardsService.GetPlayerScoreAsync(
                    leaderboardId);

            Debug.Log(
                $"My score: {entry.Score}, " +
                $"Rank: {entry.Rank + 1}");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Player score retrieval failed. " +
                $"Leaderboard ID: {leaderboardId}");
            Debug.LogException(exception);
        }
    }
}
