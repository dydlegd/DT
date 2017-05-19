using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class Hall_DealCommand : SingletonMonoBase<Hall_DealCommand>
    {
        Hall_NetworkClient network = new Hall_NetworkClient();
        private static List<string> mCmdStrList = new List<string>();

        void Start()
        {
            Init();
        }

        public void Init()
        {
            network.Init();
        }

        public void ReConnect()
        {
            network.Connect();
        }

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

        public void OnDestroy()
        {
            network.CloseNetwork();
        }

        public string GetCmdType()
        {
            mCmdStrList.Clear();
            NetDataBuf dataBuf = null;
            string type = network.GetCommand(ref mCmdStrList, ref dataBuf);
            if (type != "")
            {
                Debuger.Log("GetCommand :" + type);
                switch (type)
                {
                    case "GETVER":
                        ParseVersion(dataBuf);
                        break;
                    default: break;
                }
            }
            return type;
        }

        private void ParseVersion(NetDataBuf dataBuf)
        {
            string[] arrstr = dataBuf.sb.ToString().Split('/');
            int index = 2;
            GameInfoCenter.Instance.GetGameInfo(GameType.麻将).Version_Server = Int32.Parse(arrstr[index]);
            Hall_GameApp.Instance.CheckHallUpdate();
        }

        public void SendGetVersion()
        {
            network.SendGetVersion();
        }
    }

}
