using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Player_Right : Panel_Player
    {
        public override void Init(PanelType type)
        {
            base.Init(type);
            type = PanelType.Player_RIGHT;
        }

        protected override void UpdatePosition_IN()
        {
            Vector3 startPos = GetMJStartPos_IN();
            for (int i = 0; i < MJ_IN_List.Count; i++)
            {
                Vector3 pos = startPos;
                pos.y += i * GameGD.MJ_RIGHT_IN_SPACE;

                if (DataCenter.Instance.hFdata.IsHuiFanging
                    && DataCenter.Instance.hFdata.CurMoPai._playerId == data.playerId
                    && DataCenter.Instance.hFdata.CurMoPai._mjType != MJType.Null
                    && i == MJ_IN_List.Count - 1
                    )
                {
                    pos.y += 20;
                }

                MJ_IN_List[i].SetPosition(pos);
            }
        }
        protected override void UpdatePosition_OUT()
        {
            for (int i = 0; i < MJ_OUT_List.Count; i++)
            {
                Vector3 pos = MJ_OUT_START_TRANS.localPosition;
                pos.y += GameGD.MJ_RIGHT_OUT_SPACE_W * (i % GameGD.MJ_OUT_COL_SUM);
                pos.x += GameGD.MJ_RIGHT_OUT_SPACE_H * (i / GameGD.MJ_OUT_COL_SUM);
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
                    pos.y += GameGD.MJ_RIGHT_CPG_SPACE * (count-gangCount);
                    pos.y += i * GameGD.MJ_CPG_SPACE;

                    if (type == CPG_TYPE.暗杠 || type == CPG_TYPE.杠)
                    {
                        if (j == 3)//最后一个
                        {
                            pos.y -= GameGD.MJ_RIGHT_CPG_SPACE * 2 - 10;
                            MJ_CPG_List[count].Depth = 100;
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
            pos.y -= MJ_IN_List.Count * GameGD.MJ_RIGHT_IN_SPACE;

            return pos;
        }

        protected override Vector3 GetLastPosition_CPG()
        {
            Vector3 pos = MJ_CPG_START_TRANS.localPosition;
            if (data.MJ_CPG_List.Count > 0)
            {
                pos.y += GameGD.MJ_RIGHT_CPG_SPACE * (MJ_CPG_List.Count - 1);
                pos.y += (data.MJ_CPG_List.Count - 1) * GameGD.MJ_CPG_SPACE;
            }

            return pos;
        }

        protected override void SortMJDepth()
        {
            base.SortMJDepth();
            for (int i = MJ_IN_List.Count-1; i >= 0 ; i--)
            {
                MJ_IN_List[i].Depth += MJ_IN_List.Count - i;
            }
            for (int i = MJ_OUT_List.Count - 1; i >= 0; i--)
            {
                MJ_OUT_List[i].Depth += MJ_OUT_List.Count - i;
            }
            for (int i = MJ_CPG_List.Count - 1; i >= 0; i--)
            {
                MJ_CPG_List[i].Depth += MJ_CPG_List.Count - i;
            }
        }
    }

}

