using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace KoitanLib
{
    public class Debug : MonoBehaviour
    {
        public static Debug Instance { get; private set; }
        [SerializeField]
        Canvas debugCanvas;
        [SerializeField]
        private DebugText debugText;
        [SerializeField]
        private DebugMeshTriangles debugMesh;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                DontDestroyOnLoad(debugCanvas);
                DontDestroyOnLoad(debugText);
                DontDestroyOnLoad(debugMesh);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift))
            {
                debugCanvas.gameObject.SetActive(!debugCanvas.gameObject.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("テスト");
            }
        }

        /// <summary>
        /// Scripting Define Symbols から"KOITAN_DEBUG"を消すと呼び出されなくなる
        /// </summary>
        /// <param name="o"></param>
        [Conditional("KOITAN_DEBUG")]
        public static void Log(object o)
        {
            UnityEngine.Debug.Log(o);
        }

        [Conditional("KOITAN_DEBUG")]
        public static void Display(Func<string> message, MonoBehaviour target, int priority = 0)
        {
            Instance.debugText.Display(message, target, priority);
        }

        [Conditional("KOITAN_DEBUG")]
        public static void DrawRect2D(float width, float height, Vector3 center, Color color)
        {
            Instance.debugMesh.CreateRect2D(width, height, center, color);
        }

        [Conditional("KOITAN_DEBUG")]
        public static void DrawLine2D(Vector3 start, Vector3 end, float width, Color color)
        {
            Instance.debugMesh.CreateLine2D(start, end, width, color);
        }

        [Conditional("KOITAN_DEBUG")]
        public static void DrawPath2D(float width,Color color,params Vector3[] path)
        {
            Instance.debugMesh.CreateSimplePath2D(width, color, path);
        }

        [Conditional("KOITAN_DEBUG")]
        public static void DrawArrow2D(Vector3 start, Vector3 end, float width, Color color)
        {
            Instance.debugMesh.CreateArrow2D(start, end, width, color);
        }

        [Conditional("KOITAN_DEBUG")]
        public static void DrawTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Color color)
        {
            Instance.debugMesh.CreateTriangle(p0, p1, p2, color);
        }

        [Conditional("KOITAN_DEBUG")]
        public static void ShowText(string str,Vector3 pos)
        {
            DebugTextMeshDisplayer.AddText(str, pos);
        }
    }
}
