using UnityEngine;
using System.Collections;
using System;

namespace DYD
{
    public class Panel_CreateRoom : UIPanelBase
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
                ViewCenter.Instance.HidePanel<UIPanelBase>(type);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_Create").gameObject).onClick = delegate
            {
                ViewCenter.Instance.HidePanel<UIPanelBase>(type);
                ControlCenter.Instance.CreateRoom(GetPlayRule());
            };
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            string strArea = DataCenter.Instance.gamedata.GameArea.ToString();
            GameUtility.FindDeepChild(gameObject, strArea).GetComponent<UIToggle>().value = true;
        }

        public void SelectArea(UIToggle toggle)
        {
            if (toggle.value)
            {
                AREA_ENUM areaSelect = (AREA_ENUM)Enum.Parse(typeof(AREA_ENUM), toggle.transform.name);
                DataCenter.Instance.gamedata.GameArea = areaSelect;
                DealCommand.Instance.SendZBCJ();
                GameUtility.FindDeepChild(gameObject, "GameRule").gameObject.SetActive(false);
            }            
        }

        private string GetPlayRule()
        {
            string rule = "";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("局数_{0}", 0)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("局数_{0}", 1)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("局数_{0}", 2)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_{0}", 0)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_{0}", 1)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_{0}", 2)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_Rule_{0}", 0)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_Rule_{0}", 1)).GetComponent<UIToggle>().value ? "1" : "0";
            rule += GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_Rule_{0}", 2)).GetComponent<UIToggle>().value ? "1" : "0";
            return rule;
        }

        //更新规则
        public void UpdateGameRule(string[] strRules)
        {
            GameUtility.FindDeepChild(gameObject, "GameRule").gameObject.SetActive(true);

            int startIndex = 0;
            for (int i = 0; i < GameGD.JU_SHU_ITEM_SUM; i++)
            {
                GameGD.ruleArr[startIndex + i] = strRules[startIndex + i] + "局";
                //GameUtility.FindDeepChild(gameObject, string.Format("局数_{0}/Label", i)).GetComponent<UILabel>().text = GameGD.ruleArr[startIndex + i];

                Transform ts = GameUtility.FindDeepChild(gameObject, string.Format("局数_{0}", i));
                UpdateGameRule(ts, startIndex + i, strRules[startIndex + i], 1);
            }

            startIndex = GameGD.PLAYRULE_1_START_INDEX;
            for (int i = 0; i < GameGD.PLAYRULE_1_ITEM_SUM; i++)
            {
                GameGD.ruleArr[startIndex + i] = strRules[startIndex + i];
                Transform ts = GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_{0}", i));
                int toggleGroup = 2;
                if (DataCenter.Instance.gamedata.GameArea == AREA_ENUM.桂林) toggleGroup = 0;
                UpdateGameRule(ts, startIndex + i, strRules[startIndex + i], toggleGroup);
            }

            startIndex = GameGD.PLAYRULE_2_START_INDEX;
            for (int i = 0; i < GameGD.PLAYRULE_2_ITEM_SUM; i++)
            {
                GameGD.ruleArr[startIndex + i] = strRules[startIndex + i];
                Transform ts = GameUtility.FindDeepChild(gameObject, string.Format("钓鱼_Rule_{0}", i));
                int toggleGroup = 3;
                if (DataCenter.Instance.gamedata.GameArea == AREA_ENUM.柳州 && i==2) toggleGroup = 0;
                UpdateGameRule(ts, startIndex + i, strRules[startIndex + i], toggleGroup);
            }
        }

        private void UpdateGameRule(Transform ts, int index, string rule, int group=0)
        {
            GameGD.ruleArr[index] = rule;
            ts.gameObject.SetActive(true);        
            ts.GetComponentInChildren<UILabel>().text = GameGD.ruleArr[index];
            ts.GetComponent<UIToggle>().group = group;
            if (GameGD.ruleArr[index] == "") ts.gameObject.SetActive(false);
        }
    }
}

