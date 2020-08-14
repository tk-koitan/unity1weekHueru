using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MyMesh : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private Mesh mesh;
    private List<Vector3> vertextList = new List<Vector3>();
    private List<Vector2> uvList = new List<Vector2>();
    private List<int> indexList = new List<int>();

    [ContextMenu("Create")]
    void Start()
    {
        mesh = CreatePlaneMesh();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        for (var i = 0; i < vertextList.Count / 3; i++)
        {
            Vector3 rand = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            for (int j = 0; j < 3; j++)
            {
                vertextList[i * 3 + j] += rand;
            }
        }
        mesh.SetVertices(vertextList);
    }

    Mesh CreatePlaneMesh()
    {
        var mesh = new Mesh();
        vertextList.Clear();
        uvList.Clear();
        indexList.Clear();

        vertextList.Add(new Vector3(0, 0, 0));//0番頂点
        vertextList.Add(new Vector3(0, 1, 0)); //1番頂点
        vertextList.Add(new Vector3(1, 0, 0)); //2番頂点
        /*
        vertextList.Add(new Vector3(1, 0, 0)); //2番頂点
        vertextList.Add(new Vector3(0, 1, 0)); //1番頂点
        vertextList.Add(new Vector3(1, 1, 0));  //3番頂点
        */

        uvList.Add(new Vector2(0, 0));
        uvList.Add(new Vector2(0, 1));
        uvList.Add(new Vector2(1, 0));
        /*
        uvList.Add(new Vector2(1, 0));
        uvList.Add(new Vector2(0, 1));
        uvList.Add(new Vector2(1, 1));
        */

        indexList.AddRange(new[] { 0, 1, 2});

        mesh.SetVertices(vertextList);//meshに頂点群をセット
        mesh.SetUVs(0, uvList);//meshにテクスチャのuv座標をセット（今回は割愛)
        mesh.SetIndices(indexList.ToArray(), MeshTopology.Triangles, 0);//メッシュにどの頂点の順番で面を作るかセット
        return mesh;
    }

    public Mesh CutTriangles()
    {
        var mesh = new Mesh();
        return mesh;
    }

    void OnDrawGizmos()
    {
        /*
        for (int i = 0; i < vertextList.Count - 1; i++)
        {
            GizmosExtensions2D.DrawArrow2D(vertextList[i], vertextList[i + 1]);
        }
        */
    }
}