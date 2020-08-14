using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPoint : MonoBehaviour
{
    [SerializeField]
    private Vector3 p0;
    [SerializeField]
    private Vector3 p1;
    [SerializeField]
    private Vector3 p2;

    [SerializeField]
    private Vector3[] linePoints;

    [SerializeField]
    private Vector3 dir = Vector3.down;

    private Vector3 mPos;
    private Vector3 wPos;

    private bool isCrossTmp;
    private Vector3 p;
    private Vector3 target;
    private bool isCross;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mPos = Input.mousePosition;
        mPos.z = 0;
        wPos = Camera.main.ScreenToWorldPoint(mPos);
        wPos.z = 0;

        //初期化
        isCrossTmp = false;
        isCross = true;

        //当たり判定
        target = wPos + dir;
        for (int i = 0; i < linePoints.Length - 1; i++)
        {
            Vector3 a = linePoints[i];
            Vector3 b = linePoints[i + 1];
            (isCrossTmp, p) = DirectedLineCrossPoint(a, b, wPos, target);
            if (isCrossTmp)
            {
                isCross = true;
                target = p;
            }
        }

        //描画
        for (int i = 0; i < linePoints.Length - 1; i++)
        {
            KoitanLib.Debug.DrawArrow2D(linePoints[i], linePoints[i + 1], 0.025f, Color.white);
            Vector3 center = (linePoints[i] + linePoints[i + 1]) / 2;
            Vector3 vec = linePoints[i + 1] - linePoints[i];
            Vector3 normal = new Vector3(-vec.y, vec.x).normalized;
            KoitanLib.Debug.DrawArrow2D(center, center + normal * 0.025f * 8, 0.025f, Color.yellow);
        }
        KoitanLib.Debug.DrawArrow2D(wPos, wPos + dir, 0.05f, Color.white);
        if (isCross)
        {
            KoitanLib.Debug.DrawArrow2D(wPos, target, 0.05f, Color.red);
        }
        //KoitanLib.Debug.DrawTriangle(p0, p1, p2, Color.white);
    }

    (bool isCross, Vector3 p) LineCrossPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3 v = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = d - c;
        float cross_v1_v2 = Cross(v1, v2);
        float cross_v_v1 = Cross(v, v1);
        float cross_v_v2 = Cross(v, v2);
        float t1 = cross_v_v2 / cross_v1_v2;
        float t2 = cross_v_v1 / cross_v1_v2;
        bool isCross = (0f <= t1 && t1 <= 1f && 0f <= t2 && t2 <= 1f);
        Vector3 p = a + t1 * v1;
        return (isCross, p);
    }

    //有向線分abの法線方向から有向線分cdが侵入した場合
    (bool isCross, Vector3 p) DirectedLineCrossPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3 v = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = d - c;
        float cross_v1_v2 = Cross(v1, v2);
        float cross_v_v1 = Cross(v, v1);
        float cross_v_v2 = Cross(v, v2);
        if (cross_v_v1 < 0)
        {
            float t1 = cross_v_v2 / cross_v1_v2;
            float t2 = cross_v_v1 / cross_v1_v2;
            bool isCross = (0f <= t1 && t1 <= 1f && 0f <= t2 && t2 <= 1f);
            Vector3 p = a + t1 * v1;
            return (isCross, p);
        }
        else
        {
            return (false, d);
        }
    }

    float Dot(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    float Cross(Vector3 a, Vector3 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}
