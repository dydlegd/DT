using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Prompt : UIPanelBase
    {
        private UILabel label;

        public override void Init(PanelType type)
        {
            base.Init(type);
            label = GameUtility.FindDeepChild(gameObject, "Label").GetComponent<UILabel>();
        }

        public void ShowText(string text, float showTime = 1.5f)
        {
            ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.Prompt);
            label.text = text;
            CancelInvoke("HideText");
            Invoke("HideText", showTime);
        }

        private void HideText()
        {
            ViewCenter.Instance.HidePanel<UIPanelBase>(PanelType.Prompt);
        }
    }
}

