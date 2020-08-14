using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMethodTest
{
    [RuntimeInitializeOnLoadMethod]
    static void AddUpdate()
    {
        StaticUpdateManager.updateAction += () => Debug.Log("StaticUpdate");
    }
}
