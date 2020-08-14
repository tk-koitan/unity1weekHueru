using UnityEditor;
using UnityEditor.SceneManagement;

namespace KoitanLib
{
    public class SceneLauncher : EditorWindow
    {
        [MenuItem("KoitanLib/SceneLauncher/Open Scene 0", priority = 0)]
        public static void OpenScene0()
        {
            bool isOK = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (isOK)
            {
                EditorSceneManager.OpenScene(EditorPrefs.GetString("SceneLaunchrPath0", "Assets/Scenes/SampleScene.unity"), OpenSceneMode.Single);
            }
        }

        [MenuItem("KoitanLib/SceneLauncher/Open Scene 1", priority = 0)]
        public static void OpenScene1()
        {
            bool isOK = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (isOK)
            {
                EditorSceneManager.OpenScene(EditorPrefs.GetString("SceneLaunchrPath1", "Assets/Scenes/SampleScene.unity"), OpenSceneMode.Single);
            }
        }

        [MenuItem("KoitanLib/SceneLauncher/Open Scene 2", priority = 0)]
        public static void OpenScene2()
        {
            bool isOK = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (isOK)
            {
                EditorSceneManager.OpenScene(EditorPrefs.GetString("SceneLaunchrPath2", "Assets/Scenes/SampleScene.unity"), OpenSceneMode.Single);
            }
        }

        [MenuItem("KoitanLib/SceneLauncher/Open Scene 3", priority = 0)]
        public static void OpenScene3()
        {
            bool isOK = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (isOK)
            {
                EditorSceneManager.OpenScene(EditorPrefs.GetString("SceneLaunchrPath3", "Assets/Scenes/SampleScene.unity"), OpenSceneMode.Single);
            }
        }

        [MenuItem("KoitanLib/SceneLauncher/Open Scene 4", priority = 0)]
        public static void OpenScene4()
        {
            bool isOK = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (isOK)
            {
                EditorSceneManager.OpenScene(EditorPrefs.GetString("SceneLaunchrPath4", "Assets/Scenes/SampleScene.unity"), OpenSceneMode.Single);
            }
        }
    }
}