using UnityEngine;
using System.Collections;
using cn.sharesdk.unity3d;

namespace DYD
{
    public class Panel_Hall : UIPanelBase
    {
        public override int Depth
        {
            get
            {
                return base.Depth;
            }
            set
            {
                base.Depth = value;
                GameUtility.FindDeepChild(gameObject, "Scroll View").GetComponent<UIPanel>().depth = value + 1;
            }
        }
        private UILabel label_Coin;
        private UILabel label_Diamond;
        private UILabel label_Name;
        private UILabel label_ID;
        private UISprite sprite_HeadIcon;
        public override void Init(PanelType type)
        {
            base.Init(type);
            EventInit();
            label_Coin = GameUtility.FindDeepChild(gameObject, "Coin").GetComponent<UILabel>();
            label_Diamond = GameUtility.FindDeepChild(gameObject, "Diamond").GetComponent<UILabel>();
            label_Name = GameUtility.FindDeepChild(gameObject, "Name").GetComponent<UILabel>();
            label_ID = GameUtility.FindDeepChild(gameObject, "ID").GetComponent<UILabel>();
            sprite_HeadIcon = GameUtility.FindDeepChild(gameObject, "HeadIcon").GetComponent<UISprite>();
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            RefleshPlayerInfo();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_自由匹配").gameObject).onClick = delegate
            {  
                //Handheld.PlayFullScreenMovie("MJ_Movie.mp4", Color.black, FullScreenMovieControlMode.Full);
                //GameApp.Instance.SetGameState(GAME_STATE.MAIN);
                DealCommand.Instance.SendJoinRoom_自由匹配();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_创建房间").gameObject).onClick = delegate
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.CreateRoom);                
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_加入游戏").gameObject).onClick = delegate
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.JoinRoom);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_玩法介绍").gameObject).onClick = delegate
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.Help);                
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_战绩").gameObject).onClick = delegate
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.Record);                                
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_设置").gameObject).onClick = delegate
            {
                ViewCenter.Instance.ShowPanel<UIPanelBase>(PanelType.Setting);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_分享").gameObject).onClick = delegate
            {
                ShareContent content = new ShareContent();
                content.SetText(Tags.Title_Name + "，快来和我一起玩游戏吧！");
                content.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
                //content.SetImageUrl("http://fir.im/1946");
                content.SetTitle(Tags.Title_Name);
                content.SetTitleUrl("http://fir.im/1946");
                content.SetSite("Mob-ShareSDK");
                content.SetSiteUrl("http://fir.im/1946");
                content.SetUrl("http://fir.im/1946");
                content.SetComment("描述");
                //content.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
                content.SetShareType(ContentType.Webpage);
                WeiXinCenter.Instance.ShareWeChatMoments(content);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_反馈").gameObject).onClick = delegate
            {
                ControlCenter.Instance.CaptureShare();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Add_Gold").gameObject).onClick = delegate
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sContactAdmin + DataCenter.Instance.gamedata.AdministratorName, "", null);                
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Add_Diamond").gameObject).onClick = delegate
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sContactAdmin + DataCenter.Instance.gamedata.AdministratorName, "", null);                                
            };

            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "开始录音").gameObject).onClick = delegate
            {
                GameApp.Instance.StartRecording();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "结束录音").gameObject).onClick = delegate
            {
                GameApp.Instance.EndRecording();
            };

            NotificationCenter.Instance.AddNotificationEvent(NotificateMsgType.PLAYER_SCORE, SetLabel_Coin);
            NotificationCenter.Instance.AddNotificationEvent(NotificateMsgType.PLAYER_DIAMOND, SetLabel_Diamond);
        }

        private void SetLabel_Coin(Notification node)
        {
            int coin = (int)node.data;
            SetLabel_Coin(coin);
        }
        private void SetLabel_Diamond(Notification node)
        {
            int diamond = (int)node.data;
            SetLabel_Diamond(diamond);
        }

        private void SetLabel_Coin(int num)
        {
            label_Coin.text = "" + num;
        }
        private void SetLabel_Diamond(int num)
        {
            label_Diamond.text = "" + num;
        }
        private void SetLabel_Name(string name)
        {
            label_Name.text = name;
        }
        private void SetLabel_ID(string id)
        {
            label_ID.text = id;
        }

        public void RefleshPlayerInfo()
        {
            PlayerInfo info = DataCenter.Instance.gamedata.playerInfo;
            sprite_HeadIcon.atlas = null;
            sprite_HeadIcon.atlas = ResourcesManager.GetAtlas(Tags.MJ_DynamicAtlas);
            sprite_HeadIcon.spriteName = info.WXTX_Icon_SpriteName;
            SetLabel_Coin(info.coin);
            SetLabel_Diamond(info.diamond);
            SetLabel_Name(info.WXName);
            SetLabel_ID(info.GameID);
        }
    }
}

