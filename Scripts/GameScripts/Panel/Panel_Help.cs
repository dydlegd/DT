using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Help : UIPanelBase
    {
        public override int Depth
        {
            get
            {
                return base.Depth;
            }
            set
            {
                base.Depth = value;
                GameUtility.FindDeepChild(gameObject, "ScrollView").GetComponent<UIPanel>().depth = value + 1;
            }
        }

        public override void Init(PanelType type)
        {
            base.Init(type);
            AddColliderMode(PanelColliderMode.Normal);
            EventInit();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_Close").gameObject).onClick = delegate
            {
                AudioManager.Play(Tags.Audio_Name.Btn);
                HidePanel();
            };
        }
    }
}

