using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using CielaSpike;

namespace DYD
{
    public class DynamicAtlas : SingletonMonoBase<DynamicAtlas>
    {
        private List<Texture> atlasTexList = new List<Texture>();
        public UIAtlas Atla { get { return ResourcesManager.GetAtlas(Tags.MJ_DynamicAtlas); } }
        private Action CallBackFunc = null;
        void Start()
        {
            AddTexture(ResourcesManager.GetTexture("people"));
            AddTexture(ResourcesManager.GetTexture("playOffline"));
        }

        public void AddTexture(Texture2D tex, Action callBack=null)
        {
            for (int i = 0; i < atlasTexList.Count; )
            {
                if (atlasTexList[i].name == tex.name)
                {
                    atlasTexList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            atlasTexList.Add(tex);

            
            CallBackFunc = callBack;
            //StartCoroutine(PackAtlas());

            CreateAtlas.CreatAtlasFromTex(Atla, atlasTexList);
            if (CallBackFunc != null)
            {
                CallBackFunc();
                CallBackFunc = null;
            }
        }

        private IEnumerator PackAtlas()
        {
            CreateAtlas.CreatAtlasFromTex(Atla, atlasTexList);
            if (CallBackFunc != null)
            {
                CallBackFunc();
                CallBackFunc = null;
            }
            yield return null;

            //yield return this.StartCoroutineAsync(CreateAtlasFunc());
        }

        IEnumerator CreateAtlasFunc()
        {
            CreateAtlas.CreatAtlasFromTex(Atla, atlasTexList);
            if (CallBackFunc != null)
            {
                CallBackFunc();
                CallBackFunc = null;
            }
            yield return null;
        }
    }
}

