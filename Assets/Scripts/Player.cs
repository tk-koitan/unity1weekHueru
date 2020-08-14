using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;
    private Vector3 input = new Vector3();
    [SerializeField]
    private Bullet bulletOriginal;
    [SerializeField]
    private float coolTime = 0.2f;
    private float currentCoolTime;
    public static Action FireBulletEvent;
    public static Player Instance;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FireBulletEvent += OnBullet;
    }

    // Update is called once per frame
    void Update()
    {
        input = Vector3.zero;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            input.x = 1f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            input.x = -1f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            input.y = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            input.y = -1f;
        }

        if (currentCoolTime > 0)
        {
            currentCoolTime -= Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            currentCoolTime = coolTime;
            FireBulletEvent();
        }
        transform.position += input * speed * Time.deltaTime;
    }

    public void OnBullet()
    {
        Bullet b = Instantiate(bulletOriginal, transform.position, Quaternion.identity);
        b.SetVelocity(Vector3.up * speed, this);
    }
}
