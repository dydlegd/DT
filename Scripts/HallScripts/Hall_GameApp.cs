using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Hall_GameApp : SingletonMonoBase<Hall_GameApp>
    {

        // Use this for initialization
        void Start()
        {
            Debuger.EnableLog = GD.ENABLELOG;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Init();
            Debuger.Log("Game_Hall");
        }

        private void Init()
        {
            ResourcesManager.Instance.Init();
            GameInfoCenter.Instance.Init();
            Hall_ViewCenter.Instance.Init();
            //EventInit();

            //CheckHallUpdate();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Game_MJ").gameObject).onClick = delegate
            {
                
            };
        }

        public void CheckHallUpdate()
        {
            LoadSceneCenter.Instance.LoadScene(GameType.麻将, DownHallProcess, DownHallFinished);
        }

        private void DownHallProcess(float process)
        {
            Debuger.Log("大厅下载进度：" + process);
            Hall_ViewCenter.Instance.GetPanel<Hall_Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(string.Format("正在升级，请稍候\n\n{0}/100", (int)(process * 100)));
        }
        private void DownHallFinished(bool bSucceed)
        {
            Debuger.Log("大厅下载结果：" + bSucceed);
            if (bSucceed)
            {
                DownHallProcess(1);
                Hall_ViewCenter.Instance.HidePanel<Hall_Panel_MessageBox>(PanelType.MessageBox);
            }
            else
            {
                Hall_ViewCenter.Instance.GetPanel<Hall_Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox("下载出错，请重试 !", "", delegate { CheckHallUpdate(); });
            }
        }
    }
}

