using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace DYD
{
    public class Panel_GameOver : UIPanelBase
    {
        private UILabel label_time;
        private UILabel label_当前玩法;
        private MJ_Manager _MJ_Mgr;
        private PlayerInfo_GameOver[] playeInfos;
        private UISprite sprite_Result;//输赢结果        

        public override void Init(PanelType type)
        {
            Transform[] childs = GetComponentsInChildren<Transform>();
            foreach (Transform item in childs)
            {
                if (item.name == "MJ")
                {
                    item.gameObject.SetActive(false);
                }
            }

            _MJ_Mgr = GetComponentInChildren<MJ_Manager>();
            _MJ_Mgr.Init();

            label_time = GameUtility.FindDeepChild(gameObject, "时间").GetComponent<UILabel>();
            label_当前玩法 = GameUtility.FindDeepChild(gameObject, "当前玩法").GetComponent<UILabel>();
            sprite_Result = GameUtility.FindDeepChild(gameObject, "Result").GetComponent<UISprite>();

            playeInfos = GetComponentsInChildren<PlayerInfo_GameOver>(true);
            foreach (PlayerInfo_GameOver item in playeInfos)
            {
                item.Init(_MJ_Mgr);
            }

            base.Init(type);
            AddColliderMode(PanelColliderMode.Normal);

            EventInit();    
            
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            _MJ_Mgr.DespawnAll();
            RefleshTime();
            RefleshPanelData();
            UpdateLoseWinResult();
            //label_当前玩法.text = "当前玩法 : "+ DataCenter.Instance.gamedata.PlayRule_1+ "  " + DataCenter.Instance.gamedata.PlayRule_2;
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_炫耀一下").gameObject).onClick = delegate
            {
                ControlCenter.Instance.CaptureShare();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_Exit").gameObject).onClick = delegate
            {
                GameApp.Instance.SetGameState(GAME_STATE.HALL);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_再来一局").gameObject).onClick = delegate
            {
                if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.自由匹配)
                {
                    DealCommand.Instance.SendJoinRoom_自由匹配();
                }
                else if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.创建房间)
                {
                    DataCenter.Instance.GoToNextPlate();
                    DealCommand.Instance.SendPrepare();                    
                    GameApp.Instance.SetGameState(GAME_STATE.PREPARE);
                }                
            };
        }

        private void RefleshTime()
        {
            System.DateTime dt = SystemInfoMgr.GetDataTime();
            label_time.text = System.DateTime.Now.ToString("g");
        }

        private void RefleshPanelData()
        {
            foreach (PlayerInfo_GameOver item in playeInfos)
            {
                item.UpdateData();
            }
        }

        private void UpdateLoseWinResult()
        {
            PlayerData pd = DataCenter.Instance.players[DataCenter.Instance.gamedata.PlayerID];
            switch (pd.gameOver.GameResult)
            {
                case GameOver_Result.赢:
                    sprite_Result.spriteName = Tags.GameOver_Result_Name.Result_赢;
                    break;
                case GameOver_Result.输:
                    sprite_Result.spriteName = Tags.GameOver_Result_Name.Result_输;
                    break;
                case GameOver_Result.流局:
                    sprite_Result.spriteName = Tags.GameOver_Result_Name.Result_流局;
                    break;
                case GameOver_Result.不输不赢:
                    sprite_Result.spriteName = Tags.GameOver_Result_Name.Result_不输不赢;
                    break;
                default:
                    break;
            }
            sprite_Result.MakePixelPerfect();
        }

        //更新玩家显示信息
        public void UpdateShowInfo(string[] infoAry)
        {
            //foreach (PlayerInfo_GameOver item in playeInfos)
            for(int i=0; i<playeInfos.Length&&i<infoAry.Length; i++)
            {
                playeInfos[i].SetShowInfo(infoAry[i]);
            }
        }

        public void Set_当前玩法(string text)
        {
            label_当前玩法.text = text;
        }
    }
}

