using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class Player_中鱼 : MonoBehaviour
    {
        private MJ_Manager _MJ_Mgr;
        private UISprite sprite_BG;
        private List<MJ> MJList = new List<MJ>();

        public void Init(MJ_Manager _Mgr)
        {
            _MJ_Mgr = _Mgr;
            sprite_BG = GetComponentInChildren<UISprite>();
            Show(false);
            GameUtility.FindDeepChild(gameObject, "GameObject").gameObject.SetActive(false);
        }

        public void ReInit()
        {
            DespawnAllMJ();
            Show(false);
        }

        public void ShowZhongYu(List<MJType> dataList)
        {
            Show(true);
            //if (dataList.Count>0)
            {                
                StartCoroutine(ShowZhongYuLogic(dataList));
            }            
        }

        public void Show(bool bShow)
        {
            NGUITools.SetActive(gameObject, bShow);
        }

        private IEnumerator ShowZhongYuLogic(List<MJType> dataList)
        {
            int SpaceW = GameGD.MJ_BOTTOM_CPG_W;
            ResetSpriteBG(SpaceW, dataList.Count);

            Vector3 pos = new Vector3(-(dataList.Count - 1) / 2.0f * SpaceW, 0, -1);

            int playerId = DataCenter.Instance.gamedata.PlayerID;
            for (int i = 0; i < dataList.Count; i++)
            {
                MJ mj = AddMJ(playerId, dataList[i], MJ_STAGE.CPG);
                mj.transform.localPosition = pos;
                mj.Depth = 2000;
                pos.x += SpaceW;
                if (DataCenter.Instance.players[playerId].gameOver.IsZhongYu(dataList[i]))
                {
                    mj.sprite.color = Color.green;
                }
                yield return new WaitForSeconds(0.5f);
            }            
        }

        private void ResetSpriteBG(int space, int len)
        {
            int bgW = space * len + 20;
            if (len == 0)
            {
                bgW = 0;
                sprite_BG.GetComponent<UISprite>().enabled = false;
            }
            else
            {
                sprite_BG.GetComponent<UISprite>().enabled = true;
            }
            sprite_BG.SetDimensions(bgW, sprite_BG.height);
        }

        private MJ AddMJ(int playerId, MJType type, MJ_STAGE stage, bool bBack = false)
        {
            MJ mj = _MJ_Mgr.SpawnMJ(transform, type, playerId, stage, bBack, null);
            mj.Depth = 2000;
            mj.transform.parent = transform;
            MJList.Add(mj);
            //NGUITools.SetDirty(mj);
            return mj;
        }

        private void DespawnAllMJ()
        {
            while(MJList.Count>0)
            {
                DespawnMJ(MJList[0]);
            }
        }

        private void DespawnMJ(MJ mj)
        {
            MJList.Remove(mj);
            _MJ_Mgr.DespawnMJ(mj.transform);
        }
    }
}

