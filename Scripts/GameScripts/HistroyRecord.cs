using UnityEngine;
using System.Collections;

namespace DYD
{
    public class HistroyRecord : MonoBehaviour
    {
        [HideInInspector]
        public HistroyRecordData data;
        private UILabel label;
        public void Init(ref HistroyRecordData _data)
        {
            data = _data;
            label = GetComponent<UILabel>();

            string context = data.WXName_东 + "  总盈利：" + data.WinScore_东 + "    "
                            + data.WXName_南 + "  总盈利：" + data.WinScore_南 + "    "
                            + data.WXName_西 + "  总盈利：" + data.WinScore_西 + "    "
                            + data.WXName_北 + "  总盈利：" + data.WinScore_北 + "    ";
            //SetContext(context);
            SetContext(data.Info_1);
            EventInit();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_HF").gameObject).onClick = delegate
            {
                ViewCenter.Instance.GetPanel<Panel_HuiFang>(PanelType.HuiFang).StartHF(data);
            };
        }

        private void SetContext(string text)
        {
            label.text = text;
        }
    }
}

