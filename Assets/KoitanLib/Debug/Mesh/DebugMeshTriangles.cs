using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KoitanLib
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DebugMeshTriangles : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private List<Vector3> vertices = new List<Vector3>(1024);
        private List<Color> colors = new List<Color>(1024);
        private List<int> indices = new List<int>(1024);
        private int head = 0;
        private Mesh mesh;

        private Queue<Action> meshQueue = new Queue<Action>();

        private bool isWire = false;

        private void Awake()
        {
            //原点に合わせておく
            transform.position = new Vector3();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            //メッシュを反映する
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            meshRenderer.sortingLayerName = "Debug";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftShift))
            {
                isWire = !isWire;
            }
        }

        //一番最後に実行されるように設定する
        private void LateUpdate()
        {
            /*
            Debug.Log(string.Join(", ", vertices));
            Debug.Log(string.Join(", ", indices));
            */
            //メッシュを反映する
            //meshをnewしないとバグる
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetColors(colors);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            meshFilter.mesh = mesh;
            //バッファを初期化
            vertices.Clear();
            colors.Clear();
            indices.Clear();
            head = 0;

            //meshFilter.mesh.SetIndices(indices, MeshTopology.LineStrip, 0);                            
        }

        public void CreateTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Color c)
        {
            //頂点位置
            vertices.Add(p0);
            vertices.Add(p1);
            vertices.Add(p2);
            //頂点カラー
            colors.Add(c);
            colors.Add(c);
            colors.Add(c);
            //頂点インデックス(Triangles)
            indices.Add(0 + head);
            indices.Add(1 + head);
            indices.Add(2 + head);
            //インデックスの位置をずらす
            head += 3;
        }

        public void CreateRect2D(float width, float height, Vector3 center, Color color)
        {
            //頂点位置
            vertices.Add(new Vector3(width / 2, height / 2) + center);
            vertices.Add(new Vector3(-width / 2, height / 2) + center);
            vertices.Add(new Vector3(-width / 2, -height / 2) + center);
            vertices.Add(new Vector3(width / 2, -height / 2) + center);
            //頂点カラー
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            //頂点インデックス(Triangles)
            indices.Add(2 + head);
            indices.Add(1 + head);
            indices.Add(0 + head);
            indices.Add(0 + head);
            indices.Add(3 + head);
            indices.Add(2 + head);
            //インデックスの位置をずらす
            head += 4;
        }

        public void CreateLine2D(Vector3 start, Vector3 end, float width, Color color)
        {
            Vector3 dir = end - start;
            Vector3 normal = new Vector3(-dir.y, dir.x).normalized;
            //頂点位置
            vertices.Add(start + normal * width);
            vertices.Add(start - normal * width);
            vertices.Add(end - normal * width);
            vertices.Add(end + normal * width);
            //頂点カラー
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            //頂点インデックス(Triangles)
            indices.Add(2 + head);
            indices.Add(1 + head);
            indices.Add(0 + head);
            indices.Add(0 + head);
            indices.Add(3 + head);
            indices.Add(2 + head);
            //インデックスの位置をずらす
            head += 4;
        }

        /// <summary>
        /// パスを描く
        /// シンプルバージョン
        /// </summary>
        /// <param name="width"></param>
        /// <param name="color"></param>
        /// <param name="path"></param>
        public void CreateSimplePath2D(float width, Color color, params Vector3[] path)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                CreateLine2D(path[i], path[i + 1], width, color);
            }
        }

        /// <summary>
        /// 可変長引数が使えます
        /// 角度が浅い時に線が伸びる不具合あり
        /// </summary>
        /// <param name="width"></param>
        /// <param name="color"></param>
        /// <param name="path"></param>
        public void CreatePath2D(float width, Color color, params Vector3[] path)
        {
            //パスの長さが1以下だったら描かない
            if (path.Length <= 1) return;
            //頂点位置と頂点インデックス
            Vector3 sdir, snormal, edir, enormal;
            bool isUp = true;//交差点が上か
            edir = path[1] - path[0];
            enormal = new Vector3(-edir.y, edir.x).normalized;

            vertices.Add(path[0] + enormal * width);
            vertices.Add(path[0] - enormal * width);


            for (int i = 0; i < path.Length - 2; i++)
            {
                sdir = edir;
                snormal = enormal;
                edir = path[i + 2] - path[i + 1];
                enormal = new Vector3(-edir.y, edir.x).normalized;
                //交差してる方を調べる
                if (Inner2D(snormal, edir) >= 0)
                {
                    Vector3 cp = CrossPoint(sdir, path[i + 1] + snormal * width, edir, path[i + 1] + enormal * width);
                    vertices.Add(path[i + 1] - snormal * width);
                    vertices.Add(cp);
                    vertices.Add(path[i + 1] - enormal * width);
                    if (isUp)
                    {
                        indices.Add(2 + i * 3 + head);
                        indices.Add(1 + i * 3 + head);
                        indices.Add(0 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                        indices.Add(0 + i * 3 + head);
                        indices.Add(4 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                    }
                    else
                    {
                        indices.Add(3 + i * 3 + head);
                        indices.Add(1 + i * 3 + head);
                        indices.Add(0 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                        indices.Add(0 + i * 3 + head);
                        indices.Add(4 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                    }
                    isUp = true;
                }
                else
                {
                    Vector3 cp = CrossPoint(sdir, path[i + 1] - snormal * width, edir, path[i + 1] - enormal * width);
                    vertices.Add(path[i + 1] + snormal * width);
                    vertices.Add(cp);
                    vertices.Add(path[i + 1] + enormal * width);
                    if (isUp)
                    {
                        indices.Add(2 + i * 3 + head);
                        indices.Add(0 + i * 3 + head);
                        indices.Add(1 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                        indices.Add(1 + i * 3 + head);
                        indices.Add(4 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                    }
                    else
                    {
                        indices.Add(3 + i * 3 + head);
                        indices.Add(0 + i * 3 + head);
                        indices.Add(1 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                        indices.Add(1 + i * 3 + head);
                        indices.Add(4 + i * 3 + head);
                        indices.Add(2 + i * 3 + head);
                        indices.Add(3 + i * 3 + head);
                    }
                    isUp = false;
                }
            }

            vertices.Add(path[path.Length - 1] + enormal * width);
            vertices.Add(path[path.Length - 1] - enormal * width);

            int j = path.Length - 2;
            if (isUp)
            {
                indices.Add(3 + j * 3 + head);
                indices.Add(1 + j * 3 + head);
                indices.Add(0 + j * 3 + head);
                indices.Add(2 + j * 3 + head);
                indices.Add(3 + j * 3 + head);
                indices.Add(0 + j * 3 + head);
            }
            else
            {
                indices.Add(2 + j * 3 + head);
                indices.Add(1 + j * 3 + head);
                indices.Add(0 + j * 3 + head);
                indices.Add(3 + j * 3 + head);
                indices.Add(2 + j * 3 + head);
                indices.Add(0 + j * 3 + head);
            }


            //頂点数
            int cnt = 3 * path.Length - 2;
            //頂点カラー
            for (int i = 0; i < vertices.Count; i++)
            {
                colors.Add(color);
            }
            //インデックスの位置をずらす
            head += vertices.Count;
        }

        //一つの方向の矢印
        public void CreateArrow2D(Vector3 start, Vector3 end, float width, Color color)
        {
            Vector3 dir = (end - start).normalized;
            Vector3 normal = new Vector3(-dir.y, dir.x);
            CreateLine2D(start, end - dir * width * 2, width, color);
            CreateTriangle(end - dir * width * 2 + normal * width * 2, end, end - dir * width * 2 - normal * width * 2, color);
        }

        public static float Cross2D(Vector3 v1, Vector3 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        public static float Inner2D(Vector3 v1, Vector3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static Vector3 CrossPoint(Vector3 vec1, Vector3 a1, Vector3 vec2, Vector3 a2)
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
}
