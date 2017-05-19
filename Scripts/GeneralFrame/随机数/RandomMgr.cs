using UnityEngine;
using System.Collections;

namespace DYD
{

    public class RandomMgr
    {

        public static int Range(int min, int max)
        {
            return Random.Range(min, max + 1);
        }

        public static float Range(float min, float max)
        {
            return Random.Range(min, max);
        }
    }

}