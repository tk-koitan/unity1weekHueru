using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicBrushMesh : MonoBehaviour
{
    struct VertexData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    private MeshFilter _filter;
    private MeshFilter Filter => _filter ?? (_filter = GetComponent<MeshFilter>());

    [SerializeField]
    private List<Vector3> _vertices = new List<Vector3>();

    [SerializeField]
    private List<int> _indices = new List<int>();

    [SerializeField]
    private float _radius = 0.1f;

    private bool _isInit;
    private int _offsetIndex = 0;

    private bool _isMouseDown;

    private Mesh _mesh;

    [SerializeField]
    private Camera _cam;

    [SerializeField]
    private Vector3 _mousePos;

    void Awake()
    {
        // 筆跡を滑らかにするためにfps60にする
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        _isMouseDown = Input.GetMouseButton(0);
        if (_isMouseDown)
        {
            _mousePos = Input.mousePosition;

            // マウス座標をワールド座標に変換
            _mousePos.z = Mathf.Abs(_cam.transform.position.z);
            var pos = _cam.ScreenToWorldPoint(_mousePos);
            Draw(pos);
        }
    }

    public void Draw(Vector3 pos)
    {
        var data = CreateVertex(pos);
        if (_mesh == null)
        {
            _mesh = new Mesh();
        }

        _mesh.vertices = data.vertices;
        _mesh.SetIndices(data.indices, MeshTopology.Triangles, 0);
        _mesh.RecalculateNormals();
        Filter.mesh = _mesh;
    }


    VertexData CreateVertex(Vector3 pos)
    {
        if (_isInit == false)
        {
            // 初回処理
            _isInit = true;
            var pt0 = new Vector3(pos.x - _radius * 0.5f, pos.y, 0);
            var pt1 = new Vector3(pos.x + _radius * 0.5f, pos.y, 0);
            var pt2 = new Vector3(pos.x - _radius * 0.5f, pos.y, 0);
            var pt3 = new Vector3(pos.x + _radius * 0.5f, pos.y, 0);
            _vertices.Add(pt0);
            _vertices.Add(pt1);
            _vertices.Add(pt2);
            _vertices.Add(pt3);
        }
        else
        {
            // 2回目以降
            var pt2 = new Vector3(pos.x - _radius * 0.5f, pos.y, 0);
            var pt3 = new Vector3(pos.x + _radius * 0.5f, pos.y, 0);
            _vertices.Add(pt2);
            _vertices.Add(pt3);
        }

        _indices.Add(_offsetIndex);
        _indices.Add(_offsetIndex + 1);
        _indices.Add(_offsetIndex + 3);
        _indices.Add(_offsetIndex);
        _indices.Add(_offsetIndex + 3);
        _indices.Add(_offsetIndex + 2);

        // 変数更新処理
        _offsetIndex += 2;

        return new VertexData()
        {
            vertices = _vertices.ToArray(),
            indices = _indices.ToArray()
        };
    }
}