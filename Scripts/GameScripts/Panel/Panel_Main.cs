using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Main : UIPanelBase
    {
        public GameObject ObjFangWei_东;
        public GameObject ObjFangWei_西;
        public GameObject ObjFangWei_南;
        public GameObject ObjFangWei_北;
        private TweenAlpha _flickFangWeiTween;
        public GameObject diPanObj;
        public UILabel label_Time;

        public override void Init(PanelType type)
        {
            base.Init(type);
            ShowPanelDirectly();
        }

        private void StopCurFlickFangWei()
        {
            if (_flickFangWeiTween)
            {
                _flickFangWeiTween.value = 0;
                _flickFangWeiTween.enabled = false;
            }
        }

        public void FlickFangWei(DNXB_DIR dir)
        {
            StopCurFlickFangWei();
            switch (dir)
            {
                case DNXB_DIR.北:
                    _flickFangWeiTween = ObjFangWei_北.GetComponent<TweenAlpha>();
                    break;
                case DNXB_DIR.东:
                    _flickFangWeiTween = ObjFangWei_东.GetComponent<TweenAlpha>();
                    break;
                case DNXB_DIR.南:
                    _flickFangWeiTween = ObjFangWei_南.GetComponent<TweenAlpha>();
                    break;
                case DNXB_DIR.西:
                    _flickFangWeiTween = ObjFangWei_西.GetComponent<TweenAlpha>();
                    break;
                default:
                    break;
            }

            if (_flickFangWeiTween)
            {
                _flickFangWeiTween.enabled = true;
                _flickFangWeiTween.ResetToBeginning();
                _flickFangWeiTween.PlayForward();
            }
        }

        public void SetDiPanDir(DNXB_DIR dir)
        {
            switch (dir)
            {
                case DNXB_DIR.北:
                    diPanObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case DNXB_DIR.东:
                    diPanObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case DNXB_DIR.南:
                    diPanObj.transform.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case DNXB_DIR.西:
                    diPanObj.transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                default:
                    break;
            }
        }

        public void SetLabelTime(int time)
        {
            label_Time.text = "" + time;
        }
    }
}

