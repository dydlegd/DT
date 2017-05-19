using UnityEngine;
using System.Collections;

namespace DYD
{

    public class GD
    {
        public const bool SIMULATE_CHUPAI = false;//模拟出牌
        public const bool RELEASE = false;
        public const bool ENABLELOG = true;
        public const int DESIGN_SCREEN_W = 1280;
        public const int DESIGN_SCREEN_H = 720;
        public const int DESIGN_SCREEN_W_2 = DESIGN_SCREEN_W / 2;
        public const int DESIGN_SCREEN_H_2 = DESIGN_SCREEN_H / 2;
        public static float SCAL_W = 1f;
        public static float SCAL_H = 1f;
        public const int PLAYER_SUM = 4;
        public const string HEAD_ICON_NAME = "MyHead";

        //StreamingAssert路径
        public static readonly string PathURL =
#if UNITY_ANDROID   //安卓
        "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE  //iPhone  
        Application.dataPath + "/Raw/";  
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR  //windows平台和web平台  
        "file://" + Application.dataPath + "/StreamingAssets/";  
#else  
        string.Empty;  
#endif  

//#if UNITY_EDITOR
//        string filepath = Application.dataPath +"/StreamingAssets"+"/my.xml";
//#elif UNITY_IPHONE
//      string filepath = Application.dataPath +"/Raw"+"/my.xml";
//#elif UNITY_ANDROID
//      string filepath = "jar:file://" + Application.dataPath + "!/assets/"+"/my.xml;
//#endif
    }
}
