using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverFixed : MonoBehaviour
{
    private float speed = 10f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
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
        transform.position += (Vector3)input / 100f;

        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position = new Vector3();
        }
    }
}
