using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTest : MonoBehaviour
{
    public int num;
    [SerializeField]
    Test2DRect original;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < num; i++)
        {
            Instantiate(original, new Vector3(Random.Range(-9f, 9f), Random.Range(-5f, 5f)), Quaternion.identity);
        }
        original.gameObject.SetActive(false);
    }
}
