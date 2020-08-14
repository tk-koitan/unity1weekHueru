using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoitanLib
{
    public class DebugMeshRect : DebugMesh
    {
        [SerializeField]
        private float width;
        [SerializeField]
        private float height;
        [SerializeField]
        private Vector3 center;
        [SerializeField]
        private Transform target;

        protected override void CreateMesh(List<Vector3> vertices, List<Color> colors, List<int> lIndices, List<int> tIndices)
        {
            //必要なフラグを立てる
            vFlag = true;
            liFlag = true;
            tiFlag = true;
            //頂点位置
            vertices.Add(new Vector3(width / 2, height / 2) + center + target.position);
            vertices.Add(new Vector3(-width / 2, height / 2) + center + target.position);
            vertices.Add(new Vector3(-width / 2, -height / 2) + center + target.position);
            vertices.Add(new Vector3(width / 2, -height / 2) + center + target.position);
            //頂点カラー
            colors.Add(Color.white);
            colors.Add(Color.red);
            colors.Add(Color.white);
            colors.Add(Color.blue);
            //頂点インデックス(LineStrip)
            lIndices.Add(0);
            lIndices.Add(1);
            lIndices.Add(2);
            lIndices.Add(3);
            lIndices.Add(0);
            //頂点インデックス(Triangles)
            tIndices.Add(2);
            tIndices.Add(1);
            tIndices.Add(0);
            tIndices.Add(0);
            tIndices.Add(3);
            tIndices.Add(2);
        }

        protected override void ChangeVetices(List<Vector3> vertices)
        {
            vertices[0] = new Vector3(width / 2, height / 2) + center + target.position;
            vertices[1] = new Vector3(-width / 2, height / 2) + center + target.position;
            vertices[2] = new Vector3(-width / 2, -height / 2) + center + target.position;
            vertices[3] = new Vector3(width / 2, -height / 2) + center + target.position;
        }
    }
}
