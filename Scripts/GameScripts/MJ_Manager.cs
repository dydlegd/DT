using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class MJ_Manager : Spawn
    {
        public Transform prefab;

        public void Init()
        {
            pool = CreatePool(transform.name);
            CreatePrefab(pool, prefab, 0);
        }

        public MJ SpawnMJ(Transform parent, MJType type, int playerId, MJ_STAGE stage, bool bBack, Panel_Player playerPanel)
        {
            Transform ts = pool.Spawn(prefab,pool.transform);
            ts.transform.localScale = Vector3.one;
            MJ mj = ts.GetComponent<MJ>();
            mj.Init(type, stage, playerId, playerPanel);
            int w = 0, h = 0;
            GetSpriteWH(ref w, ref h, playerId, stage);
            mj.InitSprite(GetSpriteName(type, playerId, stage, bBack), w, h);
            return mj;
        }

        public void DespawnMJ(Transform t)
        {
            pool.Despawn(t);
        }

        public void DespawnAll()
        {
            pool.DespawnAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">麻将类型</param>
        /// <param name="seatDir">方位</param>
        /// <param name="bIN">是否为手中的麻将</param>
        /// <param name="bBack">背面</param>
        /// <returns></returns>
        private string GetSpriteName(MJType type, SEAT_DIR seatDir, MJ_STAGE stage, bool bBack)
        {
            string spriteName = "";
            
            switch (seatDir)
            {
                case SEAT_DIR.DIR_LEFT:
                    {
                        if (stage == MJ_STAGE.IN && !DataCenter.Instance.hFdata.IsHuiFanging)
                            spriteName = Tags.MJ_SpriteName_L;
                        else
                            spriteName = type.ToString() + "_L";

                        if (bBack)
                            spriteName = Tags.MJ_SpriteName_Back_L;
                    }
                    break;
                case SEAT_DIR.DIR_BOTTOM:
                    {
                        if (stage == MJ_STAGE.IN)
                            spriteName = type.ToString() + "_MINE";
                        else
                            spriteName = type.ToString() + "_B";

                        if (bBack)
                            spriteName = Tags.MJ_SpriteName_Back_B;
                    }
                    break;
                case SEAT_DIR.DIR_RIGHT:
                    {
                        if (stage == MJ_STAGE.IN && !DataCenter.Instance.hFdata.IsHuiFanging)
                            spriteName = Tags.MJ_SpriteName_R;
                        else
                            spriteName = type.ToString() + "_R";
                        if (bBack)
                            spriteName = Tags.MJ_SpriteName_Back_R;
                    }
                    break;
                case SEAT_DIR.DIR_TOP:
                    {
                        if (stage == MJ_STAGE.IN && !DataCenter.Instance.hFdata.IsHuiFanging)
                            spriteName = Tags.MJ_SpriteName_T;
                        else
                            spriteName = type.ToString() + "_B";
                        if (bBack)
                            spriteName = Tags.MJ_SpriteName_Back_T;
                    }
                    break;
                default:
                    break;
            }
            return spriteName;
        }

        public string GetSpriteName(MJType type, int playerId, MJ_STAGE stage, bool bBack)
        {
            SEAT_DIR dir = GameGD.GetSeatDir(playerId);
            string spriteName = GetSpriteName(type, dir, stage, bBack);

            return spriteName;
        }

        public void GetSpriteWH(ref int w, ref int h, int playerId, MJ_STAGE stage)
        {
            SEAT_DIR dir = GameGD.GetSeatDir(playerId);
            switch (dir)
            {
                case SEAT_DIR.DIR_LEFT:
                    {
                        if (stage == MJ_STAGE.IN)
                        {
                            w = GameGD.MJ_LEFT_IN_W; h = GameGD.MJ_LEFT_IN_H;
                        }
                        else if (stage == MJ_STAGE.OUT)
                        {
                            w = GameGD.MJ_LEFT_OUT_W; h = GameGD.MJ_LEFT_OUT_H;
                        }
                        else if (stage == MJ_STAGE.CPG)
                        {
                            w = GameGD.MJ_LEFT_CPG_W; h = GameGD.MJ_LEFT_CPG_H;
                        }
                        if(DataCenter.Instance.hFdata.IsHuiFanging)
                        {
                            w = GameGD.MJ_LEFT_CPG_W; h = GameGD.MJ_LEFT_CPG_H;
                        }
                    }
                    break;
                case SEAT_DIR.DIR_BOTTOM:
                    {
                        if (stage == MJ_STAGE.IN)
                        {
                            w = GameGD.MJ_BOTTOM_IN_W; h = GameGD.MJ_BOTTOM_IN_H;
                        }
                        else if (stage == MJ_STAGE.OUT)
                        {
                            w = GameGD.MJ_BOTTOM_OUT_W; h = GameGD.MJ_BOTTOM_OUT_H;
                        }
                        else if (stage == MJ_STAGE.CPG)
                        {
                            w = GameGD.MJ_BOTTOM_CPG_W; h = GameGD.MJ_BOTTOM_CPG_H;
                        }
                        else if (stage == MJ_STAGE.CPGResult)
                        {
                            w = GameGD.MJ_BOTTOM_CPG_RESULT_W; h = GameGD.MJ_BOTTOM_CPG_RESULT_H;
                        }
                    }
                    break;
                case SEAT_DIR.DIR_RIGHT:
                    {
                        if (stage == MJ_STAGE.IN)
                        {
                            w = GameGD.MJ_RIGHT_IN_W; h = GameGD.MJ_RIGHT_IN_H;
                        }
                        else if (stage == MJ_STAGE.OUT)
                        {
                            w = GameGD.MJ_RIGHT_OUT_W; h = GameGD.MJ_RIGHT_OUT_H;
                        }
                        else if (stage == MJ_STAGE.CPG)
                        {
                            w = GameGD.MJ_RIGHT_CPG_W; h = GameGD.MJ_RIGHT_CPG_H;
                        }
                        if (DataCenter.Instance.hFdata.IsHuiFanging)
                        {
                            w = GameGD.MJ_RIGHT_CPG_W; h = GameGD.MJ_RIGHT_CPG_H;
                        }
                    }
                    break;
                case SEAT_DIR.DIR_TOP:
                    {
                        if (stage == MJ_STAGE.IN)
                        {
                            w = GameGD.MJ_TOP_IN_W; h = GameGD.MJ_TOP_IN_H;
                        }
                        else if (stage == MJ_STAGE.OUT)
                        {
                            w = GameGD.MJ_TOP_OUT_W; h = GameGD.MJ_TOP_OUT_H;
                        }
                        else if (stage == MJ_STAGE.CPG)
                        {
                            w = GameGD.MJ_TOP_CPG_W; h = GameGD.MJ_TOP_CPG_H;
                        }
                        if (DataCenter.Instance.hFdata.IsHuiFanging)
                        {
                            w = GameGD.MJ_TOP_CPG_W; h = GameGD.MJ_TOP_CPG_H;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

