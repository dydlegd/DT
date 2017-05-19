using UnityEngine;
using System.Collections;

/// <summary>
/// 由于服务器有时不发头像数据，故写此类
/// </summary>

namespace DYD
{
    public class WXTXCenter : SingletonMonoBase<WXTXCenter>
    {
        private float fTimer = 0f;
        private bool bActive = false;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (bActive)
            {
                fTimer += Time.deltaTime;
                if (fTimer > 6)
                {
                    fTimer = 0;
                    if (DataCenter.Instance.IsWXTXRecComplete())
                    {
                        bActive = false;
                    }
                    else
                    {
                        DealCommand.Instance.SendGETTX();
                    }
                }
            }
           
        }

        //重新开始计时
        public void ReStartTimer()
        {
            fTimer = 0;
            bActive = true;
        }
    }
}

