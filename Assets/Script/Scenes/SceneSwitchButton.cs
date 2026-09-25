using UnityEngine;

public class SceneSwitchButton : MonoBehaviour
{
    //Scene名の選択(Unity上から)
    [SerializeField] private ScreenManager.SceneType _sceneType;

    //ScreenManagerのInstanceとして登録されたもののChangeSceneメソッドを呼び出し
    public void OnClickRequestSceneChange()
    {
        ScreenManager.Instance.ChangeScene(_sceneType);
    }
}
