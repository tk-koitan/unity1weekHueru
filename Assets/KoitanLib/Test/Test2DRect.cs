using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2DRect : MonoBehaviour
{
    public Vector2 velocity;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        rb.velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (velocity.x > 0 && transform.position.x > 9f) velocity.x = -velocity.x;
        if (velocity.x < 0 && transform.position.x < -9f) velocity.x = -velocity.x;
        if (velocity.y > 0 && transform.position.y > 5f) velocity.y = -velocity.y;
        if (velocity.y < 0 && transform.position.y < -5f) velocity.y = -velocity.y;
        rb.velocity = velocity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.DrawLine(transform.position, collision.transform.position);
    }
}
