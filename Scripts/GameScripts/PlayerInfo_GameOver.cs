using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class PlayerInfo_GameOver : MonoBehaviour
    {
        public int playerId = 0;
        private MJ_Manager _MJ_Mgr;
        [HideInInspector]
        public Vector3 startMJPos;
        private GameOver_中鱼 _GameOver中鱼;
        private UILabel label_Info;
        private UILabel label_DeFen;
        private UISprite sprite_Zhuang;//庄图标
        private UISprite sprite_胡;
        

        public void Init(MJ_Manager _Mgr)
        {
            _MJ_Mgr = _Mgr;
            startMJPos = GameUtility.FindDeepChild(gameObject, "StartPos").localPosition;
            label_Info = GameUtility.FindDeepChild(gameObject, "Info").GetComponent<UILabel>();
            label_DeFen = GameUtility.FindDeepChild(gameObject, "得分").GetComponent<UILabel>();
            sprite_Zhuang = GameUtility.FindDeepChild(gameObject, "庄").GetComponent<UISprite>();
            sprite_胡 = GameUtility.FindDeepChild(gameObject, "胡").GetComponent<UISprite>();

            _GameOver中鱼 = GetComponentInChildren<GameOver_中鱼>();
            _GameOver中鱼.Init(_MJ_Mgr);

            ShowZhongYu(false);
        }

        public void UpdateData()
        {
            UpdateLabelInfo();
            Vector3 pos = startMJPos;
            PlayerData data = DataCenter.Instance.players[playerId];
            int iSpace = GameGD.MJ_BOTTOM_CPG_W - 5;

            //吃碰杠的牌
            for (int i = 0; i < data.MJ_CPG_List.Count; i++)
            {
                CPG_Struct cs = data.MJ_CPG_List[i];
                for (int j = 0; j < cs.MJList.Count; j++)
                {
                    MJ mj = null;
                    if (cs.type == CPG_TYPE.暗杠)
                    {
                        if(j<=2)
                        {
                            mj = AddMJ(DataCenter.Instance.gamedata.PlayerID, cs.MJList[j], MJ_STAGE.CPG, true);
                        }
                    }
                    if (mj==null) 
                        mj = AddMJ(DataCenter.Instance.gamedata.PlayerID, cs.MJList[j], MJ_STAGE.CPG);

                    if (i != 0 || j != 0)
                    {
                        pos.x += iSpace;
                    }
                    
                    switch (cs.dir)
                    {
                        case SEAT_DIR.DIR_BOTTOM:
                            break;
                        case SEAT_DIR.DIR_RIGHT:
                            if (j == 2) mj.sprite.color = Color.green;
                            break;
                        case SEAT_DIR.DIR_TOP:
                            if (cs.type == CPG_TYPE.杠 || cs.type == CPG_TYPE.暗杠)
                            {
                                if (j == 3) mj.sprite.color = Color.green;
                            }
                            else
                            {
                                if (j == 1) mj.sprite.color = Color.green;
                            }
                            break;
                        case SEAT_DIR.DIR_LEFT:
                            if (j == 0) mj.sprite.color = Color.green;
                            break;
                        case SEAT_DIR.DIR_NULL:
                            break;
                        default:
                            break;
                    }

                    Vector3 MJPos = pos;
                    if (cs.type == CPG_TYPE.杠 || cs.type == CPG_TYPE.暗杠)
                    {
                        if (j == 3)
                        {
                            MJPos.y += 10;
                            pos.x -= iSpace;
                            MJPos.x -= 2 * iSpace;
                            pos.x += 5;
                        }
                    }

                    mj.transform.localPosition = MJPos;
                }
                pos.x += 15;
            }
                        
            //手中的牌
            if (data.MJ_CPG_List.Count != 0) pos.x += 30;
            
            for (int i = 0; i < data.MJ_IN_List.Count; i++)
            {
                MJ mj = AddMJ(DataCenter.Instance.gamedata.PlayerID, data.MJ_IN_List[i], MJ_STAGE.CPG);
                mj.transform.localPosition = pos;
                pos.x += GameGD.MJ_BOTTOM_CPG_W-5;
            }


            

            //胡牌
            sprite_胡.gameObject.SetActive(false);
            if (data.gameOver.IsWin)
            {
                pos.x += 150;
                sprite_胡.gameObject.SetActive(true);
                sprite_胡.transform.localPosition = new Vector3(pos.x, sprite_胡.transform.localPosition.y, 0);
                pos.x += 50;
                MJ mj = AddMJ(DataCenter.Instance.gamedata.PlayerID, data.gameOver.huPaiMJType, MJ_STAGE.CPG);
                mj.transform.localPosition = pos;
                //mj.GetComponent<UISprite>().SetAnchor(sprite_胡,0,50)
                //mj.GetComponent<UISprite>().leftAnchor.relative = 1f;
                //mj.GetComponent<UISprite>().leftAnchor.absolute = 10;
            }      

            //中鱼
            ShowZhongYu(false);
            if(DataCenter.Instance.players[playerId].gameOver.IsWin)
            {
                ShowZhongYu(DataCenter.Instance.players[playerId].gameOver.ZhongYuList);
            }

            label_DeFen.text = "" + DataCenter.Instance.players[playerId].gameOver.DeFen;
        }

        private MJ AddMJ(int playerId, MJType type, MJ_STAGE stage, bool bBack = false)
        {
            MJ mj = _MJ_Mgr.SpawnMJ(transform, type, playerId, stage, bBack, null);
            mj.transform.parent = transform;

            return mj;
        }

        private void ShowZhongYu(List<MJType> zhongYuList)
        {
            if (zhongYuList.Count > 0)
            {
                ShowZhongYu(true);
                _GameOver中鱼.RefleshData(zhongYuList, playerId);
            }            
        }
        private void ShowZhongYu(bool bShow)
        {
            _GameOver中鱼.gameObject.SetActive(bShow);
        }

        private void UpdateLabelInfo()
        {
            PlayerData data = DataCenter.Instance.players[playerId];

            sprite_Zhuang.gameObject.SetActive(DataCenter.Instance.gamedata.ZhuangDir == data.playerInfo.DNXBDir);


            ////方位
            //label_Info.text = data.playerInfo.DNXBDir.ToString() + "  ";
            ////label_Info.text += data.playerInfo.WXID + "  ";
            //label_Info.text += data.playerInfo.WXName + "  ";

            

            //if (data.gameOver.IsWin)
            //{
            //    label_Info.text += "+中鱼" + data.gameOver.ZhongYuOKList.Count + "  ";
            //}            

            ////吃的数量大于3笔
            //if (GetCPGTypeSum(CPG_TYPE.吃)>=3)
            //{
            //    label_Info.text += "吃" + GetCPGTypeSum(CPG_TYPE.吃) + "笔" + "  ";
            //}

            ////明杠
            //if (GetCPGTypeSum(CPG_TYPE.杠) > 0 || GetCPGTypeSum(CPG_TYPE.暗杠) > 0)
            //{
            //    int score = GetCPGTypeSum(CPG_TYPE.杠) * 5 + GetCPGTypeSum(CPG_TYPE.暗杠) * 15;
            //    label_Info.text += "杠分" + score + "  ";
            //}

            ////自摸
            //if (data.gameOver.IsWin && data.gameOver.fangPaoDir == DNXB_DIR.Null)
            //{
            //    label_Info.text += "自摸" + "  ";
            //}

            ////胡牌类型
            //if (data.gameOver.huTypeList.Count>0)
            //{
            //    for (int i = 0; i < data.gameOver.huTypeList.Count; i++)
            //    {                    
            //        label_Info.text += data.gameOver.huTypeList[i].ToString();  
            //        if(i!=data.gameOver.huTypeList.Count-1)
            //            label_Info.text += "+"; 
            //    }
            //    label_Info.text += "  ";
            //}

            ////点炮
            //if (data.gameOver.fangPaoDir == data.playerInfo.DNXBDir)
            //{
            //    label_Info.text += "点炮" + "  ";
            //}
            

            //if (DataCenter.Instance.gamedata.ZhuangDir == data.playerInfo.DNXBDir)
            //{
            //    label_Info.text += "庄家" + "  ";
            //}
        }

        public void SetShowInfo(string info)
        {
            label_Info.text = info;
        }

        //获取指定类型的数量
        private int GetCPGTypeSum(CPG_TYPE type)
        {
            int count = 0;
            PlayerData data = DataCenter.Instance.players[playerId];
            for (int i = 0; i < data.MJ_CPG_List.Count; i++)
            {
                if(data.MJ_CPG_List[i].type==type)
                {
                    count++;
                }
            }
            return count;
        }
    }
}

