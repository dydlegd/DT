using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Record : UIPanelBase
    {
        private HistroyRecordManager _mHistroyRecordMgr;
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
            _mHistroyRecordMgr = GameUtility.FindDeepChild(gameObject, "HistroyRecordMgr").GetComponent<HistroyRecordManager>();
            _mHistroyRecordMgr.Init();

            base.Init(type);
            AddColliderMode(PanelColliderMode.Normal);
            EventInit();            
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            ClearAllHistroyRecord();
            DealCommand.Instance.SendGetZJ();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_Close").gameObject).onClick = delegate
            {
                AudioManager.Play(Tags.Audio_Name.Btn);
                HidePanel();
            };
        }

        public void AddHistoryRecord(ref HistroyRecordData data)
        {
            _mHistroyRecordMgr.AddHistroyRecord(ref data);
            GameUtility.FindDeepChild(gameObject, "Label_NoRecord").gameObject.SetActive(false);
        }

        private void ClearAllHistroyRecord()
        {
            _mHistroyRecordMgr.ClearAllHistroyRecord();
            GameUtility.FindDeepChild(gameObject, "Label_NoRecord").gameObject.SetActive(true);
        }
    }

}
