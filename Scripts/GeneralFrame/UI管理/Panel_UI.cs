using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class Panel_UI : UIPanelBase
    {
        public UILabel label_surplusMJ;
        public UILabel label_GameRule;
        public UILabel label_GameRule2;
        public UILabel label_RoomID;
        public UISprite sprite_ChuPai;
        public GameObject mMicrophone;

        public override void Init(PanelType type)
        {
            ShowPanelDirectly();
            EventInit();
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            RefleshLabel_RoomID();
            mMicrophone.SetActive(true);
            if(DataCenter.Instance.gamedata.gameMode==GAME_MODE.自由匹配
                ||DataCenter.Instance.hFdata.IsHuiFanging
                )
            {
                mMicrophone.SetActive(false);
            }
        }

        public void SetLabelSurplusMJSum(int sum)
        {
            label_surplusMJ.text = "剩 " + sum + " 张";
        }

        public void SetLabelSurplusMJSum(int curJuShu, int totalJuShu, int mjSurplusSum)
        {
            label_surplusMJ.text = curJuShu + "/" + totalJuShu + "  剩 " + mjSurplusSum + " 张";
        }
        public void SetGameRule(string text)
        {
            //for (int i = GameGD.PLAYRULE_1_START_INDEX; i <= GameGD.PLAYRULE_1_END_INDEX; i++)
            //{
            //    if(text[i]=='1')
            //    {
            //        label_GameRule.text = GameGD.ruleArr[i];
            //    }
            //}
            //for (int i = GameGD.PLAYRULE_2_START_INDEX; i <= GameGD.PLAYRULE_2_END_INDEX; i++)
            //{
            //    if (text[i] == '1')
            //    {
            //        label_GameRule2.text = GameGD.ruleArr[i];
            //    }
            //}
        }

        public void ShowChuPai(SEAT_DIR dir, MJType type)
        {
            sprite_ChuPai.gameObject.SetActive(true);
            sprite_ChuPai.spriteName = type.ToString() + "_MINE";
            Vector3 pos = Vector3.zero;

            switch (dir)
            {
                case SEAT_DIR.DIR_LEFT:
                    pos = new Vector3(-370, 0, 0);
                    break;
                case SEAT_DIR.DIR_BOTTOM:
                    pos = new Vector3(0, -150, 0);
                    break;
                case SEAT_DIR.DIR_RIGHT:
                    pos = new Vector3(370, 0, 0);
                    break;
                case SEAT_DIR.DIR_TOP:
                    pos = new Vector3(0, 230, 0);
                    break;
                default:
                    break;
            }
            sprite_ChuPai.transform.localPosition = pos;

            CancelInvoke("HideChuPai");
            Invoke("HideChuPai", 0.7f);
        }
        private void HideChuPai()
        {
            sprite_ChuPai.gameObject.SetActive(false);
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(this.gameObject,"Btn_Setting").gameObject).onClick = delegate
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.Setting);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(this.gameObject, "Btn_Chat").gameObject).onClick = delegate
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.Chat);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(this.gameObject, "Btn_Exit").gameObject).onClick = delegate
            {
                if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.自由匹配)
                {
                    //ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sIsReturnToHall, "", delegate { DealCommand.Instance.SendLeaveRoom(); }, "", null);
                    ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sIsReturnToHall, "", delegate { DealCommand.Instance.SendLeaveRoom(); }, "", null);
                }
                if (DataCenter.Instance.hFdata.IsHuiFanging)
                {
                    ViewCenter.Instance.GetPanel<Panel_HuiFang>(PanelType.HuiFang).HFLK();
                }
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_DismissRoom").gameObject).onClick = delegate
            {
                if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.创建房间)
                {
                    ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sIsDismissRoom, "", delegate { DealCommand.Instance.SendDissolveRoom(); }, "", null);
                }
            };
           
        }
        private void RefleshLabel_RoomID()
        {
            if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.创建房间)
            {
                label_RoomID.text = "房间ID：" + DataCenter.Instance.gamedata.RoomNum;
            }
            else
            {
                label_RoomID.text = "";
            }
        }

        public void SetShowRule1(string rule)
        {
            label_GameRule.text = rule;
        }
        public void SetShowRule2(string rule)
        {
            label_GameRule2.text = rule;
        }        
    }
}

