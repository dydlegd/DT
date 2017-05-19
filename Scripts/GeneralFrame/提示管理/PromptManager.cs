using UnityEngine;
using System.Collections;
using System;

namespace DYD
{

    public enum PromptButtonType
    {
        PromptBt_OK,
        PromptBt_Cancel
    }

    public enum PromptMsgType
    {

    }

    public enum PromptType
    {
        Normal,
        Error
    }

    public class PromptManager : SingletonMonoBase<PromptManager>
    {

        public void Init()
        {
            Debuger.Log("PromptManager Init Succeed !");
        }

        public void ShowPrompt(Prompt prompt)
        {

        }
    }

    public class Prompt
    {
        public PromptType type { get; set; }
        public string text { get; set; }

        public Prompt(PromptType type, string text)
        {
            this.type = type;
            this.text = text;
        }

        public virtual void CallBackFunc(PromptButtonType btType)
        {

        }
    }

    public class PromptOK : Prompt
    {
        public Action action { get; set; }

        public PromptOK(PromptType type, string text, Action action = null)
            : base(type, text)
        {
            this.action = action;
        }

        public override void CallBackFunc(PromptButtonType btType)
        {
            base.CallBackFunc(btType);
            if (action != null) action();
        }
    }

    public class PromptOKCancel : Prompt
    {
        public Action actionOK { get; set; }
        public Action actionCancel { get; set; }

        public PromptOKCancel(PromptType type, string text, Action actionOK = null, Action actionCancel = null)
            : base(type, text)
        {
            this.actionOK = actionOK;
            this.actionCancel = actionCancel;
        }

        public override void CallBackFunc(PromptButtonType btType)
        {
            base.CallBackFunc(btType);
            if (btType == PromptButtonType.PromptBt_OK)
            {
                if (actionOK != null) actionOK();
            }
            else if (btType == PromptButtonType.PromptBt_Cancel)
            {
                if (actionCancel != null) actionCancel();
            }
        }
    }

    

}