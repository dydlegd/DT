using UnityEngine;
using System.Collections;
using System;

namespace DYD
{
    public class Panel_MessageBox : UIPanelBase
    {
        private GameObject Btn_L, Btn_M, Btn_R;
        private UILabel label;
        public override void Init(PanelType type)
        {
            base.Init(type);

            EnteredCall = delegate { AudioManager.Play(Tags.Audio_Name.ErrorPrompt); };

            AddColliderMode(PanelColliderMode.Normal);
            Btn_L = GameUtility.FindDeepChild(this.gameObject, "Btn_L").gameObject;
            Btn_M = GameUtility.FindDeepChild(this.gameObject, "Btn_M").gameObject;
            Btn_R = GameUtility.FindDeepChild(this.gameObject, "Btn_R").gameObject;
            label = GameUtility.FindDeepChild(this.gameObject, "Text").GetComponent<UILabel>();

            Reset(false);
        }

        private void Reset(bool bShow)
        {
            NGUITools.SetActive(Btn_L, false);
            NGUITools.SetActive(Btn_M, false);
            NGUITools.SetActive(Btn_R, false);
            if (bShow)
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.MessageBox);
            }
        }

        public void ShowMessageBox(string msg, float showTime, Action action = null)
        {
            Reset(true);
            StartCoroutine(ShowMessageBoxShowTime(msg, showTime, action));
        }

        private IEnumerator ShowMessageBoxShowTime(string msg, float showTime, Action action)
        {
            
            yield return new WaitForSeconds(showTime);
            if (action != null) action();
        }

        public void ShowMessageBox(string msg, string centerStr, UIEventListener.VoidDelegate callBack)
        {
            Reset(true);
            SetText(msg);
            ShowBtn(Btn_M, callBack);
        }
        public void ShowMessageBox(string msg, string leftStr, UIEventListener.VoidDelegate leftCallBack, string rightStr, UIEventListener.VoidDelegate rightCallBack)
        {
            Reset(true);
            SetText(msg);
            ShowBtn(Btn_L, leftCallBack);
            ShowBtn(Btn_R, rightCallBack);
        }

        private void ShowBtn(GameObject btn, UIEventListener.VoidDelegate callBack)
        {
            NGUITools.SetActive(btn, true);
            UIEventListener.Get(btn).onClick = OnClosePanel;
            if (callBack!=null)
            {
                UIEventListener.Get(btn).onClick += callBack;                
            }            
        }

        private void OnClosePanel(GameObject go) { HidePanel(); }

        private void SetText(string text)
        {
            label.text = text;
        }

    }

}
