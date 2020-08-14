using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class DynamicCreateMesh : MonoBehaviour
{
    [SerializeField]
    private Material _mat;

    [SerializeField]
    private Vector2 offset;

    private void Start()
    {
        var mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3 (0, 1f),
            new Vector3 (1f, -1f),
            new Vector3 (-1f, -1f),
        };
        mesh.uv = new Vector2[] {
            new Vector2 (0.5f, 1f),
            new Vector2 (1f, 0),
            new Vector2 (0, 0),
        };
        mesh.triangles = new int[] {
            0, 1, 2
        };

        mesh.uv = new Vector2[] {
            new Vector2 (0.5f, 1f) + offset,
            new Vector2 (1f, 0) + offset,
            new Vector2 (0, 0) + offset,
        };

        mesh.RecalculateNormals();
        var filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        var renderer = GetComponent<MeshRenderer>();
        renderer.material = _mat;
    }

    private void Update()
    {
        var filter = GetComponent<MeshFilter>();        
        filter.sharedMesh.uv = new Vector2[] {
            new Vector2 (0.5f, 1f) + offset,
            new Vector2 (1f, 0) + offset,
            new Vector2 (0, 0) + offset,
        };             
    }
}