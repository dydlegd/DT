using UnityEngine;
using System.Collections;

namespace DYD
{
    public class DataSaveCenter : SingletonMonoBase<DataSaveCenter>
    {

        public void SavePlayerInfo()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            PlayerPrefs.SetString("WX_Name", playerInfo.WXName);
            PlayerPrefs.SetString("WX_ID", playerInfo.WXID);
            //GDFunc.SaveJpg(playerInfo.WXTX_Texture, Application.persistentDataPath + "/" + GD.HEAD_ICON_NAME + ".jpg");
            GDFunc.SaveBytes(playerInfo.WXTX_Datas, Application.persistentDataPath + "/" + GD.HEAD_ICON_NAME);
            PlayerPrefs.Save();
        }

        public bool ReadPlayerInfo()
        {
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            playerInfo.WXName = PlayerPrefs.GetString("WX_Name", "");
            playerInfo.WXID = PlayerPrefs.GetString("WX_ID", "");
            if (playerInfo.WXName != "" && playerInfo.WXID != "")
            {
                playerInfo.WXTX_Icon_SpriteName = GD.HEAD_ICON_NAME;
                //playerInfo.WXTX_Texture = GDFunc.Instance.ReadTexture("file:///"+Application.persistentDataPath + "/" + GD.HEAD_ICON_NAME + ".jpg");
                playerInfo.WXTX_Texture.name = GD.HEAD_ICON_NAME;
                //playerInfo.WXTX_Datas = playerInfo.WXTX_Texture.EncodeToJPG();
                playerInfo.WXTX_Datas = GDFunc.ReadBytes(Application.persistentDataPath + "/" + GD.HEAD_ICON_NAME);
                playerInfo.WXTX_Texture.LoadImage(playerInfo.WXTX_Datas);
                DynamicAtlas.Instance.AddTexture(playerInfo.WXTX_Texture);                
                return true;
            }
            return false;
        }
    }
}

