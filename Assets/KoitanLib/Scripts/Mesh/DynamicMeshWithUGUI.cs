using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DynamicMeshWithUGUI : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // 頂点の順番
        vh.AddTriangle(0, 1, 2);

        // UIVertex:各頂点の情報
        var v0 = new UIVertex();
        v0.position = new Vector3(-100f, -100f);
        // 修正箇所 : 色情報追加
        v0.color = new Color32(255, 0, 255, 255);
        var v1 = new UIVertex();
        v1.position = new Vector3(0, 100f);
        // 修正箇所 : 色情報追加
        v1.color = new Color32(255, 255, 255, 255);
        var v2 = new UIVertex();
        v2.position = new Vector3(100f, -100f);
        // 修正箇所 : 色情報追加
        v2.color = new Color32(255, 255, 0, 255);

        // 頂点情報を渡す
        vh.AddVert(v0);
        vh.AddVert(v1);
        vh.AddVert(v2);
    }
}