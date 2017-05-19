using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class DealCommand : SingletonMonoBase<DealCommand>
    {
        NetworkClient network = new NetworkClient();

        private static byte[] mCmdBuf = new byte[256];

        private static List<string> mCmdStrList = new List<string>();

        void Start()
        {
            Init();
        }

        public bool IsConnected()
        {
            return network.status == NetworkBase.ConnectState.Connected;
        }

        public void Init()
        {
            network.Init();
        }

        public void ReConnect()
        {
            network.Connect();
        }

        // Update is called once per frame
        void Update()
        {
            //有可能一帧内收到很多条命令，但是也有可能有戏进入后台，退出来后，积压命令太多
            //所以一帧最多处理20条命令，防止主线程出现假死状态
            for (int i = 0; i < 20; i++)
            {
                if (GetCmdType() == "") break;
            }

            //NetHeartBeat.Instance.Update();
            network.UpdateLogic();
        }


        public void Disconnect()
        {
            network.Disconnect();
        }

        public string GetCmdType()
        {
            mCmdStrList.Clear();
            NetDataBuf dataBuf = null;
            string type = network.GetCommand(ref mCmdStrList, ref dataBuf);
            if(type!="")
            {
                Debuger.Log("GetCommand :" + type);
                switch (type)
                {
                    case "VER":
                        break;
                    case "VERED":
                        break;
                    case "USER":
                        ParseUser(dataBuf);
                        SendME();
                        if (DataCenter.Instance.gamedata.playerInfo.WXTX_Texture.name == GD.HEAD_ICON_NAME) DealCommand.Instance.SendWXTX();
                        break;
                    case "USERED":
                        ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sEnterGameFail, "", null);
                        break;
                    case "USERZX":
                        ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sOnline, "", null);
                        break;
                    case "WXTXED":
                        ParseSelfWXTX(dataBuf);
                        break;
                    case "JRFJ":
                        ParseJoinRoomSucceed(dataBuf);
                        break;
                    case "JRFJED":
                        ParseJoinRoomFailed(dataBuf);
                        break;
                    case "WXTX":
                        ParseWXTX(dataBuf);
                        break;
                    case "CJZZED"://创建房间
                        ParseCreateRoom(dataBuf);
                        break;
                    case "CHUPAI":
                        ParseChuPai(dataBuf);
                        break;
                    case "PAI":
                        ParsePai(dataBuf);
                        break;
                    case "KAIJU":
                        ParseKaiJu(dataBuf);
                        break;
                    case "MOPAI":
                        ParseMoPai(dataBuf);
                        break;
                    case "DPAI":
                        ParseDaPai(dataBuf);
                        break;
                    case "HUPAI":
                        ParseHuPai(dataBuf);
                        break;
                    case "JG":
                        ParseJG(dataBuf);
                        break;
                    case "ANGANG":
                        ParseANGANG(dataBuf);
                        break;
                    case "GANG":
                        ParseGANG(dataBuf);
                        break;
                    case "FW":
                        ParseFangWei(dataBuf);
                        break;
                    case "SY":
                        ParseSY(dataBuf);
                        break;
                    case "ZM":
                        ParseZM(dataBuf);
                        break;
                    case "GETZJ1":
                        ParseGetZJ(dataBuf,true);
                        break;
                    case "GETZJ2":
                        ParseGetZJ(dataBuf, false);
                        break;
                    case "YUYING":
                        ParseYuYing(dataBuf);
                        break;
                    case "HF":
                        ParseHF(dataBuf);
                        break;
                    case "LT":
                        ParseLT(dataBuf);
                        break;
                    case "BQ":
                        ParseBQ(dataBuf);
                        break;
                    case "SH":
                        ParseSH(dataBuf);
                        break;
                    case "KSTP":
                        ParseLeaveRoomTP(dataBuf);
                        break;
                    case "LKFJ":
                        ParseLeaveRoom(dataBuf);
                        break;
                    case "ZJ":
                        ParseRecord(dataBuf);
                        break;
                    case "WJXX":
                        ParsePlayerInfo(dataBuf);
                        break;
                    case "ZBCJ":
                        ParseGameRule(dataBuf);
                        break;
                    case "SYXS":
                        ParseSYXS(dataBuf);
                        break;
                    case "WFXS":
                        ParseWFXS(dataBuf);
                        break;
                    case "JSFJ":
                        ParseJSFJ(dataBuf);
                        break;
                    case "WJFS":
                        ParseWJFS(dataBuf);
                        break;
                    case "DDGZ":
                        ParseDDGZ(dataBuf);
                        break;
                    case "FHDT":
                        ParseFHDT(dataBuf);
                        break;
                    case "HFED":
                        ParseHFED(dataBuf);
                        break;
                    case "WJLK":
                        ParseWJLK(dataBuf);
                        break;
                    case "HEART":
                        NetHeartBeat.Instance.HeartSucceed();
                        break;
                    default: break;
                }                
            }
            return type;
        }

        /// <summary>
        /// 协议中数字对应的麻将
        /// </summary>
        /// <param name="netData"></param>
        /// <returns></returns>
        private MJType ValueToMJType(int val)
        {
            return (MJType)(val);
        }
        private int MJTypeToValue(MJType type)
        {
            return (int)type;
        }
        private string MJValueToString(int value)
        {
            string text = string.Format("{0:D2}", value);
            return text;
        }

        public void SendVersion(int iClientVersion)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendVersion(playerInfo.MachineNum, iClientVersion);
        }
        private void SendVersion(int iMachine, int iClientVersion)
        {
            network.SendVersion(iMachine, iClientVersion);
        }
        public void SendLogin()
        {
            if (DataCenter.Instance.gamedata.IsHaveLogin)
            {
                Debuger.Log("您已经登录过了！----" + DataCenter.Instance.gamedata.playerInfo.WXName);
                return;
            }
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendLogin(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, playerInfo.WXName);
        }
        public void SendLogin(string wxName)
        {
            if (DataCenter.Instance.gamedata.IsHaveLogin)
            {
                Debuger.Log("您已经登录过了！----"+DataCenter.Instance.gamedata.playerInfo.WXName);
                return;
            }
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendLogin(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, wxName);
        }
        private void SendLogin(int iMachine, string password, string wxId, string wxName)
        {
            network.SendLogin(iMachine, password, wxId, wxName);
        }
        public void SendWXTX()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendWXTX(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, playerInfo.WXTX_Datas.Length, playerInfo.WXTX_Datas);
        }

        private void SendWXTX(int dataTotalLen, byte[] dataBuf)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendWXTX(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, dataTotalLen, dataBuf);
        }
        private void SendWXTX(int iMachine, string password, string wxId, int dataTotalLen, byte[] dataBuf)
        {
            network.SendWXTX(iMachine, password, wxId, dataTotalLen, dataBuf);
        }
        public void SendGetPai(string randPassword)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendGetPai(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, randPassword);
        }
        private void SendGetPai(int iMachine, string password, string wxId, string randPassword)
        {
            network.SendGetPai(iMachine, password, wxId, randPassword, DataCenter.Instance.gamedata.RoomNum);
        }
        public void SendJoinRoom_自由匹配()
        {
            DataCenter.Instance.GoToNextPlate(true);
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            DataCenter.Instance.gamedata.gameMode = GAME_MODE.自由匹配;
            SendJoinRoom_自由匹配(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID);
        }
        private void SendJoinRoom_自由匹配(int iMachine, string password, string wxId)
        {
            network.SendJoinRoom_自由匹配(iMachine, password, wxId);
        }
        public void SendJoinRoom(int roomId)
        {
            DataCenter.Instance.GoToNextPlate(true);
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            DataCenter.Instance.gamedata.gameMode = GAME_MODE.创建房间;
            SendJoinRoom(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, roomId);
        }
        private void SendJoinRoom(int iMachine, string password, string wxId, int roomId)
        {
            network.SendJoinRoom(iMachine, password, wxId, roomId);
        }
        public void SendCreateRoom(int juShu, string playRule)
        {
            DataCenter.Instance.GoToNextPlate(true);
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            AREA_ENUM area = DataCenter.Instance.gamedata.GameArea;
            SendCreateRoom(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, juShu, playRule, area);
        }
        private void SendCreateRoom(int iMachine, string password, string wxId, int juShu, string playRule, AREA_ENUM area)
        {
            string strArea = "" + (int)area;
            network.SendCreateRoom(iMachine, password, wxId, juShu, playRule, strArea);
        }

        public void SendChuPai(MJType type)
        {
            int playerId = DataCenter.Instance.gamedata.PlayerID;
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendChuPai(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, playerId, type);
        }
        private void SendChuPai(int iMachine, string password, string wxId, int playerId, MJType type)
        {
            string weizhi = "" + DirToValue(DataCenter.Instance.gamedata.SelfDNXBDir);
            string sMJ = string.Format("{0:D2}", MJTypeToValue(type));
            network.SendChuPai(iMachine, password, wxId, weizhi, sMJ);
        }

        public void SendHeart()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendHeart(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID);
        }
        private void SendHeart(int iMachine, string password, string wxId)
        {
            network.SendHeart(iMachine, password, wxId);
        }

        public void SendCPGH(ResultType type, List<MJType> ChiMJList=null, MJType AnGangMJ=MJType.Null)
        {
            int playerId = DataCenter.Instance.gamedata.PlayerID;
            string strChiMJ = "";
            string strAnGang = "";

            int data = 0;
            switch (type)
            {
                case ResultType.吃:
                    data = 1;
                    for (int i = 0; i < ChiMJList.Count; i++)
                    {
                        strChiMJ += MJValueToString(MJTypeToValue(ChiMJList[i]));
                    }
                    break;
                case ResultType.碰:
                    data = 2;
                    break;
                case ResultType.杠:
                    data = 3;
                    break;
                case ResultType.暗杠:
                    data = 4;
                    strAnGang = MJValueToString(MJTypeToValue(AnGangMJ));
                    break;
                case ResultType.糊:
                    data = 5;
                    break;
                case ResultType.过:
                    data = 0;
                    break;
                case ResultType.自摸:
                    data = 6;
                    break;
                default:
                    break;
            }
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            
            SendCPGH(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, playerId, data, strChiMJ, strAnGang);
        }
        private void SendCPGH(int iMachine, string password, string wxId, int playerId, int data, string ChiPai, string AnGangPai)
        {
            string weizhi = "" + DirToValue(DataCenter.Instance.gamedata.SelfDNXBDir);
            network.SendCPGH(iMachine, password, wxId, weizhi, data, ChiPai, AnGangPai);
        }

        //准备
        public void SendPrepare()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendPrepare(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID);
        }
        private void SendPrepare(int iMachine, string password, string wxId)
        {
            network.SendPrepare(iMachine, password, wxId);
        }
        //离开房间
        public void SendLeaveRoom()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendLeaveRoom(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID);
        }
        private void SendLeaveRoom(int iMachine, string password, string wxId)
        {
            network.SendLKFJ(iMachine, password, wxId);
        }
        //解散房间
        public void SendDissolveRoom()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendDissolveRoom(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID);
        }
        private void SendDissolveRoom(int iMachine, string password, string wxId)
        {
            network.SendJSFJ(iMachine, password, wxId);
        }
        //其他玩家离开房间投票
        public void SendLeaveRoomTP(bool bAgree)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            int iAgree = bAgree ? 1 : 2;
            SendLeaveRoomTP(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, iAgree);
        }
        private void SendLeaveRoomTP(int iMachine, string password, string wxId, int iAgree)
        {
            network.SendLeaveRoomTP(iMachine, password, wxId, iAgree);
        }
        //聊天
        public void SendLT(string context)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendLT(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, context);
        }
        private void SendLT(int iMachine, string password, string wxId, string context)
        {
            network.SendLT(iMachine, password, wxId, context);
        }
        //获取前20场战绩
        public void SendGetZJ()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendGetZJ(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID);
        }
        private void SendGetZJ(int iMachine, string password, string wxId)
        {
            network.SendGetZJ(iMachine, password, wxId);
        }
        //语音聊天
        public void SendYuYing(int dataTotalLen, byte[] dataBuf)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendYuYing(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, dataTotalLen, dataBuf);
        }
        private void SendYuYing(int iMachine, string password, string wxId, int dataTotalLen, byte[] dataBuf)
        {
            network.SendYuYing(iMachine, password, wxId, dataTotalLen, dataBuf);
        }

        //表情
        public void SendBQ(string face_name)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendBQ(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, face_name);
        }
        private void SendBQ(int iMachine, string password, string wxId, string face_name)
        {
            network.SendBQ(iMachine, password, wxId, face_name);
        }

        //说话
        public void SendSH(string clipName)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            SendSH(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, clipName);
        }
        private void SendSH(int iMachine, string password, string wxId, string clipName)
        {
            network.SendSH(iMachine, password, wxId, clipName);
        }
        //获取玩法规则
        public void SendZBCJ()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            SendZBCJ(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, gameData.GameArea);
        }
        private void SendZBCJ(int iMachine, string password, string wxId, AREA_ENUM area)
        {
            string strArea = "" + (int)area;
            network.SendZBCJ(iMachine, password, wxId, strArea);
        }
        //回放开始
        public void SendHFKS(HistroyRecordData hr)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            network.SendHFKS(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, hr.RoomID);         
        }
        //回放暂停
        public void SendHFZT(HistroyRecordData hr)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            network.SendHFZT(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, hr.RoomID);
        }
        //回放恢复
        public void SendHFHF(HistroyRecordData hr)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            network.SendHFHF(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, hr.RoomID);
        }
        //回放上一局
        public void SendHFDEC(HistroyRecordData hr)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            network.SendHFDEC(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, hr.RoomID);
        }
        //回放下一局
        public void SendHFINC(HistroyRecordData hr)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            network.SendHFINC(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, hr.RoomID);
        }
        //回放离开
        public void SendHFLK(HistroyRecordData hr)
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            network.SendHFLK(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, hr.RoomID);
        }
        //
        public void SendME()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            network.SendME(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID);
        }
        //获取头像
        public void SendGETTX()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            GameData gameData = DataCenter.Instance.gamedata;
            string dataStr = "";
            for (int i = 0; i <GD.PLAYER_SUM; i++)
            {
                //dataStr += "" + (i+1) + "/";
                if (i==gameData.PlayerID)
                {
                    dataStr += "1";                    
                }
                else
                {
                    dataStr += ((DataCenter.Instance.players[i].playerInfo.WXTX_Icon_SpriteName != "") ? "1" : "0");
                }
                if (i != GD.PLAYER_SUM - 1) dataStr += "/";
            }            
            
            network.SendGETTX(playerInfo.MachineNum, playerInfo.Password, playerInfo.WXID, dataStr);            
        }
        #region 数据解析
        //用户信息
        private void ParseUser(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
        
            int strIndex = 2;
            DataCenter.Instance.gamedata.playerInfo.GameID = arrstr[strIndex++];
            DataCenter.Instance.gamedata.playerInfo.coin = Int32.Parse(arrstr[strIndex++]);
            DataCenter.Instance.gamedata.playerInfo.diamond = Int32.Parse(arrstr[strIndex++]);
            GameApp.Instance.SetGameState(GAME_STATE.HALL);            
        }
        //自己微信头像
        private void ParseSelfWXTX(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            if (Int32.Parse(arrstr[strIndex]) == 1)
            {
                //服务器接收成功
                Debuger.Log("服务器头像接收成功");
            }
            else if (Int32.Parse(arrstr[strIndex]) == 0)
            {
                //服务器接收失败，重发
                Debuger.Log("服务器头像接收失败，重新发送");
                SendWXTX();
            }
        }
        //杠
        private void ParseGANG(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = DataCenter.Instance.gamedata.PlayerID;
            DataCenter.Instance.players[playerId].resultList.Clear();
            Result_Struct rs = new Result_Struct();
            rs.type = ResultType.杠;
            MJType mjType = ValueToMJType(Int32.Parse(arrstr[2]));
            rs.MJList.Add(mjType);
            rs.MJList.Add(mjType);
            rs.MJList.Add(mjType);
            rs.MJList.Add(mjType);
            DataCenter.Instance.players[playerId].resultList.Add(rs);
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).UpdateResultBtn(DataCenter.Instance.players[playerId].resultList);
        }
        //暗杠
        private void ParseANGANG(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            
            //if (ValueToDir(Int32.Parse(arrstr[2]))==SEAT_DIR.DIR_BOTTOM)
            {
                int playerId = DataCenter.Instance.gamedata.PlayerID;
                DataCenter.Instance.players[playerId].resultList.Clear();
                for (int i = 0; i < arrstr[3].Length; i += 2)
                {
                    Result_Struct rs = new Result_Struct();
                    rs.type = ResultType.暗杠;
                    MJType mjType = ValueToMJType(Int32.Parse(arrstr[3].Substring(i, 2)));
                    rs.MJList.Add(mjType);
                    rs.MJList.Add(mjType);
                    rs.MJList.Add(mjType);
                    rs.MJList.Add(mjType);
                    DataCenter.Instance.players[playerId].resultList.Add(rs);
                }
                ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).UpdateResultBtn(DataCenter.Instance.players[playerId].resultList);
            }
        }
        private void ParsePai(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            PlayerData[] players = new PlayerData[4];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new PlayerData(i);
            }
            //------手中牌----------//
            int strIndex = 2;
            for (int i = 0; i < 4; i++,strIndex++)
            {
                players[i].AddMJ_IN_Data(ParsePlayerMJData(arrstr[strIndex]));
            }
            //------吃的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") players[i].AddMJ_CPG_Data(ParsePlayerCMJData(arrstr[strIndex], i));
            }
            //------碰的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") players[i].AddMJ_CPG_Data(ParsePlayerPMJData(arrstr[strIndex], i));
            }
            //------杠的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") players[i].AddMJ_CPG_Data(ParsePlayerGMJData(arrstr[strIndex], i));
            }
            //------打的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") players[i].MJ_OUT_List = ParsePlayerMJData(arrstr[strIndex]);
            }
            //------摸的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex]!="")
                {
                    players[i].MoPai(ParsePlayerMJData(arrstr[strIndex])[0]);
                }                
            }
            //------暗杠----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") players[i].AddMJ_CPG_Data(ParsePlayerANGMJData(arrstr[strIndex]));
            }

            ControlCenter.Instance.SetSurplusMJSum(Int32.Parse(arrstr[strIndex]));
            for (int i = 0; i < 4; i++)
            {
                ControlCenter.Instance.UpdatePlayerData(players[i]);
            }

            Debuger.Log("SurplusMJSum: " + DataCenter.Instance.gamedata.SurplusMJSum);
        }

        private List<MJType> ParsePlayerMJData(string context)
        {
            List<MJType> list = new List<MJType>();
            int index = 0;

            for (int i = 0; i < context.Length; i += 2, index += 2)
            {
                string str = context.Substring(index, 2);
                list.Add(ValueToMJType(Int32.Parse(str)));
            }           

            return list;
        }

        //吃的牌
        private List<CPG_Struct> ParsePlayerCMJData(string context, int playerId)
        {
            string[] arrstr = context.Split('-');
            List<CPG_Struct> CPGList = new List<CPG_Struct>();

            for (int i = 0; i < arrstr.Length; i++)
            {
                List<MJType> MJList = ParsePlayerMJData(arrstr[i]);
                CPG_Struct cs = new CPG_Struct();
                cs.Init(CPG_TYPE.吃, new List<MJType> { MJList[0], MJList[1], MJList[2] }, SEAT_DIR.DIR_NULL);
                CPGList.Add(cs);
            }
            
            return CPGList;
        }
        //碰的牌
        private List<CPG_Struct> ParsePlayerPMJData(string context, int playerId)
        {
            string[] arrstr = context.Split('-');
            List<CPG_Struct> CPGList = new List<CPG_Struct>();

            for (int i = 0; i < arrstr.Length; i++)
			{
                List<MJType> MJList = ParsePlayerMJData(arrstr[i]);            
                CPG_Struct cs = new CPG_Struct();
                DNXB_DIR dnxbDir = ValueToDNXBDir(Int32.Parse(arrstr[i].Substring(6, 2)));
                SEAT_DIR dir = GameGD.GetSeatDir(dnxbDir, DataCenter.Instance.players[playerId].playerInfo.DNXBDir);
                cs.Init(CPG_TYPE.碰, new List<MJType> { MJList[0], MJList[1], MJList[ 2] }, dir);
                CPGList.Add(cs);
			}
            
            return CPGList;
        }
        //杠的牌
        private List<CPG_Struct> ParsePlayerGMJData(string context, int playerId)
        {
            string[] arrstr = context.Split('-');
            List<CPG_Struct> CPGList = new List<CPG_Struct>();

            for (int i = 0; i < arrstr.Length; i++)
            {
                List<MJType> MJList = ParsePlayerMJData(arrstr[i]);
                CPG_Struct cs = new CPG_Struct();
                DNXB_DIR dnxbDir = ValueToDNXBDir(Int32.Parse(arrstr[i].Substring(8, 2)));
                SEAT_DIR dir = GameGD.GetSeatDir(dnxbDir, DataCenter.Instance.players[playerId].playerInfo.DNXBDir);
                cs.Init(CPG_TYPE.杠, new List<MJType> { MJList[0], MJList[1], MJList[2], MJList[3] }, dir);
                CPGList.Add(cs);
            }
            
            return CPGList;
        }
        //暗杠的牌
        private List<CPG_Struct> ParsePlayerANGMJData(string context)
        {
            List<MJType> MJList = ParsePlayerMJData(context);
            List<CPG_Struct> CPGList = new List<CPG_Struct>();
            for (int i = 0; i < MJList.Count; i++)
            {
                CPG_Struct cs = new CPG_Struct();
                cs.Init(CPG_TYPE.暗杠, new List<MJType> { MJList[i], MJList[i], MJList[i], MJList[i] }, SEAT_DIR.DIR_NULL);
                CPGList.Add(cs);
            }
            return CPGList;
        }

        //微信头像
        private void ParseWXTX(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[2]));
            PlayerInfo playerInfo = DataCenter.Instance.players[playerId].playerInfo;

            playerInfo.WXTX_Datas = new byte[dataBuf.FullDataLen];
            Array.Copy(dataBuf.byteBuf, dataBuf.DataLen - dataBuf.FullDataLen, playerInfo.WXTX_Datas, 0, dataBuf.FullDataLen);
            playerInfo.WXTX_Texture.LoadImage(playerInfo.WXTX_Datas);
            playerInfo.WXTX_Texture.name = "Head_Icon_" + playerId;
            playerInfo.WXTX_Icon_SpriteName = "Head_Icon_" + playerId;
            DynamicAtlas.Instance.AddTexture(playerInfo.WXTX_Texture, ControlCenter.Instance.UpdatePlayerInfo);
            //ControlCenter.Instance.UpdatePlayerInfo();
            //NotificationCenter.Instance.PostNotificationEvent(NotificateMsgType.WXTX,new Notification(playerId));
            WXTXCenter.Instance.ReStartTimer();
        }

        //加入房间成功
        private void ParseJoinRoomSucceed(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int index = 2;
            //房间号
            if (arrstr[index] != "") DataCenter.Instance.gamedata.RoomNum = Int32.Parse(arrstr[index]);
            index++;
            //局数
            if (arrstr[index] != "")
            {
                int totalJuShu = Int32.Parse(arrstr[index]);
                ControlCenter.Instance.SetSurplusMJSum(1, totalJuShu, 83);
            }
            index++;
            //玩法
            ControlCenter.Instance.SetGameRule(arrstr[index++]);
            //位置
            DataCenter.Instance.gamedata.SelfDNXBDir = ValueToDNXBDir(Int32.Parse(arrstr[index++]));
            //最大倒计时
            DataCenter.Instance.gamedata.MaxDownTime = Int32.Parse(arrstr[index++]);
            //随机密码
            DataCenter.Instance.gamedata.RandPassword = arrstr[index++];
            //房主
            DataCenter.Instance.gamedata.FangZhuDNXBDir = ValueToDNXBDir(Int32.Parse(arrstr[index]));
            //DataCenter.Instance.players[DataCenter.Instance.gamedata.PlayerID].playerInfo = DataCenter.Instance.gamedata.playerInfo;
            //NotificationCenter.Instance.PostNotificationEvent(NotificateMsgType.WXTX, new Notification(DataCenter.Instance.gamedata.PlayerID));
            GameApp.Instance.SetGameState(GAME_STATE.PREPARE);
            ControlCenter.Instance.UpdatePlayerInfo();
        }
        //加入房间失败
        private void ParseJoinRoomFailed(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            if (Int32.Parse(arrstr[2]) == 0)
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sJoinRoomFail_NoFind, "", null);
                Debuger.Log("没有找到该房间");
            }
            else if (Int32.Parse(arrstr[2]) == 1)
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sJoinRoomFail_RoomFullPeople, "", null);
                Debuger.Log("房间已满员，加入失败");
            }
            else if (Int32.Parse(arrstr[2]) == 2)
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sJoinRoomFail_FullPeople, "", null);
                Debuger.Log("服务器已满员，加入失败");
            }
            else if (Int32.Parse(arrstr[2]) == 3)
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sJoinRoomFail_CoinLess, "", null);
                Debuger.Log("金币不足，加入失败");
            }
        }

        //吃碰杠糊
        private void ParseResult_CPGH(ResultType type, string context,ref List<Result_Struct> resultList, int playerId)
        {
            if (context!="")
            {
                if (type==ResultType.吃)
                {
                    string[] arrstr = context.ToString().Split('-');
                    for (int i = 0; i < arrstr.Length; i++)
                    {
                        Result_Struct rs = new Result_Struct();
                        rs.type = type;
                        rs.MJList = ParsePlayerMJData(arrstr[i]);
                        rs.MJList.Add(DataCenter.Instance.gamedata.CurChuMJ.type);
                        rs.MJList.Sort();
                        resultList.Add(rs);
                    }  
                }
                else if (type==ResultType.碰||type==ResultType.杠)
                {
                    Result_Struct rs = new Result_Struct();
                    rs.type = type;
                    rs.MJList = ParsePlayerMJData(context.Substring(0, 2));
                    //rs.dir = ValueToDir(Int32.Parse(context.Substring(2, 2)));

                    DNXB_DIR dnxbDir = ValueToDNXBDir(Int32.Parse(context.Substring(2, 2)));
                    SEAT_DIR dir = GameGD.GetSeatDir(dnxbDir, playerId);
                    rs.dir = dir;

                    resultList.Add(rs);
                }
                else if (type == ResultType.糊)
                {
                    string[] arrstr = context.ToString().Split('-');
                    Result_Struct rs = new Result_Hu_Struct();
                    rs.type = type;
                    resultList.Add(rs);
                }
            }
        }

        private void ParseChuPai(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            
            //------吃牌----------//            
            int strIndex = 2;
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].resultList.Clear();
                if (arrstr[strIndex] != "" && DataCenter.Instance.gamedata.PlayerID == i)
                {
                    ParseResult_CPGH(ResultType.吃, arrstr[strIndex], ref  DataCenter.Instance.players[i].resultList, i);
                }
            }

            //------碰牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "" && DataCenter.Instance.gamedata.PlayerID == i)
                {
                    ParseResult_CPGH(ResultType.碰, arrstr[strIndex], ref  DataCenter.Instance.players[i].resultList, i);
                }
            }

            //------杠牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "" && DataCenter.Instance.gamedata.PlayerID == i)
                {
                    ParseResult_CPGH(ResultType.杠, arrstr[strIndex], ref  DataCenter.Instance.players[i].resultList, i);
                }
            }

            //------糊牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "")
                {
                    ParseResult_CPGH(ResultType.糊, arrstr[strIndex], ref  DataCenter.Instance.players[i].resultList, i);
                }
            }

            int playerId = DataCenter.Instance.gamedata.PlayerID;

            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).UpdateResultBtn(DataCenter.Instance.players[playerId].resultList);
        }

        //创建房间
        private void ParseCreateRoom(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int result = Int32.Parse(arrstr[2]);            
            if (result==0)//成功
            {
                DataCenter.Instance.gamedata.RoomNum = Int32.Parse(arrstr[3]);
                Debuger.Log("创建房间成功！ID: " + DataCenter.Instance.gamedata.RoomNum);


                //ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sCreateRoomSucceed + "\nID: " + DataCenter.Instance.gamedata.RoomNum, "", null);
                ViewCenter.Instance.HidePanel<UIPanelBase>(PanelType.CreateRoom);
                DealCommand.Instance.SendJoinRoom(DataCenter.Instance.gamedata.RoomNum);
            }
            else if (result == 1)//失败，玩家砖石不够
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sCreateRoomFail_LessDiamond, "", null);
                Debuger.Log("创建房间失败！玩家砖石不够");
            }
            else if (result == 2)//失败，房间已满
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sCreateRoomFail_FullRoom, "", null);
                Debuger.Log("创建房间失败！房间已满");
            }
        }
        //开局
        private void ParseKaiJu(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int strIndex = 2;
            DataCenter.Instance.gamedata.Dice_Num_1 = Int32.Parse(arrstr[strIndex++]);
            DataCenter.Instance.gamedata.Dice_Num_2 = Int32.Parse(arrstr[strIndex++]);
            ControlCenter.Instance.SetZhuangDir(ValueToDNXBDir(Int32.Parse(arrstr[strIndex++])));
            DataCenter.Instance.gamedata.CurJuShu = Int32.Parse(arrstr[strIndex++]);
            DataCenter.Instance.gamedata.gameMode = ValueToGameMode(Int32.Parse(arrstr[strIndex++]));
            Debuger.Log("Dice_1: "+Int32.Parse(arrstr[2])+"  Dice_2: "+Int32.Parse(arrstr[3])+"  庄: "+DataCenter.Instance.gamedata.ZhuangDir);
            GameApp.Instance.SetGameState(GAME_STATE.MAIN);
        }
        //摸牌
        private void ParseMoPai(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[2]));
            DataCenter.Instance.players[playerId].MoPai(ParsePlayerMJData(arrstr[3])[0]);
            ControlCenter.Instance.SetSurplusMJSum(Int32.Parse(arrstr[4]));            
            ControlCenter.Instance.ReflushPlayerPanel(playerId);
            TimeDown.Instance.StartDownTime();
        }
        //打牌
        private void ParseDaPai(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[2]));
            MJType mjType = ValueToMJType(Int32.Parse(arrstr[3]));
            ControlCenter.Instance.SetTingPai(playerId, ParsePlayerMJData(arrstr[4]));                
            ControlCenter.Instance.ChuPai(playerId, mjType);            
        }
        //胡牌
        private void ParseHuPai(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int strIndex = 2;
            //东南西北胡牌
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOver.huTypeList.Clear();
                string huContext = arrstr[strIndex];
                for (int j = 0; j < huContext.Length; j+=2)
                {
                    HuType huPaiType = (HuType)Int32.Parse(huContext.Substring(j, 2));                    
                    DataCenter.Instance.players[i].gameOver.huTypeList.Add(huPaiType);  
                }                           
            }

            //放炮方位
            if (arrstr[strIndex] != "")
            {
                for (int i = 0; i < 4; i++)
                {
                    DataCenter.Instance.players[i].gameOver.fangPaoDir = ValueToDNXBDir(Int32.Parse(arrstr[strIndex]));
                }                
            }
            strIndex++;

            //摸鱼
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOver.ZhongYuList = ParsePlayerMJData(arrstr[strIndex]);
            }

            //中鱼
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOver.ZhongYuOKList = ParsePlayerMJData(arrstr[strIndex]);
            }

            GameApp.Instance.SetGameState(GAME_STATE.ZHONGYU);
        }

        //结果
        private void ParseJG(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
        
            int strIndex = 2;

            //------赢输分----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOver.DeFen = Int32.Parse(arrstr[strIndex]);
            }

            //------总分----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].playerInfo.coin = Int32.Parse(arrstr[strIndex]);
            }

            //------手中的牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].MJ_IN_List = ParsePlayerMJData(arrstr[strIndex]);
            }


            //------吃的牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].MJ_CPG_List.Clear();

                 if (arrstr[strIndex]!="")
                 {
                     string[] chiArrstr = arrstr[strIndex].Split('-');
                     for (int j = 0; j < chiArrstr.Length; j++)
                     {
                         DataCenter.Instance.players[i].AddMJ_CPG_Data(ParsePlayerCMJData(chiArrstr[j], i));
                     }     
                 }                           
            }

            //------碰的牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex]!="")
                {
                    string[] pengArrstr = arrstr[strIndex].Split('-');
                    for (int j = 0; j < pengArrstr.Length; j++)
                    {
                        MJType mjType = ValueToMJType(Int32.Parse(pengArrstr[j].Substring(0, 2)));
                        //SEAT_DIR dir = ValueToDir(Int32.Parse(pengArrstr[j].Substring(2, 2)));

                        DNXB_DIR dnxbDir = ValueToDNXBDir(Int32.Parse(pengArrstr[j].Substring(2, 2)));
                        SEAT_DIR dir = GameGD.GetSeatDir(dnxbDir, i);

                        CPG_Struct cs = new CPG_Struct();
                        cs.Init(CPG_TYPE.碰, new List<MJType> { mjType, mjType, mjType }, dir);
                        DataCenter.Instance.players[i].MJ_CPG_List.Add(cs);
                    }
                }                
            }

            //------杠的牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex]!="")
                {
                    string[] gangArrstr = arrstr[strIndex].Split('-');
                    for (int j = 0; j < gangArrstr.Length; j++)
                    {
                        MJType mjType = ValueToMJType(Int32.Parse(gangArrstr[j].Substring(0, 2)));
                        //SEAT_DIR dir = ValueToDir(Int32.Parse(gangArrstr[j].Substring(2, 2)));

                        DNXB_DIR dnxbDir = ValueToDNXBDir(Int32.Parse(gangArrstr[j].Substring(2, 2)));
                        SEAT_DIR dir = GameGD.GetSeatDir(dnxbDir, i);

                        CPG_Struct cs = new CPG_Struct();
                        cs.Init(CPG_TYPE.杠, new List<MJType> { mjType, mjType, mjType, mjType }, dir);
                        DataCenter.Instance.players[i].MJ_CPG_List.Add(cs);
                    }
                }                
            }

            //------暗杠的牌----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "")
                {
                    string[] gangArrstr = arrstr[strIndex].Split('-');
                    for (int j = 0; j < gangArrstr.Length; j++)
                    {
                        MJType mjType = ValueToMJType(Int32.Parse(gangArrstr[j].Substring(0, 2)));
                        //SEAT_DIR dir = ValueToDir(Int32.Parse(gangArrstr[j].Substring(2, 2)));

                        DNXB_DIR dnxbDir = ValueToDNXBDir(Int32.Parse(gangArrstr[j].Substring(2, 2)));
                        SEAT_DIR dir = GameGD.GetSeatDir(dnxbDir, i);
                        if (dnxbDir == DNXB_DIR.Null) dir = SEAT_DIR.DIR_NULL;

                        CPG_Struct cs = new CPG_Struct();
                        cs.Init(CPG_TYPE.暗杠, new List<MJType> { mjType, mjType, mjType, mjType }, dir);
                        DataCenter.Instance.players[i].MJ_CPG_List.Add(cs);
                    }
                }
            }

            //------胡的牌----------//            
            for (int i = 0; i < 4; i++)
            {
                if (arrstr[strIndex]!="")
                {
                    DataCenter.Instance.players[i].gameOver.huPaiMJType = ValueToMJType(Int32.Parse(arrstr[strIndex]));                    
                }
            }
            strIndex++;

            GameApp.Instance.SetGameState(GAME_STATE.GAMEOVER);
        }

        //声音
        private void ParseSY(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int val = Int32.Parse(arrstr[2]);
            SEX sex = SEX.Boy;
            if (Int32.Parse(arrstr[3])==0)
                sex = SEX.Boy;
            else if (Int32.Parse(arrstr[3]) == 1)
                sex = SEX.Girl;

            int playerId = ValueToPlayerId(Int32.Parse(arrstr[4]));

            if (val == 1)//吃
            {
                ControlCenter.Instance.PlaySound_Result(ResultType.吃, sex);
                ControlCenter.Instance.GetPlayerPanel(playerId).ShowAnim(ResultType.吃);
                ControlCenter.Instance.HideCurShowArrow();
            }
            else if (val == 2)//碰
            {
                ControlCenter.Instance.PlaySound_Result(ResultType.碰, sex);
                ControlCenter.Instance.GetPlayerPanel(playerId).ShowAnim(ResultType.碰);
                ControlCenter.Instance.HideCurShowArrow();
            }
            else if (val == 3)//杠
            {
                ControlCenter.Instance.PlaySound_Result(ResultType.杠, sex);
                ControlCenter.Instance.GetPlayerPanel(playerId).ShowAnim(ResultType.杠);
                ControlCenter.Instance.HideCurShowArrow();
            }
            else if (val == 4)//糊
            {
                ControlCenter.Instance.PlaySound_Result(ResultType.糊, sex);
                ControlCenter.Instance.GetPlayerPanel(playerId).ShowAnim(ResultType.糊);
            }
        }

        //自摸
        private void ParseZM(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int playerId = DataCenter.Instance.gamedata.PlayerID;
            DataCenter.Instance.players[playerId].resultList.Clear();
            Result_Struct rs = new Result_Struct();
            rs.type = ResultType.自摸;
            DataCenter.Instance.players[playerId].resultList.Add(rs);
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).UpdateResultBtn(DataCenter.Instance.players[playerId].resultList);
        }

        //方位
        private void ParseFangWei(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            DNXB_DIR dir = ValueToDNXBDir(Int32.Parse(arrstr[2]));
            ControlCenter.Instance.SetCurChuPaiDir(dir);
        }

        //玩家信息
        private void ParsePlayerInfo(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int strIndex = 2;
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].playerInfo.WXName = arrstr[strIndex];
            }
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex]!="")
                {
                    DataCenter.Instance.players[i].playerInfo.sex = Int32.Parse(arrstr[strIndex]) == 0 ? SEX.Boy : SEX.Girl;
                    
                }
            }
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "")
                {
                    DataCenter.Instance.players[i].playerInfo.coin = Int32.Parse(arrstr[strIndex]);

                }
            }
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "")
                {
                    DataCenter.Instance.players[i].playerInfo.GameID = arrstr[strIndex];

                }
            }
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "")
                {
                    DataCenter.Instance.players[i].playerInfo.ip = arrstr[strIndex];

                }
            }
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "")
                {
                    DataCenter.Instance.players[i].playerInfo.Geolocation = arrstr[strIndex];
                }
            }
            ControlCenter.Instance.UpdatePlayerInfo();
            WXTXCenter.Instance.ReStartTimer();
        }

        //离开房间
        private void ParseLeaveRoom(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int result = Int32.Parse(arrstr[2]);
            if (result == 1)//离开成功
            {
                GameApp.Instance.SetGameState(GAME_STATE.HALL);
            }
            else if (result == 2)//离开失败
            {
                if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.自由匹配)
                {
                    ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox("中途无法离开游戏！", "", null);                    
                }
                else if (DataCenter.Instance.gamedata.gameMode == GAME_MODE.创建房间)
                {
                    ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sDismissRoomFail, "", null);
                }
            }
            else if (result == 3)//离开投票
            {
                ViewCenter.Instance.GetPanel<Panel_Prompt>(PanelType.Prompt).ShowText(Tags.TextPrompt.sDismissRoomWait, 5);
            }
        }

        //离开房间投票
        private void ParseLeaveRoomTP(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[2]));
            string strName = DataCenter.Instance.players[playerId].playerInfo.WXName;
            ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(strName + "\n" + Tags.TextPrompt.sDismissRoomApply, "", delegate { SendLeaveRoomTP(true); }, "", delegate { SendLeaveRoomTP(false); });
        }

        //战绩结果
        private void ParseRecord(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int strIndex = 2;
            //胡牌次数
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOverRecord.HuPaiCount = Int32.Parse(arrstr[strIndex]);
            }
            //点炮次数
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOverRecord.DianPaoCount = Int32.Parse(arrstr[strIndex]);
            }
            //公杠次数
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOverRecord.GongGangCount = Int32.Parse(arrstr[strIndex]);
            }
            //暗杠次数
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOverRecord.AnGangCount = Int32.Parse(arrstr[strIndex]);
            }
            //中码次数
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOverRecord.ZhongMaCount = Int32.Parse(arrstr[strIndex]);
            }
            //总战绩
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].gameOverRecord.ZongProfit = Int32.Parse(arrstr[strIndex]);
            }

            ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.GameOver_Record);
        }

        //聊天
        private void ParseLT(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[2]));
            string context = arrstr[3];
            ControlCenter.Instance.ShowLT(playerId, context);
        }
        //表情
        private void ParseBQ(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[2]));
            string context = arrstr[3];
            ControlCenter.Instance.ShowBQ(playerId, context);            
        }
        //说话
        private void ParseSH(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[2]));
            string clipName = arrstr[3];
            ControlCenter.Instance.ShowSH(playerId, clipName);
        }
        //前20场的战绩统计
        private void ParseGetZJ(NetDataBuf dataBuf, bool bClear=false)
        {
            if (bClear)
            {
                DataCenter.Instance.histroyRecordList.Clear();
            }

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            for (int i = 2; i < arrstr.Length; i++)
            {
                if (arrstr[i]!="")
                {
                    //string[] histroyArrstr = arrstr[i].Split(',');
                    //HistroyRecordData hr = new HistroyRecordData();
                    //hr.WXName_东 = histroyArrstr[0];
                    //hr.WinScore_东 = Int32.Parse(histroyArrstr[1]);
                    //hr.WXName_南 = histroyArrstr[2];
                    //hr.WinScore_南 = Int32.Parse(histroyArrstr[3]);
                    //hr.WXName_西 = histroyArrstr[4];
                    //hr.WinScore_西 = Int32.Parse(histroyArrstr[5]);
                    //hr.WXName_北 = histroyArrstr[6];
                    //hr.WinScore_北 = Int32.Parse(histroyArrstr[7]);


                    HistroyRecordData hr = new HistroyRecordData();
                    hr.Info_1 = arrstr[i];
                    hr.RoomID = Int32.Parse(arrstr[i].Substring(0, 6));
                    ViewCenter.Instance.GetPanel<Panel_Record>(PanelType.Record).AddHistoryRecord(ref hr);
                }                
            }
        }
        //语音
        private void ParseYuYing(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int playerId = Int32.Parse(arrstr[2]);

            byte[] dataAry = new byte[dataBuf.FullDataLen];
            Array.Copy(dataBuf.byteBuf, dataBuf.DataLen - dataBuf.FullDataLen, dataAry, 0, dataBuf.FullDataLen);
            Debuger.Log("语音数据：" + dataAry);

            byte[] depressBuf = DataZipCenter.DeCompressByGZIP(dataAry);
            MicrophoneInput.getInstance().PlayClipData(depressBuf);
        }
        //暗杠的牌（回放）
        private List<CPG_Struct> ParsePlayerANGMJData_HF(string context)
        {
            List<MJType> MJList = ParsePlayerMJData(context);
            List<CPG_Struct> CPGList = new List<CPG_Struct>();
            for (int i = 0; i < MJList.Count; i+=4)
            {
                CPG_Struct cs = new CPG_Struct();
                cs.Init(CPG_TYPE.暗杠, new List<MJType> { MJList[i], MJList[i+1], MJList[i+2], MJList[i+3] }, SEAT_DIR.DIR_NULL);
                CPGList.Add(cs);
            }
            return CPGList;
        }
        //回放
        private void ParseHF(NetDataBuf dataBuf)
        {
            DataCenter.Instance.hFdata.ReInit();

            string[] arrstr = dataBuf.sb.ToString().Split('/');
            for (int i = 0; i < 4; i++)
            {
                DataCenter.Instance.players[i].Clear();
            }
            //------手中牌----------//
            int strIndex = 2;
            for (int i = 0; i < 4; i++,strIndex++)
            {
                DataCenter.Instance.players[i].AddMJ_IN_Data(ParsePlayerMJData(arrstr[strIndex]));
            }
            //------吃的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") DataCenter.Instance.players[i].AddMJ_CPG_Data(ParsePlayerCMJData(arrstr[strIndex], i));
            }
            //------碰的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") DataCenter.Instance.players[i].AddMJ_CPG_Data(ParsePlayerPMJData(arrstr[strIndex], i));
            }
            //------杠的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") DataCenter.Instance.players[i].AddMJ_CPG_Data(ParsePlayerGMJData(arrstr[strIndex], i));
            }
            //------打的牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") DataCenter.Instance.players[i].MJ_OUT_List = ParsePlayerMJData(arrstr[strIndex]);
            }
            //------暗杠----------//            
            for (int i = 0; i < 4; i++, strIndex++)
            {
                if (arrstr[strIndex] != "") DataCenter.Instance.players[i].AddMJ_CPG_Data(ParsePlayerANGMJData_HF(arrstr[strIndex]));
            }
            //剩余牌数
            DataCenter.Instance.gamedata.SurplusMJSum = Int32.Parse(arrstr[strIndex++]);
            //总局数
            DataCenter.Instance.gamedata.JuShu = Int32.Parse(arrstr[strIndex++]);
            //当前局数
            DataCenter.Instance.gamedata.CurJuShu = Int32.Parse(arrstr[strIndex++]);
            //当前方位
            DNXB_DIR dir = ValueToDNXBDir(Int32.Parse(arrstr[strIndex++]));
            ControlCenter.Instance.SetCurChuPaiDir(dir);
            //------分数----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].playerInfo.coin = Int32.Parse(arrstr[strIndex]);
            }
            //庄家位置
            ControlCenter.Instance.SetZhuangDir(ValueToDNXBDir(Int32.Parse(arrstr[strIndex++])));
            //当前出牌
            string chuPaiStr = arrstr[strIndex++];
            if (chuPaiStr!="")
            {
                int playerId = ValueToPlayerId(Int32.Parse(chuPaiStr.Substring(0,2)));
                MJType mjType = ValueToMJType(Int32.Parse(chuPaiStr.Substring(2,2)));
                //ControlCenter.Instance.ChuPai(playerId, mjType);
                DataCenter.Instance.hFdata.CurChuPai._playerId = playerId;
                DataCenter.Instance.hFdata.CurChuPai._mjType = mjType;
            }
            //当前吃牌
            string chiPaiStr = arrstr[strIndex++];
            if (chiPaiStr!="")
            {
                string[] chiPaiStrAry = chiPaiStr.Split('-');
                if (chiPaiStrAry.Length > 0)
                {
                    int playerId = ValueToPlayerId(Int32.Parse(chiPaiStrAry[0]));
                    List<MJType> mjList = ParsePlayerMJData(chiPaiStrAry[1]);
                    DataCenter.Instance.hFdata.CurChiPai._playerId = playerId;
                    DataCenter.Instance.hFdata.CurChiPai._MJList = mjList;
                }
            }            
            //当前碰牌
            string pengPaiStr = arrstr[strIndex++];
            if (pengPaiStr!="")
            {
                string[] pengPaiStrAry = pengPaiStr.Split('-');
                if (pengPaiStrAry.Length > 0)
                {
                    int playerId = ValueToPlayerId(Int32.Parse(pengPaiStrAry[0]));
                    List<MJType> mjList = ParsePlayerMJData(pengPaiStrAry[1]);
                    DataCenter.Instance.hFdata.CurPengPai._playerId = playerId;
                    DataCenter.Instance.hFdata.CurPengPai._MJList = mjList;
                }
            }
           
            //当前杠牌
            string gangPaiStr = arrstr[strIndex++];
            if (gangPaiStr!="")
            {
                string[] gangPaiStrAry = gangPaiStr.Split('-');
                if (gangPaiStrAry.Length > 0)
                {
                    int playerId = ValueToPlayerId(Int32.Parse(gangPaiStrAry[0]));
                    List<MJType> mjList = ParsePlayerMJData(gangPaiStrAry[1]);
                    DataCenter.Instance.hFdata.CurGangPai._playerId = playerId;
                    DataCenter.Instance.hFdata.CurGangPai._MJList = mjList;
                }
            }
            
            //当前暗杠牌
            string anGangPaiStr = arrstr[strIndex++];
            if (anGangPaiStr!="")
            {
                string[] anGangPaiStrAry = anGangPaiStr.Split('-');
                if (anGangPaiStrAry.Length > 0)
                {
                    int playerId = ValueToPlayerId(Int32.Parse(anGangPaiStrAry[0]));
                    List<MJType> mjList = ParsePlayerMJData(anGangPaiStrAry[1]);
                    DataCenter.Instance.hFdata.CurAnGangPai._playerId = playerId;
                    DataCenter.Instance.hFdata.CurAnGangPai._MJList = mjList;
                }
            }            
            //------胡牌----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                List<MJType> dataList = ParsePlayerMJData(arrstr[strIndex]);
                ControlCenter.Instance.SetTingPai(i, dataList);
                DataCenter.Instance.players[i].MJ_TingPai_List = dataList;      
            }
            //------钓鱼----------//
            bool bGameOver = false;
            for (int i = 0; i < 4; i++, strIndex++)
            {
                List<MJType> dataList = ParsePlayerMJData(arrstr[strIndex]);
                DataCenter.Instance.players[i].gameOver.ZhongYuList = dataList;
                if (dataList.Count > 0) bGameOver = true;
            }
            //------中鱼----------//
            for (int i = 0; i < 4; i++, strIndex++)
            {
                List<MJType> dataList = ParsePlayerMJData(arrstr[strIndex]);
                DataCenter.Instance.players[i].gameOver.ZhongYuOKList = dataList;
            }
            if (bGameOver) GameApp.Instance.SetGameState(GAME_STATE.ZHONGYU);
            //------最大鱼的数量----------//
            DataCenter.Instance.gamedata.MaxZhongYuNum = Int32.Parse(arrstr[strIndex++]);
            //当前摸牌
            string moPaiStr = arrstr[strIndex++];
            if (moPaiStr!="")
            {
                int playerId = ValueToPlayerId(Int32.Parse(moPaiStr.Substring(0, 2)));
                MJType mjType = ValueToMJType(Int32.Parse(moPaiStr.Substring(2, 2)));
                //DataCenter.Instance.players[playerId].MoPai(mjType);
                DataCenter.Instance.hFdata.CurMoPai._playerId = playerId;
                DataCenter.Instance.hFdata.CurMoPai._mjType = mjType;
            }
            //玩家GameID
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].playerInfo.GameID = arrstr[strIndex];
            }
            //玩家微信名
            for (int i = 0; i < 4; i++, strIndex++)
            {
                DataCenter.Instance.players[i].playerInfo.WXName = arrstr[strIndex];
            }
            //方位
            DataCenter.Instance.gamedata.SelfDNXBDir = ValueToDNXBDir(Int32.Parse(arrstr[strIndex++]));

            ControlCenter.Instance.HuiFang();
        }
        //玩法规则
        private void ParseGameRule(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            string[] gameRules = new string[arrstr.Length-2];
            for (int i = 0; i < gameRules.Length; i++)
			{
			    gameRules[i] = arrstr[i+2];
			}
            ViewCenter.Instance.GetPanel<Panel_CreateRoom>(PanelType.CreateRoom).UpdateGameRule(gameRules);
            ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.CreateRoom); 
        }
        //玩法显示
        private void ParseWFXS(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int startIndex = 2;
            ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).SetShowRule1(arrstr[startIndex++]);
            ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).SetShowRule2(arrstr[startIndex++]);
        }
        //输赢显示
        private void ParseSYXS(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            string[] infoAry = new string[4];
            for (int i = 0; i < 4; i++, strIndex++)
            {
                infoAry[i] = arrstr[i + 2];
            }
            ViewCenter.Instance.GetPanel<Panel_GameOver>(PanelType.GameOver).UpdateShowInfo(infoAry);
            ViewCenter.Instance.GetPanel<Panel_GameOver>(PanelType.GameOver).Set_当前玩法(arrstr[strIndex]);
        }
        //玩家分数
        private void ParseWJFS(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            DataCenter.Instance.gamedata.playerInfo.coin = Int32.Parse(arrstr[strIndex++]);
            DataCenter.Instance.gamedata.playerInfo.diamond = Int32.Parse(arrstr[strIndex++]);
        }
        //解散房间
        private void ParseJSFJ(NetDataBuf dataBuf)
        {
            if (!ControlCenter.Instance.IsPlaying) return;

            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            bool bJSFJ = Int32.Parse(arrstr[strIndex++]) > 0 ? true : false;
            if (bJSFJ) GameApp.Instance.SetGameState(GAME_STATE.HALL);
        }
        //游戏规则
        private void ParseDDGZ(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            ViewCenter.Instance.GetPanel<Panel_Prepare>(PanelType.Prepare).SetLabel_玩法(arrstr[strIndex++]);
        }
        //返回大厅
        private void ParseFHDT(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            GameApp.Instance.SetGameState(GAME_STATE.HALL);
        }
        //回放结果
        private void ParseHFED(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            if (Int32.Parse(arrstr[strIndex]) == 0)//没有录像
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sHuiFang_NoRecord, "", null);                
            }
            else if (Int32.Parse(arrstr[strIndex]) == 1)//回放结束
            {
                GameApp.Instance.SetGameState(GAME_STATE.HALL);
            }
            else if (Int32.Parse(arrstr[strIndex]) == 2)//已经到第一局
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sHuiFang_FirstJu, "", null);                
            }
            else if (Int32.Parse(arrstr[strIndex]) == 3)//已经到最后一局
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sHuiFang_EndJu, "", null);                
            }
            else if (Int32.Parse(arrstr[strIndex]) == 4)//正常
            {
                GameApp.Instance.SetGameState(GAME_STATE.HUIFANG);
            }
            else if (Int32.Parse(arrstr[strIndex]) == 5)//暂停
            {
                Time.timeScale = 0f;
            }
            else if (Int32.Parse(arrstr[strIndex]) == 6)//恢复
            {
                Time.timeScale = 1f;
            }
        }
        //玩家离开
        private void ParseWJLK(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');

            int strIndex = 2;
            int playerId = ValueToPlayerId(Int32.Parse(arrstr[strIndex]));
            PlayerInfo playerInfo = DataCenter.Instance.players[playerId].playerInfo;
            string promptText = playerInfo.WXName+"离开了房间";
            ViewCenter.Instance.GetPanel<Panel_Prompt>(PanelType.Prompt).ShowText(promptText);

            DataCenter.Instance.players[playerId].playerInfo.WXName = "";
            DataCenter.Instance.players[playerId].playerInfo.WXTX_Icon_SpriteName = "";
            ControlCenter.Instance.UpdatePlayerInfo();
            
        }
        #endregion
        //值转换成方位
        private SEAT_DIR ValueToDir(int value)
        {
            DNXB_DIR dnxb_dir = ValueToDNXBDir(value);
            if (dnxb_dir==DNXB_DIR.Null)
            {
                return SEAT_DIR.DIR_NULL;
            }               

            return GameGD.GetSeatDir(dnxb_dir);
        }

        private DNXB_DIR ValueToDNXBDir(int value)
        {
            DNXB_DIR dnxb_dir = DNXB_DIR.Null;
            if (value == 1)
                dnxb_dir = DNXB_DIR.东;
            else if (value == 2)
                dnxb_dir = DNXB_DIR.南;
            else if (value == 3)
                dnxb_dir = DNXB_DIR.西;
            else if (value == 4)
                dnxb_dir = DNXB_DIR.北;

            return dnxb_dir;
        }

        private int ValueToPlayerId(int value)
        {
            return --value;
        }

        private GAME_MODE ValueToGameMode(int value)
        {
            GAME_MODE mode = GAME_MODE.自由匹配;
            if (value == 1)
                mode = GAME_MODE.自由匹配;
            else if (value == 2)
                mode = GAME_MODE.创建房间;
            else if (value == 3)
                mode = GAME_MODE.比赛房间;
            return mode;
        }

        //方位转换成协议中的值
        private int DirToValue(DNXB_DIR dir)
        {
            int val = 0;
            if (dir == DNXB_DIR.东)
                val = 1;
            else if (dir == DNXB_DIR.南)
                val = 2;
            else if (dir == DNXB_DIR.西)
                val = 3;
            else if (dir == DNXB_DIR.北)
                val = 4;
            return val;
        }

        public void OnDestroy()
        {
            network.CloseNetwork();
        }
    }
}

