using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoitanLib
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TestMesh : MonoBehaviour
    {
        List<Vector3> buffer = new List<Vector3>(100);
        [SerializeField]
        float Length = 2f;
        [SerializeField]
        int n = 100;
        [SerializeField]
        float a = 1;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        int cnt = 0;

        // Start is called before the first frame update
        void Start()
        {
            var mesh = new Mesh();            
            List<Color> colors = new List<Color>();            

            for (int i = 0; i < n; i++)
            {
                buffer.Add(new Vector3(-Length / 2f + Length / n * i, a * Mathf.Sin(0.1f * Mathf.PI * i), 0));
                colors.Add(Color.white);
            }
            colors.Add(Color.red);
            colors.Add(Color.red);
            buffer.Add(new Vector3(-Length / 2f, 1f, 0));
            buffer.Add(new Vector3(Length / 2f, 1f, 0));

            mesh.SetVertices(buffer);
            mesh.SetColors(colors);

            var indices = new List<int>();            
            for (int i = 0; i < n - 1; i++)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }
            indices.Add(n);
            indices.Add(n + 1);

            //mesh.SetTriangles(triangles, 0);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = "Debug";
        }

        // Update is called once per frame
        void Update()
        {
            
            for (int i = 0; i < n; i++)
            {
                Vector3 vec = buffer[i];
                vec.y = a * Mathf.Sin(2 * Mathf.PI * (Time.time - buffer[i].x / 2f));
                buffer[i] = vec;
            }            
            meshFilter.mesh.SetVertices(buffer);            
            /*
            Vector3 vec = buffer[cnt];
            vec.y = Time.unscaledDeltaTime * 60f;
            buffer[cnt] = vec;
            meshFilter.mesh.SetVertices(buffer);
            */
            //meshFilter.mesh.SetVertices(buffer, 0, cnt);            
            //cnt = (cnt + 1) % n;
            //meshFilter.mesh.SetVertices()

        }
    }
}
