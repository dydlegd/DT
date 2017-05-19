using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class GameApp : SingletonMonoBase<GameApp>
    {
        //[DllImport("Alg")]
        //private extern static int Add(int a, int b);

        private GAME_STATE mGameState = GAME_STATE.Null;
        public GAME_STATE gameState { get { return mGameState; } }

        void Awake()
        {
            Init();
        }

        // Use this for initialization
        void Start()
        {
            if (GD.SIMULATE_CHUPAI) StartCoroutine(GameTestLogic());

            //string strData = "GETZJ2/(长度)/(第11场记录)/(第12场记录)/(第13场记录)/(第14场记录)/(第15场记录)/(第16场记录)/(第17场记录)/(第18场记录)/(第19场记录)/(第20场记录)GETZJ2/(长度)/(第11场记录)/(第12场记录)/(第13场记录)/(第14场记录)/(第15场记录)/(第16场记录)/(第17场记录)/(第18场记录)/(第19场记录)/(第20场记录)\nGETZJ2/(长度)/(第11场记录)/(第12场记录)/(第13场记录)/(第14场记录)/(第15场记录)/(第16场记录)/(第17场记录)/(第18场记录)/(第19场记录)/(第20场记录)GETZJ2/(长度)/(第11场记录)/(第12场记录)/(第13场记录)/(第14场记录)/(第15场记录)/(第16场记录)/(第17场记录)/(第18场记录)/(第19场记录)/(第20场记录)";
            //byte[] dataBuf = Encoding.UTF8.GetBytes(strData);
            //byte[] pressBuf = DataZipCenter.CompressByGZIP(dataBuf);
            //byte[] depressBuf = DataZipCenter.DeCompressByGZIP(pressBuf);

            //Debuger.Log("Add: " + Add(5, 5));
            //Debuger.Log("dataBufLen:" + dataBuf.Length + "--" + dataBuf);
            //Debuger.Log("pressBufLen:" + pressBuf.Length + "--" + pressBuf);
            //Debuger.Log("depressBufLen:" + depressBuf.Length + "--" + Encoding.UTF8.GetString(depressBuf));

            
        }

        private PlayerData GetNewPlayerData(int playerId)
        {
            PlayerData data = new PlayerData(playerId);
            for (int i = 0; i < 1; i++)
            {
                CPG_Struct cs = new CPG_Struct();
                cs.type = CPG_TYPE.碰;
                cs.Init(CPG_TYPE.碰, new List<MJType>() { MJType.条_1 + playerId, MJType.条_1 + playerId, MJType.条_1 + playerId }, SEAT_DIR.DIR_RIGHT);
                data.MJ_CPG_List.Add(cs);

                cs = new CPG_Struct();
                cs.type = CPG_TYPE.杠;
                cs.Init(CPG_TYPE.杠, new List<MJType>() { MJType.条_5, MJType.条_5, MJType.条_5, MJType.条_5 }, SEAT_DIR.DIR_RIGHT);
                data.MJ_CPG_List.Add(cs);

                cs = new CPG_Struct();
                cs.type = CPG_TYPE.暗杠;
                cs.Init(CPG_TYPE.暗杠, new List<MJType>() { MJType.条_6, MJType.条_6, MJType.条_6, MJType.条_6 }, SEAT_DIR.DIR_RIGHT);
                data.MJ_CPG_List.Add(cs);
            }
            for (int i = 0; i < 8; i++)
            {
                data.MJ_OUT_List.Add((MJType)(RandomMgr.Range((int)MJType.万_1, (int)MJType.MJ_白板)));
            }
            for (int i = 0; i < 7; i++)
            {
                data.MJ_IN_List.Add((MJType)(RandomMgr.Range((int)MJType.万_1, (int)MJType.MJ_白板)));
            }

            return data;
        }

        private void Init()
        {
            Debuger.EnableLog = GD.ENABLELOG;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            ResourcesManager.Instance.Init();
            PromptManager.Instance.Init();
            GlobalManager.Instance.Init();
            ControlCenter.Instance.Init();

            DataCenter.Instance.gamedata.SelfDNXBDir = DNXB_DIR.东;

            //ControlCenter.Instance.UpdatePlayerData(GetNewPlayerData(0));
            //ControlCenter.Instance.UpdatePlayerData(GetNewPlayerData(1));
            //ControlCenter.Instance.UpdatePlayerData(GetNewPlayerData(2));
            //ControlCenter.Instance.UpdatePlayerData(GetNewPlayerData(3));

            AudioManager.PlayBackground(Tags.Audio_Name.bg[0]);
            
#if UNITY_STANDALONE||UNITY_EDITOR
            SetGameState(GAME_STATE.HALL);
#elif UNITY_ANDROID
            SetGameState(GAME_STATE.LOGIN);
#elif UNITY_IPHONE
            SetGameState(GAME_STATE.LOGIN);
#endif
        }

        public void StartRecording()
        {
            SoundRecordCenter.TryStartRecording();
        }

        public void EndRecording()
        {
            int length = 0;
            AudioClip clip = null;
            if (SoundRecordCenter.EndRecording(out length, out clip))
            {
                byte[] dataBuf = SoundRecordCenter.GetData(clip);
                byte[] dataBuf2 = SoundRecordCenter.GetClipData(clip);
                Debuger.Log("录间数据长度：" + dataBuf.Length);
                Debuger.Log("录间数据长度2：" + dataBuf2.Length);
                byte[] pressBuf = DataZipCenter.CompressByGZIP(dataBuf);
                Debuger.Log("压缩后数据长度：" + pressBuf.Length);
                byte[] pressBuf2 = DataZipCenter.CompressByGZIP(dataBuf2);
                Debuger.Log("压缩后数据长度2：" + pressBuf2.Length);

                clip = SoundRecordCenter.SetData(clip, DataZipCenter.DeCompressByGZIP(pressBuf));

                AudioManager.Play(clip, false);
            }
        }

        private void ImageDownLoadCall(DownLoadResDesc res)
        {
            Debuger.Log("图片下载完成！");
            Texture2D tex = res.texture;
        }

        // Update is called once per frame
        void Update()
        {
            ExitGameLogic();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                //StartRecording();
                MicrophoneInput.getInstance().StartRecord();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                //EndRecording();
                MicrophoneInput.getInstance().StopRecord();
                byte[] dataBuf = MicrophoneInput.getInstance().GetClipData();
                Debuger.Log("录间数据长度：" + dataBuf.Length);
                byte[] pressBuf = DataZipCenter.CompressByGZIP(dataBuf);
                Debuger.Log("压缩后数据长度：" + pressBuf.Length);
                //MicrophoneInput.getInstance().PlayRecord();
                byte[] depressBuf = DataZipCenter.DeCompressByGZIP(pressBuf);
                Debuger.Log("解压后数据长度：" + depressBuf.Length);
                MicrophoneInput.getInstance().PlayClipData(depressBuf);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ////ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).Show_胡();
                ////ViewCenter.Instance.GetPanel<Panel_Prompt>(PanelType.Prompt).ShowText("小李子，在干啥呢？");
                ////ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox("开始游戏了！", "", null);
                //GameOverData pd = DataCenter.Instance.players[0].gameOver;
                //pd.huTypeList.Add(HuType.平胡);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd = DataCenter.Instance.players[3].gameOver;
                //pd.huTypeList.Add(HuType.平胡);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //pd.ZhongYuList.Add(MJType.条_1);
                //ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.GameOver);

            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                DataCenter.Instance.players[0].resultList.Clear();
                Result_Struct result = new Result_Struct();
                result.type = ResultType.碰;
                result.MJList.Add(MJType.MJ_北);
                result.MJList.Add(MJType.MJ_北);
                result.MJList.Add(MJType.MJ_北);
                DataCenter.Instance.players[0].resultList.Add(result);
                result = new Result_Struct();
                result.type = ResultType.吃;
                result.MJList.Add(MJType.万_1);
                result.MJList.Add(MJType.万_2);
                result.MJList.Add(MJType.万_3);
                DataCenter.Instance.players[0].resultList.Add(result);
                result = new Result_Struct();
                result.type = ResultType.吃;
                result.MJList.Add(MJType.万_2);
                result.MJList.Add(MJType.万_3);
                result.MJList.Add(MJType.万_4);
                DataCenter.Instance.players[0].resultList.Add(result);
                //result = new Result_Struct();
                //result.type = ResultType.杠;
                //result.MJList.Add(MJType.万_2);
                //DataCenter.Instance.players[0].resultList.Add(result);
                ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).UpdateResultBtn(DataCenter.Instance.players[0].resultList);
                //ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowResultMJ(DataCenter.Instance.players[0].resultList, ResultType.吃);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                //DealCommand.Instance.SendLogin("浮夸");

                //List<Texture> texList = new List<Texture>();
                //texList.Add(ResourcesManager.GetTexture("XT"));
                //texList.Add(ResourcesManager.GetTexture("fankui"));
                //CreateAtlas.CreatAtlasFromTex(ResourcesManager.GetAtlas(Tags.MJ_DynamicAtlas), texList);
                //CreateAtlas.CreatAtlasFromTex(ResourcesManager.GetAtlas(Tags.MJ_DynamicAtlas), texList);
                //DataCenter.Instance.players[0].playerInfo.WXTX_Icon_SpriteName = "fankui";
                //DataCenter.Instance.players[1].playerInfo.WXTX_Icon_SpriteName = "XT";
                //DataCenter.Instance.players[2].playerInfo.WXTX_Icon_SpriteName = "fankui";
                //DataCenter.Instance.players[3].playerInfo.WXTX_Icon_SpriteName = "XT";
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                //Texture2D texture = ResourcesManager.GetTexture("XT");
                //byte[] dataBuf = texture.EncodeToJPG();
                //DealCommand.Instance.SendWXTX(dataBuf.Length, dataBuf);

                //ResDownLoadCenter.Instance.DownLoad("http://wx.qlogo.cn/mmopen/nJZe5ZQpplkuvhlrg9n2hwFyVBwfuCHGuHJia0YX73xTNykkAmE5UNNshicZy5eyBOgj967ibPa7zQEzibpNr4gZQT4ZAibzDkrSq/0", DownLoadResType.Image, ImageDownLoadCall);
                DealCommand.Instance.SendGETTX();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                HistroyRecordData hr = new HistroyRecordData();
                hr.WXName_东 = "东";
                hr.WinScore_东 = -100;
                hr.WXName_南 = "南";
                hr.WinScore_南 = 100;
                hr.WXName_西 = "西";
                hr.WinScore_西 = 0;
                hr.WXName_北 = "北";
                hr.WinScore_北 = 10;

                ////DataCenter.Instance.histroyRecordList.Add(hr);
                ViewCenter.Instance.GetPanel<Panel_Record>(PanelType.Record).AddHistoryRecord(ref hr);
                //ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.GameOver_Record);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_BOTTOM).ShowAnim(ResultType.糊);
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_RIGHT).ShowAnim(ResultType.吃);
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_TOP).ShowAnim(ResultType.碰);
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_LEFT).ShowAnim(ResultType.杠);

                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_BOTTOM).ShowMsg_Face("face_00");
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_RIGHT).ShowMsg_Face("face_01");
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_TOP).ShowMsg_Face("face_02");
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_LEFT).ShowMsg_Face("face_03");

                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_BOTTOM).ShowMsg_Dialogue("怎么又断线了，网络怎么这么差");
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_RIGHT).ShowMsg_Dialogue("怎么又断线了，网络怎么这么差");
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_TOP).ShowMsg_Dialogue("怎么又断线了，网络怎么这么差");
                //ViewCenter.Instance.GetPanel<Panel_Player>(PanelType.Player_LEFT).ShowMsg_Dialogue("怎么又断线了，网络怎么这么差");

                string ip = "", area = "", networkOperator = "";
                if (GDFunc.GetPublicIPAddress(ref ip, ref area, ref networkOperator))
                {
                    Debuger.Log("IP:" + ip + "----" + area + "----" + networkOperator);
                }
            }
        }

        private int playerId = 0;
        private IEnumerator GameTestLogic()
        {
            DataCenter.Instance.gamedata.CurDownTime = 3;
            ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).SetLabelTime(DataCenter.Instance.gamedata.CurDownTime);
            DataCenter.Instance.players[playerId].MoPai((MJType)(RandomMgr.Range((int)MJType.万_1, (int)MJType.MJ_白板)));
            ControlCenter.Instance.ReflushPlayerPanel(playerId);

            while(true)
            {
                if (mGameState==GAME_STATE.MAIN)
                {
                    yield return new WaitForSeconds(1.0f);
                    DataCenter.Instance.gamedata.CurDownTime--;
                    ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).SetLabelTime(DataCenter.Instance.gamedata.CurDownTime);
                    if (DataCenter.Instance.gamedata.CurDownTime == 0)
                    {
                        ControlCenter.Instance.ChuPai(playerId, DataCenter.Instance.players[playerId].MJ_IN_List[0]);

                        yield return new WaitForSeconds(1.0f);

                        GoToNextPlayer();
                    }
                    ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).SetLabelTime(DataCenter.Instance.gamedata.CurDownTime);
                }
                else
                {
                    yield return null;
                }               
            }
        }

        public void GoToNextPlayer()
        {
            DataCenter.Instance.gamedata.CurDownTime = 3;
            playerId++;
            if (playerId > 3) playerId = 0;
            //摸牌
            DataCenter.Instance.players[playerId].MoPai((MJType)(RandomMgr.Range((int)MJType.万_1, (int)MJType.MJ_白板)));

            ControlCenter.Instance.ReflushPlayerPanel(playerId);
        }

        public void SetGameState(GAME_STATE state)
        {
            //if (mGameState == state) return;

            mGameState = state;

            switch (mGameState)
            {
                case GAME_STATE.Null:
                    break;
                case GAME_STATE.LOGIN:
                    ViewCenter.Instance.HideAllPanel();
                    ViewCenter.Instance.GetPanel<Panel_Login>(PanelType.Login).ShowPanelDirectly();
                    break;
                case GAME_STATE.HALL:
                    DataCenter.Instance.hFdata.IsHuiFanging = false;
                    DataCenter.Instance.gamedata.gameMode = GAME_MODE.Null;
                    ViewCenter.Instance.HideAllPanel();
                    ViewCenter.Instance.GetPanel<Panel_Hall>(PanelType.Hall).ShowPanel();
                    break;
                case GAME_STATE.PREPARE:
                    ViewCenter.Instance.HideAllPanel();
                    ViewCenter.Instance.GetPanel<Panel_Prepare>(PanelType.Prepare).ShowPanel();
                    break;                
                case GAME_STATE.MAIN:
                    ViewCenter.Instance.HideAllPanel();
                    ControlCenter.Instance.GoToNextPlate();
                    ViewCenter.Instance.GetPanel<Panel_Hall>(PanelType.Hall).HidePanel();
                    ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).ShowPanel();
                    ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).ShowPanel();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_BOTTOM).ShowPanelDirectly();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_RIGHT).ShowPanelDirectly();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_TOP).ShowPanelDirectly();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_LEFT).ShowPanelDirectly();
                    DealCommand.Instance.SendGetPai(DataCenter.Instance.gamedata.RandPassword);
                    ControlCenter.Instance.UpdatePlayerInfo();
                    break;
                case GAME_STATE.HUIFANG:
                    DataCenter.Instance.hFdata.IsHuiFanging = true;
                    DataCenter.Instance.GoToNextPlate();
                    ViewCenter.Instance.HideAllPanel();
                    ControlCenter.Instance.GoToNextPlate();
                    ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).ShowPanel();
                    ViewCenter.Instance.GetPanel<Panel_UI>(PanelType.UIManager).ShowPanel();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_BOTTOM).ShowPanelDirectly();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_RIGHT).ShowPanelDirectly();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_TOP).ShowPanelDirectly();
                    ViewCenter.Instance.GetPanel<UIPanelBase>(PanelType.Player_LEFT).ShowPanelDirectly();
                    ViewCenter.Instance.ShowPanel<Panel_HuiFang>(PanelType.HuiFang);                    
                    ControlCenter.Instance.UpdatePlayerInfo();
                    break;
                case GAME_STATE.ZHONGYU:
                    ControlCenter.Instance.ShowZhongYu();
                    //Invoke("SetGameOver", 5);
                    break;
                case GAME_STATE.GAMEOVER:
                    ViewCenter.Instance.ShowPanel<Panel_GameOver>(PanelType.GameOver);
                    break;
                default:
                    break;
            }
        }

        private void ExitGameLogic()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sIsExitGame, "", delegate { Application.Quit(); }, "", null);
            }
        }

        private void SetGameOver()
        {
            SetGameState(GAME_STATE.GAMEOVER);
        }
    }
}

