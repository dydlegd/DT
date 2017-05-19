using UnityEngine;
using System.Collections;

namespace DYD
{
    public class VideoPlay_Phone : MonoBehaviour
    {
        public MediaPlayerCtrl scrMedia;
        public string mediaName;

        void Start()
        {
            if (scrMedia != null)
            {
                scrMedia.Load(mediaName);
                scrMedia.Play();
            }
        }
    }
}

