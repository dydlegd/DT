using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class Panel_Login : UIPanelBase
    {
        private bool bCanClick = true;
        public override void Init(PanelType type)
        {
            base.Init(type);
            AddColliderMode(PanelColliderMode.WithBg);
            EventInit();
        }

        void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_微信登录").gameObject).onClick = delegate
            {
                if (bCanClick && DealCommand.Instance.IsConnected())
                {
                    bool bAgree = GameUtility.FindDeepChild(gameObject, "同意").GetComponent<UIToggle>().value;
                    if (bAgree)
                    {
                        if (DataSaveCenter.Instance.ReadPlayerInfo())
                        {
                            DealCommand.Instance.SendLogin();                            
                        }
                        else
                        {
                            WeiXinCenter.Instance.Login();
                            WeiXinCenter.Instance.CallBack_AuthResult = CallBack_AuthResult;
                            WeiXinCenter.Instance.CallBack_GetUserInfoResult = CallBack_GetUserInfoResult;
                        }
                    }
                    else
                    {
                        ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sAgreeProtocal, "", null);
                    }
                    bCanClick = false;
                }                
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_切换帐号").gameObject).onClick = delegate
            {
                if (DealCommand.Instance.IsConnected())
                {
                    if (bCanClick)
                    {
                        WeiXinCenter.Instance.Login();
                        WeiXinCenter.Instance.CallBack_AuthResult = CallBack_AuthResult;
                        WeiXinCenter.Instance.CallBack_GetUserInfoResult = CallBack_GetUserInfoResult;
                        bCanClick = false;
                    }                    
                }
            };
        }

        private void SetCanClick()
        {
            bCanClick = true;
        }

        private void CallBack_AuthResult(bool bSucceed)
        {
            bCanClick = true;
        }

        private void CallBack_GetUserInfoResult(bool bSucceed)
        {
            if (bSucceed)
            {
                PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
                playerInfo.WXName = WeiXinCenter.Instance.WXInfo.Name;
                playerInfo.WXID = WeiXinCenter.Instance.WXInfo.Unionid;
                //playerInfo.sex = WeiXinLogin.Instance.WXInfo.Sex;
                WWWHelperCenter.Instance.DownLoad(WeiXinCenter.Instance.WXInfo.HeadUrl, DownLoadResType.Image, HeadDownLoadCall);
                DealCommand.Instance.SendLogin();
                bCanClick = false;
            }
            else
            {
                bCanClick = true;
            }
        }

        private void HeadDownLoadCall(DownLoadResDesc res)
        {
            Debuger.Log("头像下载完成");
            PlayerInfo playerInfo = DataCenter.Instance.gamedata.playerInfo;
            playerInfo.WXTX_Icon_SpriteName = GD.HEAD_ICON_NAME;
            Debuger.Log("原始头像数据长度：" + res.bytes.Length);
            playerInfo.WXTX_Texture = res.texture;
            //Debuger.Log("头像数据长度JPG：" + res.texture.EncodeToJPG().Length);
            //Debuger.Log("缩放后头像数据长度JPG：" + GDFunc.ScaleTexture(res.texture, 100, 100).EncodeToJPG().Length);
            //Debuger.Log("缩放后头像数据长度PNG：" + GDFunc.ScaleTexture(res.texture, 100, 100).EncodeToPNG().Length);
            //playerInfo.WXTX_Texture = GDFunc.ScaleTexture(res.texture, 100, 100);
            //playerInfo.WXTX_Texture.LoadImage(res.bytes);
            //playerInfo.WXTX_Texture.LoadImage(res.texture.EncodeToJPG());
            playerInfo.WXTX_Texture.name = playerInfo.WXTX_Icon_SpriteName;
            playerInfo.WXTX_Datas = res.bytes;
            //playerInfo.WXTX_Datas = res.texture.EncodeToJPG();
            //playerInfo.WXTX_Datas = playerInfo.WXTX_Texture.EncodeToPNG();
            Debuger.Log("最终头像数据长度：" + playerInfo.WXTX_Datas.Length);
            DealCommand.Instance.SendWXTX();
            DynamicAtlas.Instance.AddTexture(playerInfo.WXTX_Texture);
              
            //List<Texture> texList = new List<Texture>();
            //texList.Add(playerInfo.WXTX_Texture);            
            //CreateAtlas.CreatAtlasFromTex(ResourcesManager.GetAtlas(Tags.MJ_DynamicAtlas), texList);

            ViewCenter.Instance.GetPanel<Panel_Hall>(PanelType.Hall).RefleshPlayerInfo();
            ControlCenter.Instance.UpdatePlayerInfo();
            DataSaveCenter.Instance.SavePlayerInfo();
        }
    }
}

