using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimator : MonoBehaviour
{
    Animator animator;
    [SerializeField]
    int cnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.Play("TestTan", 0, cnt / 60f);        
        cnt++;
    }


}
