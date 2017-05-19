using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_GameOver_Record : UIPanelBase
    {

        public override void Init(PanelType type)
        {
            base.Init(type);
            AddColliderMode(PanelColliderMode.Normal);

            EventInit();
        }

        public override void ShowPanel()
        {             
 	         base.ShowPanel();
             RefleshPanel();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_炫耀一下").gameObject).onClick = delegate
            {
                ControlCenter.Instance.CaptureShare();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Btn_返回大厅").gameObject).onClick = delegate
            {
                GameApp.Instance.SetGameState(GAME_STATE.HALL);
            };
        }

        private void RefleshPanel()
        {
            GameUtility.FindDeepChild(gameObject, "房间号").GetComponent<UILabel>().text = "房间号：" + DataCenter.Instance.gamedata.RoomNum;
            for (int i = 0; i < 4; i++)
            {
                RefleshData(i);
            }
        }

        private void RefleshData(int playerId)
        {
            GameOverRecordData data = DataCenter.Instance.players[playerId].gameOverRecord;
            PlayerInfo playerInfo = DataCenter.Instance.players[playerId].playerInfo;

            GameObject parent = GameUtility.FindDeepChild(gameObject, string.Format("Player_GameOver_Record_{0}",playerId)).gameObject;

            GameUtility.FindDeepChild(parent, "胡牌次数/Num").GetComponent<UILabel>().text = "" + data.HuPaiCount;
            GameUtility.FindDeepChild(parent, "点炮次数/Num").GetComponent<UILabel>().text = "" + data.DianPaoCount;
            GameUtility.FindDeepChild(parent, "公杠次数/Num").GetComponent<UILabel>().text = "" + data.GongGangCount;
            GameUtility.FindDeepChild(parent, "暗杠次数/Num").GetComponent<UILabel>().text = "" + data.AnGangCount;
            GameUtility.FindDeepChild(parent, "中码次数/Num").GetComponent<UILabel>().text = "" + data.ZhongMaCount;
            GameUtility.FindDeepChild(parent, "总战绩/Num").GetComponent<UILabel>().text = "" + data.ZongProfit;

            GameUtility.FindDeepChild(parent, "Head").GetComponent<UISprite>().atlas = null;
            GameUtility.FindDeepChild(parent, "Head").GetComponent<UISprite>().atlas = ResourcesManager.GetAtlas(Tags.MJ_DynamicAtlas);
            GameUtility.FindDeepChild(parent, "Head").GetComponent<UISprite>().spriteName = playerInfo.WXTX_Icon_SpriteName;
            GameUtility.FindDeepChild(parent, "Head/Name").GetComponent<UILabel>().text = playerInfo.WXName;
            GameUtility.FindDeepChild(parent, "Head/ID").GetComponent<UILabel>().text = playerInfo.GameID;

        }
    }
}

