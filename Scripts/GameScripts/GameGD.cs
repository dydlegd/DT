using UnityEngine;
using System.Collections;

namespace DYD
{
    public class GameGD : SingletonBase<GameGD>
    {
        public const int JU_SHU_ITEM_SUM = 3;//局数选择的数量
        public const int PLAYRULE_1_ITEM_SUM = 3;//玩法规则1选择的数量
        public const int PLAYRULE_2_ITEM_SUM = 3;//玩法规则1选择的数量

        public static int JU_SHU_START_INDEX = 0;
        public static int JU_SHU_END_INDEX = JU_SHU_ITEM_SUM-1;
        public static int PLAYRULE_1_START_INDEX = JU_SHU_ITEM_SUM;
        public static int PLAYRULE_1_END_INDEX = JU_SHU_ITEM_SUM + PLAYRULE_1_ITEM_SUM - 1;
        public static int PLAYRULE_2_START_INDEX = JU_SHU_ITEM_SUM + PLAYRULE_1_ITEM_SUM;
        public static int PLAYRULE_2_END_INDEX = JU_SHU_ITEM_SUM + PLAYRULE_1_ITEM_SUM + PLAYRULE_2_ITEM_SUM - 1;
        public static string[] ruleArr = new string[JU_SHU_ITEM_SUM + PLAYRULE_1_ITEM_SUM + PLAYRULE_2_ITEM_SUM];//{ "4局", "8局", "不钓鱼", "钓4条鱼", "钓6条鱼", "一五九钓鱼", "跟庄钓鱼", "门清" };

        public const int MJ_OUT_ZORDER = 0;
        public const int MJ_IN_ZORDER = 100;

        public const int MJ_TOTAL_SUM = 136;
        public const int MJ_OUT_COL_SUM = 10;//打出去的麻将列数
        public const int MJ_SELECT_OFFSET = 10;//选中的麻将的偏移量

        public const int MJ_CPG_SPACE = 10;//吃碰杠的间距

        public const int MJ_BOTTOM_IN_W = 74;//手中麻将的宽度
        public const int MJ_BOTTOM_IN_H = 110;//手中麻将的高度
        public const int MJ_BOTTOM_IN_SPACE = MJ_BOTTOM_IN_W;//手中麻将的间距
        public const int MJ_BOTTOM_OUT_W = 40;
        public const int MJ_BOTTOM_OUT_H = 55;
        public const int MJ_BOTTOM_OUT_SPACE_W = 38;
        public const int MJ_BOTTOM_OUT_SPACE_H = 42;
        public const int MJ_BOTTOM_CPG_W = MJ_BOTTOM_OUT_W;
        public const int MJ_BOTTOM_CPG_H = MJ_BOTTOM_OUT_H;
        public const int MJ_BOTTOM_CPG_SPACE = MJ_BOTTOM_OUT_SPACE_W;

        public const int MJ_LEFT_IN_W = 22;
        public const int MJ_LEFT_IN_H = 59;
        public const int MJ_LEFT_IN_SPACE = 29;
        public const int MJ_LEFT_OUT_W = 48;
        public const int MJ_LEFT_OUT_H = 40;
        public const int MJ_LEFT_OUT_SPACE_W = 28;
        public const int MJ_LEFT_OUT_SPACE_H = MJ_LEFT_OUT_W;
        public const int MJ_LEFT_CPG_W = MJ_LEFT_OUT_W;
        public const int MJ_LEFT_CPG_H = MJ_LEFT_OUT_H;
        public const int MJ_LEFT_CPG_SPACE = MJ_LEFT_OUT_SPACE_W;

        public const int MJ_RIGHT_IN_W = MJ_LEFT_IN_W;
        public const int MJ_RIGHT_IN_H = MJ_LEFT_IN_H;
        public const int MJ_RIGHT_IN_SPACE = MJ_LEFT_IN_SPACE;
        public const int MJ_RIGHT_OUT_W = MJ_LEFT_OUT_W;
        public const int MJ_RIGHT_OUT_H = MJ_LEFT_OUT_H;
        public const int MJ_RIGHT_OUT_SPACE_W = MJ_LEFT_OUT_SPACE_W;
        public const int MJ_RIGHT_OUT_SPACE_H = MJ_LEFT_OUT_SPACE_H;
        public const int MJ_RIGHT_CPG_W = MJ_RIGHT_OUT_W;
        public const int MJ_RIGHT_CPG_H = MJ_RIGHT_OUT_H;
        public const int MJ_RIGHT_CPG_SPACE = MJ_RIGHT_OUT_SPACE_W;

        public const int MJ_TOP_IN_W = 39;
        public const int MJ_TOP_IN_H = 59;
        public const int MJ_TOP_IN_SPACE = 36;
        public const int MJ_TOP_OUT_W = MJ_BOTTOM_OUT_W;
        public const int MJ_TOP_OUT_H = MJ_BOTTOM_OUT_H;
        public const int MJ_TOP_OUT_SPACE_W = MJ_BOTTOM_OUT_SPACE_W;
        public const int MJ_TOP_OUT_SPACE_H = MJ_BOTTOM_OUT_SPACE_H;
        public const int MJ_TOP_CPG_W = MJ_BOTTOM_CPG_W;
        public const int MJ_TOP_CPG_H = MJ_BOTTOM_CPG_H;
        public const int MJ_TOP_CPG_SPACE = MJ_TOP_OUT_SPACE_W;

        public const int MJ_BOTTOM_CPG_RESULT_W = 40;
        public const int MJ_BOTTOM_CPG_RESULT_H = 55;

        //获取玩家相对自己的方位
        public static SEAT_DIR GetSeatDir(int playerId)
        {
            int dir = (int)(playerId - (int)DataCenter.Instance.gamedata.SelfDNXBDir);
            if (dir < 0) dir += 4;
            return (SEAT_DIR)(dir % 4);
        }

        //其他玩家相对自己的方位
        public static SEAT_DIR GetSeatDir(DNXB_DIR dnxbDir)
        {
            int temp = (int)(dnxbDir) - (int)(DataCenter.Instance.gamedata.SelfDNXBDir);
            if (temp < 0) temp += 4;
            SEAT_DIR dir = (SEAT_DIR)(temp % 4);
            return dir;
        }

        //其他玩家相对指定玩家ID的方位
        public static SEAT_DIR GetSeatDir(DNXB_DIR dnxbDir, int playerId)
        {
            return GetSeatDir(dnxbDir, DataCenter.Instance.players[playerId].playerInfo.DNXBDir);
        }

        //其他玩家相对指定玩家的方位
        public static SEAT_DIR GetSeatDir(DNXB_DIR dnxbDir, DNXB_DIR selfDir)
        {
            int temp = (int)(dnxbDir) - (int)(selfDir);
            if (temp < 0) temp += 4;
            SEAT_DIR dir = (SEAT_DIR)(temp % 4);
            return dir;
        }

        //获取相对自己的方位的玩家东南西北方
        public static DNXB_DIR GetDNXBDir(SEAT_DIR dir)
        {
            int temp = (int)(dir) + (int)(DataCenter.Instance.gamedata.SelfDNXBDir);
            DNXB_DIR dnxb = (DNXB_DIR)(temp % 4);
            return dnxb;
        }
        //根据相对自己的方位获取玩家ID
        public static int GetPlayerID(SEAT_DIR dir)
        {
            for (int i = 0; i < 4; i++)
            {
                if (GetSeatDir(i) == dir)
                    return i;
            }
            Debuger.LogError("GameGD GetPlayerID Error ! " + dir);
            return -1;
        }
    }

}
