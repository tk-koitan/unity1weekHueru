using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using TMPro;

namespace KoitanLib
{
    public class DebugText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textMesh;
        private List<DebugTextElem> debugTextElems = new List<DebugTextElem>();
        StringBuilder sb = new StringBuilder(1024);//適当

        private void Update()
        {
            sb.Clear();
            for (int i = debugTextElems.Count - 1; i >= 0; i--)
            {
                if (debugTextElems[i].target == null)
                {
                    debugTextElems.RemoveAt(i);
                    Debug.Log("対象のコンポーネントは削除されました:" + i);
                }
                else
                {
                    sb.Append(debugTextElems[i].message());
                    sb.Append(Environment.NewLine);                    
                }
            }
            textMesh.SetText(sb);
        }

        public void Display(Func<string> message, MonoBehaviour target, int priority = 0)
        {
            debugTextElems.Add(new DebugTextElem(message, target, priority));
            debugTextElems.Sort((a, b) => b.priority - a.priority);
        }

        public class DebugTextElem
        {
            public Func<string> message { get; private set; }
            public MonoBehaviour target { get; private set; }
            public int priority { get; private set; }

            public DebugTextElem(Func<string> message, MonoBehaviour target, int priority = 0)
            {
                this.message = message;
                this.target = target;
                this.priority = priority;
            }
        }
    }
}
