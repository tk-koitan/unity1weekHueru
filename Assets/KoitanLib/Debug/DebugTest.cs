using UnityEngine;
using KoitanLib;

public class DebugTest : MonoBehaviour
{
    //始まってからのフレーム
    int cnt = 0;

    private void Start()
    {
        KoitanLib.Debug.Display(() => $"Frame{cnt}({cnt / 60}.{cnt % 60})", this);
    }

    private void Update()
    {
        cnt++;
    }
}