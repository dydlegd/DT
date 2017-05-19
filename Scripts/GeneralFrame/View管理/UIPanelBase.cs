using UnityEngine;
using System.Collections;

namespace DYD
{

    public abstract class UIPanelBase : MonoBehaviour
    {
        protected UIPanel Panel { get { return GetComponent<UIPanel>(); } }

        public bool IsShow { get; set; }
        public virtual int Depth { get { return Panel.depth; } set { Panel.depth = value; } }
        public PanelType type { get; set; }
        private PanelColliderMode ColliderMode { get; set; }
        protected EventDelegate.Callback EnteredCall;//进入完成后的回调
        protected EventDelegate.Callback QuitedCall;//退出完成后的回调
        private bool IsFirstEnter = true;


        public virtual void Init(PanelType type)
        {
            HidePanelDirectly();
            this.type = type;
            UITweener tweener = GetComponent<UITweener>();
            if (tweener!=null)tweener.enabled = false;
        }


        public virtual void ShowPanel()
        {            
            EnterAnimation(EnteredCall);
        }

        public virtual void HidePanel()
        {            
            QuitAnimation(QuitedCall);
        }

        protected virtual void OnFirstEnter()
        {

        }

        protected virtual void EnterAnimation(EventDelegate.Callback onComplete)
        {
            ShowPanelDirectly();
            if (IsFirstEnter)
            {
                OnFirstEnter();
                IsFirstEnter = false;
            }
            UITweener tweener = GetComponent<UITweener>();
            if (tweener!=null)
            {
                tweener.enabled = true;
                //tweener.ResetToBeginning();
                tweener.PlayForward();
                EventDelegate.Set(tweener.onFinished, onComplete);
            }
            else
            {                
                if (onComplete != null) onComplete();
            }
        }

        protected virtual void QuitAnimation(EventDelegate.Callback onComplete)
        {
            UITweener tweener = GetComponent<UITweener>();
            if (tweener != null)
            {
                //tweener.ResetToBeginning();
                tweener.PlayReverse();
                onComplete += HidePanelDirectly;
                EventDelegate.Set(tweener.onFinished, onComplete);
            }
            else
            {                
                if (onComplete != null) onComplete();
                HidePanelDirectly();
            }        
        }

        public void ShowPanelDirectly()
        {
            IsShow = true;
            //gameObject.SetActive(true);
            NGUITools.SetActive(this.gameObject, true);
        }

        public void HidePanelDirectly()
        {
            IsShow = false;
            //gameObject.SetActive(false);
            NGUITools.SetActive(this.gameObject, false);
        }

        protected void AddColliderMode(PanelColliderMode mode)
        {
            if (ColliderMode == mode) return;

            ColliderMode = mode;
            switch (ColliderMode)
            {
                case PanelColliderMode.None:
                    break;
                case PanelColliderMode.Normal:
                    GameUtility.AddColliderBgToTarget(this.gameObject, "Mask", ResourcesManager.GetAtlas("Atlas_MJ_Common"), true);
                    break;
                case PanelColliderMode.WithBg:
                    GameUtility.AddColliderBgToTarget(this.gameObject, "Mask", ResourcesManager.GetAtlas("Atlas_MJ_Common"), false);
                    break;
                default:
                    break;
            }
        }
    }

}