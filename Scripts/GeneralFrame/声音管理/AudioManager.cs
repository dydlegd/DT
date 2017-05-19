using UnityEngine;
using System.Collections;
using System.Collections.Generic;//泛型集合命名空间

namespace DYD
{
    public class AudioManager : SingletonMonoBase<AudioManager>
    {

        //public AudioClip[] AudioClipArray;                              //音频剪辑数组
        //private static Dictionary<string, AudioClip> _DicAudioClipLib;  //音频库

        private static List<AudioSourceEffectObj> _AudioSourceEffectArray = new List<AudioSourceEffectObj>();
        private static AudioSource _AudioSource_AudioBackGround;

        private static GameObject managerObj;

        public static float audioEffectVolumes = 1;
        public static float audioBackgroundVolumes = 1;

        private const string SavePrefs_VolumeEffect_Key = "Volume_Effect";
        private const string SavePrefs_VolumeBackground_Key = "Volume_Background";

        //private static bool _isMute = false;//是否静音

        private class AudioSourceEffectObj
        {
            public GameObject go;
            public AudioSource source;

            public AudioSourceEffectObj(string name)
            {
                go = new GameObject(name);
                go.transform.parent = managerObj.transform;
                source = null;
            }
        }

        void OnEnable()
        {
            _AudioSourceEffectArray.Clear();

            managerObj = GameObject.Find("AudioManager");
            if (managerObj == null)
            {
                managerObj = new GameObject("AudioManager");
            }

            if (managerObj.GetComponent<AudioListener>()==null)
            {
                managerObj.AddComponent<AudioListener>();
            }

            _AudioSource_AudioBackGround = managerObj.GetComponent<AudioSource>();
            if (_AudioSource_AudioBackGround == null)
            {
                _AudioSource_AudioBackGround = managerObj.AddComponent<AudioSource>();
            }

            audioBackgroundVolumes = PlayerPrefs.GetFloat(SavePrefs_VolumeBackground_Key, 1);
            audioEffectVolumes = PlayerPrefs.GetFloat(SavePrefs_VolumeEffect_Key, 1);
            SetAudioBackgroundVolumes(audioBackgroundVolumes);
            SetAudioEffectVolumes(audioEffectVolumes);
        }



        private static AudioSource PlayBackground(AudioClip audioClip, bool loop)
        {
            //防止重复播放
            //if (_AudioSource_AudioBackGround.clip == audioClip)
            //{
            //    return;
            //}

            _AudioSource_AudioBackGround.volume = audioBackgroundVolumes;

            if (audioClip)
            {
                _AudioSource_AudioBackGround.clip = audioClip;
                _AudioSource_AudioBackGround.loop = loop;
                _AudioSource_AudioBackGround.Play();
                return _AudioSource_AudioBackGround;
            }
            else
            {
                Debuger.LogWarning("[AudioManager.cs/PlayBackground()] audioClip==null ! Please checked! ");
            }
            return null;
        }

        public static AudioSource PlayBackground(string strAudioName, bool loop = true)
        {
            if (!string.IsNullOrEmpty(strAudioName))
            {
                //PlayBackground(_DicAudioClipLib[strAudioName]);
                return PlayBackground(ResourcesManager.GetAudioClip(strAudioName), loop);
            }
            else
            {
                Debuger.LogWarning("[AudioManager.cs/PlayBackground()] strAudioName==null ! Please checked! ");
            }
            return null;
        }

        public static void PauseBackground()
        {
            if (_AudioSource_AudioBackGround.clip)
            {
                _AudioSource_AudioBackGround.Pause();
            }
            else
            {
                Debuger.LogWarning("[AudioManager.cs/PauseBackground()] _AudioSource_AudioBackGround.clip==null ! Please checked! ");
            }
        }

        public static void ResumeBackground()
        {
            if (_AudioSource_AudioBackGround.clip)
            {
                _AudioSource_AudioBackGround.Play();
            }
            else
            {
                Debuger.LogWarning("[AudioManager.cs/ResumeBackground()] _AudioSource_AudioBackGround.clip==null ! Please checked! ");
            }
        }

        public static void StopBackground()
        {
            if (_AudioSource_AudioBackGround)
            {
                _AudioSource_AudioBackGround.Stop();
            }
            else
            {
                Debuger.LogWarning("[AudioManager.cs/StopBackground()] _AudioSource_AudioBackGround==null ! Please checked! ");
            }
        }

        public static AudioSource Play(AudioClip audioClip, bool bLoop)
        {
            //处理全局音效音量
            //_AudioSource_AudioEffect.volume = GlobalManager.parameter.audioEffectVolumes;

            if (audioClip)
            {
                //_AudioSource_AudioEffect.clip = audioClip;
                //_AudioSource_AudioEffect.Play();
                AudioSourceEffectObj audioObj = null;
                for (int i = 0; i < _AudioSourceEffectArray.Count; i++)
                {
                    if (!_AudioSourceEffectArray[i].source.isPlaying)
                    {
                        audioObj = _AudioSourceEffectArray[i];
                        break;
                    }
                }
                if (audioObj == null)
                {
                    audioObj = new AudioSourceEffectObj("Audio:" + audioClip.name);
                    audioObj.source = audioObj.go.AddComponent<AudioSource>();
                    _AudioSourceEffectArray.Add(audioObj);
                }
                audioObj.go.name = "Audio:" + audioClip.name;
                audioObj.source.clip = audioClip;
                audioObj.source.volume = audioEffectVolumes;
                audioObj.source.loop = bLoop;
                audioObj.source.Play();
                return audioObj.source;
            }
            else
            {
                Debuger.LogWarning("[AudioManager.cs/Play()] audioClip==null ! Please checked! ");
            }
            return null;
        }

        public static AudioSource Play(string strAudioEffectName, bool bLoop = false)
        {
            if (!string.IsNullOrEmpty(strAudioEffectName))
            {
                //Play(_DicAudioClipLib[strAudioEffectName]);
                return Play(ResourcesManager.GetAudioClip(strAudioEffectName), bLoop);
            }
            else
            {
                Debuger.LogWarning("[AudioManager.cs/Play()] strAudioEffectName==null ! Please checked! ");
            }
            return null;
        }

        //改变背景音量 
        public static void SetAudioBackgroundVolumes(float floAudioBGVolumes)
        {
            if (!_AudioSource_AudioBackGround) return;
            _AudioSource_AudioBackGround.volume = floAudioBGVolumes;
            audioBackgroundVolumes = floAudioBGVolumes;
            PlayerPrefs.SetFloat(SavePrefs_VolumeBackground_Key, audioBackgroundVolumes);
        }

        //改变音效音量
        public static void SetAudioEffectVolumes(float floAudioEffectVolumes)
        {
            for (int i = 0; i < _AudioSourceEffectArray.Count; i++)
            {
                _AudioSourceEffectArray[i].source.volume = floAudioEffectVolumes;
            }
            audioEffectVolumes = floAudioEffectVolumes;
            PlayerPrefs.SetFloat(SavePrefs_VolumeEffect_Key, audioEffectVolumes);
        }

        public static bool IsPlayingBackground()
        {
            if (_AudioSource_AudioBackGround)
            {
                return _AudioSource_AudioBackGround.isPlaying;
            }
            return false;
        }

        public static void SetMute(bool bMute)
        {
            _AudioSource_AudioBackGround.volume = bMute ? 0f : audioBackgroundVolumes;
            for (int i = 0; i < _AudioSourceEffectArray.Count; i++)
            {
                _AudioSourceEffectArray[i].source.volume = bMute ? 0f : audioEffectVolumes;
            }
        }
    }

}