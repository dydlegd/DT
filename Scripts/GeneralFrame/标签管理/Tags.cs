using UnityEngine;
using System.Collections;

namespace DYD
{

    public class Tags
    {
        public struct TextPrompt
        {
            public const string sIsExitGame = "是否退出游戏？";
            public const string sNetConnectFailed = "网络连接失败，请检查网络后重试";
            public const string sNetConnecting = "正在连接网络，请稍候...";
            public const string sNetDisconnect = "网络断开连接，请检查网络后重试！";
            public const string sContactAdmin = "请加微信号：代理咨询请联系\n";
            public const string sCreateRoomSucceed = "创建房间成功！";
            public const string sCreateRoomFail_LessDiamond = "创建房间失败！\n玩家砖石不够";
            public const string sCreateRoomFail_FullRoom = "创建房间失败！\n房间已满";
            public const string sJoinRoomFail_NoFind = "没有找到该房间";
            public const string sEnterGameFail = "登录游戏失败";
            public const string sOnline = "您已经在线";
            public const string sJoinRoomFail_RoomFullPeople = "房间已满员，加入失败";
            public const string sJoinRoomFail_FullPeople = "服务器已满员，加入失败";
            public const string sJoinRoomFail_CoinLess = "金币不足，加入失败";
            public const string sAgreeProtocal = "未同意用户使用协议";
            public const string sIsReturnToHall = "是否返回大厅？";
            public const string sIsDismissRoom = "您确定解散房间吗？";
            public const string sIsLeaveRoom = "是否离开游戏？";
            public const string sDismissRoomApply = "申请解散房间，是否同意？";
            public const string sDismissRoomFail = "其他玩家不同意解散";
            public const string sDismissRoomWait = "正在等待其他玩家是否同意解散";
            public const string sLeaveRoomFail = "其他玩家不同意离开";
            public const string sLeaveRoomWait = "正在等待其他玩家是否同意离开";
            public const string sLeaveRoomApply = "申请离开房间，是否同意？";            
            public const string sHuiFang_NoRecord = "没有录像";
            public const string sHuiFang_FirstJu = "已经是第一局了";
            public const string sHuiFang_EndJu = "已经是最后一局了";
        }

        public const string Title_Name = "桂林麻将";

        public const string MJ_SpriteName_L = "MJ_8";
        public const string MJ_SpriteName_R = "MJ_9";
        public const string MJ_SpriteName_T = "MJ_5";
        public const string MJ_SpriteName_Back_B = "MJ_2";
        public const string MJ_SpriteName_Back_T = "MJ_2";
        public const string MJ_SpriteName_Back_L = "MJ_6";
        public const string MJ_SpriteName_Back_R = "MJ_6";

        public const string MJ_DynamicAtlas = "DynamicAtlas";

        public struct GameOver_Result_Name
        {
            public const string Result_赢 = "js_yl";
            public const string Result_输 = "lose";
            public const string Result_不输不赢 = "nothing";
            public const string Result_流局 = "liuju";
            public const string Result_自摸 = "zimo";
            public const string Result_胡牌 = "hupai";
            public const string Result_点炮 = "dianpao";
        }        

        public struct Audio_Name
        {
            public static string[] bg = new string[] { "bg_1" };
            public const string ErrorPrompt = "confirm";
            public const string EffectHu = "effect_hu";
            public const string DaPai = "dapai";
            public const string Lost = "Lost";
            public const string ReceiveMsg = "ReceiveMsg";
            public const string UpLine = "Upline";
            public const string Win = "win";
            public const string Btn = "Button32";

            public struct CPG
            {
                public const string Boy_chi = "boy_chi";
                public const string Boy_gang = "boy_gang";
                public const string Boy_gangkai = "boy_gangkai";
                public const string Boy_hu = "boy_hu";
                public const string Boy_peng = "boy_peng";
                public const string Boy_zimo = "boy_zimo";

                public const string Girl_chi = "girl_chi";
                public const string Girl_gang = "girl_gang";
                public const string Girl_gangkai = "girl_gangkai";
                public const string Girl_hu = "girl_hu";
                public const string Girl_hu2 = "girl_hu2";
                public const string Girl_peng = "girl_peng";
                public const string Girl_zimo = "girl_zimo";
            }

            public struct MJ
            {
                public const string Tiao_Boy_1 = "1tiao";
                public const string Tiao_Boy_2 = "2tiao";
                public const string Tiao_Boy_3 = "3tiao";
                public const string Tiao_Boy_4 = "4tiao";
                public const string Tiao_Boy_5 = "5tiao";
                public const string Tiao_Boy_6 = "6tiao";
                public const string Tiao_Boy_7 = "7tiao";
                public const string Tiao_Boy_8 = "8tiao";
                public const string Tiao_Boy_9 = "9tiao";
                public const string Tong_Boy_1 = "1tong";
                public const string Tong_Boy_2 = "2tong";
                public const string Tong_Boy_3 = "3tong";
                public const string Tong_Boy_4 = "4tong";
                public const string Tong_Boy_5 = "5tong";
                public const string Tong_Boy_6 = "6tong";
                public const string Tong_Boy_7 = "7tong";
                public const string Tong_Boy_8 = "8tong";
                public const string Tong_Boy_9 = "9tong";
                public const string Wan_Boy_1 = "1wan";
                public const string Wan_Boy_2 = "2wan";
                public const string Wan_Boy_3 = "3wan";
                public const string Wan_Boy_4 = "4wan";
                public const string Wan_Boy_5 = "5wan";
                public const string Wan_Boy_6 = "6wan";
                public const string Wan_Boy_7 = "7wan";
                public const string Wan_Boy_8 = "8wan";
                public const string Wan_Boy_9 = "9wan";
                public const string Feng_Boy_东 = "mj_boy_东";
                public const string Feng_Boy_南 = "mj_boy_南";
                public const string Feng_Boy_西 = "mj_boy_西";
                public const string Feng_Boy_北 = "mj_boy_北";
                public const string Feng_Boy_中 = "mj_boy_红中";
                public const string Feng_Boy_发 = "mj_boy_发财";
                public const string Feng_Boy_白 = "mj_boy_白板";


                public const string Tiao_Girl_1 = "g_1tiao";
                public const string Tiao_Girl_2 = "g_2tiao";
                public const string Tiao_Girl_3 = "g_3tiao";
                public const string Tiao_Girl_4 = "g_4tiao";
                public const string Tiao_Girl_5 = "g_5tiao";
                public const string Tiao_Girl_6 = "g_6tiao";
                public const string Tiao_Girl_7 = "g_7tiao";
                public const string Tiao_Girl_8 = "g_8tiao";
                public const string Tiao_Girl_9 = "g_9tiao";
                public const string Tong_Girl_1 = "g_1tong";
                public const string Tong_Girl_2 = "g_2tong";
                public const string Tong_Girl_3 = "g_3tong";
                public const string Tong_Girl_4 = "g_4tong";
                public const string Tong_Girl_5 = "g_5tong";
                public const string Tong_Girl_6 = "g_6tong";
                public const string Tong_Girl_7 = "g_7tong";
                public const string Tong_Girl_8 = "g_8tong";
                public const string Tong_Girl_9 = "g_9tong";
                public const string Wan_Girl_1 = "g_1wan";
                public const string Wan_Girl_2 = "g_2wan";
                public const string Wan_Girl_3 = "g_3wan";
                public const string Wan_Girl_4 = "g_4wan";
                public const string Wan_Girl_5 = "g_5wan";
                public const string Wan_Girl_6 = "g_6wan";
                public const string Wan_Girl_7 = "g_7wan";
                public const string Wan_Girl_8 = "g_8wan";
                public const string Wan_Girl_9 = "g_9wan";
                public const string Feng_Girl_东 = "mj_girl_东";
                public const string Feng_Girl_南 = "mj_girl_南";
                public const string Feng_Girl_西 = "mj_girl_西";
                public const string Feng_Girl_北 = "mj_girl_北";
                public const string Feng_Girl_中 = "mj_girl_红中";
                public const string Feng_Girl_发 = "mj_girl_发财";
                public const string Feng_Girl_白 = "mj_girl_白板";
            }
            
            public struct Chat
            {
                public static string[] Boy_Chat = new string[] 
                { 
                    "不要吵了，专心玩游戏吧", 
                    "不要走，决战到天亮", 
                    "很高兴见到各位",
                    "不好意思，我要离开一会",
                    "和你合作真是太愉快了",
                    "快点吧，我等到花都谢了",
                    "你的牌打的太好了",
                    "你是MM，还是GG",
                    "我们交个朋友吧",
                    "下次再玩吧，我要走了",
                    "再见了，我会想念大家的",
                    "怎么又断线了，网络怎么这么差"
                };

                public static string[] Girl_Chat = new string[] 
                { 
                    "不要吵了，专心玩游戏吧", 
                    "不要走，决战到天亮", 
                    "很高兴见到各位",
                    "不好意思，我要离开一会",
                    "和你合作真是太愉快了",
                    "快点吧，我等到花都谢了",
                    "你的牌打的太好了",
                    "你是MM，还是GG",
                    "我们交个朋友吧",
                    "下次再玩吧，我要走了",
                    "再见了，我会想念大家的",
                    "怎么又断线了，网络怎么这么差"
                };
            }
        }
    }

}