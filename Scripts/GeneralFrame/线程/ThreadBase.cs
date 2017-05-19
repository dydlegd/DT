using UnityEngine;
using System.Collections;
using System.Threading;

namespace DYD
{

    public abstract class ThreadBase
    {

        protected Thread thread;

        public void StartThread(bool isBackground=true)
        {
            thread = new Thread(RunThread);
            thread.IsBackground = isBackground;
            thread.Start();
        }

        protected abstract void RunThread();

        public void ExitThread()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }
    }

}