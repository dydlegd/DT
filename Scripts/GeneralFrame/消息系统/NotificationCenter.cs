using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public delegate void OnDelegateFunc(Notification node);

    public class NotificationCenter
    {

        private static NotificationCenter _Instance = null;

        private Dictionary<NotificateMsgType, OnDelegateFunc> _mapFun = new Dictionary<NotificateMsgType, OnDelegateFunc>();

        public static NotificationCenter Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new NotificationCenter();
                }
                return _Instance;
            }
        }

        public void AddNotificationEvent(NotificateMsgType key, OnDelegateFunc func)
        {
            if (!_mapFun.ContainsKey(key))
            {
                _mapFun[key] = null;
            }
            _mapFun[key] += func;
        }

        public void RemoveNotificationEvent(NotificateMsgType key, OnDelegateFunc func)
        {
            if (!_mapFun.ContainsKey(key))
            {
                return;
            }
            _mapFun[key] -= func;
        }

        public void RemoveAllNotificationEvent(NotificateMsgType key)
        {
            _mapFun[key] = null;
        }

        public void PostNotificationEvent(NotificateMsgType key, Notification node = null)
        {
            if (!_mapFun.ContainsKey(key)) return;

            _mapFun[key](node);
        }
    }

}
