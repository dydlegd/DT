using UnityEngine;
using System.Collections;

namespace DYD
{
    public class NetHeartBeat : SingletonBase<NetHeartBeat>
    {
        private float fTimer = 0;
        private int count = 0;
        public bool Connected { get; set; }

        // Update is called once per frame
        public void Update()
        {
            if (Connected)
            {
                fTimer += Time.deltaTime;
                if (fTimer > 3)
                {
                    fTimer = 0;
                    SendHeart();
                }
            }            
        }

        public void HeartSucceed()
        {
            fTimer = 0;
            count = 0;
        }

        private void SendHeart()
        {
            DealCommand.Instance.SendHeart();
            count++;
            if (count>=3)
            {
                count = 0;
                Connected = false;
                DealCommand.Instance.Disconnect();                
            }
        }
    }
}

