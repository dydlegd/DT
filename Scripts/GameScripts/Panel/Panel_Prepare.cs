using UnityEngine;
using System.Collections;
using cn.sharesdk.unity3d;

namespace DYD
{
    public class Panel_Prepare : UIPanelBase
    {
        private UILabel label_当前玩法;
        public override void Init(PanelType type)
        {
            EventInit();
            for (int i = 0; i < 4; i++)
            {
                SetPrepare(i, false);
            }

            base.Init(type);
            AddColliderMode(PanelColliderMode.Normal);


            label_当前玩法 = GameUtility.FindDeepChild(gameObject, "玩法").GetComponent<UILabel>();
        }

        public override void ShowPanel()
        {
            base.ShowPanel();            
            if (DataCenter.Instance.gamedata.gameMode==GAME_MODE.创建房间)
            {
                GameUtility.FindDeepChild(gameObject, "房间号").GetComponent<UILabel>().text = "房间号：" + DataCenter.Instance.gamedata.RoomNum;                
            }
            else
            {
                GameUtility.FindDeepChild(gameObject, "房间号").GetComponent<UILabel>().text = "";
            }

            GameUtility.FindDeepChild(gameObject, "Btn_DismissRoom").gameObject.SetActive(DataCenter.Instance.gamedata.gameMode == GAME_MODE.创建房间);
            GameUtility.FindDeepChild(gameObject, "Btn_InviteFriends").gameObject.SetActive(DataCenter.Instance.gamedata.gameMode == GAME_MODE.创建房间);
            RefleshPlayerData();
        }

        private void EventInit()
        {

            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_Close").gameObject).onClick = delegate
            {
                ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sIsLeaveRoom, "", delegate { DealCommand.Instance.SendLeaveRoom(); }, "", null);
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_DismissRoom").gameObject).onClick = delegate
            {
                if (DataCenter.Instance.gamedata.IsFangZhu)
                {
                    ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sIsDismissRoom, "", delegate { DealCommand.Instance.SendDissolveRoom(); }, "", null);
                }
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_InviteFriends").gameObject).onClick = delegate
            {
                ShareContent content = new ShareContent();
                content.SetText(Tags.Title_Name + ",快来和我一起玩游戏吧！\n房间号：" + DataCenter.Instance.gamedata.RoomNum + "\n玩法：" + label_当前玩法.text);
                content.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
                content.SetTitle(Tags.Title_Name + "！房号：" + DataCenter.Instance.gamedata.RoomNum);
                content.SetTitleUrl("http://fir.im/1946");
                content.SetSite("Mob-ShareSDK");
                content.SetSiteUrl("http://fir.im/1946");
                content.SetUrl("http://fir.im/1946");
                content.SetComment("描述");
                //content.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
                content.SetShareType(ContentType.Webpage);
                WeiXinCenter.Instance.ShareWeChatFriend(content);
            };
        }

        public void RefleshPlayerData()
        {
            for (int i = 0; i < 4; i++)
            {
                if (DataCenter.Instance.players[i].playerInfo.WXName != "")
                {
                    SetPrepare(i, true);
                }     
                else
                {
                    SetPrepare(i, false);
                }
            }
        }

        private void SetPrepare(int playerId, bool bPrepare)
        {
            PlayerInfo player = DataCenter.Instance.players[playerId].playerInfo;
            SEAT_DIR dir = GameGD.GetSeatDir(player.DNXBDir);
            GameObject go = null;
            switch (dir)
            {
                case SEAT_DIR.DIR_BOTTOM:
                    go = GameUtility.FindDeepChild(gameObject, "Player_B").gameObject;
                    break;
                case SEAT_DIR.DIR_RIGHT:
                    go = GameUtility.FindDeepChild(gameObject, "Player_R").gameObject;
                    break;
                case SEAT_DIR.DIR_TOP:
                    go = GameUtility.FindDeepChild(gameObject, "Player_T").gameObject;
                    break;
                case SEAT_DIR.DIR_LEFT:
                    go = GameUtility.FindDeepChild(gameObject, "Player_L").gameObject;
                    break;
                case SEAT_DIR.DIR_NULL:
                    break;
                default:
                    break;
            }

            if (go != null)
            {
                UISprite headIconSprite = GameUtility.FindDeepChild(go, "Head").GetComponent<UISprite>();
                headIconSprite.atlas = null;
                headIconSprite.atlas = ResourcesManager.GetAtlas(Tags.MJ_DynamicAtlas);
                if (bPrepare)
                    headIconSprite.spriteName = player.WXTX_Icon_SpriteName;
                else
                    headIconSprite.spriteName = "";

                UISprite prepareOKSprite = GameUtility.FindDeepChild(go, "PrepareOK").GetComponent<UISprite>();
                prepareOKSprite.gameObject.SetActive(bPrepare);

                UILabel nameLabel = GameUtility.FindDeepChild(go, "Name").GetComponent<UILabel>();
                if (bPrepare)
                    nameLabel.text = player.WXName;
                else
                    nameLabel.text = "";

                UILabel coinLabel = GameUtility.FindDeepChild(go, "Coin").GetComponent<UILabel>();
                if (bPrepare)
                    coinLabel.text = "" + player.coin;
                else
                    coinLabel.text = "";
            }
        }

        private void UpdateHeadIcon(Notification node)
        {
            int playerId = (int)node.data;

            SetPrepare(playerId, true);
        }

        public void SetLabel_玩法(string text)
        {
            label_当前玩法.text = text;
        }
    }
}

