using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class Panel_Chat : UIPanelBase
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

        public GameObject DialogueItemObj;
        public GameObject FaceItemObj;
        private UIInput inputFiled;

        public override void Init(PanelType type)
        {
            base.Init(type);
            EventInit();
            inputFiled = GameUtility.FindDeepChild(gameObject, "Input Filed").GetComponent<UIInput>();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_表情").gameObject).onClick = delegate
            {
                ShowFace(true);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_语言").gameObject).onClick = delegate
            {
                ShowDialogue(true);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_发送").gameObject).onClick = delegate
            {
                SendInputFiled();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "ColliderRect").gameObject).onClick = delegate
            {
                HidePanel();
            };
        }

        private void InitFaceItems()
        {

        }
        private void InitDialogueItems()
        {
            ShowDialogue(true);
            UIGrid grid = GameUtility.FindDeepChild(gameObject, "Dialogue_Items").GetComponent<UIGrid>();

            List<Transform> childList = grid.GetChildList();
            for (int i = 0; i < childList.Count; i++)
            {
                grid.RemoveChild(childList[i]);
                childList[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < Tags.Audio_Name.Chat.Boy_Chat.Length; i++)
            {
                GameObject go = GDFunc.CreateGameObject(DialogueItemObj, grid.transform);
                go.GetComponentInChildren<UILabel>().text = Tags.Audio_Name.Chat.Boy_Chat[i];
                UIEventListener.Get(go).onClick = delegate
                {
                    OnClickDialogue(go);
                };
                grid.AddChild(go.transform);
            }
        }

        private void ShowFace(bool bShow)
        {
            GameUtility.FindDeepChild(gameObject, "Btn_表情").GetComponent<UIToggle>().value = bShow;
            GameObject ItemsCenter = GameUtility.FindDeepChild(gameObject, "Face_Items").gameObject;
            if (bShow)
            {
                ShowDialogue(false);                
            }
            ItemsCenter.SetActive(bShow);
        }

        private void ShowDialogue(bool bShow)
        {
            GameUtility.FindDeepChild(gameObject, "Btn_语言").GetComponent<UIToggle>().value = bShow;
            GameObject ItemsCenter = GameUtility.FindDeepChild(gameObject, "Dialogue_Items").gameObject;

            if (bShow)
            {
                ShowFace(false);
            }
            ItemsCenter.SetActive(bShow);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
        }

        //清除聊天输入框
        private void ClearInputFiled()
        {
            inputFiled.value = "";
        }

        //发送聊天
        private void SendInputFiled()
        {
            DealCommand.Instance.SendLT(inputFiled.value);
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowMsg_Dialogue(inputFiled.value);
            ClearInputFiled();            
            HidePanel();
        }

        private void OnClickDialogue(GameObject go)
        {
            string text = go.GetComponentInChildren<UILabel>().text;
            Debuger.Log(text);

            string audioClipName = "";
            if (DataCenter.Instance.gamedata.playerInfo.sex == SEX.Boy)
                audioClipName = "chat_boy_";
            else
                audioClipName = "char_girl_";

            audioClipName += go.GetComponentInChildren<UILabel>().text;
            AudioManager.Play(audioClipName);
            DealCommand.Instance.SendSH(audioClipName);
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowMsg_Dialogue(go.GetComponentInChildren<UILabel>().text);
            HidePanel();
        }

        protected override void OnFirstEnter()
        {
            base.OnFirstEnter();
            InitFaceItems();
            InitDialogueItems();
            ShowFace(true);
        }
    }
}

