using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticUpdateManager : MonoBehaviour
{
    public static Action updateAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateAction();
    }
}
