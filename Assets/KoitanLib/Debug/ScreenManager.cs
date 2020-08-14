using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.Universal;

namespace KoitanLib
{
    public class ScreenManager : MonoBehaviour
    {
        public static int screenWidth
        {
            get { return Screen.width; }
        }
        public static int screenHeight
        {
            get { return Screen.height; }
        }
        public static bool screenIsFull
        {
            get { return Screen.fullScreen; }
        }
        public static int vSync
        {
            get { return QualitySettings.vSyncCount; }
        }

        public static int resolutionNum { get; private set; } = 0;

        public static Vector2Int Resolution()
        {
            switch (resolutionNum)
            {
                case 0:
                    return new Vector2Int(720, 405);
                case 1:
                    return new Vector2Int(1280, 720);
                case 2:
                    return new Vector2Int(1440, 810);
                case 3:
                    return new Vector2Int(1920, 1080);
                default:
                    return new Vector2Int(1280, 720);
            }
        }

        public static string ResolutionStr()
        {
            return screenWidth.ToString() + "x" + screenHeight.ToString();
        }

        public static void SetResolution(int num)
        {
            resolutionNum = num;
            Vector2Int res = Resolution();
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        }

        public static void SetNext()
        {
            resolutionNum = ((resolutionNum + 1) + 4) % 4;
            Vector2Int res = Resolution();
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        }

        public static void SetPrev()
        {
            resolutionNum = ((resolutionNum - 1) + 4) % 4;
            Vector2Int res = Resolution();
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        }

        public static void SetFullScreen(bool isFull)
        {
            Screen.SetResolution(Screen.width, Screen.height, isFull);
        }

        public static string FullScreenStr()
        {
            return screenIsFull ? "ON" : "OFF";
        }

        public static string PostEffectStr()
        {
            //return Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing ? "ON" : "OFF";
            return "OFF";
        }
    }
}