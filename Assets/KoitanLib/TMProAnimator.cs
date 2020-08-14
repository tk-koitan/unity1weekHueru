using UnityEngine;
using TMPro;
using System;

public class TMProAnimator : MonoBehaviour
{
    [SerializeField]
    private float distanceMove = 1;

    [SerializeField]
    private float animationSpeed = 1;

    private TextMeshProUGUI textMeshPro;
    private Func<Vector3[]> vertAnim;

    private void Awake()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        vertAnim = () =>
        {
            Vector3 dic = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            return new Vector3[] { dic * distanceMove, dic * distanceMove, dic * distanceMove, dic * distanceMove };
        };
    }

    void Update()
    {
        // メッシュ更新
        textMeshPro.ForceMeshUpdate();

        //テキストメッシュプロの情報
        var textInfo = textMeshPro.textInfo;

        //テキスト数がゼロであれば表示しない
        if (textInfo.characterCount == 0)
        {
            return;
        }


        //1文字毎にloop
        for (int index = 0; index < textInfo.characterCount; index++)
        {
            //1文字単位の情報
            var charaInfo = textInfo.characterInfo[index];

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

            //移動する分
            float sinValue = Mathf.Sin(Time.time * animationSpeed + 10 * index);

            Vector3 dic = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            // メッシュ情報にアニメーション後の頂点情報を入れる

            /*
            destVertices[vertexIndex + 0] += distanceMove * (Vector3.down * sinValue);
            destVertices[vertexIndex + 1] += distanceMove * (Vector3.down * sinValue);
            destVertices[vertexIndex + 2] += distanceMove * (Vector3.down * sinValue);
            destVertices[vertexIndex + 3] += distanceMove * (Vector3.down * sinValue);
            */

            
            Vector3[] vertDic = vertAnim();

            destVertices[vertexIndex + 0] += vertDic[0];
            destVertices[vertexIndex + 1] += vertDic[1];
            destVertices[vertexIndex + 2] += vertDic[2];
            destVertices[vertexIndex + 3] += vertDic[3];
            
        }

        //ジオメトリ更新
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            //メッシュ情報を、実際のメッシュ頂点へ反映
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}