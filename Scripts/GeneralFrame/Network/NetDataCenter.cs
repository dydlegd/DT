using UnityEngine;
using System.Collections;

namespace DYD
{
    public class NetDataCenter : SingletonBase<NetDataCenter>
    {
        protected Queue mQueueCommand = Queue.Synchronized(new Queue());
        protected Queue mQueueSend = Queue.Synchronized(new Queue());

        public void Init()
        {

        }
        public void AddQueueReceive(NetDataBuf sendBuf)
        {
            mQueueCommand.Enqueue(sendBuf);
        }
        public void AddQueueSend(NetDataBuf sendBuf)
        {
            mQueueSend.Enqueue(sendBuf);
        }
        public bool GetQueueSend(ref NetDataBuf sendBuf)
        {
            if (mQueueSend.Count > 0)
            {
                sendBuf = (NetDataBuf)mQueueSend.Dequeue();
                return true;
            }
            return false;
        }
        public bool GetQueueReceive(ref NetDataBuf data)
        {
            if (mQueueCommand.Count > 0)
            {
                //data = (NetDataBuf)mQueueCommand.Peek();
                data = (NetDataBuf)mQueueCommand.Dequeue();
                if (data.IsFullDataPacket)
                {
                    return true;
                }    
                else
                {
                    AddQueueReceive(data);
                    return false;
                }
            }
            return false;
        }

        public bool GetLastRecDataBuf(ref NetDataBuf data)
        {
            if (mQueueCommand.Count>0)
            {
                object[] dataAry = (object[])mQueueCommand.ToArray();
                data = (NetDataBuf)dataAry[dataAry.Length - 1];
                return true;
            }
            return false;
        }

        public void Clear()
        {
            mQueueCommand.Clear();
            mQueueSend.Clear();
        }
    }

}
