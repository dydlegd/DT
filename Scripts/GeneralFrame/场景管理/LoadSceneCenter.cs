using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace DYD
{
    public enum SceneName
    {
        Hall,
        MJ_Main
    }

    public class LoadSceneCenter : SingletonBase<LoadSceneCenter>
    {
#if UNITY_ANDROID
        public const string UrlPath = "http://139.129.50.224/Android/";
#elif UNITY_IPHONE
        public const string UrlPath = "http://139.129.50.224//IOS/";
#elif UNITY_STANDALONE
        public const string UrlPath = "http://139.129.50.224/Win32/";
#endif

        private List<LoadScene> loaderList = new List<LoadScene>();
        
        private AssetBundle mBundle = null;

        public void LoadScene(GameType gameType, Action<float> onDownLoad, Action<bool> onFinished)
        {
            GameInfo info = GameInfoCenter.Instance.GetGameInfo(gameType);
            if (info.IsNeedUpdate)
            {
                Hall_ViewCenter.Instance.GetPanel<Hall_Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox("有新版本，请更新", "", delegate { LoadSceneCenter.Instance.LoadScene(info.mSceneName, onDownLoad, onFinished); });
            }
            else
            {
                LoadSceneCenter.Instance.LoadScene(info.mSceneName, onDownLoad, onFinished);
            }
        }

        public void LoadScene(SceneName sceneName, Action<float> onDownLoad, Action<bool> onFinished)
        {
            switch (sceneName)
            {
                case SceneName.Hall:
                    LoadScene(GameType.大厅, "Game_Hall.apk", "Game_Hall", UrlPath, onDownLoad, onFinished);
                    break;
                case SceneName.MJ_Main:
                    LoadScene(GameType.麻将, "MJ.unity3d", "MJ_Main", UrlPath, onDownLoad, onFinished);
                    break;
                default:
                    break;
            }
        }

        public void OnComplete(LoadScene loader, bool bSucceed)
        {
            if (loader.onFinished != null) loader.onFinished(bSucceed);
            if (bSucceed)
            {
                GameInfoCenter.Instance.GetGameInfo(loader.mGameType).UpdateVerion();
                LoadAssetBundle(loader.mFile);
                Application.LoadLevel(loader.mSceneName);
                DestroyLoader(loader);
            }   
        }

        private void LoadScene(GameType _gameType, string _fileName, string _sceneName, string _urlPath, Action<float> onDownLoad, Action<bool> onFinished)
        {
            bool bNeedUpdate = GameInfoCenter.Instance.GetGameInfo(_gameType).IsNeedUpdate;

            if (bNeedUpdate)
            {
                var loader = new GameObject("LoadScene").AddComponent<LoadScene>();
                loader.Init(_gameType, _fileName, _sceneName, _urlPath, bNeedUpdate);
                loader.onDownload = onDownLoad;
                loader.onFinished = onFinished;
                loaderList.Add(loader);
            }
            else
            {
                Application.LoadLevel(_sceneName);
            }
        }

        private void LoadAssetBundle(string file)
        {
            if (mBundle != null) Dispose();
            mBundle = AssetBundle.CreateFromFile(file);
        }

        private void Dispose()
        {
            if (mBundle != null)
            {
                mBundle.Unload(true);
                mBundle = null;
            }
        }

        private void DestroyLoader(LoadScene loader)
        {
            loaderList.Remove(loader);
            loader.DestroySelf();
        }
    }

}
