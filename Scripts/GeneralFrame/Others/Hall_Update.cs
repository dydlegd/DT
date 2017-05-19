using UnityEngine;
using System.Collections;
using System;

namespace DYD
{
    public class Hall_Update : MonoBehaviour
    {
        private const string apkName = "Hall.apk";
        private string mUrlPath = LoadSceneCenter.UrlPath;
        private string mUrlFile { get { return mUrlPath + "/" + apkName; } }
        private string mDataPath { get { return Application.persistentDataPath; } }
        private string mFile { get { return mDataPath + "/" + apkName; } }

        public Action<float> onDownload;

        void Start()
        {
            StartCoroutine(StartLoad());
        }

        public static void Update_Hall()
        {
            var loader = new GameObject("Hall_Loader").AddComponent<Hall_Update>();
        }

        IEnumerator StartLoad()
        {
            var dl = KHttpDownloader.Load(mUrlFile, mFile, 0);
            while (!dl.IsFinished)
            {
                yield return null;
                if (onDownload != null) onDownload(dl.Progress);
            }
            dl.Dispose();

            if (!dl.IsError)
            {
                GameInfoCenter.Instance.GetGameInfo(GameType.大厅).UpdateVerion();
                Application.OpenURL(mFile);
            }   
            else
            {
                //下载出错，请重新下载
                StartCoroutine(StartLoad());
            }
        }
    }
}

