using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;
using MeshCut2D;

public class MeshCutter : MonoBehaviour
{
    // スクリーン座標をワールド座標に変換した位置座標
    private Vector3 mousePos;
    private Vector3 origin;
    MeshFilter meshFilter;
    MeshCutResult meshCR;
    [SerializeField]
    GameObject meshPrefab;

    // Use this for initialization
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCR = ConvertMeshToMeshCutResult(meshFilter.mesh);
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3でマウス位置座標を取得する
        Vector3 screenPos = Input.mousePosition;
        // Z軸修正
        screenPos.z = 10f;
        // マウス位置座標をスクリーン座標からワールド座標に変換する
        mousePos = Camera.main.ScreenToWorldPoint(screenPos);

        if (Input.GetMouseButtonDown(0))
        {
            origin = mousePos;
        }

        if (Input.GetMouseButton(0))
        {
            //Debug.DrawLine(origin, mousePos);
        }


        if (Input.GetMouseButtonUp(0))
        {
            MeshCutResult result1 = new MeshCutResult(), result2 = new MeshCutResult();
            MeshCut2D.MeshCut2D.Cut(meshCR.vertices, meshCR.uv, meshCR.indices, meshCR.indices.Count, origin.x - transform.position.x, origin.y - transform.position.y, mousePos.x - transform.position.x, mousePos.y - transform.position.y, result1, result2);

            Mesh m = ConvertMeshCutResultToMesh(result1);
            if (m.vertexCount != 0)
            {
                GameObject obj = Instantiate(meshPrefab, transform.position, transform.rotation);
                MeshFilter fil = obj.GetComponent<MeshFilter>();
                fil.sharedMesh = m;
                MeshCollider col = obj.GetComponent<MeshCollider>();
                col.sharedMesh = m;
            }

            m = ConvertMeshCutResultToMesh(result2);
            if (m.vertexCount != 0)
            {
                GameObject obj = Instantiate(meshPrefab, transform.position, transform.rotation);
                MeshFilter fil = obj.GetComponent<MeshFilter>();
                fil.sharedMesh = m;
                MeshCollider col = obj.GetComponent<MeshCollider>();
                col.sharedMesh = m;
            }

            Destroy(meshFilter.gameObject);            
        }

        /*
        for (int i = 0; i < result1.indices.Count / 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 from = result1.vertices[result1.indices[i * 3 + j]];
                Vector2 to = result1.vertices[result1.indices[i * 3 + (j + 1) % 3]];
                Debug.DrawLine(from, to);
            }
        }

        for (int i = 0; i < result2.indices.Count / 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 from = result2.vertices[result2.indices[i * 3 + j]];
                Vector2 to = result2.vertices[result2.indices[i * 3 + (j + 1) % 3]];
                Debug.DrawLine(from, to);
            }
        }
        */

        /*
        private void OnDrawGizmos()
        {
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;

                for (int i = 0; i < mesh.vertexCount / 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 from = mesh.vertices[i * 3 + j];
                        Vector2 to = mesh.vertices[i * 3 + (j + 1) % 3];
                        Vector2 v = from - originPos;
                        Vector2 v1 = (Vector2)transform.position - originPos;
                        Vector2 v2 = to - from;
                        float cross = Cross2D(v1, v2);
                        if (cross != 0f)
                        {
                            float t1 = Cross2D(v, v2) / cross;
                            float t2 = Cross2D(v, v1) / cross;
                            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
                            {
                                Vector2 hitPos = from + t2 * v2;
                                Gizmos.color = Color.red;
                                GizmosExtensions2D.DrawWireCircle2D(hitPos, 0.1f);
                                Debug.Log("point:" + hitPos);
                            }
                        }
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(from, to);
                    }
                }
            }
            Gizmos.color = Color.white;
            GizmosExtensions2D.DrawArrow2D(originPos, transform.position);
        }
        */
    }

    private float Cross2D(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    private MeshCutResult CreatePlaneMesh()
    {
        MeshCutResult r = new MeshCutResult();
        r.AddRectangle(0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0);
        return r;
    }

    private Mesh ConvertMeshCutResultToMesh(MeshCutResult r)
    {
        Mesh m = new Mesh();
        m.SetVertices(r.vertices);
        m.SetColors(r.colors);
        m.SetIndices(r.indices.ToArray(), MeshTopology.Triangles, 0);
        m.SetUVs(0, r.uv);
        m.RecalculateBounds();
        m.RecalculateNormals();
        return m;
    }

    private MeshCutResult ConvertMeshToMeshCutResult(Mesh m)
    {
        MeshCutResult r = new MeshCutResult();
        r.vertices.AddRange(m.vertices);
        r.colors.AddRange(m.colors32);
        r.indices.AddRange(m.GetIndices(0));
        r.uv.AddRange(m.uv);
        //Debug.Log(string.Join(",", r.vertices));
        return r;
    }
}

namespace MeshCut2D
{
    // MeshCutの結果を管理するクラス
    public class MeshCutResult
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<Color32> colors = new List<Color32>();
        public List<int> indices = new List<int>();
        public List<Vector2> uv = new List<Vector2>();

        public void Clear()
        {
            vertices.Clear();
            colors.Clear();
            uv.Clear();
            indices.Clear();
        }

        public void AddTriangle(
            float x1, float y1,
            float x2, float y2,
            float x3, float y3,
            float uv1X, float uv1Y,
            float uv2X, float uv2Y,
            float uv3X, float uv3Y
            )
        {
            Vector3 v1 = new Vector3(x1, y1, 0);
            Vector3 v2 = new Vector3(x2, y2, 0);
            Vector3 v3 = new Vector3(x3, y3, 0);
            int i1 = vertices.IndexOf(v1);
            int i2 = vertices.IndexOf(v2);
            int i3 = vertices.IndexOf(v3);
            if (i1 == -1)
            {
                vertices.Add(new Vector3(x1, y1, 0));
                uv.Add(new Vector2(uv1X, uv1Y));
            }
            if (i2 == -1)
            {
                vertices.Add(new Vector3(x2, y2, 0));
                uv.Add(new Vector2(uv2X, uv2Y));
            }
            if (i3 == -1)
            {
                vertices.Add(new Vector3(x3, y3, 0));
                uv.Add(new Vector2(uv3X, uv3Y));
            }
            indices.Add(vertices.IndexOf(v1));
            indices.Add(vertices.IndexOf(v2));
            indices.Add(vertices.IndexOf(v3));
        }



        public void AddRectangle(
            float x1, float y1,
            float x2, float y2,
            float x3, float y3,
            float x4, float y4,
            float uv1_X, float uv1_Y,
            float uv2_X, float uv2_Y,
            float uv3_X, float uv3_Y,
            float uv4_X, float uv4_Y
        )
        {
            Vector3 v1 = new Vector3(x1, y1, 0);
            Vector3 v2 = new Vector3(x2, y2, 0);
            Vector3 v3 = new Vector3(x3, y3, 0);
            Vector3 v4 = new Vector3(x4, y4, 0);
            int i1 = vertices.IndexOf(v1);
            int i2 = vertices.IndexOf(v2);
            int i3 = vertices.IndexOf(v3);
            int i4 = vertices.IndexOf(v4);
            if (i1 == -1)
            {
                vertices.Add(new Vector3(x1, y1, 0));
                uv.Add(new Vector2(uv1_X, uv1_Y));
            }
            if (i2 == -1)
            {
                vertices.Add(new Vector3(x2, y2, 0));
                uv.Add(new Vector2(uv2_X, uv2_Y));
            }
            if (i3 == -1)
            {
                vertices.Add(new Vector3(x3, y3, 0));
                uv.Add(new Vector2(uv3_X, uv3_Y));
            }
            if (i4 == -1)
            {
                vertices.Add(new Vector3(x4, y4, 0));
                uv.Add(new Vector2(uv4_X, uv4_Y));
            }
            i1 = vertices.IndexOf(v1);
            i2 = vertices.IndexOf(v2);
            i3 = vertices.IndexOf(v3);
            i4 = vertices.IndexOf(v4);
            indices.Add(i1);
            indices.Add(i2);
            indices.Add(i3);
            indices.Add(i1);
            indices.Add(i3);
            indices.Add(i4);
        }
    }

    public class MeshCut2D
    {
        public static void Cut(
            IList<Vector3> vertices,
            IList<Vector2> uv,
            IList<int> indices,
            int indexCount,
            float x1, // LinePoint1
            float y1, // LinePoint1
            float x2, // LinePoint2
            float y2, // LinePoint2
            MeshCutResult _resultsA,
            MeshCutResult _resultsB)
        {
            _resultsA.Clear();
            _resultsB.Clear();

            for (int i = 0; i < indexCount; i += 3)
            {
                // 使いやすいように変数に代入しているだけ
                int indexA = indices[i + 0];
                int indexB = indices[i + 1];
                int indexC = indices[i + 2];
                Vector3 a = vertices[indexA];
                Vector3 b = vertices[indexB];
                Vector3 c = vertices[indexC];
                float uvA_X = uv[indexA].x;
                float uvA_Y = uv[indexA].y;
                float uvB_X = uv[indexB].x;
                float uvB_Y = uv[indexB].y;
                float uvC_X = uv[indexC].x;
                float uvC_Y = uv[indexC].y;

                bool aSide = IsClockWise(x1, y1, x2, y2, a.x, a.y);
                bool bSide = IsClockWise(x1, y1, x2, y2, b.x, b.y);
                bool cSide = IsClockWise(x1, y1, x2, y2, c.x, c.y);
                if (aSide == bSide && aSide == cSide)
                {
                    var triangleResult = aSide ? _resultsA : _resultsB;
                    triangleResult.AddTriangle(
                        a.x, a.y, b.x, b.y, c.x, c.y,
                        uvA_X, uvA_Y, uvB_X, uvB_Y, uvC_X, uvC_Y
                        );
                }
                else if (aSide != bSide && aSide != cSide)
                {
                    float abX, abY, caX, caY, uvAB_X, uvAB_Y, uvCA_X, uvCA_Y;
                    GetIntersectionLineAndLineStrip(
                        x1, y1,
                        x2, y2,
                        a.x, a.y,
                        b.x, b.y,
                        uvA_X, uvA_Y,
                        uvB_X, uvB_Y,
                        out abX, out abY,
                        out uvAB_X, out uvAB_Y);
                    GetIntersectionLineAndLineStrip(
                        x1, y1,
                        x2, y2,
                        c.x, c.y,
                        a.x, a.y,
                        uvC_X, uvC_Y,
                        uvA_X, uvA_Y,
                        out caX, out caY,
                        out uvCA_X, out uvCA_Y);
                    var triangleResult = aSide ? _resultsA : _resultsB;
                    var rectangleResult = aSide ? _resultsB : _resultsA;
                    triangleResult.AddTriangle(
                        a.x, a.y,
                        abX, abY,
                        caX, caY,
                        uvA_X, uvA_Y,
                        uvAB_X, uvAB_Y,
                        uvCA_X, uvCA_Y
                        );
                    rectangleResult.AddRectangle(
                        b.x, b.y,
                        c.x, c.y,
                        caX, caY,
                        abX, abY,
                        uvB_X, uvB_Y,
                        uvC_X, uvC_Y,
                        uvCA_X, uvCA_Y,
                        uvAB_X, uvAB_Y
                        );
                }
                else if (bSide != aSide && bSide != cSide)
                {
                    float abX, abY, bcX, bcY, uvAB_X, uvAB_Y, uvBC_X, uvBC_Y;
                    GetIntersectionLineAndLineStrip(
                        x1, y1,
                        x2, y2,
                        a.x, a.y,
                        b.x, b.y,
                        uvA_X, uvA_Y,
                        uvB_X, uvB_Y,
                        out abX, out abY,
                        out uvAB_X, out uvAB_Y);
                    GetIntersectionLineAndLineStrip(
                        x1, y1,
                        x2, y2,
                        b.x, b.y,
                        c.x, c.y,
                        uvB_X, uvB_Y,
                        uvC_X, uvC_Y,
                        out bcX, out bcY,
                        out uvBC_X, out uvBC_Y);
                    var triangleResult = bSide ? _resultsA : _resultsB;
                    var rectangleResult = bSide ? _resultsB : _resultsA;
                    triangleResult.AddTriangle(
                        b.x, b.y,
                        bcX, bcY,
                        abX, abY,
                        uvB_X, uvB_Y,
                        uvBC_X, uvBC_Y,
                        uvAB_X, uvAB_Y
                        );
                    rectangleResult.AddRectangle(
                        c.x, c.y,
                        a.x, a.y,
                        abX, abY,
                        bcX, bcY,
                        uvC_X, uvC_Y,
                        uvA_X, uvA_Y,
                        uvAB_X, uvAB_Y,
                        uvBC_X, uvBC_Y
                        );
                }
                else if (cSide != aSide && cSide != bSide)
                {
                    float bcX, bcY, caX, caY, uvBC_X, uvBC_Y, uvCA_X, uvCA_Y;
                    GetIntersectionLineAndLineStrip(
                        x1, y1,
                        x2, y2,
                        b.x, b.y,
                        c.x, c.y,
                        uvB_X, uvB_Y,
                        uvC_X, uvC_Y,
                        out bcX, out bcY,
                        out uvBC_X, out uvBC_Y);
                    GetIntersectionLineAndLineStrip(
                        x1, y1,
                        x2, y2,
                        c.x, c.y,
                        a.x, a.y,
                        uvC_X, uvC_Y,
                        uvA_X, uvA_Y,
                        out caX, out caY,
                        out uvCA_X, out uvCA_Y);
                    var triangleResult = cSide ? _resultsA : _resultsB;
                    var rectangleResult = cSide ? _resultsB : _resultsA;
                    triangleResult.AddTriangle(
                        c.x, c.y,
                        caX, caY,
                        bcX, bcY,
                        uvC_X, uvC_Y,
                        uvCA_X, uvCA_Y,
                        uvBC_X, uvBC_Y
                        );
                    rectangleResult.AddRectangle(
                        a.x, a.y,
                        b.x, b.y,
                        bcX, bcY,
                        caX, caY,
                        uvA_X, uvA_Y,
                        uvB_X, uvB_Y,
                        uvBC_X, uvBC_Y,
                        uvCA_X, uvCA_Y
                        );
                }
            }
        }

        private static void GetIntersectionLineAndLineStrip(
            float x1, float y1, // Line Point
            float x2, float y2, // Line Point
            float x3, float y3, // Line Strip Point
            float x4, float y4, // Line Strip Point
            float uv3_X, float uv3_Y,
            float uv4_X, float uv4_Y,
            out float x, out float y,
            out float uvX, out float uvY)
        {
            float s1 = (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
            float s2 = (x2 - x1) * (y1 - y4) - (y2 - y1) * (x1 - x4);

            float c = s1 / (s1 + s2);

            x = x3 + (x4 - x3) * c;
            y = y3 + (y4 - y3) * c;

            uvX = uv3_X + (uv4_X - uv3_X) * c;
            uvY = uv3_Y + (uv4_Y - uv3_Y) * c;
        }

        private static bool IsClockWise(
            float x1, float y1,
            float x2, float y2,
            float x3, float y3)
        {
            return (x2 - x1) * (y3 - y2) - (y2 - y1) * (x3 - x2) > 0;
        }
    }
}
