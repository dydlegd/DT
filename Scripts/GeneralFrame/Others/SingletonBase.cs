using UnityEngine;
using System.Collections;

namespace DYD
{

    public abstract class SingletonBase<T>
        where T : SingletonBase<T>, new()
    {
        private static T _Instance = null;

        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new T();
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