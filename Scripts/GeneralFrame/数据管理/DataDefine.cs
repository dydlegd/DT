using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class HistroyRecordData
    {
        public string WXName_东 { get; set; }
        public int WinScore_东 { get; set; }
        public string WXName_南 { get; set; }
        public int WinScore_南 { get; set; }
        public string WXName_西 { get; set; }
        public int WinScore_西 { get; set; }
        public string WXName_北 { get; set; }
        public int WinScore_北 { get; set; }

        public string Info_1 { get; set; }
        public int RoomID { get; set; }
        public string Info_2 { get; set; }
    }
    public class PlayerInfo
    {
        public SEX sex;
        public string WXName;
        private int mCoin;
        public int coin { get { return mCoin; } set { mCoin = value; NotificationCenter.Instance.PostNotificationEvent(NotificateMsgType.PLAYER_SCORE, new Notification(value)); } }
        private int mDiamond;
        public int diamond { get { return mDiamond; } set { mDiamond = value; NotificationCenter.Instance.PostNotificationEvent(NotificateMsgType.PLAYER_DIAMOND, new Notification(value)); } }
        public string ip;
        public string Geolocation { get; set; }//地理位置
        public int roodCard;
        public int MachineNum { get; set; }
        public string Password { get; set; }
        public string WXID { get; set; }
        public string GameID { get; set; }
        public byte[] WXTX_Datas;
        public Texture2D WXTX_Texture= new Texture2D(100, 100);
        public string WXTX_Icon_SpriteName;//微信头像精灵名称
        public DNXB_DIR DNXBDir { get; set; }
        public bool PrepareOK { get; set; }//准备

        public PlayerInfo()
        {
            WXTX_Icon_SpriteName = "";
        }

        public void GoToNextPlate()
        {
            
        }
    }

    public class GameData
    {
        public bool IsHaveLogin { get; set; }
        public int CurDownTime { get; set; }//当前时间
        public DNXB_DIR CurChuPaiDir { get; set; }//当前出牌的方位
        public int SurplusMJSum { get; set; }//剩余麻将数量
        public MJ CurChuMJ { get; set; }//上一个出的麻将
        public int RoomNum { get; set; }//房间号
        public int JuShu { get; set; }//局数
        public int CurJuShu { get; set; }//当前局数
        public bool IsTrusteeship { get; set; }//是否托管
        private DNXB_DIR mDNXBDir;//东南西北方向
        public DNXB_DIR SelfDNXBDir 
        {
            get { return mDNXBDir; }
            set
            {
                mDNXBDir = value;
                playerInfo.DNXBDir = value;
                //playerInfo = DataCenter.Instance.players[GameGD.GetPlayerID(SEAT_DIR.DIR_BOTTOM)].playerInfo;
                ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).SetDiPanDir(mDNXBDir);
                DataCenter.Instance.players[PlayerID].playerInfo.coin = playerInfo.coin;
                DataCenter.Instance.players[PlayerID].playerInfo.diamond = playerInfo.diamond;
                DataCenter.Instance.players[PlayerID].playerInfo.WXName = playerInfo.WXName;
                DataCenter.Instance.players[PlayerID].playerInfo.WXTX_Icon_SpriteName = playerInfo.WXTX_Icon_SpriteName;
                DataCenter.Instance.players[PlayerID].playerInfo.DNXBDir = playerInfo.DNXBDir;
            }
        }
        public int MaxDownTime { get; set; }//最大倒计时
        public string RandPassword { get; set; }//随机密码
        public string GameRule { get; set; }//游戏规则        
        public string AdministratorName { get; set; }
        public bool IsChuPaing { get; set; }
        public DNXB_DIR ZhuangDir { get; set; }
        public int Dice_Num_1 { get; set; }
        public int Dice_Num_2 { get; set; }
        public int PlayerID { get { return (int)SelfDNXBDir; } }//自身玩家ID（0－3）
        public int MaxZhongYuNum { get; set; }
        public PlayerInfo playerInfo = new PlayerInfo();// DataCenter.Instance.players[GameGD.GetPlayerID(SEAT_DIR.DIR_BOTTOM)].playerInfo;
        public GAME_MODE gameMode { get; set; }
        public DNXB_DIR FangZhuDNXBDir { get; set; }
        public bool IsFangZhu { get { return SelfDNXBDir == FangZhuDNXBDir; } }
        public bool IsZhuangJia { get { return ZhuangDir == SelfDNXBDir; } }
        public int ZhuangPlayerID { get { return (int)ZhuangDir; } }
        public int FangZhuPlayerID { get { return (int)FangZhuDNXBDir; } }
        public AREA_ENUM GameArea { get; set; }
        public string PlayRule_1
        {
            get
            {
                for (int i = GameGD.PLAYRULE_1_START_INDEX; i <= GameGD.PLAYRULE_1_END_INDEX; i++)
                {
                    if (GameRule[i] == '1')
                    {
                        return GameGD.ruleArr[i];
                    }
                }
                return "";
            }
        }
        public string PlayRule_2
        {
            get
            {
                for (int i = GameGD.PLAYRULE_2_START_INDEX; i <= GameGD.PLAYRULE_2_END_INDEX; i++)
                {
                    if (GameRule[i] == '1')
                    {
                        return GameGD.ruleArr[i];
                    }
                }
                return "";
            }
        }
        public GameData()
        {
            IsHaveLogin = false;
            CurDownTime = 0;
            CurChuPaiDir = DNXB_DIR.Null;
            SurplusMJSum = 0;
            CurChuMJ = null;
            gameMode = GAME_MODE.创建房间;
            FangZhuDNXBDir = DNXB_DIR.Null;
            playerInfo.MachineNum = 10001;
            playerInfo.Password = "667788";
            playerInfo.WXID = "duang" + RandomMgr.Range(0, 100000);
            AdministratorName = "XXX.bt";
            GameArea = AREA_ENUM.桂林;
        }

        public void GoToNextPlate()
        {

        }
    }    

    public class CPG_Struct
    {
        public CPG_TYPE type;
        public List<MJType> MJList;
        public SEAT_DIR dir = SEAT_DIR.DIR_NULL;//吃碰框哪个方位的

        public void Init(CPG_TYPE type, List<MJType> data, SEAT_DIR dir)
        {
            MJList = new List<MJType>();
            this.type = type;
            MJList = data;
            this.dir = dir;
            Debuger.Log("吃碰杠：" + type.ToString() + "  dir: " + dir.ToString());
        }
    }

    public class Result_Struct
    {
        public ResultType type;
        public List<MJType> MJList = new List<MJType>();
        public SEAT_DIR dir = SEAT_DIR.DIR_NULL;//吃碰框哪个方位的
    }

    public class Result_Hu_Struct: Result_Struct
    {
        public HuType huType = HuType.Null;
    }

    public class GameOverRecordData
    {
        public int HuPaiCount { get; set; }//胡牌次数
        public int DianPaoCount { get; set; }//点炮次数
        public int GongGangCount { get; set; }//公杠次数
        public int AnGangCount { get; set; }//暗杠次数
        public int ZhongMaCount { get; set; }//中码次数
        public int ZongProfit { get; set; }//总战绩
    }

    public class GameOverData
    {
        public List<HuType> huTypeList = new List<HuType>();
        public int DeFen { get; set; }//得分
        public GameOver_Result GameResult 
        { 
            get 
            {
                if(DataCenter.Instance.HasHuPai())
                {
                    if (DeFen > 0)
                        return GameOver_Result.赢;
                    else if (DeFen < 0)
                        return GameOver_Result.输;
                    else
                        return GameOver_Result.不输不赢;
                }
                else
                {
                    return GameOver_Result.流局;
                }
            } 
        }
        public List<MJType> ZhongYuList = new List<MJType>();//摸鱼
        public List<MJType> ZhongYuOKList = new List<MJType>();//中鱼
        public bool IsZhongYu(MJType type)
        {
            for (int i = 0; i < ZhongYuOKList.Count; i++)
            {
                if (ZhongYuOKList[i] == type)
                    return true;
            }
            return false;
        }
        public bool IsWin { get { return huTypeList.Count > 0; } }
        public DNXB_DIR fangPaoDir = DNXB_DIR.Null;
        public MJType huPaiMJType;

        public void GoToNextPlate()
        {
            huTypeList.Clear();
            fangPaoDir = DNXB_DIR.Null;
            huPaiMJType = MJType.Null;
            ZhongYuList.Clear();
            ZhongYuOKList.Clear();
        }
    }

    public class PlayerData
    {
        private int mPlayerID = 0;
        public int playerId { get { return mPlayerID; } set { mPlayerID = value; playerInfo.DNXBDir = (DNXB_DIR)(playerId); } }        
        public List<MJType> MJ_TingPai_List = new List<MJType>();
        public List<MJType> MJ_IN_List = new List<MJType>();//手中的麻将
        public List<MJType> MJ_OUT_List = new List<MJType>();//打出去的麻将
        public List<CPG_Struct> MJ_CPG_List = new List<CPG_Struct>();//吃碰杠等的麻将
        public List<Result_Struct> resultList = new List<Result_Struct>();        
        public PlayerInfo playerInfo = new PlayerInfo();
        public GameOverData gameOver = new GameOverData();
        public GameOverRecordData gameOverRecord = new GameOverRecordData();
        public List<MJType> cannotChuMJList = new List<MJType>();//不能打的麻将

        public PlayerData(int _playerId)
        {
            playerId = _playerId;
        }

        public void ReInit()
        {
            
        }

        public void SetData(PlayerData data)
        {
            playerId = data.playerId;
            MJ_IN_List.Clear(); MJ_OUT_List.Clear(); MJ_CPG_List.Clear();
            AddMJ_IN_Data(data.MJ_IN_List);
            AddMJ_OUT_Data(data.MJ_OUT_List);
            AddMJ_CPG_Data(data.MJ_CPG_List);
        }

        public void AddMJ_IN_Data(List<MJType> data, bool bSort = true)
        {
            for (int i = 0; i < data.Count; i++)
            {
                MJ_IN_List.Add(data[i]);
            }
            if (bSort) SortMJ();
        }

        public void MoPai(MJType type)
        {
            //if (type>0)
            {
                cannotChuMJList.Clear();
                AddMJ_IN_Data(new List<MJType> { type }, false);
                ControlCenter.Instance.SetCurChuPaiDir(playerInfo.DNXBDir);
                if(ControlCenter.Instance.IsCanControl) Debuger.Log("MoPai: " + type);
            }           
        }
        
        private void AddMJ_OUT_Data(List<MJType> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                MJ_OUT_List.Add(data[i]);
            }
        }
        public void AddMJ_CPG_Data(List<CPG_Struct> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                MJ_CPG_List.Add(data[i]);
            }
        }

        public void AddCannotChuMJ(MJType type)
        {
            cannotChuMJList.Add(type);
        }

        public void SortMJ()
        {
            SortMJ(ref MJ_IN_List);
        }

        private void SortMJ(ref List<MJType> list)
        {
            list.Sort(SortFunc);
        }

        private int SortFunc(MJType x, MJType y)
        {
            if (x > y)
                return 1;
            else if (x == y)
                return 0;
            else
                return -1;
        }

        public void Clear()
        {
            MJ_IN_List.Clear(); 
            MJ_OUT_List.Clear(); 
            MJ_CPG_List.Clear();
            MJ_TingPai_List.Clear();
            resultList.Clear();
        }

        public void GoToNextPlate(bool bClearWXTX = false)
        {
            gameOver.GoToNextPlate();
            playerInfo.GoToNextPlate();
            playerInfo.WXName = "";
            if (bClearWXTX) playerInfo.WXTX_Icon_SpriteName = "";
            playerInfo.Geolocation = "";
            //if (mPlayerID == DataCenter.Instance.gamedata.PlayerID)
            //{
            //    playerInfo.WXTX_Icon_SpriteName = DataCenter.Instance.gamedata.playerInfo.WXTX_Icon_SpriteName;
            //    playerInfo.WXName = DataCenter.Instance.gamedata.playerInfo.WXName;
            //    playerInfo.coin = DataCenter.Instance.gamedata.playerInfo.coin;
            //    playerInfo.diamond = DataCenter.Instance.gamedata.playerInfo.diamond;
            //}
            Clear();
        }
    }

    public class HFData
    {
        public class HF_ChuPai
        {
            public int _playerId;
            public MJType _mjType;
            public void ReInit()
            {
                //_playerId = 0;
                _mjType = MJType.Null;
            }
        }
        public class HF_ChiPai
        {
            public int _playerId;
            public List<MJType> _MJList = new List<MJType>();
            public void ReInit()
            {
                _playerId = 0;
                _MJList.Clear();
            }
        }
        public class HF_PengPai
        {
            public int _playerId;
            public List<MJType> _MJList = new List<MJType>();
            public void ReInit()
            {
                _playerId = 0;
                _MJList.Clear();
            }
        }
        public class HF_GangPai
        {
            public int _playerId;
            public List<MJType> _MJList = new List<MJType>();
            public void ReInit()
            {
                _playerId = 0;
                _MJList.Clear();
            }
        }
        public class HF_AnGangPai
        {
            public int _playerId;
            public List<MJType> _MJList = new List<MJType>();
            public void ReInit()
            {
                _playerId = 0;
                _MJList.Clear();
            }
        }
        public class HF_MoPai
        {
            public int _playerId;
            public MJType _mjType;
            public void ReInit()
            {
                _playerId = 0;
                _mjType = MJType.Null;
            }
        }
        public bool IsHuiFanging { get; set; }//是否正在回放
        public HF_ChuPai CurChuPai = new HF_ChuPai();
        public HF_ChiPai CurChiPai = new HF_ChiPai();
        public HF_PengPai CurPengPai = new HF_PengPai();
        public HF_GangPai CurGangPai = new HF_GangPai();
        public HF_AnGangPai CurAnGangPai = new HF_AnGangPai();
        public HF_MoPai CurMoPai = new HF_MoPai();

        /// <summary>
        /// 当前是否有吃碰杠
        /// </summary>
        /// <returns></returns>
        public bool IsCPGIng()
        {
            if (CurChiPai._MJList.Count > 0 
                || CurPengPai._MJList.Count > 0
                || CurGangPai._MJList.Count > 0
                )
            {
                return true;
            }
            return false;
        }

        public HFData()
        {

        }

        public void ReInit()
        {
            CurChuPai.ReInit();
            CurChiPai.ReInit();
            CurPengPai.ReInit();
            CurGangPai.ReInit();
            CurAnGangPai.ReInit();
            CurMoPai.ReInit();
        }
    }

}