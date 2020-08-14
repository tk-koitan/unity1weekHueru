using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverRigid : MonoBehaviour
{
    Rigidbody2D rb;
    float speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.velocity = input;

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position = new Vector3();
        }
    }
}
