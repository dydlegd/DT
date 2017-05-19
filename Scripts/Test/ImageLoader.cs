using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class ImageLoader : MonoBehaviour
    {

        //需要加载动态图片的对象
        public UISprite m_img;
        //自用的Atlas
        public UIAtlas m_uiAtlas;

        void Start()
        {
            //ImageLoad(ResourcesManager.GetTexture("XT"));

            //List<Texture> texList = new List<Texture>();
            //texList.Add(ResourcesManager.GetTexture("XT"));
            //texList.Add(ResourcesManager.GetTexture("fankui"));
            //CreateAtlas.CreatAtlasFromTex(m_uiAtlas, texList);
            //m_img.atlas = m_uiAtlas;
            //m_img.spriteName = "XT";
        }

        /// <summary>
        /// 加载的贴图
        /// </summary>
        /// <param name="tex">Tex.</param>
        public void ImageLoad(Texture2D tex)
        {
            if (tex == null)
            {
                return;
            }
            if (tex.name == m_img.spriteName)
            {
                return;
            }
            //准备对象和材质球
            if (m_uiAtlas == null)
            {
                Material mat;
                Shader shader = Shader.Find("Unlit/Transparent Colored");
                mat = new Material(shader);
                m_uiAtlas = this.gameObject.AddComponent<UIAtlas>();
                m_uiAtlas.spriteMaterial = mat;
            }
            //设定贴图
            m_uiAtlas.spriteMaterial.mainTexture = tex;
            //为对应UISprite接口，给Atlas加对象
            UISpriteData sprite = new UISpriteData();
            sprite.name = tex.name;
            sprite.SetRect(0, 0, tex.width, tex.height);
            m_uiAtlas.spriteList.Clear();
            m_uiAtlas.spriteList.Add(sprite);
            //设置完成
            m_img.atlas = m_uiAtlas;
            m_img.spriteName = tex.name;
        }
    }
}

