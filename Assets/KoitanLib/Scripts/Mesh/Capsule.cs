using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Capsule : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MeshRenderer Renderer => _renderer != null ? _renderer : (_renderer = GetComponent<MeshRenderer>());

    private MeshFilter _filter;
    private MeshFilter Filter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

    private Mesh _mesh;

    void Start()
    {
        Create();
    }

    [SerializeField]
    private Vector2Int _divide = new Vector2Int(20, 20);

    /// <summary>
    /// 高さ
    /// </summary>
    [SerializeField]
    public float _height = 1f;

    /// <summary>
    /// 半径
    /// </summary>
    [SerializeField]
    float _radius = 0.5f;

    [ContextMenu("Create")]
    void Create()
    {
        int divideH = _divide.x;
        int divideV = _divide.y;

        var data = CreateCapsule(divideH, divideV, _height, _radius);
        if (_mesh == null)
            _mesh = new Mesh();
        _mesh.vertices = data.vertices;
        _mesh.SetIndices(data.indices, MeshTopology.Triangles, 0);
        Filter.mesh = _mesh;
        _mesh.RecalculateNormals();
    }

    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    MeshData CreateCapsule(int divideH, int divideV, float height = 1f, float radius = 0.5f)
    {
        divideH = divideH < 4 ? 4 : divideH;
        divideV = divideV < 4 ? 4 : divideV;
        radius = radius <= 0 ? 0.001f : radius;

        // 偶数のみ有効
        if (divideV % 2 != 0)
        {
            divideV++;
        }

        int cnt = 0;

        // =============================
        // 頂点座標作成
        // =============================

        int vertCount = divideH * divideV + 2;
        var vertices = new Vector3[vertCount];

        // 中心角
        float centerEulerRadianH = 2f * Mathf.PI / (float)divideH;
        float centerEulerRadianV = 2f * Mathf.PI / (float)divideV;

        float offsetHeight = height * 0.5f;

        // 天面
        vertices[cnt++] = new Vector3(0, radius + offsetHeight, 0);

        // カプセル上部
        for (int vv = 0; vv < divideV / 2; vv++)
        {
            var vRadian = (float)(vv + 1) * centerEulerRadianV / 2f;

            // 1辺の長さ
            var tmpLen = Mathf.Abs(Mathf.Sin(vRadian) * radius);

            var y = Mathf.Cos(vRadian) * radius;
            for (int vh = 0; vh < divideH; vh++)
            {
                var pos = new Vector3(
                    tmpLen * Mathf.Sin((float)vh * centerEulerRadianH),
                    y + offsetHeight,
                    tmpLen * Mathf.Cos((float)vh * centerEulerRadianH)
                );
                // サイズ反映
                vertices[cnt++] = pos;
            }
        }

        // カプセル下部
        int offset = divideV / 2;
        for (int vv = 0; vv < divideV / 2; vv++)
        {
            var yRadian = (float)(vv + offset) * centerEulerRadianV / 2f;

            // 1辺の長さ
            var tmpLen = Mathf.Abs(Mathf.Sin(yRadian) * radius);

            var y = Mathf.Cos(yRadian) * radius;
            for (int vh = 0; vh < divideH; vh++)
            {
                var pos = new Vector3(
                    tmpLen * Mathf.Sin((float)vh * centerEulerRadianH),
                    y - offsetHeight,
                    tmpLen * Mathf.Cos((float)vh * centerEulerRadianH)
                );
                // サイズ反映
                vertices[cnt++] = pos;
            }
        }

        // 底面
        vertices[cnt] = new Vector3(0, -radius - offsetHeight, 0);

        // =============================
        // 頂点インデックス作成
        // =============================

        int topAndBottomTriCount = divideH * 2;
        // 側面三角形の数
        int aspectTriCount = divideH * (divideV - 2 + 1) * 2;

        int[] indices = new int[(topAndBottomTriCount + aspectTriCount) * 3];

        //天面
        int offsetIndex = 0;
        cnt = 0;
        for (int i = 0; i < divideH * 3; i++)
        {
            if (i % 3 == 0)
            {
                indices[cnt++] = 0;
            }
            else if (i % 3 == 1)
            {
                indices[cnt++] = 1 + offsetIndex;
            }
            else if (i % 3 == 2)
            {
                var index = 2 + offsetIndex++;
                // 蓋をする
                index = index > divideH ? indices[1] : index;
                indices[cnt++] = index;
            }
        }

        // 側面Index

        /* 頂点を繋ぐイメージ
         * 1 - 2
         * |   |
         * 0 - 3
         * 
         * 0, 1, 2
         * 0, 2, 3
         *
         * 注意 : 1周した時にClampするのを忘れないように。
         */

        // 開始Index番号
        int startIndex = indices[1];

        // 天面、底面を除いたカプセルIndex要素数
        int sideIndexLen = aspectTriCount * 3;

        int lap1stIndex = 0;

        int lap2ndIndex = 0;

        // 一周したときのindex数
        int lapDiv = divideH * 2 * 3;

        int createSquareFaceCount = 0;

        for (int i = 0; i < sideIndexLen; i++)
        {
            // 一周の頂点数を超えたら更新(初回も含む)
            if (i % lapDiv == 0)
            {
                lap1stIndex = startIndex;
                lap2ndIndex = startIndex + divideH;
                createSquareFaceCount++;
            }

            if (i % 6 == 0 || i % 6 == 3)
            {
                indices[cnt++] = startIndex;
            }
            else if (i % 6 == 1)
            {
                indices[cnt++] = startIndex + divideH;
            }
            else if (i % 6 == 2 || i % 6 == 4)
            {
                if (i > 0 &&
                    (i % (lapDiv * createSquareFaceCount - 2) == 0 ||
                     i % (lapDiv * createSquareFaceCount - 4) == 0)
                )
                {
                    // 1周したときのClamp処理
                    // 周回ポリゴンの最後から2番目のIndex
                    indices[cnt++] = lap2ndIndex;
                }
                else
                {
                    indices[cnt++] = startIndex + divideH + 1;
                }
            }
            else if (i % 6 == 5)
            {
                if (i > 0 && i % (lapDiv * createSquareFaceCount - 1) == 0)
                {
                    // 1周したときのClamp処理
                    // 周回ポリゴンの最後のIndex
                    indices[cnt++] = lap1stIndex;
                }
                else
                {
                    indices[cnt++] = startIndex + 1;
                }

                // 開始Indexの更新
                startIndex++;
            }
            else
            {
                Debug.LogError("Invalid : " + i);
            }
        }


        // 底面Index
        offsetIndex = vertices.Length - 1 - divideH;
        lap1stIndex = offsetIndex;
        var finalIndex = vertices.Length - 1;
        int len = divideH * 3;

        for (int i = len - 1; i >= 0; i--)
        {
            if (i % 3 == 0)
            {
                // 底面の先頂点
                indices[cnt++] = finalIndex;
                offsetIndex++;
            }
            else if (i % 3 == 1)
            {
                indices[cnt++] = offsetIndex;
            }
            else if (i % 3 == 2)
            {
                var value = 1 + offsetIndex;
                if (value >= vertices.Length - 1)
                {
                    value = lap1stIndex;
                }

                indices[cnt++] = value;
            }
        }


        return new MeshData()
        {
            indices = indices,
            vertices = vertices
        };
    }
}