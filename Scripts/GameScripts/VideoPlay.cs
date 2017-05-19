using UnityEngine;
using System.Collections;

namespace DYD
{
    public class VideoPlay : MonoBehaviour
    {
        public string strFileName;
#if UNITY_STANDALONE
        public MovieTexture movTexture;

        void Start()
        {
            renderer.material.mainTexture = movTexture;
            movTexture.loop = true;
            movTexture.Play();
        }
#endif
    }
}

