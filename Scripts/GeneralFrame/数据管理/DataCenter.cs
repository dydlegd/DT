using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{

    public class DataCenter : SingletonBase<DataCenter>
    {

        public PlayerData[] players = new PlayerData[GD.PLAYER_SUM];
        public GameData gamedata = new GameData();
        public List<HistroyRecordData> histroyRecordList = new List<HistroyRecordData>();
        public HFData hFdata = new HFData();
        

        public bool Init()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new PlayerData(i);
            }
            

            Debuger.Log("DataCenter Init Succeed !");
            return true;
        }

        //是否有人胡牌
        public bool HasHuPai()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if(players[i].gameOver.huTypeList.Count>0)
                {
                    return true;
                }
            }
            return false;
        }

        public PlayerData GetPlayerData(string wxName)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].playerInfo.WXName == wxName)
                {
                    return players[i];
                }
            }
            return null;
        }

        public void GoToNextPlate(bool bClearWXTX=false)
        {
            gamedata.GoToNextPlate();
            for (int i = 0; i < players.Length; i++)
            {
                players[i].GoToNextPlate(bClearWXTX);
            }
        }

        //所有头像数据是否接收完毕
        public bool IsWXTXRecComplete()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].playerInfo.WXTX_Icon_SpriteName == "" && i != gamedata.PlayerID)
                {
                    return false;
                }
            }
            return true;
        }
    }

}