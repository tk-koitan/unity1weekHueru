using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody rb;
    Player owner;
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    private Bullet bulletOriginal;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (owner != null)
        {
            /*
            targetPos = owner.transform.position + new Vector3(1,0,0);
            transform.position += (targetPos - transform.position) * ratio;
            */
        }
    }

    public void SetOwner(Player owner)
    {
        this.owner = owner;
        gameObject.tag = "Player";
        GetComponent<Renderer>().material = owner.GetComponent<Renderer>().material;
        rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (owner != null)
        {
            if (collision.gameObject.tag == "Player")
            {
                rb.isKinematic = true;
                transform.SetParent(Player.Instance.transform);
                rb.velocity = Vector3.zero;
                gameObject.tag = "Player";
                gameObject.layer = LayerMask.NameToLayer("Player");
                Player.FireBulletEvent += OnBullet;
            }
        }
    }

    public void OnBullet()
    {
        Bullet b = Instantiate(bulletOriginal, transform.position, Quaternion.identity);
        b.SetVelocity(transform.up * speed, owner);
    }
}
