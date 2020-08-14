using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoverFrame : MonoBehaviour
{
    private float speed = 10f;

    [SerializeField]
    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        //transform.DOMoveX(5, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            input.x = -speed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            input.x = speed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            input.y = speed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            input.y = -speed;
        }
        transform.position += (Vector3)input / 60f;

        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position = new Vector3();
        }

        //当たり判定を可視化
        KoitanLib.Debug.DrawRect2D(2, 2, transform.position, color);
        KoitanLib.Debug.DrawRect2D(2, 2, Vector3.zero, color);
        KoitanLib.Debug.DrawLine2D(transform.position, Vector3.zero, 0.1f, color);
        //KoitanLib.Debug.DrawLine2D(transform.position, Vector3.zero, 0.1f, color);
        //KoitanLib.Debug.DrawPath2D(0.1f, color, transform.position, transform.position + new Vector3(1, 1), new Vector3(1, -1), Vector3.zero);
    }
}
