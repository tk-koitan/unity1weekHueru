using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KoitanLib
{
    public abstract class DebugMesh : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private List<Vector3> vertices = new List<Vector3>(1024);
        private List<Color> colors = new List<Color>(1024);
        private List<int> lIndices = new List<int>(1024);
        private List<int> tIndices = new List<int>(1024);


        protected bool vFlag = false;
        protected bool cFlag = false;
        protected bool liFlag = false;
        protected bool tiFlag = false;
        [SerializeField]
        protected bool isLine = false;        

        private void Awake()
        {
            //原点に合わせておく
            transform.position = new Vector3();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            //メッシュを作る
            CreateMesh(vertices, colors, lIndices, tIndices);
            //メッシュを反映する
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetColors(colors);
            mesh.SetIndices(lIndices, MeshTopology.LineStrip, 0);
            meshFilter.mesh = mesh;
            meshRenderer.sortingLayerName = "Debug";
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            //フラグで計算するか分岐
            if (vFlag)
            {
                ChangeVetices(vertices);
                meshFilter.mesh.SetVertices(vertices);
            }
            if (cFlag)
            {
                ChangeColors(colors);
                meshFilter.mesh.SetColors(colors);
            }
            if (isLine)
            {
                if (liFlag)
                {
                    ChangeLIndices(lIndices);
                    meshFilter.mesh.SetIndices(lIndices, MeshTopology.LineStrip, 0);
                }                
            }
            else
            {
                if (tiFlag)
                {
                    ChangeTIndices(tIndices);
                    meshFilter.mesh.SetIndices(tIndices, MeshTopology.Triangles, 0);
                }
            }
        }

        

        //最初にメッシュを作る
        protected abstract void CreateMesh(List<Vector3> vertices, List<Color> colors, List<int> lIndices, List<int> tIndices);
        protected virtual void ChangeVetices(List<Vector3> vertices)
        {
            //何もしない
        }
        protected virtual void ChangeColors(List<Color> colors)
        {
            //何もしない
        }
        protected virtual void ChangeLIndices(List<int> indices)
        {
            //何もしない
        }
        protected virtual void ChangeTIndices(List<int> indices)
        {
            //何もしない
        }
    }
}