using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KoitanLib
{
    public class DebugTextMeshDisplayer : MonoBehaviour
    {
        private static Queue<string> strQue = new Queue<string>();
        private static Queue<Vector3> posQue = new Queue<Vector3>();
        private static Queue<int> sizeQue = new Queue<int>();

        private TextMeshPro textMeshPro;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            textMeshPro = GetComponent<TextMeshPro>();
        }

        void Update()
        {
            //初期化
            textMeshPro.text = string.Empty;
            //文字代入
            while (strQue.Count > 0)
            {
                textMeshPro.text += strQue.Dequeue();
            }

            // メッシュ更新
            textMeshPro.ForceMeshUpdate();

            //テキストメッシュプロの情報
            var textInfo = textMeshPro.textInfo;

            //テキスト数がゼロであれば表示しない
            if (textInfo.characterCount == 0)
            {
                return;
            }


            int index = 0;
            while (posQue.Count > 0)
            {
                Vector3 pos = posQue.Dequeue();
                int size = sizeQue.Dequeue();
                Vector3 dir = new Vector3();
                //1文字毎にloop
                for (int i = 0; i < size; i++)
                {
                    //1文字単位の情報
                    var charaInfo = textInfo.characterInfo[i + index];

                    //ジオメトリない文字はスキップ
                    if (!charaInfo.isVisible)
                    {
                        continue;
                    }

                    //Material参照しているindex取得
                    int materialIndex = charaInfo.materialReferenceIndex;

                    //頂点参照しているindex取得
                    int vertexIndex = charaInfo.vertexIndex;

                    //テキスト全体の頂点を格納(変数のdestは、destinationの略)
                    Vector3[] destVertices = textInfo.meshInfo[materialIndex].vertices;

                    // メッシュ情報にアニメーション後の頂点情報を入れる
                    float width = destVertices[vertexIndex + 3].x - destVertices[vertexIndex + 0].x;
                    float height = destVertices[vertexIndex + 1].y - destVertices[vertexIndex + 0].y;

                    if (i == 0)
                    {
                        dir = pos - destVertices[vertexIndex + 0];
                    }

                    destVertices[vertexIndex + 0] += dir;
                    destVertices[vertexIndex + 1] += dir;
                    destVertices[vertexIndex + 2] += dir;
                    destVertices[vertexIndex + 3] += dir;
                }
                index += size;
            }

            //ジオメトリ更新
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                //メッシュ情報を、実際のメッシュ頂点へ反映
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }

        public static void AddText(string str, Vector3 pos)
        {
            strQue.Enqueue(str);
            posQue.Enqueue(pos);
            sizeQue.Enqueue(str.Length);
        }
    }
}