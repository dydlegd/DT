using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_HuiFang : UIPanelBase
    {
        private HistroyRecordData mHrd;//播放的历史
        private bool bPause = false;//是否暂停
        private string mSpriteName_暂停 = "播放图标_2";
        private string mSpriteName_恢复 = "播放图标_3";

        public override void Init(PanelType type)
        {
            base.Init(type);
            //AddColliderMode(PanelColliderMode.WithBg);
            EventInit();
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
        }

        void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_L").gameObject).onClick = delegate
            {
                DealCommand.Instance.SendHFDEC(mHrd);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_M").gameObject).onClick = delegate
            {
                if (bPause)
                    ResumeHF();
                else
                    PauseHF();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_R").gameObject).onClick = delegate
            {
                DealCommand.Instance.SendHFINC(mHrd);
            };
        }

        public void StartHF(HistroyRecordData hr)
        {
            if (hr != null)
            {
                mHrd = hr;
                bPause = false;
                DealCommand.Instance.SendHFKS(mHrd);
                GameUtility.FindDeepChild(gameObject, "Btn_M").GetComponent<UISprite>().spriteName = mSpriteName_恢复;
                GameUtility.FindDeepChild(gameObject, "Btn_M").GetComponent<UIButton>().normalSprite = mSpriteName_恢复;
            }
        }

        private void PauseHF()
        {
            bPause = true;
            DealCommand.Instance.SendHFZT(mHrd);
            GameUtility.FindDeepChild(gameObject, "Btn_M").GetComponent<UISprite>().spriteName = mSpriteName_暂停;
            GameUtility.FindDeepChild(gameObject, "Btn_M").GetComponent<UIButton>().normalSprite = mSpriteName_暂停;
        }

        private void ResumeHF()
        {
            bPause = false;
            DealCommand.Instance.SendHFHF(mHrd);
            GameUtility.FindDeepChild(gameObject, "Btn_M").GetComponent<UISprite>().spriteName = mSpriteName_恢复;
            GameUtility.FindDeepChild(gameObject, "Btn_M").GetComponent<UIButton>().normalSprite = mSpriteName_恢复;
        }
        public void HFLK()
        {
            DealCommand.Instance.SendHFLK(mHrd);
            GameApp.Instance.SetGameState(GAME_STATE.HALL);
        }
    }
}

