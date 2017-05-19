using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DYD
{
    public class WWWHelperCenter : SingletonBase<WWWHelperCenter>
    {
        private List<WWWHelper> ResDownLoaderList = new List<WWWHelper>();
        public bool IsDownLoading { get { return ResDownLoaderList.Count > 0; } }
        
        public void DownLoad(string url, DownLoadResType resType, Action<DownLoadResDesc> callBack)
        {
            WWWHelper loader = CreateDownLoader(resType);
            loader.DownLoad(url, callBack);
        }

        public void DestroyDownLoader(WWWHelper loader)
        {
            ResDownLoaderList.Remove(loader);
            loader.Destroy();
        }

        private WWWHelper CreateDownLoader(DownLoadResType resType)
        {
            GameObject go = new GameObject();
            go.name = "ResDownLoader";
            WWWHelper loader = null;
            switch (resType)
            {
                case DownLoadResType.Null:
                    loader = go.AddComponent<WWWHelper>();
                    break;
                case DownLoadResType.Image:
                    loader = go.AddComponent<ResImageDownLoader>();
                    break;
                default:
                    break;
            }            
            return loader;
        }
    }

}
