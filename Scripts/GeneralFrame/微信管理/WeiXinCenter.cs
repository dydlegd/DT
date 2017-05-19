using UnityEngine;
using System.Collections;
using cn.sharesdk.unity3d;
using System.Text.RegularExpressions;
using System.Globalization;
using System;

namespace DYD
{
    public class WeiXinCenter : SingletonMonoBase<WeiXinCenter>
    {
        public class WeiXinInfo
        {
            public string Name { get; set; }
            public string Sex { get; set; }
            public string Unionid { get; set; }
            public string Openid { get; set; }
            public string HeadUrl { get; set; }
            public Texture2D HeadTex { get; set; }
            public string Province { get; set; }
            public string City { get; set; }
        }
        public WeiXinInfo WXInfo = new WeiXinInfo();

        private ShareSDK ssdk;

        public Action<bool> CallBack_AuthResult = null;
        public Action<bool> CallBack_GetUserInfoResult = null;
        public Action<bool> CallBack_ShareResult = null;
        
        void Start()
        {
            ssdk = this.gameObject.GetComponent<ShareSDK>();
            ssdk.authHandler = OnAuthResultHandler;
            ssdk.showUserHandler = OnGetUserInfoResultHandler;
            ssdk.shareHandler = OnShareResultHandler;
        }

        public void Login()
        {
            Debuger.Log("调用微信登录接口");
            ssdk.Authorize(PlatformType.WeChat);
        }

        public void ShareWeChatFriend(ShareContent content)
        {
#if UNITY_ANDROID||UNITY_IPHONE
            ssdk.ShareContent(PlatformType.WeChat, content);
#endif
        }

        public void ShareWeChatMoments(ShareContent content)
        {
#if UNITY_ANDROID||UNITY_IPHONE
            ssdk.ShowShareContentEditor(PlatformType.WeChatMoments, content);
#endif
        }

        //微信nickname字段转成中文
        private string DecodeNickName(string s)
        {
            Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
            return reUnicode.Replace(s, m =>
            {
                short c;
                if (short.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out c))
                {
                    return "" + (char)c;
                }
                return m.Value;
            });
        }

        void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
        {
            if (state == ResponseState.Success)
            {
                ssdk.GetUserInfo(PlatformType.WeChat);
                Debuger.Log("微信SDK调用成功！");
                if (CallBack_AuthResult != null) CallBack_AuthResult(true);
            }
            else if (state == ResponseState.Fail)
            {
                Debuger.Log("微信SDK调用失败！");
#if UNITY_ANDROID
            
#elif UNITY_IPHONE
			
#endif
                if (CallBack_AuthResult != null) CallBack_AuthResult(false);

            }
            else if (state == ResponseState.Cancel)
            {
                Debuger.Log("微信SDK调用Cancel！");
                if (CallBack_AuthResult != null) CallBack_AuthResult(false);
            }
            
        }

        void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
        {
            if (state == ResponseState.Success)
            {
                Debuger.Log("微信获取用户信息成功");

                string strInfo = MiniJSON.jsonEncode(result);
                Debuger.Log("用户信息：" + strInfo);
                
                string strHeadImgUrl = (string)result["headimgurl"];
                Debuger.Log("WXHeadImgUrl：" + strHeadImgUrl);
                WXInfo.HeadUrl = strHeadImgUrl;

                string strProvince = (string)result["province"];
                Debuger.Log("WXProvince：" + strProvince);
                WXInfo.Province = strProvince;


                string strUnionid = (string)result["unionid"];
                Debuger.Log("WXUnionid：" + strUnionid);
                WXInfo.Unionid = strUnionid;

                string strOpenid = (string)result["openid"];
                Debuger.Log("WXOpenid：" + strOpenid);
                WXInfo.Openid = strOpenid;

                string strName = DecodeNickName((string)result["nickname"]);
                Debuger.Log("WXName：" + strName);
                WXInfo.Name = strName;

                string strSex = (string)result["sex"].ToString();
                Debuger.Log("WXSex：" + strSex);
                WXInfo.Sex = strSex;

                if (CallBack_GetUserInfoResult != null) CallBack_GetUserInfoResult(true);
            }
            else if (state == ResponseState.Fail)
            {
                Debuger.Log("微信获取用户信息失败");
#if UNITY_ANDROID
            
#elif UNITY_IPHONE
			
#endif
                if (CallBack_GetUserInfoResult != null) CallBack_GetUserInfoResult(false);
            }
            else if (state == ResponseState.Cancel)
            {
                Debuger.Log("微信获取用户信息Cancel");
                if (CallBack_GetUserInfoResult != null) CallBack_GetUserInfoResult(false);
            }
        }
        void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
        {
            if (state == ResponseState.Success)
            {
                Debuger.Log("分享成功");
            }
            else if (state == ResponseState.Fail)
            {
#if UNITY_ANDROID
            
#elif UNITY_IPHONE
			
#endif
                Debuger.Log("分享失败："+"error code = "+result["error_code"]+",error msg = "+result["error_msg"]);

            }
            else if (state == ResponseState.Cancel)
            {
                Debuger.Log("分享取消");                
            }
            if (CallBack_ShareResult != null) CallBack_ShareResult(state == ResponseState.Success);
        }
    }
}

