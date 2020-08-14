using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;

namespace KoitanLib
{
    public class StatisticsManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform meter;

        private float[] dtBuffer = new float[60];
        private float dtParS = 0;
        private int cnt = 0;
        private float[] gpuBuffer = new float[60];
        private float gpuParS = 0;
        private int gCnt = 0;
        private float fps = 0;

        /// <summary>
        /// CPUの1フレーム当たりの処理時間
        /// </summary>
        public float CpuFrameTime { get; private set; }
        /// <summary>
        /// GPUの1フレーム当たりの処理時間
        /// </summary>
        public float GpuFrameTime { get; private set; }

        private FrameTiming[] _frameTimings = new FrameTiming[1];

        void Start()
        {
            QualitySettings.vSyncCount = 1;
            //Debug.Display(() => "CPU:" + CpuFrameTime + "ms", this);
            Debug.Display(() => "GPU:" + gpuParS + "ms", this);
            /*
            Debug.Display(() => "dt:" + Time.deltaTime * 1000 + "ms", this);
            Debug.Display(() => "dt(unscaled):" + Time.unscaledDeltaTime * 1000 + "ms", this);
            Debug.Display(() => "dtParS:" + dtpars * 1000 + "ms", this);
            */
            Debug.Display(() => "FPS:" + fps.ToString("F1"), this);
            //Debug.Display(() => "vSync:" + QualitySettings.vSyncCount, this);
            Debug.Display(() => "Memory(Used/Total):(" + ((Profiler.GetTotalAllocatedMemoryLong() >> 10) / 1024f).ToString("F1") + "/" + ((Profiler.GetTotalReservedMemoryLong() >> 10) / 1024f).ToString("F1") + ")MB", this);
            //Debug.Display(() => "Resolution:" + ScreenManager.ResolutionStr(), this);
        }

        private void Update()
        {
            dtBuffer[cnt] = Time.unscaledDeltaTime;
            dtParS = dtBuffer.Average();
            //gpuBuffer
            cnt++;
            if (cnt >= 60)
            {
                cnt = 0;
                fps = 1f / dtParS;
            }

            //メモリ更新
            meter.localScale = new Vector3(dtParS * 60f, 1);

            // フレーム情報をキャプチャする
            FrameTimingManager.CaptureFrameTimings();

            // 必要なフレーム数分の情報を取得する
            // 戻り値は実際に取得できたフレーム情報の数
            var numFrames = FrameTimingManager.GetLatestTimings((uint)_frameTimings.Length, _frameTimings);
            if (numFrames == 0) // 2020.02.16修正しました
            {
                // 1フレームの情報も得られていない場合はスキップ
                return;
            }

            // CPUの処理時間、CPUの処理時間を格納
            CpuFrameTime = (float)(_frameTimings[0].cpuFrameTime);
            GpuFrameTime = (float)(_frameTimings[0].gpuFrameTime);

            gpuBuffer[gCnt] = (float)(_frameTimings[0].gpuFrameTime);
            gCnt++;
            if (gCnt >= 60)
            {
                gCnt = 0;
                gpuParS = gpuBuffer.Average();
            }
        }
    }
}