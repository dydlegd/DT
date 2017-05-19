using UnityEngine;
using System.Collections;

namespace DYD
{

    //垂直同步数
    public enum VSyncCountSetting
    {
        DontSync,
        EveryVBlank,
        EverSecondVBlank
    }

    public class ShowFps : MonoBehaviour
    {

        public VSyncCountSetting VSyncCount = VSyncCountSetting.DontSync;//用于快捷设置Unity Quanity设置中的垂直同步相关参数

        public int targetFrameRate = 60;

        public float f_UpdateInterval = 0.5F;

        private float f_LastInterval;

        private int i_Frames = 0;

        private float f_Fps;

        void Start()
        {
            //设置垂直同步相关参数
            switch (VSyncCount)
            {
                //默认设置，不等待垂直同步，可以获取更高的帧率数
                case VSyncCountSetting.DontSync:
                    QualitySettings.vSyncCount = 0;
                    break;
                //EveryVBlank，相当于帧率设为最大60
                case VSyncCountSetting.EveryVBlank:
                    QualitySettings.vSyncCount = 1;
                    break;
                //EverSecondVBlank情况，相当于帧率设为最大30
                case VSyncCountSetting.EverSecondVBlank:
                    QualitySettings.vSyncCount = 2;
                    break;
            }
            Application.targetFrameRate = targetFrameRate;

            f_LastInterval = Time.realtimeSinceStartup;

            i_Frames = 0;
        }

        void OnGUI()
        {
            GUILayout.Label("FPS:" + f_Fps.ToString("f1"));
        }

        void Update()
        {
            ++i_Frames;

            if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
            {
                f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);

                i_Frames = 0;

                f_LastInterval = Time.realtimeSinceStartup;
            }

            //++i_Frames;

            //f_LastInterval += Time.unscaledDeltaTime;

            //if ( f_LastInterval >= f_UpdateInterval )
            //{
            //    f_Fps = i_Frames / f_LastInterval;

            //    i_Frames = 0;
            //    f_LastInterval = 0f;
            //}
        }
    }

}
