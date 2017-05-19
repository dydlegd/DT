using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using cn.sharesdk.unity3d;

namespace DYD
{
    public class ControlCenter : SingletonMonoBase<ControlCenter>
    {
        public bool IsCanControl
        {
            get
            {
                return DataCenter.Instance.gamedata.CurChuPaiDir == DataCenter.Instance.gamedata.SelfDNXBDir;
            }
        }
        public bool IsChuPaing { get { return IsCanControl && DataCenter.Instance.gamedata.IsChuPaing && GameApp.Instance.gameState == GAME_STATE.MAIN; } }
        //是否正在游戏中
        public bool IsPlaying 
        { 
            get 
            {
                if (GameApp.Instance.gameState == GAME_STATE.MAIN
                    || GameApp.Instance.gameState == GAME_STATE.ZHONGYU
                    || GameApp.Instance.gameState == GAME_STATE.GAMEOVER
                    )
                {
                    return true;
                }
                return false;
            } 
        }

        public void Init()
        {
            DataCenter.Instance.Init();
            ViewCenter.Instance.Init();           
        }

        private void EventInit()
        {
        }

        public void UpdatePlayerData(PlayerData data)
        {
            DataCenter.Instance.players[data.playerId].SetData(data);
            ReflushPlayerPanel(data.playerId);
        }


        private PanelType GetPlayerPanelType(int playerId)
        {
            PanelType type = PanelType.Player_LEFT;            
            if (GameGD.GetSeatDir(playerId) == SEAT_DIR.DIR_LEFT)
                type = PanelType.Player_LEFT;
            else if (GameGD.GetSeatDir(playerId) == SEAT_DIR.DIR_BOTTOM)
                type = PanelType.Player_BOTTOM;
            else if (GameGD.GetSeatDir(playerId) == SEAT_DIR.DIR_RIGHT)
                type = PanelType.Player_RIGHT;
            else if (GameGD.GetSeatDir(playerId) == SEAT_DIR.DIR_TOP)
                type = PanelType.Player_TOP;
            return type;
        }

        public void ResetPlayerPanelPosition(int playerId)
        {
            GetPlayerPanel(playerId).UpdatePosition();
        }

        public void ReflushPlayerPanel(int playerId)
        {
            GetPlayerPanel(playerId).ReflushPanel(DataCenter.Instance.players[playerId]);            
        }

        public Panel_Player GetPlayerPanel(int playerId)
        {
            return ViewCenter.Instance.GetPanel<Panel_Player>(GetPlayerPanelType(playerId));
        }

        public void ChuPai(int playerId, MJType type)
        {
            AudioManager.Play(Tags.Audio_Name.DaPai);
            if (IsCanControl)
            {
                int index = DataCenter.Instance.players[playerId].MJ_IN_List.IndexOf(type);
                DataCenter.Instance.gamedata.IsChuPaing = false;
                if (index >= 0)
                {
                    Debuger.Log("ControlCenter ChuPai !  " + type + "  " + playerId);
                    PlaySound_MJ(type);
                    DataCenter.Instance.players[playerId].MJ_OUT_List.Add(type);
                    DataCenter.Instance.players[playerId].MJ_IN_List.RemoveAt(index);
                    //DataCenter.Instance.players[playerId].MJ_IN_List.Add((MJType)(RandomMgr.Range((int)MJType.万_1, (int)MJType.MJ_白板)));
                }
                else
                {
                    Debuger.LogError("ControlCenter ChuPai Error !  " + type + "  " + playerId);
                }
            }
            else
            {
                PlaySound_MJ(type);
                DataCenter.Instance.players[playerId].MJ_OUT_List.Add(type);
                DataCenter.Instance.players[playerId].MJ_IN_List.RemoveAt(0);
            }
            DataCenter.Instance.players[playerId].SortMJ();
            ReflushPlayerPanel(playerId);
            ShowCurChuPai(playerId);
            //GetPlayerPanel(playerId).SetCanDragMJ(false);
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).HideAllBtnResult();
            TimeDown.Instance.StopDownTime();
        }

        public void ChuPai(MJ mj)
        {
            if (!IsChuPaing) return;

            MJType type = mj.type;
            int playerId = mj.PlayerID;
            
            if (GD.SIMULATE_CHUPAI)
            {
                ChuPai(playerId, type);
                GameApp.Instance.GoToNextPlayer();
            }

            Debuger.Log("ControlCenter ChuPai !  " + type + "  " + playerId);
            DealCommand.Instance.SendChuPai(type);
        }

        private void ShowCurChuPai(int playerId)
        {
            MJ mj = GetPlayerPanel(playerId).LastOutMJ;
            if (mj != null)
            {
                SetCurChuMJ(mj);
                ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).ShowChuPai(GameGD.GetSeatDir(playerId), mj.type);
                //UIManager.Instance.ShowChuPai(GameGD.GetSeatDir(playerId), mj.type);
            }            
        }

        private void SetCurChuMJ(MJ mj)
        {
            if (mj == null) return;
            
            if (DataCenter.Instance.gamedata.CurChuMJ!=null)
            {
                DataCenter.Instance.gamedata.CurChuMJ.ShowArrow(false);
            }
            DataCenter.Instance.gamedata.CurChuMJ = mj;
            DataCenter.Instance.gamedata.CurChuMJ.ShowArrow(true);
            Debuger.Log("ChuMJ :" + mj.type);
        }

        private int GetResultSum(List<Result_Struct> resultList, ResultType type)
        {
            int count = 0;
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].type == type)
                    count++;
            }
            return count;
        }
        private int FindResultIndex(List<Result_Struct> resultList, ResultType type)
        {
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].type == type)
                    return i;
            }
            return -1;
        }

        public void Btn_Result_OnClick(ResultType type)
        {
            Debuger.Log("Btn_Result :" + type);
            int playerId = DataCenter.Instance.gamedata.PlayerID;
            if (GetResultSum(DataCenter.Instance.players[playerId].resultList,type) >= 2)
            {
                ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowResultMJ(DataCenter.Instance.players[playerId].resultList, type);            
            }
            else
            {
                Btn_Result_OnClick_Send(type);
            }            
        }

        public void Btn_Result_OnClick_Send(ResultType type, int ChiIndex = -1, MJType AnGangMJ = MJType.Null)
        {
            int playerId = DataCenter.Instance.gamedata.PlayerID;
            List<Result_Struct> resultList = DataCenter.Instance.players[playerId].resultList;
            if (type==ResultType.吃)
            {
                if (ChiIndex == -1)
                    ChiIndex = FindResultIndex(resultList, type);
                resultList[ChiIndex].MJList.Remove(DataCenter.Instance.gamedata.CurChuMJ.type);
                DealCommand.Instance.SendCPGH(type, resultList[ChiIndex].MJList, MJType.Null);
                DataCenter.Instance.players[playerId].AddCannotChuMJ(DataCenter.Instance.gamedata.CurChuMJ.type);
                //GetPlayerPanel(DataCenter.Instance.gamedata.PlayerID).SetCannotSelect(DataCenter.Instance.gamedata.CurChuMJ.type);
            }
            else if (type == ResultType.暗杠)
            {
                if (AnGangMJ==MJType.Null)
                {
                    int index = FindResultIndex(resultList, type);
                    AnGangMJ = resultList[index].MJList[0];
                }
                DealCommand.Instance.SendCPGH(type, null, AnGangMJ);
            }
            else
            {
                DealCommand.Instance.SendCPGH(type);
            }
                
            
            TimeDown.Instance.StartDownTime();
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).HideAllBtnResult();
            PlaySound_Result(type,DataCenter.Instance.gamedata.playerInfo.sex);

            if (type != ResultType.过)
            {
                SetCurChuPaiDir(DataCenter.Instance.gamedata.SelfDNXBDir);
                ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowAnim(type);
            }
            else if (type != ResultType.糊 && type != ResultType.过)
            {
                DataCenter.Instance.gamedata.IsChuPaing = false;
            }
            //switch (type)
            //{
            //    case ResultType.吃:
            //        break;
            //    case ResultType.碰:
            //        DataCenter.Instance.players[playerId].MJ_IN_List.Remove(DataCenter.Instance.gamedata.CurChuMJ.type);
            //        DataCenter.Instance.players[playerId].MJ_IN_List.Remove(DataCenter.Instance.gamedata.CurChuMJ.type);
            //        break;
            //    case ResultType.杠:
            //        break;
            //    case ResultType.糊:
            //        break;
            //    case ResultType.过:
            //        break;
            //    case ResultType.自摸:
            //        break;
            //    default:
            //        break;
            //}
            //ReflushPlayerPanel(playerId);
        }

        public void PlaySound_Result(ResultType result, SEX sex)
        {
            switch (result)
            {
                case ResultType.吃:
                    if (sex==SEX.Boy)
                        AudioManager.Play(Tags.Audio_Name.CPG.Boy_chi);
                    else
                        AudioManager.Play(Tags.Audio_Name.CPG.Girl_chi);
                    break;
                case ResultType.碰:
                    if (sex == SEX.Boy)
                        AudioManager.Play(Tags.Audio_Name.CPG.Boy_peng);
                    else
                        AudioManager.Play(Tags.Audio_Name.CPG.Girl_peng);
                    break;
                case ResultType.暗杠:
                case ResultType.杠:
                    if (sex == SEX.Boy)
                        AudioManager.Play(Tags.Audio_Name.CPG.Boy_gang);
                    else
                        AudioManager.Play(Tags.Audio_Name.CPG.Girl_gang);
                    break;
                case ResultType.糊:
                    if (sex == SEX.Boy)
                        AudioManager.Play(Tags.Audio_Name.CPG.Boy_hu);
                    else
                        AudioManager.Play(Tags.Audio_Name.CPG.Girl_hu);
                    break;
                case ResultType.过:                    
                    break;
                case ResultType.自摸:
                    if (sex == SEX.Boy)
                        AudioManager.Play(Tags.Audio_Name.CPG.Boy_zimo);
                    else
                        AudioManager.Play(Tags.Audio_Name.CPG.Girl_zimo);
                    break;
                    break;
                default:
                    break;
            }
        }

        private void PlaySound_MJ(MJType type)
        {
            string audioClipName = "";
            string sex = "";
            if (DataCenter.Instance.gamedata.playerInfo.sex == SEX.Boy)
                sex += "Boy_";
            else
                sex += "Girl_";
            if (type >= MJType.万_1 && type <= MJType.万_9)
                audioClipName += "Wan_" + sex + (int)(type - MJType.万_1 + 1);
            else if (type >= MJType.条_1 && type <= MJType.条_9)
                audioClipName += "Tiao_" + sex + (int)(type - MJType.条_1 + 1);
            else if (type >= MJType.筒_1 && type <= MJType.筒_9)
                audioClipName += "Tong_" + sex + (int)(type - MJType.筒_1 + 1);
            else if (type == MJType.MJ_东)
                audioClipName += "Feng_" + sex+"东";
            else if (type == MJType.MJ_南)
                audioClipName += "Feng_" + sex + "南";
            else if (type == MJType.MJ_西)
                audioClipName += "Feng_" + sex + "西";
            else if (type == MJType.MJ_北)
                audioClipName += "Feng_" + sex + "北";
            else if (type == MJType.MJ_红中)
                audioClipName += "Feng_" + sex + "中";
            else if (type == MJType.MJ_发)
                audioClipName += "Feng_" + sex + "发";
            else if (type == MJType.MJ_白板)
                audioClipName += "Feng_" + sex + "白";

            AudioManager.Play(audioClipName);
        }

        public void SetZhuangDir(DNXB_DIR dir)
        {
            DataCenter.Instance.gamedata.ZhuangDir = dir;
            for (int i = 0; i < 4; i++)
			{
                GetPlayerPanel(i).ShowZhuang(DataCenter.Instance.gamedata.ZhuangPlayerID == i);		 
			}            
        }

        //托管
        public void SetTrusteeship(bool bTrustee)
        {
            DataCenter.Instance.gamedata.IsTrusteeship = bTrustee;
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowTrusteeship(bTrustee);
        }

        public void SetSurplusMJSum(int num)
        {
            GameData gameData = DataCenter.Instance.gamedata;
            DataCenter.Instance.gamedata.SurplusMJSum = num;
            
            if (gameData.gameMode==GAME_MODE.自由匹配)
            {
                ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).SetLabelSurplusMJSum(num);
            }
            else
            {
                SetSurplusMJSum(gameData.CurJuShu, gameData.JuShu, num);
            }
        }
        public void SetSurplusMJSum(int curJuShu, int totalJuShu, int mjSurplusSum)
        {
            DataCenter.Instance.gamedata.CurJuShu = curJuShu;
            DataCenter.Instance.gamedata.JuShu = totalJuShu;
            ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).SetLabelSurplusMJSum(curJuShu, totalJuShu, mjSurplusSum);
        }
        public void SetGameRule(string rule)
        {
            DataCenter.Instance.gamedata.GameRule = rule;
            ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).SetGameRule(rule);
        }

        public void SetCurChuPaiDir(DNXB_DIR dir)
        {
            DataCenter.Instance.gamedata.CurChuPaiDir = dir;
            DataCenter.Instance.gamedata.IsChuPaing = true;
            ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).FlickFangWei(dir);        
        }

        public void CreateRoom(string playRule)
        {
            DealCommand.Instance.SendCreateRoom(1, playRule); 
        }

        public void ShowZhongYu()
        {
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_BOTTOM).ShowZhongYu();
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_RIGHT).ShowZhongYu();
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_TOP).ShowZhongYu();
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_LEFT).ShowZhongYu();
        }

        public void HuiFang()
        {
            HFData hfData = DataCenter.Instance.hFdata;
            if (hfData.CurMoPai._mjType != MJType.Null)
            {
                int index = DataCenter.Instance.players[hfData.CurMoPai._playerId].MJ_IN_List.LastIndexOf(hfData.CurMoPai._mjType);
                DataCenter.Instance.players[hfData.CurMoPai._playerId].MJ_IN_List.RemoveAt(index);
                DataCenter.Instance.players[hfData.CurMoPai._playerId].AddMJ_IN_Data(new List<MJType> { hfData.CurMoPai._mjType }, false);
                ControlCenter.Instance.SetCurChuPaiDir(DataCenter.Instance.players[hfData.CurMoPai._playerId].playerInfo.DNXBDir);
                TimeDown.Instance.StartDownTime();
            } 

            for (int i = 0; i < 4; i++)
            {
                HuiFang(i);
            }

            
            if (hfData.CurChuPai._mjType != MJType.Null)
            {
                AudioManager.Play(Tags.Audio_Name.DaPai);
                PlaySound_MJ(hfData.CurChuPai._mjType);                
                //GetPlayerPanel(playerId).SetCanDragMJ(false);
                ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).HideAllBtnResult();
                TimeDown.Instance.StopDownTime();
            }
            if (hfData.IsCPGIng())
            {
                HideCurShowArrow();
            }
            else
            {
                if (hfData.CurMoPai._mjType != MJType.Null 
                    && DataCenter.Instance.gamedata.CurChuMJ == null
                    )
                {
                    
                }   
                else
                {
                    ShowCurChuPai(hfData.CurChuPai._playerId);
                }
            }
            
        }

        private void HuiFang(int playerId)
        {
            GameData gameData = DataCenter.Instance.gamedata;
            PlayerData playerData = DataCenter.Instance.players[playerId];

            ReflushPlayerPanel(playerId);
            ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).SetLabelSurplusMJSum(gameData.CurJuShu, gameData.JuShu, gameData.SurplusMJSum);
            SetCurChuPaiDir(gameData.CurChuPaiDir);
            UpdatePlayerInfo(playerId);
            SetZhuangDir(gameData.ZhuangDir);
            ControlCenter.Instance.UpdatePlayerInfo();
        }

        public void UpdatePlayerInfo()
        {
            ViewCenter.Instance.GetPanel<Panel_Prepare>(PanelType.Prepare).RefleshPlayerData();
            for (int i = 0; i < 4; i++)
            {
                UpdatePlayerInfo(i);
            }
        }

        private void UpdatePlayerInfo(int playerId)
        {
            GetPlayerPanel(playerId).RefleshPlayerInfo(DataCenter.Instance.players[playerId]);
        }

        public void ShowLT(int playerId, string context)
        {
            GetPlayerPanel(playerId).ShowMsg_Dialogue(context);            
        }

        public void SetTingPai(int playerId, List<MJType> tingPaiList)
        {
            DataCenter.Instance.players[playerId].MJ_TingPai_List = tingPaiList;
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowTingPai(tingPaiList);
        }

        //表情
        public void ShowBQ(int playerId, string face_name)
        {
            GetPlayerPanel(playerId).ShowMsg_Face(face_name);            
        }

        //说话
        public void ShowSH(int playerId, string clipName)
        {
            AudioManager.Play(clipName);
            string[] arrstr = clipName.Split('_');
            if (arrstr.Length>=3)
            {
                GetPlayerPanel(playerId).ShowMsg_Dialogue(arrstr[2]);                
            }
        }

        public void GoToNextPlate()
        {
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_BOTTOM).ReInit();
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_RIGHT).ReInit();
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_TOP).ReInit();
            ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_LEFT).ReInit();
            SetTrusteeship(false);
            HideCurShowArrow();
        }

        public void HideCurShowArrow()
        {
            if (DataCenter.Instance.gamedata.CurChuMJ!=null)
            {
                DataCenter.Instance.gamedata.CurChuMJ.ShowArrow(false);
                DataCenter.Instance.gamedata.CurChuMJ = null;
            }
        }

        public void CaptureShare()
        {
            StartCoroutine(CaptureShareToFirend());
        }

        private IEnumerator CaptureShareToFirend()
        {
            //GDFunc.Instance.CaptureByUnity("share.png");

            Texture2D tex = null;
            yield return StartCoroutine(GDFunc.Instance.CaptureByRect(new Rect(0, 0, Screen.width, Screen.height), tex, Application.persistentDataPath + "/share.png"));
            //GDFunc.Instance.CaptureByRect(new Rect(0, 0, Screen.width, Screen.height), Application.persistentDataPath + "/share.png");

            //yield return null;
            //yield return null;

            ShareContent content = new ShareContent();
            content.SetText(Tags.Title_Name + "，快来和我一起玩游戏吧！");
            //content.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
            //content.SetImageUrl("http://fir.im/1946");
            content.SetTitle(Tags.Title_Name);
            content.SetTitleUrl("http://fir.im/1946");
            content.SetSite("Mob-ShareSDK");
            content.SetSiteUrl("http://fir.im/1946");
            content.SetUrl("http://fir.im/1946");
            content.SetComment("描述");
            //content.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
            content.SetShareType(ContentType.Image);
            content.SetImagePath(Application.persistentDataPath + "/share.png");
            WeiXinCenter.Instance.ShareWeChatFriend(content);
        }
    }
}


