using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DYD
{
    public class NetDataBuf
    {
        public const int BUF_INIT_LEN = 1024;
        public Socket workSocket = null;
        public byte[] byteBuf = new byte[BUF_INIT_LEN];
        public StringBuilder sb = new StringBuilder();
        public int DataOffset { get; set; }//有效数据偏移位置
        public int DataLen { get; set; }//整个包长
        public int FullDataLen { get; set; }//完整数据包长度
        public bool IsFullDataPacket { get { return DataLen - DataOffset >= FullDataLen; } }//是否为完整数据包（分包接收用）

        public NetDataBuf()
        {

        }

        public NetDataBuf Clone()
        {
            NetDataBuf clone = new NetDataBuf(null, byteBuf, ByteType.Single);
            clone.DataOffset = DataOffset;
            clone.DataLen = DataLen;
            clone.FullDataLen = FullDataLen;
            return clone;
        }

        public NetDataBuf(Socket socket, byte[] byteBuf, ByteType byteType=ByteType.Single)
        {
            workSocket = socket;
            AddData(byteBuf, byteType);
        }

        //public void AddData(string text, ByteType byteType)
        //{
        //    byte[] buf = null;
            
        //    if (byteType == ByteType.Single)
        //        buf = Encoding.Default.GetBytes(text);
        //    else if (byteType == ByteType.Double)
        //        buf = Encoding.Unicode.GetBytes(text);
        //    sb.Append(text);

        //    CheckReBufferSize(buf.Length);
        //    buf.CopyTo(byteBuf, DataLen);
        //    DataLen += buf.Length;            
        //}
        public void AddData(byte[] buf, ByteType byteType)
        {            
            string text = "";
            if (byteType == ByteType.Single)
                text = Encoding.Default.GetString(buf);
            else if (byteType == ByteType.Double)
                text = Encoding.Unicode.GetString(buf);
            sb.Append(text);

            CheckReBufferSize(buf.Length);
            buf.CopyTo(byteBuf, DataLen);
            DataLen += buf.Length;
        }

        private void CheckReBufferSize(int newDataLen)
        {
            if (DataLen+newDataLen>byteBuf.Length)
            {
                ReBufferSize(byteBuf.Length * 2);
                CheckReBufferSize(newDataLen);
            }
        }

        public void ReBufferSize(int iSize)
        {
            Array.Resize<byte>(ref byteBuf, iSize);
        }

        public void Send()
        {
            if (workSocket != null)
            {
                try
                {
                    int sendLen = workSocket.Send(byteBuf, DataLen, SocketFlags.None);
                    Debuger.Log(string.Format("SendBuf({0}):", DataLen) + sb.ToString());
                }
                catch (Exception e)
                {
                    Debuger.LogError("发送数据异常！" + e);
                }
            }
        }
    }
}

