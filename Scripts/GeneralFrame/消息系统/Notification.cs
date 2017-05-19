using UnityEngine;
using System.Collections;

namespace DYD
{

    public class Notification
    {

        public Component sender;
        public object data;
        public string name;

        public Notification(object aData = null)
        {
            data = aData;
            name = "";
            sender = null;
        }

        public Notification(Component aSender, string aName, object aData)
        {
            sender = aSender;
            name = aName;
            data = aData;
        }
    }

}