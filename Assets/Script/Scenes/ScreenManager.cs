using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    //Singleton（唯一のインスタンス）を保持するための変数
    public static ScreenManager Instance { get ; private set;}

    [SerializeField, Min(1)] private int currentFloor = 1;
    public int CurrentFloor => currentFloor;
    public ActiveDebuffStacks ActiveDebuffs { get; private set; }

    public void SelectFloor(int floor)
    {
        if (floor < 1) throw new System.ArgumentOutOfRangeException(nameof(floor));
        currentFloor = floor;
    }

    public enum SceneType//遷移するScene候補
    {
        Stage1 = 0,
        Title = 1,
        Config = 2,
        ResultList = 3,
        FloorSelect = 4,
        Stage2 = 5,
    }

    private void Awake()
    {
        //Singleton の初期化処理
        if (Instance == null)
        {
            Instance = this;
            SetFloorForStage(gameObject.scene.name);
            DontDestroyOnLoad(gameObject);//Scene遷移しても破棄しない
        }
        else
        {
            Destroy(gameObject);//既に存在する場合は重複しないように新しく作った方を削除
        }
    }

    public void ChangeScene(SceneType scene)//Scene遷移の実装
    {
        if (!System.Enum.IsDefined(typeof(SceneType), scene))
        {
            Debug.LogError("Invalid scene selection. Reassign the scene on this button.", this);
            return;
        }
        // Keep the applied stacks when the stage player is unloaded.
        if (PlayerStatus.Instance != null)
            ActiveDebuffs = PlayerStatus.Instance.ActiveDebuffs;

        string sceneName = scene.ToString();
        SetFloorForStage(sceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void SetFloorForStage(string sceneName)
    {
        switch (sceneName)
        {
            case nameof(SceneType.Stage1):
                SelectFloor(1);
                break;
            case nameof(SceneType.Stage2):
                SelectFloor(2);
                break;
        }
    }
}
