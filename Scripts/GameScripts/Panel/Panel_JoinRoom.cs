using UnityEngine;
using System.Collections;
using System;

namespace DYD
{
    public class Panel_JoinRoom : UIPanelBase
    {
        private const int ROOM_NUM_DIGIT = 6;

        private string inputText = "";
        private UILabel[] inputLabelAry = new UILabel[ROOM_NUM_DIGIT];
        public override void Init(DYD.PanelType type)
        {
            base.Init(type);
            AddColliderMode(PanelColliderMode.WithBg);
            
            for (int i = 0; i < inputLabelAry.Length; i++)
            {
                inputLabelAry[i] = GameUtility.FindDeepChild(gameObject, "Input_" + i + "/Label").GetComponent<UILabel>();
            }
            SetInputText("");

            EventInit();
        }

        private void EventInit()
        {
            int i = 0;
            for (; i <= 9; i++)
            {
                NumEventInit(i);
            }
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "加入房间").gameObject).onClick = delegate
            {
                if (IsInputOver())
                {
                    HidePanel();
                    DealCommand.Instance.SendJoinRoom(GetInputRoomNum());
                }
                else
                {
                    ViewCenter.Instance.GetPanel<Panel_Prompt>(PanelType.Prompt).ShowText("请输入六位房间号");
                }
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "删除").gameObject).onClick = delegate
            {
                DeleteInputText();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_Close").gameObject).onClick = delegate
            {
                ViewCenter.Instance.HidePanel<UIPanelBase>(PanelType.JoinRoom);
            };
        }
        private void NumEventInit(int num)
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Num_" + num).gameObject).onClick = delegate
            {
                NumOnClick(num);
            };
        }

        private void NumOnClick(int num)
        {
            if (inputText.Length<ROOM_NUM_DIGIT)
            {
                inputText += num;
                SetInputText(inputText);
            }            
        }

        private void DeleteInputText()
        {
            if (inputText.Length>=1)
            {
                inputText = inputText.Remove(inputText.Length - 1, 1);
                SetInputText(inputText);
            }            
        }

        private void SetInputText(string text)
        {
            inputText = text;
            for (int i = 0; i < inputLabelAry.Length; i++)
            {
                inputLabelAry[i].text = "";
            }
            for (int i = 0; i < inputText.Length; i++)
            {
                inputLabelAry[i].text = "" + inputText[i];
            }
        }

        /// <summary>
        /// 是否输入完成
        /// </summary>
        /// <returns></returns>
        private bool IsInputOver()
        {
            return inputText.Length >= ROOM_NUM_DIGIT;
        }

        private int GetInputRoomNum()
        {
            int roomNum = 0;
            Int32.TryParse(inputText, out roomNum);
            return roomNum;
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            SetInputText("");
        }
    }
}

