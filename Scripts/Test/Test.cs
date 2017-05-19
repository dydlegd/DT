using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Test : MonoBehaviour
    {

        // Use this for initialization
        void Awake()
        {
            NGUITools.SetActive(this.gameObject, false);
            //this.gameObject.SetActive(false);
        }
    }
}

