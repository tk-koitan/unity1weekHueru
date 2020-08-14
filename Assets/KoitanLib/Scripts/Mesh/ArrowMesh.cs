using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ArrowMesh : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MeshRenderer Renderer => _renderer != null ? _renderer : (_renderer = GetComponent<MeshRenderer>());

    private MeshFilter _filter;
    private MeshFilter Filter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

    private Mesh _mesh;

    [SerializeField]
    private Vector3 rootPoint;

    [SerializeField]
    private Vector3 targetPoint;

    [SerializeField]
    private float radius = 0.1f;

    private void Start()
    {

    }

    [ContextMenu("Create")]
    void Create()
    {
        var data = CreatArrow(rootPoint, targetPoint);
        if (_mesh == null)
            _mesh = new Mesh();
        _mesh.vertices = data.vertices;
        _mesh.SetIndices(data.indices, MeshTopology.Triangles, 0);
        Filter.mesh = _mesh;
        _mesh.RecalculateNormals();
        Filter.mesh.RecalculateBounds();
    }

    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    private MeshData CreatArrow(Vector3 root, Vector3 target)
    {
        Vector3[] vertices = new Vector3[7];
        int[] indices = new int[9];

        Vector3 direction = target - root;
        Vector3 tangent = direction.normalized;
        Vector3 normal = new Vector3(-tangent.y, tangent.x);

        vertices[0] = root - normal * radius + direction;
        vertices[1] = root - normal * radius;
        vertices[2] = root + normal * radius;
        vertices[3] = root + normal * radius + direction;
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 2;
        indices[4] = 3;
        indices[5] = 0;
        vertices[4] = root + normal * radius * 2 + direction;
        vertices[5] = root + tangent * radius * 2 + direction;
        vertices[6] = root - normal * radius * 2 + direction;
        indices[6] = 4;
        indices[7] = 5;
        indices[8] = 6;

        return new MeshData()
        {
            indices = indices,
            vertices = vertices
        };
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 direction = targetPoint - rootPoint;
        Vector3 tangent = direction.normalized;
        Vector3 normal = new Vector3(-tangent.y, tangent.x);
        /*
        Gizmos.DrawLine(rootPoint - normal * radius, rootPoint + normal * radius);
        Gizmos.DrawRay(rootPoint - normal * radius, tangent);
        Gizmos.DrawRay(rootPoint + normal * radius, tangent);
        Gizmos.DrawRay(rootPoint - normal * radius, tangent);
        */
        GizmosExtensions2D.DrawPathDirections(
            rootPoint - normal * radius * 2 + direction,
            normal * radius,
            -direction,
            normal * radius * 2,
            direction,
            normal * radius
            );
        GizmosExtensions2D.DrawPathPoints(
            rootPoint + direction + normal * radius * 2,
            rootPoint + direction + tangent * radius * 2,
            rootPoint + direction - normal * radius * 2
            );
    }
}
