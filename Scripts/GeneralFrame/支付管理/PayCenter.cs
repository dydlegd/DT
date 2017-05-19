using UnityEngine;
using System.Collections;

namespace DYD
{
    public class PayCenter : SingletonMonoBase<PayCenter>
    {

        public void Pay(string foodName, float price)
        {
            try
            {
#if UNITY_ANDROID
            AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            joActivity.Call("pay_ByWX",price, foodName);
#endif
            }
            catch (System.Exception e)
            {
            }
        }

        public void PayCallBack(string result)
        {
            ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox("支付结果\n"+result, "", null);                
        }
    }

}
