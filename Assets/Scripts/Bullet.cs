using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 velocity;
    private Player owner;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// あいうえお
    /// </summary>
    /// <param name="velocity">aaa</param>
    public void SetVelocity(Vector3 velocity, Player owner)
    {
        this.velocity = velocity;
        this.owner = owner;
        rb.velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.tag);
        if(collision.collider.tag == "Enemy")
        {
            collision.collider.GetComponent<Enemy>().SetOwner(Player.Instance);            
        }
        Destroy(gameObject);
    }
}
