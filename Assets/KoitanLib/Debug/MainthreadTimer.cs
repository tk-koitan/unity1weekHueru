using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;

namespace KoitanLib
{
    /// <summary>
    /// メインスレッド負荷を測るクラス
    /// </summary>
    public class MainthreadTimer : MonoBehaviour
    {
        Recorder vBlankRecorder;
        Recorder playerLoopRecorder;
        float[] buffer = new float[60];
        int cnt = 0;
        float mainThreadTimeParS = 0;
        [SerializeField]
        private RectTransform meter;
        float mainthreadTime = 0;
        void Start()
        {
            // Profilerから”BehaviourUpdate”と名前の付く項目を抽出して、
            // 観測を有効化
            vBlankRecorder = Recorder.Get("WaitForTargetFPS");
            vBlankRecorder.enabled = true;
            playerLoopRecorder = Recorder.Get("PlayerLoop");
            playerLoopRecorder.enabled = true;
            Debug.Display(() => "CPU:" + mainThreadTimeParS.ToString("F1") + "ms(" + (6f * mainThreadTimeParS).ToString("F1") + "%)", this);
        }

        void Update()
        {
            mainthreadTime = (playerLoopRecorder.elapsedNanoseconds - vBlankRecorder.elapsedNanoseconds) / 1000000f;
            if (mainthreadTime < 0) mainthreadTime = 0;
            buffer[cnt] = mainthreadTime;
            cnt++;
            if (cnt >= 60f)
            {
                mainThreadTimeParS = buffer.Average();
                cnt = 0;
            }
            //メモリ更新
            meter.localScale = new Vector3(mainthreadTime * 6f / 100f, 1);
            /*
            // BehaviourUpdateが観測可能（EditorとDevelopment Players）なら、ログに処理時間（ミリ秒）を出力
            if (behaviourUpdateRecorder.isValid)
                Debug.Log("PlayerLoop time: " + behaviourUpdateRecorder.elapsedNanoseconds);
            */
        }
    }

}