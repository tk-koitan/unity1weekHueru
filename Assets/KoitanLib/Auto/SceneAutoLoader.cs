using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Awake前にManagerSceneを自動でロードするクラス
/// </summary>
public class SceneAutoLoader
{

    //ゲーム開始時(シーン読み込み前)に実行される
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadManagerScene()
    {
        string managerSceneName = "ManagerScene";
        string debugSceneName = "DebugScene";

#if KOITAN_DEBUG
        //DebugSceneを最初に読み込む
        if (!SceneManager.GetSceneByName(debugSceneName).IsValid())
        {
            SceneManager.LoadScene(debugSceneName, LoadSceneMode.Additive);
        }
#endif
        //ManagerSceneが有効でない時(まだ読み込んでいない時)だけ追加ロードするように
        if (!SceneManager.GetSceneByName(managerSceneName).IsValid())
        {
            SceneManager.LoadScene(managerSceneName, LoadSceneMode.Additive);            
        }
    }

}
