using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class GameOver_中鱼 : MonoBehaviour
    {
        private MJ_Manager _MJ_Mgr;
        private UISprite sprite_BG;
        private UISprite sprite_鱼;

        public void Init(MJ_Manager _Mgr)
        {
            _MJ_Mgr = _Mgr;
            sprite_BG = GetComponent<UISprite>();
            sprite_鱼 = GameUtility.FindDeepChild(gameObject, "鱼").GetComponent<UISprite>();

        }

        //刷新中鱼的数据
        public void RefleshData(List<MJType> dataList, int playerId)
        {            
            int SpaceW = GameGD.MJ_BOTTOM_CPG_W;
            ResetSpriteBG(SpaceW, dataList.Count);

            Vector3 pos = new Vector3(-(dataList.Count - 1) / 2.0f * SpaceW, 0, 0);
            for (int i = 0; i < dataList.Count; i++)
            {
                MJ mj = AddMJ(DataCenter.Instance.gamedata.PlayerID, dataList[i], MJ_STAGE.CPG);
                mj.transform.localPosition = pos;
                pos.x += SpaceW;
                if(DataCenter.Instance.players[playerId].gameOver.IsZhongYu(dataList[i]))
                {
                    mj.sprite.color = Color.green;
                }
            }

            sprite_鱼.gameObject.SetActive(false);

            if (DataCenter.Instance.gamedata.GameArea==AREA_ENUM.桂林)
            {
                sprite_鱼.gameObject.SetActive(true);
            }
        }

        private void ResetSpriteBG(int space, int len)
        {
            int bgW = space * len + 20;
            if (len == 0) bgW = 0;
            sprite_BG.SetDimensions(bgW, sprite_BG.height);
            sprite_BG.transform.localPosition = new Vector3(GD.DESIGN_SCREEN_W_2 - bgW / 2 + 460, sprite_BG.transform.localPosition.y, 0);
        }

        private MJ AddMJ(int playerId, MJType type, MJ_STAGE stage, bool bBack = false)
        {
            MJ mj = _MJ_Mgr.SpawnMJ(transform, type, playerId, stage, bBack, null);
            mj.transform.parent = transform;
            mj.Depth = 2000;

            return mj;
        }
    }
}

