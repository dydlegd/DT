using UnityEngine;
using System.Collections;

namespace DYD
{
    public class SystemInfoMgr : SingletonBase<SystemInfoMgr>
    {
        
        public static System.DateTime GetDataTime()
        {
            return System.DateTime.Now;
        }
    }

}
