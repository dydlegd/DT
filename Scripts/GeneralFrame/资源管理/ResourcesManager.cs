using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class ResourcesManager : SingletonMonoBase<ResourcesManager>
    {

        public List<Texture2D> textureList = new List<Texture2D>();
        private static Dictionary<string, Texture2D> _DicTexsLib = new Dictionary<string, Texture2D>();

        public List<Sprite> spritesList = new List<Sprite>();
        private static Dictionary<string, Sprite> _DicSpritesLib = new Dictionary<string, Sprite>();

        public List<AudioClip> audioClipsList = new List<AudioClip>();
        private static Dictionary<string, AudioClip> _DicAudioClipsLib = new Dictionary<string, AudioClip>();

        public List<GameObject> objectsList = new List<GameObject>();
        private static Dictionary<string, GameObject> _DicObjectsLib = new Dictionary<string, GameObject>();

        public List<UIAtlas> atlasList = new List<UIAtlas>();
        private static Dictionary<string, UIAtlas> _DicAtlasLib = new Dictionary<string, UIAtlas>();

        public void Init()
        {
            _DicTexsLib.Clear();
            _DicSpritesLib.Clear();
            _DicAudioClipsLib.Clear();
            _DicAtlasLib.Clear();
            ResourcesLoad();

            Debuger.Log("ResourcesManager Init Succeed !");
        }

        private void ResourcesLoad()
        {
            //-----------------------手动加载资源-----------------------------//





            //-------------------------------------------------------------------------//
            foreach (Texture2D tex in textureList)
            {
                CreateTexture("", tex);
            }

            foreach (Sprite sp in spritesList)
            {
                CreateSprite("", sp);
            }

            foreach (AudioClip clip in audioClipsList)
            {
                CreateAudioClip("", clip);
            }

            foreach (GameObject go in objectsList)
            {
                CreateGameObject("", go);
            }

            foreach (UIAtlas atlas in atlasList)
            {
                CreateAtlas(atlas);
            }            
        }

        public static Texture2D GetTexture(string name)
        {
            if (_DicTexsLib.ContainsKey(name))
            {
                return _DicTexsLib[name];
            }
            Debug.LogError(string.Format("ResourcesManager GetTexture {0} Failed!", name));

            return null;
        }

        public static Sprite GetSprite(string name)
        {
            if (_DicSpritesLib.ContainsKey(name))
            {
                return _DicSpritesLib[name];
            }
            Debug.LogError(string.Format("ResourcesManager GetSprite {0} Failed!", name));

            return null;
        }


        public static AudioClip GetAudioClip(string name)
        {
            if (_DicAudioClipsLib.ContainsKey(name))
            {
                return _DicAudioClipsLib[name];
            }
            Debug.LogError(string.Format("ResourcesManager GetAudioClip {0} Failed!", name));

            return null;
        }

        public static GameObject GetGameObject(string name)
        {
            if (_DicObjectsLib.ContainsKey(name))
            {
                return _DicObjectsLib[name];
            }
            Debug.LogError(string.Format("ResourcesManager GetGameObject {0} Failed!", name));

            return null;
        }
        public static UIAtlas GetAtlas(string name)
        {
            if (_DicAtlasLib.ContainsKey(name))
            {
                return _DicAtlasLib[name];
            }
            Debug.LogError(string.Format("ResourcesManager GetAtlas {0} Failed!", name));

            return null;
        }

        private static void CreateSprite(string path, Sprite sp = null)
        {
            //检查是否已经加载过
            if (sp)
            {
                if (_DicSpritesLib.ContainsKey(sp.name))
                {
                    return;
                }
            }

            if (sp == null)
            {
                sp = Resources.Load<Sprite>(path);
            }

            if (sp == null)
            {
                Debug.LogError(string.Format("ResourcesManager CreateSprite {0} Failed!", path));
            }
            _DicSpritesLib.Add(sp.name, sp);
        }

        private static void CreateTexture(string path, Texture2D tex = null)
        {
            //检查是否已经加载过
            if (tex)
            {
                if (_DicSpritesLib.ContainsKey(tex.name))
                {
                    return;
                }
            }

            if (tex == null)
            {
                tex = Resources.Load<Texture2D>(path);
            }

            if (tex == null)
            {
                Debug.LogError(string.Format("ResourcesManager CreateTexture {0} Failed!", path));
            }
            _DicTexsLib.Add(tex.name, tex);
        }

        private static void CreateAudioClip(string path, AudioClip clip = null)
        {
            //检查是否已经加载过
            if (clip)
            {
                if (_DicAudioClipsLib.ContainsKey(clip.name))
                {
                    return;
                }
            }

            if (clip == null)
            {
                clip = Resources.Load<AudioClip>(path);
            }

            if (clip == null)
            {
                Debug.LogError(string.Format("ResourcesManager CreateAudioClip {0} Failed!", path));
            }

            _DicAudioClipsLib.Add(clip.name, clip);
        }

        private static void CreateGameObject(string path, GameObject go = null)
        {
            //检查是否已经加载过
            if (go)
            {
                if (_DicObjectsLib.ContainsKey(go.name))
                {
                    return;
                }
            }

            if (go == null)
            {
                go = Resources.Load<GameObject>(path);
            }

            if (go == null)
            {
                Debug.LogError(string.Format("ResourcesManager CreateGameObject {0} Failed!", path));
            }

            _DicObjectsLib.Add(go.name, go);
        }

        private static void CreateAtlas(UIAtlas atlas)
        {
            //检查是否已经加载过
            if (_DicObjectsLib.ContainsKey(atlas.name))
            {
                return;
            }

            if (atlas == null)
            {
                Debug.LogError(string.Format("ResourcesManager CreateAtlas {0} Failed!", atlas.name));
            }

            _DicAtlasLib.Add(atlas.name, atlas);
        }

        public void DestroySprite(string name)
        {
            if (_DicSpritesLib.ContainsKey(name))
            {
                Sprite sp = _DicSpritesLib[name];
                _DicSpritesLib.Remove(name);
                Resources.UnloadAsset(sp);
            }

        }
    }

}
