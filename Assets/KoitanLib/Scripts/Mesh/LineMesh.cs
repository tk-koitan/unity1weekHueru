using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LineMesh : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MeshRenderer Renderer => _renderer != null ? _renderer : (_renderer = GetComponent<MeshRenderer>());

    private MeshFilter _filter;
    private MeshFilter Filter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

    private Mesh _mesh;

    [SerializeField]
    private List<Vector3> points = new List<Vector3>();

    [SerializeField]
    private float radius = 0.1f;

    [SerializeField]
    private AnimationCurve width = new AnimationCurve();

    [SerializeField]
    private MeshTopology meshTopology;

    private void Start()
    {
        //DebugTextManager.str = () => { return "半径:" + radius.ToString(); };
    }

    [ContextMenu("Create")]
    void Create()
    {
        var data = CreatLine(points, radius);
        if (_mesh == null)
            _mesh = new Mesh();
        _mesh.vertices = data.vertices;
        _mesh.SetIndices(data.indices, meshTopology, 0);
        Filter.mesh = _mesh;
        _mesh.RecalculateNormals();
    }

    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    private MeshData CreatLine(List<Vector3> points, float radius)
    {
        Vector3[] vertices = new Vector3[points.Count * 2];
        int[] indices = new int[(points.Count * 2 - 2) * 3];

        Vector3 oldCrossVec = Vector3.zero;
        for (int i = 0; i < points.Count - 2; i++)
        {
            Gizmos.color = Color.red;
            Vector3 tangent1 = points[i + 1] - points[i];
            Vector3 tangent2 = points[i + 2] - points[i + 1];
            Vector3 normal1 = new Vector3(-tangent1.y, tangent1.x).normalized;
            Vector3 normal2 = new Vector3(-tangent2.y, tangent2.x).normalized;
            if (i == 0)
            {
                oldCrossVec = normal1 * radius * width.Evaluate(0);
                vertices[0] = points[0] + oldCrossVec;
                vertices[1] = points[0] - oldCrossVec;
            }
            Vector3 crossVec = crossPoint(tangent1, points[i] + normal1 * radius * width.Evaluate(1f / (points.Count - 1) * (i + 1)), tangent2, points[i + 1] + normal2 * radius * width.Evaluate(1f / (points.Count - 1) * (i + 1))) - (points[i + 1]);
            vertices[i * 2 + 2] = points[i + 1] + crossVec;
            vertices[i * 2 + 3] = points[i + 1] - crossVec;
            indices[i * 6] = i * 2;
            indices[i * 6 + 1] = i * 2 + 2;
            indices[i * 6 + 2] = i * 2 + 1;
            indices[i * 6 + 3] = i * 2 + 3;
            indices[i * 6 + 4] = i * 2 + 1;
            indices[i * 6 + 5] = i * 2 + 2;
            oldCrossVec = crossVec;
            if (i == points.Count - 3)
            {
                crossVec = normal2 * radius * width.Evaluate(1);
                vertices[i * 2 + 4] = points[i + 2] + crossVec;
                vertices[i * 2 + 5] = points[i + 2] - crossVec;
                indices[i * 6 + 6] = i * 2 + 2;
                indices[i * 6 + 7] = i * 2 + 4;
                indices[i * 6 + 8] = i * 2 + 3;
                indices[i * 6 + 9] = i * 2 + 5;
                indices[i * 6 + 10] = i * 2 + 3;
                indices[i * 6 + 11] = i * 2 + 4;
            }
        }

        return new MeshData()
        {
            indices = indices,
            vertices = vertices
        };
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pivot = transform.position;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.color = Color.white;
            Vector3 tangent = points[i + 1] - points[i];
            Gizmos.DrawRay(pivot + points[i], tangent);
            Gizmos.color = Color.red;
            //Gizmos.DrawRay(pivot + (_vertices[i] + _vertices[i + 1]) / 2, normal);
        }
        Vector3 oldCrossVec = Vector3.zero;
        for (int i = 0; i < points.Count - 2; i++)
        {
            Gizmos.color = Color.red;
            Vector3 tangent1 = points[i + 1] - points[i];
            Vector3 tangent2 = points[i + 2] - points[i + 1];
            Vector3 normal1 = new Vector3(-tangent1.y, tangent1.x).normalized;
            Vector3 normal2 = new Vector3(-tangent2.y, tangent2.x).normalized;
            if (i == 0)
            {
                oldCrossVec = normal1 * radius * width.Evaluate(0);
            }
            //Gizmos.DrawRay(pivot + _vertices[i] + normal1 * _radius, tangent1);
            Gizmos.color = Color.blue;
            //GizmosExtensions2D.DrawWireCircle2D(crossPoint(tangent1, pivot + _vertices[i] + normal1 * _radius, tangent2, pivot + _vertices[i + 1] + normal2 * _radius), 0.01f);
            Vector3 crossVec = crossPoint(tangent1, pivot + points[i] + normal1 * radius * width.Evaluate(1f / (points.Count - 1) * (i + 1)), tangent2, pivot + points[i + 1] + normal2 * radius * width.Evaluate(1f / (points.Count - 1) * (i + 1))) - (pivot + points[i + 1]);
            Gizmos.DrawLine(pivot + points[i] + oldCrossVec, pivot + points[i + 1] + crossVec);
            Gizmos.DrawLine(pivot + points[i] - oldCrossVec, pivot + points[i + 1] - crossVec);
            oldCrossVec = crossVec;
            if (i == points.Count - 3)
            {
                crossVec = normal2 * radius * width.Evaluate(1);
                Gizmos.DrawLine(pivot + points[i + 1] + oldCrossVec, pivot + points[i + 2] + crossVec);
                Gizmos.DrawLine(pivot + points[i + 1] - oldCrossVec, pivot + points[i + 2] - crossVec);
            }
            //Gizmos.DrawRay(pivot + (_vertices[i] + _vertices[i + 1]) / 2, normal);
        }
    }

    private Vector3 crossPoint(Vector3 vec1, Vector3 a1, Vector3 vec2, Vector3 a2)
    {
        float cross = vec1.x * vec2.y - vec2.x * vec1.y;
        if (cross == 0)
        {
            return a2;
        }
        else
        {
            float s = (vec2.x * (a1.y - a2.y) - vec2.y * (a1.x - a2.x)) / cross;
            return s * vec1 + a1;
        }
    }
}
