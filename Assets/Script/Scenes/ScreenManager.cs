using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    //Singleton（唯一のインスタンス）を保持するための変数
    public static ScreenManager Instance;

    public enum SceneType//遷移するScene候補
    {
        SampleScene,
        testScene,
        Title,
        Config,
        Result,
        ResultList,
        FloorSelect,
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
        SceneManager.LoadScene(scene.ToString());
    }
}
