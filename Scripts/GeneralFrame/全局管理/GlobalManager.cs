using UnityEngine;
using System.Collections;
using System.IO;

namespace DYD
{

    public class GlobalManager : SingletonBase<GlobalManager>
    {

        public void Init()
        {
            GD.SCAL_W = (float)Screen.width / GD.DESIGN_SCREEN_W;
            GD.SCAL_H = (float)Screen.height / GD.DESIGN_SCREEN_H;

            Debuger.Log("GlobalManager Init Succeed !");
        }

        public static bool ReturnToHall()
        {
            if (GDFunc.OpenPackage("com.lzq.Gr_Login"))
            {
                WriteLoginData(1);
                Application.Quit();
                return true;
            }
            return false;
        }

        public static void WriteLoginData(int value)
        {
            string path = GDFunc.GetPersistentPath("com.lzq.Gr_Login");

            string text = "" + value;

            GDFunc.DeleteFile(path, "LoginInfo.txt");
            GDFunc.CreateFile(path, "LoginInfo.txt", text);
        }
    }

}