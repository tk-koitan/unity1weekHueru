using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowTest : EditorWindow
{
    static string[] strList = new string[5];
    [MenuItem("KoitanLib/SceneLauncher/Setting", priority = -10)]
    public static void ShowWindow()
    {
        for (int i = 0; i < 5; i++)
        {
            string key = "SceneLaunchrPath" + i;
            strList[i] = EditorPrefs.GetString(key, "Assets/Scenes/SampleScene.unity");
        }           
        GetWindow<WindowTest>();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("---- SceneLauncher List ----");
        for (int i = 0; i < 5; i++)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUILayout.BeginVertical();
                {
                    string label = "Scene" + i;
                    EditorGUILayout.LabelField(label);
                    strList[i] = EditorGUILayout.TextField(strList[i]);
                    if (GUILayout.Button("更新"))
                    {
                        string key = "SceneLaunchrPath" + i;
                        EditorPrefs.SetString(key, strList[i]);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
