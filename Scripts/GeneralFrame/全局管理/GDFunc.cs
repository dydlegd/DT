using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Net;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DYD
{

    public class GDFunc : SingletonMonoBase<GDFunc>
    {

        public static void Clamp(ref int value, int min, int max)
        {
            value = Mathf.Clamp(value, min, max);
        }
        public static void Clamp(ref float value, float min, float max)
        {
            value = Mathf.Clamp(value, min, max);
        }
        public static void ClampMin(ref int value, int min)
        {
            if (value < min) value = min;
        }
        public static void ClampMin(ref float value, float min, float max)
        {
            if (value < min) value = min;
        }
        public static void ClampMax(ref int value, int max)
        {
            if (value > max) value = max;
        }
        public static void ClampMax(ref float value, float max)
        {
            if (value > max) value = max;
        }

        public static Vector3 NGUIPosToWorld(GameObject obj)
        {
            Camera guiCamera = NGUITools.FindCameraForLayer(obj.layer);
            if (guiCamera == null)
            {
                return new Vector3();
            }

            Vector3 pos = guiCamera.WorldToScreenPoint(obj.transform.position);
            pos.z = 1f;
            pos = Camera.allCameras[0].ScreenToWorldPoint(pos);

            pos.z = 0f;
            return pos;
        }

        public static Vector3 NGUIPosToWorld(Vector3 point)
        {
            Vector3 pos = UICamera.currentCamera.WorldToScreenPoint(point);
            pos.z = 1f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = 0f;
            pos.x += UICamera.currentCamera.transform.position.x;
            pos.y += UICamera.currentCamera.transform.position.y;
            return pos;
        }

        public static Vector3 WorldToUI(Vector3 point)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(point);
            pos.z = 0f;
            Vector3 pos2 = UICamera.currentCamera.ScreenToWorldPoint(pos);
            pos2.z = 0f;
            return pos2;
        }

        //public static AnimationClip GetAnimationClip(Animator anim, string clipName)
        //{
        //    UnityEditorInternal.State state = GetAnimationState(anim, clipName);
        //    return state != null ? state.GetMotion() as AnimationClip : null;
        //}

        //public static UnityEditorInternal.State GetAnimationState(Animator anim, string clipName)
        //{
        //    UnityEditorInternal.State state = null;

        //    if (anim != null)
        //    {
        //        UnityEditorInternal.AnimatorController ac = anim.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
        //        UnityEditorInternal.StateMachine sm = ac.GetLayer(0).stateMachine;

        //        for (int i = 0; i < sm.stateCount; i++)
        //        {
        //            UnityEditorInternal.State _state = sm.GetState(i);
        //            if (_state.uniqueName.EndsWith("." + clipName))
        //            {
        //                state = _state;
        //                break;
        //            }
        //        }
        //    }

        //    return state;
        //}

        //public static void SetAnimSpeedWithClipTag(Animator anim, string tag, float speed, int layer = 0)
        //{
        //    if (anim != null)
        //    {
        //        UnityEditorInternal.AnimatorController ac = anim.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
        //        UnityEditorInternal.StateMachine sm = ac.GetLayer(layer).stateMachine;

        //        for (int i = 0; i < sm.stateCount; i++)
        //        {
        //            UnityEditorInternal.State _state = sm.GetState(i);
        //            if (_state.tag == tag)
        //            {
        //                _state.speed = speed;
        //                break;
        //            }
        //        }
        //    }
        //}

        //public static void SetAnimSpeedWithClipName(Animator anim, string name, float speed, int layer = 0)
        //{
        //    if (anim != null)
        //    {
        //        UnityEditorInternal.AnimatorController ac = anim.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
        //        UnityEditorInternal.StateMachine sm = ac.GetLayer(layer).stateMachine;

        //        for (int i = 0; i < sm.stateCount; i++)
        //        {
        //            UnityEditorInternal.State _state = sm.GetState(i);
        //            if (_state.uniqueName == name)
        //            {
        //                _state.speed = speed;
        //                break;
        //            }
        //        }
        //    }
        //}

        //public static void SetAnimSpeedOfLayer(Animator anim, float speed, int layer = 0)
        //{
        //    if (anim != null)
        //    {
        //        UnityEditorInternal.AnimatorController ac = anim.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
        //        UnityEditorInternal.StateMachine sm = ac.GetLayer(layer).stateMachine;

        //        for (int i = 0; i < sm.stateCount; i++)
        //        {
        //            sm.GetState(i).speed = speed;
        //        }
        //    }
        //}


        public static int FindMin(int a, int b, int c)
        {
            return ((a < b ? a : b) < c) ? (a < b ? a : b) : c;
        }

        public static int FindMax(int a, int b)
        {
            return (a > b) ? a : b;
        }

        public static int FindMin(int a, int b)
        {
            return (a < b) ? a : b;
        }

        public static string GetStreamingFilePath(string filename)
        {
            string path = "";

            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ||
              Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                path = Application.dataPath + "/StreamingAssets/" + filename;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                path = Application.dataPath + "/Raw/" + filename;
            else if (Application.platform == RuntimePlatform.Android)
                path = "jar:file://" + Application.dataPath + "!/assets/" + filename;
            else
                path = Application.dataPath + "/config/" + filename;


            return path;
        }

        public static string GetPersistentFilePath(string filename, string pkgName = "")
        {
            string filepath = GetPersistentPath(pkgName) + "/" + filename;
            return filepath;
        }

        public static string GetPersistentPath(string pkgName = "")
        {
            string[] paths = Application.persistentDataPath.Split(new string[] { "/com." }, System.StringSplitOptions.None);
            string pkgPath = paths[0] + "/" + pkgName + "/files";
            if (pkgName == "") pkgPath = Application.persistentDataPath;

            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ||
              Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                pkgPath = Application.dataPath + "/StreamingAssets";

            return pkgPath;
        }

        public static void CreateFile(string path, string name, string info)
        {
            //文件流信息
            StreamWriter sw;
            FileInfo t = new FileInfo(path + "//" + name);
            if (!t.Exists)
            {
                //如果此文件不存在则创建
                sw = t.CreateText();
            }
            else
            {
                //如果此文件存在则打开
                sw = t.AppendText();
            }
            //以行的形式写入信息
            sw.WriteLine(info);
            //关闭流
            sw.Close();
            //销毁流
            sw.Dispose();
        }

        public static ArrayList LoadFile(string path, string name)
        {
            //使用流的形式读取
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(path + "//" + name);
            }
            catch (System.Exception e)
            {
                //路径与名称未找到文件则直接返回空
                return null;
            }
            string line;
            ArrayList arrlist = new ArrayList();
            while ((line = sr.ReadLine()) != null)
            {
                //一行一行的读取
                //将每一行的内容存入数组链表容器中
                arrlist.Add(line);
            }
            //关闭流
            sr.Close();
            //销毁流
            sr.Dispose();
            //将数组链表容器返回
            return arrlist;
        }

        public static void DeleteFile(string path, string name)
        {
            File.Delete(path + "//" + name);

        }

        public static bool IsExistFile(string path, string name)
        {
            FileInfo fileInfo = new FileInfo(path + "//" + name);
            if (fileInfo.Exists)
            {
                return true;
            }
            return false;
        }

        //public static IEnumerator LoadFile(string path)
        //{
        //    WWW www = new WWW(path);
        //    yield return www;
        //}

        public static bool OpenPackage(string pkgName)
        {
            try
            {
#if UNITY_ANDROID
            AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject joPackageManager = joActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject joIntent = joPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", pkgName);
            if (null != joIntent)
            {
                joActivity.Call("startActivity", joIntent);
                return true;
            }
#endif
            }
            catch (System.Exception e)
            {

            }

            return false;
        }

        public static string GetVersionName(string pkgName = "")
        {
            try
            {
#if UNITY_ANDROID
            AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            string versionName = "";
            if (pkgName == "")
                versionName = joActivity.Call<string>("getVersionName");
            else
                versionName = joActivity.Call<string>("getVersionName", pkgName);
            return versionName;
#endif
            }
            catch (System.Exception e)
            {

            }
            return "";
        }

        public static int GetVersionCode(string pkgName = "")
        {
            try
            {
#if UNITY_ANDROID
            AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            int versionCode = 0;
            if (pkgName == "")
                versionCode = joActivity.Call<int>("getVersionCode");
            else
                versionCode = joActivity.Call<int>("getVersionCode", pkgName);
            return versionCode;
#endif
            }
            catch (System.Exception e)
            {

            }
            return 0;
        }

        public static int ChangeValue(bool bRightDir, int[] iAry, int curValue, bool bLoop = true)
        {
            int index = 0;

            for (int i = 0; i < iAry.Length; i++)
            {
                if (curValue == iAry[i])
                {
                    break;
                }
                else
                {
                    index++;
                }
            }

            if (bRightDir == false)
            {
                index--;
                if (index < 0)
                {
                    if (bLoop)
                        index = iAry.Length - 1;
                    else
                        index = 0;
                }
            }
            else if (bRightDir == true)
            {
                index++;
                if (index >= iAry.Length)
                {
                    if (bLoop)
                        index = 0;
                    else
                        index = iAry.Length - 1;
                }
            }
            return iAry[index];
        }

        public static GameObject CreateGameObject(UnityEngine.Object original, Transform parent, Vector3 pos, Quaternion rotation, Vector3 scal)
        {
            GameObject go = GameObject.Instantiate(original) as GameObject;
            if (parent != null) go.transform.parent = parent;
            go.transform.localPosition = pos;
            go.transform.localScale = scal;
            go.transform.localRotation = rotation;

            return go;
        }
        public static GameObject CreateGameObject(UnityEngine.Object original)
        {
            return CreateGameObject(original, null, Vector3.zero, Quaternion.identity, Vector3.one);
        }
        public static GameObject CreateGameObject(UnityEngine.Object original, Transform parent)
        {
            return CreateGameObject(original, parent, Vector3.zero, Quaternion.identity, Vector3.one);
        }
        public static GameObject CreateGameObject(UnityEngine.Object original, Transform parent, Vector3 pos)
        {
            return CreateGameObject(original, parent, pos, Quaternion.identity, Vector3.one);
        }
        public static GameObject CreateGameObject(UnityEngine.Object original, Transform parent, Vector3 pos, Quaternion rotation)
        {
            return CreateGameObject(original, parent, pos, rotation, Vector3.one);
        }
       

        // 获得本机局域网IP地址  
        public static string getIPAddress()
        {            
            IPAddress[] AddressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            if (AddressList.Length < 1)
            {
                return "";
            }
            return AddressList[0].ToString();
        }

        public static bool GetPublicIPAddress(ref string ip, ref string area, ref string networkOperator)
        {
            try
            {
                WebRequest request = WebRequest.Create("http://ip.qq.com/");
                request.Timeout = 10000;
                WebResponse response = request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);
                string htmlinfo = sr.ReadToEnd();
                //匹配IP的正则表达式
                Regex r = new Regex("((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|\\d)\\.){3}(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|[1-9])", RegexOptions.None);
                Match mc = r.Match(htmlinfo);
                //获取匹配到的IP
                ip = mc.Groups[0].Value;

                int startIndex = htmlinfo.IndexOf("该IP所在地为：<span>");
                string strAddress = htmlinfo.Substring(startIndex + "该IP所在地为：<span>".Length);
                int endIndex = strAddress.IndexOf("</span>");
                strAddress = strAddress.Substring(0, endIndex);
                area = strAddress.Substring(0, strAddress.IndexOf("&"));
                networkOperator = strAddress.Substring(strAddress.IndexOf(";") + 1);

                resStream.Close();
                sr.Close();

                return true;
            }
            catch (Exception e)
            {
            }            
            return false;
        }

        public static void CommonSort<T>(T[] sortArray, Func<T, T, bool> compareMethod)
        {
            bool swapped = true;
            do
            {
                swapped = false;
                for (int i = 0; i < sortArray.Length - 1; i++)
                {
                    if (compareMethod(sortArray[i], sortArray[i + 1]))
                    {
                        T temp = sortArray[i];
                        sortArray[i] = sortArray[i + 1];
                        sortArray[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);
        }

        public static int Digits(int n) //数字所占位数 
        {
            n = System.Math.Abs(n);
            n = n / 10;
            int i = 1;
            while (n > 0)
            {
                n = n / 10;
                i++;
            }
            return i;
        }

        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }

        public static void SavePng(Texture2D tex, string filePath)
        {
            try
            {
                byte[] pngData = tex.EncodeToPNG();
                File.WriteAllBytes(filePath, pngData);
                Debuger.Log("保存图片成功！" + filePath);
            }
            catch (Exception e)
            {
            }           
        }
        public static void SaveJpg(Texture2D tex, string filePath)
        {
            try
            {
                byte[] pngData = tex.EncodeToJPG();
                File.WriteAllBytes(filePath, pngData);
                Debuger.Log("保存图片成功！" + filePath);
            }
            catch (Exception e)
            {
            }
        }
        public static void SaveBytes(byte[] buf, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, buf);
            }
            catch (Exception e)
            {
            }
        }
        public static byte[] ReadBytes(string filePath)
        {
            try
            {
                //创建文件读取流
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                fileStream.Seek(0, SeekOrigin.Begin);
                //创建文件长度缓冲区
                byte[] bytes = new byte[fileStream.Length];
                //读取文件
                fileStream.Read(bytes, 0, (int)fileStream.Length);
                //释放文件读取流
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;
                return bytes;
            }
            catch (Exception e)
            {
                Debuger.LogError("读取文件失败！" + filePath + "---" + e);
            }
            return null;
        }
        public Texture2D ReadTexture(string fullPath)
        {
            Texture2D tex = null;
            try
            {
                WWW www = new WWW(fullPath);
                while (!www.isDone)
                {
                    
                }
                if (www != null && string.IsNullOrEmpty(www.error))
                {
                    tex = www.texture;
                    Debuger.Log("读取图片成功！" + fullPath);
                }
                else
                {
                    Debuger.Log("读取图片失败！" + fullPath);
                }
            }
            catch (Exception e)
            {
                Debuger.LogError(string.Format("读取图片错误！{0}", fullPath));
            }
            return tex;
        }

        public Texture2D ReadTextureByIO(string fullPath, int w, int h)
        {
            Texture2D tex = new Texture2D(w, h);

            //创建文件读取流
            FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            //释放文件读取流
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            tex.LoadImage(bytes);

            return tex;
        }

        public void CaptureByUnity(string savePath)
        {
            //图片是在Application.CaptureScreenshot调用完成后一帧，才要被保存下来的，所以你要在Application.CaptureScreenshot调用完成后的下一帧，才能去使用它保存下来的图片。不然你会发现找不到图片的。
            Application.CaptureScreenshot(savePath);
        }
        /// <summary>
        /// 根据一个Rect类型来截取指定范围的屏幕
        /// 左下角为(0,0)
        /// </summary>
        /// <param name="mRect">M rect.</param>
        /// <param name="mFileName">M file name.</param>
        public IEnumerator CaptureByRect(Rect mRect, Texture2D mTexture = null, string mFilePath = "")
        {
            //等待渲染线程结束
            yield return new WaitForEndOfFrame();

            //初始化Texture2D
            mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
            //读取屏幕像素信息并存储为纹理数据
            mTexture.ReadPixels(mRect, 0, 0);
            //应用
            mTexture.Apply();


            if (mFilePath!="")
            {
                //将图片信息编码为字节信息
                byte[] bytes = mTexture.EncodeToPNG();
                //保存
                System.IO.File.WriteAllBytes(mFilePath, bytes);
            }           

            //如果需要可以返回截图
            //return mTexture;
        }

        public IEnumerator CaptureByCamera(Camera mCamera, Rect mRect, Texture2D mTexture = null, string mFilePath = "")
        {
            //等待渲染线程结束
            yield return new WaitForEndOfFrame();

            //初始化RenderTexture
            RenderTexture mRender = new RenderTexture((int)mRect.width, (int)mRect.height, 0);
            //设置相机的渲染目标
            mCamera.targetTexture = mRender;
            //开始渲染
            mCamera.Render();

            //激活渲染贴图读取信息
            RenderTexture.active = mRender;

            mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
            //读取屏幕像素信息并存储为纹理数据
            mTexture.ReadPixels(mRect, 0, 0);
            //应用
            mTexture.Apply();

            //释放相机，销毁渲染贴图
            mCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(mRender);

            if (mFilePath != "")
            {
                //将图片信息编码为字节信息
                byte[] bytes = mTexture.EncodeToPNG();
                //保存
                System.IO.File.WriteAllBytes(mFilePath, bytes);
            }
           

            //如果需要可以返回截图
            //return mTexture;
        }
    }
}