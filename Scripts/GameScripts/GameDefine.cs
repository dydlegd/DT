
namespace DYD
{
    public enum MJType
    {
        Null,
        万_1 = 1,
        万_2,
        万_3,
        万_4,
        万_5,
        万_6,
        万_7,
        万_8,
        万_9,
        条_1,
        条_2,
        条_3,
        条_4,
        条_5,
        条_6,
        条_7,
        条_8,
        条_9,
        筒_1,
        筒_2,
        筒_3,
        筒_4,
        筒_5,
        筒_6,
        筒_7,
        筒_8,
        筒_9,       
        MJ_东,
        MJ_南,
        MJ_西,
        MJ_北,
        MJ_红中,        
        MJ_发,
        MJ_白板,
        MJ_反面,        
    }


    /// <summary>
    /// 坐位方向
    /// </summary>
    public enum SEAT_DIR
    {        
        DIR_BOTTOM,
        DIR_RIGHT,
        DIR_TOP,
        DIR_LEFT,
        DIR_NULL
    }
    /// <summary>
    /// 东西南北方向
    /// </summary>
    public enum DNXB_DIR
    {
        东,
        南,
        西,        
        北,
        Null
    }

    /// <summary>
    /// 吃碰框类型
    /// </summary>
    public enum CPG_TYPE
    {
        吃,
        碰,
        杠,
        暗杠
    }

    /// <summary>
    /// 麻将状态
    /// </summary>
    public enum MJ_STAGE
    {
        CPGResult,//吃碰杠的麻将选择
        IN,//手中的麻将
        OUT,//打出去的麻将
        CPG//吃碰杠的麻将
    }

    /// <summary>
    /// 结果类型（吃，碰，杠，糊）
    /// </summary>
    public enum ResultType
    {
        吃,
        碰,
        杠,
        暗杠,
        糊,
        过,
        自摸,
        Max
    }

    public enum HuType
    {
        Null,
        平胡,
        门清,
        七对,
        碰碰胡,
        清一色,
        混一色,
        海底捞月,
        杠上炮,
        十三幺,
        抢杠,
        全球赢,
        吃三比
    }

    public enum GameOver_Result
    {
        赢,
        输,
        流局,
        不输不赢
    }

    public enum GAME_MODE
    {
        Null,
        自由匹配,
        创建房间,
        比赛房间
    }

    public enum AREA_ENUM
    {
        Null,
        柳州,
        桂林,
        福建
    }
}
