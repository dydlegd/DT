using UnityEngine;
using System.Collections;

namespace DYD
{

    public abstract class SingletonMonoBase<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static string rootName = "MonoSingletonRoot";
        private static GameObject monoSingleRoot;

        private static T _Instance = null;

        public static T Instance
        {
            get
            {
                if (monoSingleRoot == null)
                {
                    monoSingleRoot = GameObject.Find(rootName);
                    if (monoSingleRoot == null)
                        Debuger.LogError("SingletonMonoBase Error !" + typeof(T).Name);
                }

                _Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (_Instance == null)
                {
                    _Instance = monoSingleRoot.GetComponent<T>();
                    if (_Instance == null)
                        _Instance = monoSingleRoot.AddComponent<T>();
                }

                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }
    }

}