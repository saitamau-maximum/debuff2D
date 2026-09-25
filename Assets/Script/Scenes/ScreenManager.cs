using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    //Singleton（唯一のインスタンス）を保持するための変数
    public static ScreenManager Instance;

    [SerializeField, Min(1)] private int currentFloor = 1;
    public int CurrentFloor => currentFloor;

    // Call before loading the selected floor (the same scene may serve many floors).
    public void SelectFloor(int floor)
    {
        if (floor < 1) throw new System.ArgumentOutOfRangeException(nameof(floor));
        currentFloor = floor;
    }

    public enum SceneType//遷移するScene候補
    {
        SampleScene = 0,
        testScene = 1,
        Title = 2,
        Config = 3,
        // Value 4 was the retired Result scene. Keep other serialized values stable.
        ResultList = 5,
        FloorSelect = 6,
    }

    private void Awake()
    {
        //Singleton の初期化処理
        if (Instance == null)
        {
            Instance = this;
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene.ToString());
    }
}
