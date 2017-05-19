using UnityEngine;
using System.Collections;

namespace DYD
{
    public class MJ : MonoBehaviour
    {
        private Transform mTrans;
        public MJType type { get; set; }
        public MJ_STAGE stage { get; set; }
        public UISprite sprite { get { return GetComponent<UISprite>(); } }
        public int MJ_W { get { return sprite.width; } }
        public int MJ_H { get { return sprite.height; } }
        public int Depth { get { return sprite.depth; } set { sprite.depth = value; sprite.SetDirty(); } }
        public int PlayerID { get; private set; }
        private Panel_Player playerPanel;

        public void Init(MJType type, MJ_STAGE stage, int playerId, Panel_Player playerPanel)
        {
            mTrans = this.transform;
            SetType(type);
            this.stage = stage;
            PlayerID = playerId;
            Depth = 0;
            this.playerPanel = playerPanel;
            sprite.color = Color.white;
            GetComponent<BoxCollider>().enabled = false;
            if ((stage == MJ_STAGE.IN || stage == MJ_STAGE.CPGResult) && !DataCenter.Instance.hFdata.IsHuiFanging )
            {
                GetComponent<BoxCollider>().enabled = true;
            }
            //ShowArrow(false);
            MJ_Drag md = GetComponent<MJ_Drag>();
            if (md) md.enabled = false;
        }

        void OnClick()
        {
            Debuger.Log("OnClick MJ :" + type);
            playerPanel.SelectMJ(this, true);
        }

#if UNITY_STANDALONE||UNITY_EDITOR
        //void OnHover (bool isOver)
        //{
        //    Debuger.Log("OnHover MJ :" + isOver + "    " + type);
        //    if (isOver)
        //    {
        //        playerPanel.SelectMJ(this);
        //    }            
        //}

//#elif UNITY_ANDROID||UNITY_IPHONE
        //void OnPress(bool isPressed)
        //{
        //    Debuger.Log("OnPress MJ :" + isPressed + "    " + type);
        //    //if (isPressed)
        //    {
        //        playerPanel.SelectMJ(this);
        //    }
        //}

        public void OnMouseOver()
        {
            Debuger.Log("OnMouseOver");
            playerPanel.SelectMJ(this);
        }

        public void OnMouseEnter()
        {
            Debuger.Log("OnMouseEnter");
            playerPanel.SelectMJ(this);
        }

        
#endif



        private void SetType(MJType type)
        {
            this.type = type;
        }

        public void InitSprite(string spriteName, int w, int h)
        {
            SetSpriteName(spriteName);
            sprite.SetDimensions(w, h);
        }

        public void SetSpriteName(string spriteName)
        {
            sprite.spriteName = spriteName;
        }

        public void SetPosition(Vector3 pos)
        {
            mTrans.localPosition = pos;
        }        

        public void SetColor(Color color)
        {
            sprite.color = color;
        }

        public void TranslatePosition(Vector3 translation)
        {
            SetPosition(mTrans.localPosition + translation);
        }

        public void ShowArrow(bool bShow)
        {
            Transform t = GameUtility.FindDeepChild(this.gameObject, "Arrow");

            t.gameObject.SetActive(bShow);
            //Debuger.Log("ShowArrow :" + type);
            if (bShow)
            {
                TweenPosition tp = t.GetComponent<TweenPosition>();
                int iOffsetMax = 45;
                int iOffsetMin = 35;
                switch (playerPanel.dir)
                {
                    case SEAT_DIR.DIR_LEFT:
                        t.GetComponent<UISprite>().transform.rotation = Quaternion.Euler(0, 0, -90);
                        tp.from = new Vector3(iOffsetMax-5, 5, 0);
                        tp.to = new Vector3(iOffsetMin-5, 5, 0);           
                        break;
                    case SEAT_DIR.DIR_BOTTOM:
                        tp.from = new Vector3(0, iOffsetMax, 0);
                        tp.to = new Vector3(0, iOffsetMin, 0);                        
                        break;
                    case SEAT_DIR.DIR_RIGHT:
                        t.GetComponent<UISprite>().transform.rotation = Quaternion.Euler(0, 0, 90);
                        tp.from = new Vector3(-iOffsetMax+5, 5, 0);
                        tp.to = new Vector3(-iOffsetMin+5, 5, 0);           
                        break;
                    case SEAT_DIR.DIR_TOP:
                        tp.from = new Vector3(0, iOffsetMax, 0);
                        tp.to = new Vector3(0, iOffsetMin, 0);           
                        break;
                    default:
                        break;
                }
                t.localPosition = tp.from;
                tp.ResetToBeginning();
                tp.PlayForward();
            }
        }
    }
}

