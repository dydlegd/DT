using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Player_Top : Panel_Player
    {
        public override void Init(PanelType type)
        {
            base.Init(type);
            type = PanelType.Player_TOP;
        }
        protected override void UpdatePosition_IN()
        {
            Vector3 startPos = GetMJStartPos_IN();
            for (int i = 0; i < MJ_IN_List.Count; i++)
            {
                Vector3 pos = startPos;
                pos.x -= i * GameGD.MJ_TOP_IN_SPACE;

                if (DataCenter.Instance.hFdata.IsHuiFanging
                    && DataCenter.Instance.hFdata.CurMoPai._playerId == data.playerId
                    && DataCenter.Instance.hFdata.CurMoPai._mjType != MJType.Null
                    && i == MJ_IN_List.Count - 1
                    )
                {
                    pos.x -= 20;
                }

                MJ_IN_List[i].SetPosition(pos);
            }
        }
        protected override void UpdatePosition_OUT()
        {
            for (int i = 0; i < MJ_OUT_List.Count; i++)
            {
                Vector3 pos = MJ_OUT_START_TRANS.localPosition;
                pos.x -= GameGD.MJ_TOP_OUT_SPACE_W * (i % GameGD.MJ_OUT_COL_SUM);
                pos.y += GameGD.MJ_TOP_OUT_SPACE_H * (i / GameGD.MJ_OUT_COL_SUM);
                MJ_OUT_List[i].SetPosition(pos);
            }
        }
        protected override void UpdatePosition_CPG()
        {
            int count = 0;
            int gangCount = 0;

            for (int i = 0; i < data.MJ_CPG_List.Count; i++)
            {
                CPG_TYPE type = data.MJ_CPG_List[i].type;
                for (int j = 0; j < data.MJ_CPG_List[i].MJList.Count; j++)
                {
                    Vector3 pos = MJ_CPG_START_TRANS.localPosition;
                    pos.x += GameGD.MJ_TOP_CPG_SPACE * (count-gangCount);
                    pos.x += i * GameGD.MJ_CPG_SPACE;

                    if (type == CPG_TYPE.暗杠 || type == CPG_TYPE.杠)
                    {
                        if (j == 3)//最后一个
                        {
                            pos.x -= GameGD.MJ_TOP_CPG_SPACE * 2;
                            pos.y += 15;
                        }
                    }

                    MJ_CPG_List[count].SetPosition(pos);

                    count++;
                }
                if (type == CPG_TYPE.暗杠 || type == CPG_TYPE.杠) gangCount++;
            }
        }

        protected override Vector3 GetMJStartPos_IN()
        {
            Vector3 pos = MJ_IN_START_TRANS.localPosition;

            return pos;
        }

        protected override Vector3 GetLastPosition_CPG()
        {
            Vector3 pos = MJ_CPG_START_TRANS.localPosition;
            if (data.MJ_CPG_List.Count > 0)
            {
                pos.x += GameGD.MJ_TOP_CPG_SPACE * (MJ_CPG_List.Count - 1);
                pos.x += (data.MJ_CPG_List.Count - 1) * GameGD.MJ_CPG_SPACE;
            }

            return pos;
        }

        protected override void SortMJDepth()
        {
            base.SortMJDepth();
            for (int i = MJ_OUT_List.Count - 1; i >= 0; i--)
            {
                MJ_OUT_List[i].Depth += MJ_OUT_List.Count-i;
            }
        }

        //改变碰杠的牌的颜色
        protected override void SpawnMJ_CPG(CPG_Struct data)
        {
            for (int i = 0; i < data.MJList.Count; i++)
            {
                if (data.type == CPG_TYPE.暗杠)
                {
                    if (i < 3 || dir != SEAT_DIR.DIR_BOTTOM)
                    {
                        AddMJ(data.MJList[i], MJ_STAGE.CPG, true);
                        continue;
                    }
                }
                MJ mj = AddMJ(data.MJList[i], MJ_STAGE.CPG);
                switch (data.dir)
                {
                    case SEAT_DIR.DIR_BOTTOM:
                        break;
                    case SEAT_DIR.DIR_RIGHT:
                        if (i == 0) mj.sprite.color = Color.green;
                        break;
                    case SEAT_DIR.DIR_TOP:
                        if (data.type == CPG_TYPE.杠)
                        {
                            if (i == 3) mj.sprite.color = Color.green;
                        }
                        else
                        {
                            if (i == 1) mj.sprite.color = Color.green;
                        }
                        break;
                    case SEAT_DIR.DIR_LEFT:
                        if (i == 2) mj.sprite.color = Color.green;
                        break;
                    case SEAT_DIR.DIR_NULL:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

