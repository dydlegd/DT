using UnityEngine;
using System.Collections;
using System.IO;
using System;
using CielaSpike;

namespace DYD
{
    /// <summary>
    /// 加载场景
    /// </summary>
    public class LoadScene : MonoBehaviour
    {
        public GameType mGameType { get; private set; }
        private string mDataPath { get { return Application.persistentDataPath; } }
        private string mUrlPath { get; set; }
        private string mFileName { get; set; }
        public string mSceneName { get; private set; }
        public string mFile { get { return mDataPath + "/" + mFileName; } }
        public string mUrlFile { get { return mUrlPath + "/" + mFileName; } }
        public string mZipFile { get { return mFile + ".zip"; } }
        public string mUrlZipFile { get { return mUrlFile + ".zip"; } }

        public Action<float> onDownload;
        public Action<bool> onFinished;
        private AssetBundle mBundle;

        public void Init(GameType _gameType, string _fileName, string _sceneName, string _urlPath, bool bNeedUpdate = false)
        {
            mGameType = _gameType;
            mUrlPath = _urlPath;
            mFileName = _fileName;
            mSceneName = _sceneName;
            StartCoroutine(CloadScene(bNeedUpdate));
        }

        IEnumerator CloadScene(bool bNeedUpdate)
        {
            if (bNeedUpdate) ClearAll();
            bool bSucceed = true;
            if (!File.Exists(mFile))
            {
                if (!File.Exists(mZipFile))
                {
                    bSucceed = false;
                    var dl = KHttpDownloader.Load(mUrlZipFile, mZipFile, 0);
                    while(!dl.IsFinished)
                    {
                        yield return null;
                        if (onDownload != null) onDownload(dl.Progress);
                    }
                    bSucceed = dl.IsError == false ? true : false;
                    dl.Dispose();
                }
                if (bSucceed)
                {
                    yield return this.StartCoroutineAsync(CDecompress(mFile));
                }
            }

            LoadSceneCenter.Instance.OnComplete(this, bSucceed);
        }

        IEnumerator CDecompress(string file)
        {
            DataZipCenter.DecompressFileLZMA(file + ".zip", file);
            yield return null;
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }

        private void ClearAll()
        {
            File.Delete(mFile);
            File.Delete(mZipFile);
        }
    }
}

