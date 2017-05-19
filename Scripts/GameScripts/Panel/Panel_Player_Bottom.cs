using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class Panel_Player_Bottom : Panel_Player
    {
        private GameObject TrusteeshipObj;//托管
        public List<Btn_Result> btn_resultList;
        private List<MJ> ResultMJList = new List<MJ>();//吃碰杠选择的麻将
        public Transform ResultMJMgrTrans;
        private GameObject ResultColliderBgObj;
        private List<MJ> TingPaiMJList = new List<MJ>();
        private Transform TingPaiTrans;

        public override void Init(PanelType type)
        {
            base.Init(type);
            type = PanelType.Player_BOTTOM;

            TrusteeshipObj = GameUtility.FindDeepChild(gameObject, "托管").gameObject;
            ShowTrusteeship(false);

            TingPaiTrans = GameUtility.FindDeepChild(gameObject, "TingPai");
            ShowTingPai(new List<MJType>());            

            InitBtnResult();
        }

        protected override void UpdatePosition_IN()
        {
            base.UpdatePosition_IN();
            Vector3 startPos = GetMJStartPos_IN();
            for (int i = 0; i < MJ_IN_List.Count; i++)
            {
                Vector3 pos = startPos;
                pos.x += i * GameGD.MJ_BOTTOM_IN_SPACE;

                if (DataCenter.Instance.hFdata.IsHuiFanging)
                {
                    if (DataCenter.Instance.hFdata.CurMoPai._playerId == data.playerId
                        && DataCenter.Instance.hFdata.CurMoPai._mjType != MJType.Null
                        && i == MJ_IN_List.Count - 1
                        )
                    {
                        pos.x += 20;
                    }
                }
                else
                {
                    if (i == MJ_IN_List.Count - 1 && ControlCenter.Instance.IsChuPaing)
                    {
                        pos.x += 20;
                    }
                }
                MJ_IN_List[i].SetPosition(pos);
            }
        }
        protected override void UpdatePosition_OUT()
        {
            for (int i = 0; i < MJ_OUT_List.Count; i++)
            {
                Vector3 pos = MJ_OUT_START_TRANS.localPosition;
                pos.x += GameGD.MJ_BOTTOM_OUT_SPACE_W * (i % GameGD.MJ_OUT_COL_SUM);
                pos.y -= GameGD.MJ_BOTTOM_OUT_SPACE_H * (i / GameGD.MJ_OUT_COL_SUM);
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
                    pos.x += GameGD.MJ_BOTTOM_CPG_SPACE * (count - gangCount);
                    pos.x += i * GameGD.MJ_CPG_SPACE;

                    if (type==CPG_TYPE.暗杠||type==CPG_TYPE.杠)
                    {
                        if(j==3)//最后一个
                        {
                            pos.x -= GameGD.MJ_BOTTOM_CPG_SPACE*2;
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
            Vector3 pos = GetLastPosition_CPG();
            pos.x += 100;

            return pos;
        }

        protected override Vector3 GetLastPosition_CPG()
        {
            Vector3 pos = MJ_CPG_START_TRANS.localPosition;
            if (data.MJ_CPG_List.Count > 0)
            {
                pos.x += GameGD.MJ_BOTTOM_CPG_SPACE * (MJ_CPG_List.Count - 1);
                pos.x += (data.MJ_CPG_List.Count - 1) * GameGD.MJ_CPG_SPACE;
            }

            return pos;
        }

        protected override void SortMJDepth()
        {
            base.SortMJDepth();
            for (int i = MJ_IN_List.Count - 1; i >= 0; i--)
            {
                MJ_IN_List[i].Depth += i;
            }
            for (int i = MJ_OUT_List.Count - 1; i >= 0; i--)
            {
                MJ_OUT_List[i].Depth += i;
            }
            for (int i = MJ_CPG_List.Count - 1; i >= 0; i--)
            {
                MJ_CPG_List[i].Depth += i;
            }
        }

        protected override MJ AddMJ(MJType type, MJ_STAGE stage, bool bBack = false)
        {
            MJ mj = base.AddMJ(type, stage, bBack);
            int playerId = DataCenter.Instance.gamedata.PlayerID;
            if (stage == MJ_STAGE.IN 
                && ControlCenter.Instance.IsChuPaing
                )
            {
                bool bCanSelect = true;
                for (int i = 0; i < DataCenter.Instance.players[playerId].cannotChuMJList.Count; i++)
                {
                    if(type==DataCenter.Instance.players[playerId].cannotChuMJList[i])
                    {
                        bCanSelect = false;
                        break;
                    }
                }
                if (bCanSelect)
                {
                    if (!DataCenter.Instance.hFdata.IsHuiFanging)
                    {
                        MJ_Drag md = mj.GetComponent<MJ_Drag>();
                        if (md) md.enabled = true;
                    }                    
                }
                else
                {
                    mj.SetColor(new Color(0.3f, 0.3f, 0.3f));
                }
            }            
            return mj;
        }

        private void SetCurSelectMJ(MJ mj)
        {
            if (curSelectMJ != null)
            {
                curSelectMJ.TranslatePosition(new Vector3(0, -GameGD.MJ_SELECT_OFFSET, 0));
            }
            curSelectMJ = mj;
            if (curSelectMJ != null)
            {
                curSelectMJ.TranslatePosition(new Vector3(0, GameGD.MJ_SELECT_OFFSET, 0));
            }
        }

        public override void SelectMJ(MJ mj, bool bClick = false)
        {
            //if (ControlCenter.Instance.IsChuPaing)
            {
                if (mj == curSelectMJ)
                {
                    Debuger.Log("SelectMJ MJ :" + bClick);
                    if (ControlCenter.Instance.IsChuPaing)
                    {
                        if (bClick)
                        {
                            ControlCenter.Instance.ChuPai(mj);
                            curSelectMJ = null;
                        }
                    }
                    else
                    {
                        //SetCurSelectMJ(null);
                        //if (curSelectMJ != null)
                        //{
                        //    curSelectMJ.TranslatePosition(new Vector3(0, -GameGD.MJ_SELECT_OFFSET, 0));
                        //    curSelectMJ = null;
                        //}
                    }
                }
                else
                {
                    SetCurSelectMJ(mj);
                    //if (curSelectMJ != null)
                    //{
                    //    curSelectMJ.TranslatePosition(new Vector3(0, -GameGD.MJ_SELECT_OFFSET, 0));
                    //}
                    //mj.TranslatePosition(new Vector3(0, GameGD.MJ_SELECT_OFFSET, 0));
                    //curSelectMJ = mj;
                }    
            }                    
        }

        //显示托管
        public void ShowTrusteeship(bool bTrusteed)
        {
            TrusteeshipObj.SetActive(bTrusteed);
        }

        //取消托管
        public void CancelTrusteeship()
        {
            ControlCenter.Instance.SetTrusteeship(false);
        }

        private void InitBtnResult()
        {
            for (int i = 0; i < btn_resultList.Count; i++)
            {
                btn_resultList[i].Init();
                btn_resultList[i].Show(false);
            }
            ResultColliderBgObj = GameUtility.FindDeepChild(gameObject, "ResultColliderBg").gameObject;
            ResultColliderBgObj.SetActive(false);
        }

        public void UpdateResultBtn(List<Result_Struct> data, bool bShowMJ = false)
        {            
            if (data.Count>0)
            {
                HideAllBtnResult();
                Result_Struct rs = new Result_Struct();
                rs.type = ResultType.过;
                data.Add(rs);

                int resultTypeSum = GetResultTypeSum(data);
                int count = 0;
                ResultType preType = ResultType.Max;
                Vector3 startPos = new Vector3(529, -190, 0);
                for (int i = 0; i < data.Count; i++)
                {                    
                    if (data[i].type!=preType)
                    {
                        Vector3 pos = startPos + new Vector3(-150 * (resultTypeSum - count), 0, 0);
                        ShowBtnResult(data[i], pos);
                        count++;
                        preType = data[i].type;
                    }
                }

                if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.自由匹配)
                {
                    CancelInvoke("HideAllBtnResult");
                    Invoke("HideAllBtnResult", 10);
                }
                ResultColliderBgObj.SetActive(true);
            }            
        }

        private int GetResultTypeSum(List<Result_Struct> data)
        {
            int count = 0;
            for (ResultType type = 0; type < ResultType.Max; type++)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if(data[i].type==type)
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        public void ShowResultMJ(List<Result_Struct> data, ResultType type)
        {
            DespawnAllResultMJ();
            int count = 0;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].type==type)
                {
                    for (int j = 0; j < data[i].MJList.Count; j++)
                    {
                        MJ mj = AddMJ(data[i].MJList[j], MJ_STAGE.CPGResult);
                        int index = i;
                        if (type==ResultType.吃)
                        {
                            UIEventListener.Get(mj.gameObject).onClick += delegate
                            {
                                ControlCenter.Instance.Btn_Result_OnClick_Send(type, index);
                                DespawnAllResultMJ();
                            };
                        }
                        else if (type == ResultType.暗杠)
                        {
                            MJType mjType = data[i].MJList[0];
                            UIEventListener.Get(mj.gameObject).onClick += delegate
                            {
                                ControlCenter.Instance.Btn_Result_OnClick_Send(type, -1, mjType);
                                DespawnAllResultMJ();
                            };
                        }
                       
                        Vector3 pos = Vector3.zero;
                        mj.transform.parent = ResultMJMgrTrans;
                        mj.transform.localPosition = new Vector3(count * (GameGD.MJ_BOTTOM_CPG_RESULT_W-2) + i * 20, 0);
                        ResultMJList.Add(mj);
                        count++;
                    }                    
                }
            }            
        }

        private void ShowBtnResult(Result_Struct result, Vector3 pos)
        {
            for (int i = 0; i < btn_resultList.Count; i++)
            {
                if (btn_resultList[i].type == result.type)
                {
                    btn_resultList[i].SetPosition(pos);
                    btn_resultList[i].Show(true);
                    break;
                }
            }
        }

        public override void HideAllBtnResult()
        {
            for (int i = 0; i < btn_resultList.Count; i++)
            {
                btn_resultList[i].Show(false);
            }
            DespawnAllResultMJ();
            ResultColliderBgObj.SetActive(false);
        }

        private void DespawnAllResultMJ()
        {
            while (ResultMJList.Count>0)
            {
                DespawnResultMJ(ResultMJList[0]);
            }
        }

        private void DespawnResultMJ(MJ mj)
        {
            _MJMgr.DespawnMJ(mj.transform);
            ResultMJList.Remove(mj);
            UIEventListener.Get(mj.gameObject).onClick = null;
        }

        public void ShowTingPai(List<MJType> mjList)
        {
            DestroyAllTingPaiMJ();
            if (mjList.Count>0)
            {
                GameUtility.FindDeepChild(TingPaiTrans.gameObject, "胡牌").gameObject.SetActive(true);
                Vector3 pos = Vector3.zero;
                for (int i = 0; i < mjList.Count; i++)
                {
                    MJ mj = AddMJ(mjList[i], MJ_STAGE.CPGResult);
                    mj.transform.parent = TingPaiTrans;
                    mj.transform.localPosition = pos;
                    TingPaiMJList.Add(mj);
                    pos.x += GameGD.MJ_BOTTOM_CPG_RESULT_W;
                }
            }
            else
            {
                GameUtility.FindDeepChild(TingPaiTrans.gameObject, "胡牌").gameObject.SetActive(false);
            }
        }

        private void DestroyAllTingPaiMJ()
        {
            while (TingPaiMJList.Count>0)
            {
                DestroyTingPaiMJ(TingPaiMJList[0]);
            }
        }

        private void DestroyTingPaiMJ(MJ mj)
        {
            _MJMgr.DespawnMJ(mj.transform);
            TingPaiMJList.Remove(mj);
        }

        public override void ReInit()
        {
            base.ReInit();
            ShowTingPai(new List<MJType>());
        }

        public override void ShowZhongYu()
        {
            base.ShowZhongYu();
            HideAllBtnResult();
        }

        //public override void RefleshPlayerInfo(PlayerData data)
        //{

        //}
    }

}
