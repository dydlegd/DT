using UnityEngine;
using System.Collections;
using System;

namespace DYD
{
    public enum DownLoadResType
    {
        Null,
        Image
    }
    public class DownLoadResDesc
    {
        public byte[] bytes;
        public Texture2D texture;

        public DownLoadResDesc()
        {
            texture = null;
        }
    }

    public class WWWHelper : MonoBehaviour
    {
        public byte[] bytes;
        protected WWW www;
        protected string Url { get; set; }
        public bool IsFinished { get; set; }
        protected DownLoadResDesc downLoadResDesc = new DownLoadResDesc();
        public Action<DownLoadResDesc> CallBack_Finished;

        public void DownLoad(string url, Action<DownLoadResDesc> callBack)
        {            
            Url = url;
            IsFinished = false;
            CallBack_Finished = callBack;
            StartCoroutine(StartDownLoad(url));
        }

        protected IEnumerator StartDownLoad(string url)
        {
            www = new WWW(url);
            yield return www;

            bytes = www.bytes;
            downLoadResDesc.bytes = www.bytes;
            OnFinished();
        }

        protected virtual void OnFinished()
        {
            IsFinished = true;
            if (CallBack_Finished != null) CallBack_Finished(downLoadResDesc);
            WWWHelperCenter.Instance.DestroyDownLoader(this);
        }

        public virtual void Destroy()
        {
            if(www!=null)
            {
                www.Dispose();
                www = null;
            }
            Destroy(this.gameObject);
        }
    }

    public class ResImageDownLoader:WWWHelper
    {
        protected override void OnFinished()
        {
            downLoadResDesc.texture = www.texture;
            base.OnFinished();            
        }
    }
}

