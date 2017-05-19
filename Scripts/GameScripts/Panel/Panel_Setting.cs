using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Setting : UIPanelBase
    {

        public override void Init(PanelType type)
        {
            base.Init(type);
            AddColliderMode(PanelColliderMode.WithBg);
            EventInit();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_Close").gameObject).onClick = delegate
            {
                ViewCenter.Instance.HidePanel<UIPanelBase>(PanelType.Setting);
            };
        }

        public void SetMusicVolume(UIToggle toggle)
        {
            AudioManager.SetAudioBackgroundVolumes(toggle.value ? 1 : 0);
        }
        public void SetEffectVolume(UIToggle toggle)
        {
            AudioManager.SetAudioEffectVolumes(toggle.value ? 1 : 0);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            GameUtility.FindDeepChild(this.gameObject, "音效/OFF").GetComponent<UIToggle>().value = AudioManager.audioEffectVolumes == 1;
            GameUtility.FindDeepChild(this.gameObject, "音乐/OFF").GetComponent<UIToggle>().value = AudioManager.audioBackgroundVolumes == 1;
        }
    }
}

