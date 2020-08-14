using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BlockMesh : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MeshRenderer Renderer => _renderer != null ? _renderer : (_renderer = GetComponent<MeshRenderer>());

    private MeshFilter _filter;
    private MeshFilter Filter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

    private Mesh _mesh;

    int[,] blocks =
    {
        {1,1,1,1,1,1},
        {1,0,0,0,1,1},
        {1,0,0,0,0,1},
        {1,0,0,0,0,1},
        {1,0,0,0,0,1},
        {1,1,1,1,1,1}
    };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Create")]
    void Create()
    {
        var data = CreatBlocks(blocks);
        if (_mesh == null)
            _mesh = new Mesh();
        _mesh.vertices = data.vertices;
        _mesh.SetIndices(data.indices, MeshTopology.Quads, 0);
        Filter.mesh = _mesh;
        _mesh.RecalculateNormals();
    }

    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    private MeshData CreatBlocks(int[,] data)
    {
        int height = data.GetLength(0);
        int width = data.GetLength(1);
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        int[] indices = new int[(width * height) * 4];

        for (int i = 0; i <= height; i++)
        {
            vertices[i * (width + 1)] = new Vector3(0, i);
            for (int j = 0; j < width; j++)
            {
                vertices[j + 1 + i * (width + 1)] = new Vector3(j + 1, i);
            }
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (data[i, j] == 1)
                {
                    indices[(j + i * width) * 4] = j + i * (width + 1);
                    indices[(j + i * width) * 4 + 1] = j + i * (width + 1) + 1;
                    indices[(j + i * width) * 4 + 2] = j + (i + 1) * (width + 1) + 1;
                    indices[(j + i * width) * 4 + 3] = j + (i + 1) * (width + 1);
                }
            }
        }

        return new MeshData()
        {
            indices = indices,
            vertices = vertices
        };
    }
}
