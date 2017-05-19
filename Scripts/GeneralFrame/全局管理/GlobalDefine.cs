
namespace DYD
{
    /// <summary>
    /// 性别
    /// </summary>
    public enum SEX
    {
        Boy,
        Girl
    }

    public enum GAME_STATE
    {
        Null,
        LOGIN,//麻将登录
        HALL,//麻将大厅
        PREPARE,//准备阶段
        MAIN,//主游戏
        ZHONGYU,
        GAMEOVER,//游戏结束
        HUIFANG//回放
    }

    public enum PanelColliderMode
    {
        None,      // 显示该界面不包含碰撞背景
        Normal,    // 碰撞透明背景
        WithBg,    // 碰撞非透明背景
    }

    public enum PanelType
    {
        Login,
        Main,
        BackStage,
        Prompt,
        Player_LEFT,
        Player_BOTTOM,
        Player_RIGHT,
        Player_TOP,
        Hall,
        UIManager,
        GameOver,
        GameOver_Record,
        Setting,
        Exit,
        JoinRoom,
        MessageBox,
        CreateRoom,
        Prepare,
        Help,
        Record,
        Chat,
        HuiFang
    }

    public enum Panel_Depth
    {
        Zero,
        Login,
        Hall,
        Help,
        Record,
        JoinRoom,
        CreateRoom,
        Prepare,
        Main,
        Player_Bottom,
        Player_Top,
        Player_Left,        
        Player_Right,        
        BackStage = 20,
        UIManager,
        HuiFang,
        Chat,
        Prompt,
        GameOver,
        GameOver_Record,
        Setting,
        MessageBox,
        Exit
    }
}