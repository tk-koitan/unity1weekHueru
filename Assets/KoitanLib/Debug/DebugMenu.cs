using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;
//using UnityEngine.Rendering.Universal;

namespace KoitanLib
{
    public class DebugMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textMesh;

        private static int max = 16;//数は適当
        private Action[] OnSelected = new Action[max];
        private Func<string>[] textStr = new Func<string>[max];
        private Func<string> titleStr;
        private int nowIndex = 0;
        private int maxIndex = 16;
        private Stack<Action> selectStack = new Stack<Action>(4);
        private Stack<int> indexStack = new Stack<int>(4);
        private Action nowMenu;
        private bool isOpen = false;
        StringBuilder sb = new StringBuilder(1024);//適当

        // Start is called before the first frame update
        void Start()
        {
            Close();
        }

        // Update is called once per frame
        void Update()
        {
            if (isOpen)
            {
                //閉じる
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Close();
                    return;
                }

                //カーソル移動
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    nowIndex = (nowIndex - 1 + maxIndex) % maxIndex;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    nowIndex = (nowIndex + 1 + maxIndex) % maxIndex;
                }

                //テキスト
                sb.Clear();
                sb.Append(titleStr());
                sb.Append(Environment.NewLine);
                for (int i = 0; i < maxIndex; i++)
                {
                    if (nowIndex == i)
                    {
                        sb.Append("> ");
                    }
                    else
                    {
                        sb.Append("  ");
                    }
                    sb.Append(textStr[i]());
                    sb.Append(Environment.NewLine);
                }
                textMesh.SetText(sb);

                //選んでいる位置を実行
                //キャンセル
                if (Input.GetKeyDown(KeyCode.X))
                {
                    if (indexStack.Count > 0)
                    {
                        nowIndex = indexStack.Pop();
                        nowMenu = selectStack.Pop();
                        nowMenu();
                    }
                    else
                    {
                        Close();
                    }
                    return;
                }
                else
                {
                    OnSelected[nowIndex]();
                }

            }
            else
            {
                //開く
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Open();
                }
            }
        }

        //メニュー開く
        private void Open()
        {
            isOpen = true;
            textMesh.gameObject.SetActive(true);
            nowIndex = 0;
            RootMenu();
            nowMenu = RootMenu;
        }


        //閉じる&初期化
        private void Close()
        {
            isOpen = false;
            selectStack.Clear();
            indexStack.Clear();
            textMesh.SetText(string.Empty);
            textMesh.gameObject.SetActive(false);
        }

        //便利
        private Action SetPushButton(Action onPush)
        {
            return () =>
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    //前のメニューと位置を覚えておく
                    selectStack.Push(new Action(nowMenu)); ;
                    indexStack.Push(nowIndex);
                    nowIndex = 0;
                    onPush();
                    nowMenu = onPush;
                }
            };
        }


        //メニュー
        /// <summary>
        /// 最初に開かれるメニュー
        /// </summary>
        private void RootMenu()
        {
            maxIndex = 3;
            titleStr = () => "  --- DEBUG MENU ---  ";
            textStr[0] = () => "AUDIO OPTION";
            textStr[1] = () => "SCREEN OPTION";
            textStr[2] = () => "TEST 2";
            OnSelected[0] = SetPushButton(AudioOption);
            OnSelected[1] = SetPushButton(ScreenOption);
            OnSelected[2] = SetPushButton(RootMenu);
        }

        /// <summary>
        /// 音量設定
        /// </summary>
        private void AudioOption()
        {
            maxIndex = 3;
            titleStr = () => "  --- AUDIO OPTION ---  ";
            textStr[0] = () => "TEST 0";
            textStr[1] = () => "TEST 1";
            textStr[2] = () => "TEST 2";
            OnSelected[0] = SetPushButton(RootMenu);
            OnSelected[1] = SetPushButton(RootMenu);
            OnSelected[2] = SetPushButton(RootMenu);
        }

        /// <summary>
        /// 画面設定
        /// </summary>
        private void ScreenOption()
        {
            maxIndex = 5;
            titleStr = () => "  --- SCREEN OPTION ---  ";
            textStr[0] = () => "RESOLUTION < " + ScreenManager.ResolutionStr() + " >";
            textStr[1] = () => "FULLSCREEN < " + ScreenManager.FullScreenStr() + " >";
            textStr[2] = () => "VSYNC < " + QualitySettings.vSyncCount + " >";
            textStr[3] = () => "TARGET FRAMERATE < " + Application.targetFrameRate + " >";
            textStr[4] = () => "POST EFFECT < " + ScreenManager.PostEffectStr() + " >";
            OnSelected[0] = SetRes;
            OnSelected[1] = SetFullScreen;
            OnSelected[2] = SetVSync;
            OnSelected[3] = SetTargetFPS;
            OnSelected[4] = SetPost;
        }


        private Action SetRes = () =>
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ScreenManager.SetNext();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ScreenManager.SetPrev();
            }
        };


        private Action SetFullScreen = () =>
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ScreenManager.SetFullScreen(!ScreenManager.screenIsFull);
            }
        };


        private Action SetVSync = () =>
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                QualitySettings.vSyncCount++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                QualitySettings.vSyncCount--;
            }
        };


        private Action SetTargetFPS = () =>
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Application.targetFrameRate++;
                }
                else
                {
                    Application.targetFrameRate += 10;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Application.targetFrameRate--;
                }
                else
                {
                    Application.targetFrameRate -= 10;
                }
            }
        };


        private Action SetPost = () =>
        {
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    //Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = !Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    //Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = !Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing;
                }
            }
        };
    }
}
