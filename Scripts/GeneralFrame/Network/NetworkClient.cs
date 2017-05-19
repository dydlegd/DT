using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace DYD
{
    public class NetworkClient : NetworkBase
    {
        //public const string IP = "139.129.50.224";
        //public const string IP = "127.0.0.1";
        public const string IP = "192.168.1.13";
        //public const string IP = "192.168.23.1";
        public const int PortEnglish = 6664;
        public const int PortChinese = 6663;
        public const int PortWXTX = 6665;

        private Thread sendThread;
        private Thread recThread2;//接收线程2，用于接收双字节
        private Thread recThreadWXTX;//接收线程3，用于接收微信头像
        private object recLockObj = new object();

        protected Socket _socket_Chinese;//中文接收Socket
        protected Socket _socket_WXTX;//微信头像Socket
        protected NetDecodeBuf mWXTXDecodeBuf = new NetDecodeBuf(ByteType.Single);


        protected byte[] mReceiveBuf2 = new byte[MAX_REC_BUF_LEN];
        protected byte[] mReceiveBufWXTX = new byte[MAX_REC_BUF_LEN];
        protected NetDecodeBuf mDoubleDecodeBuf = new NetDecodeBuf(ByteType.Double);

        protected override bool IsConnectOK { get { return status == ConnectState.Connected && _socket != null && _socket_Chinese != null; } }

        public override void Init()
        {
            base.Init();            

            Connect();         
        }

        public override void Connect()
        {
            //Connect(IP, PortEnglish, PortChinese);
            ThreadedConnect(IP, PortEnglish, PortChinese, PortWXTX);
        }

        private bool Connect(string hostAddress, int portEnglish, int portChinese)
        {
            if (status!=ConnectState.Connected)
            {
                SetConnectState(ConnectState.Connecting);
                IPAddress ip = IPAddress.Parse(hostAddress);
                IPEndPoint ipEP_English = new IPEndPoint(ip, portEnglish);
                IPEndPoint ipEP_Chinese = new IPEndPoint(ip, portChinese);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.NoDelay = true;
                _socket_Chinese = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket_Chinese.NoDelay = true;
                try
                {
                    _socket.Connect(ipEP_English);
                    _socket_Chinese.Connect(ipEP_Chinese);
                    SetConnectState(ConnectState.Connected);
                    if (connectedInvoker != null) connectedInvoker();
                    StartThread();
                    return true;
                }
                catch (Exception e)
                {
                    SetConnectState(ConnectState.Error);
                    if (errorInvoker != null) errorInvoker(e);                    
                    return false;
                }
            }            
            return false;
        }

        private void ThreadedConnect(string hostAddress, int portEnglish, int portChinese, int portWXTX)
        {
            connectThread = new Thread(new ParameterizedThreadStart(ThreadedConnect));
            connectThread.Start(new object[] { hostAddress, portEnglish, portChinese, portWXTX });
        }

        private void ThreadedConnect(object hostAndPort)
        {
            string hostAddress = (string)((object[])hostAndPort)[0];
            int portEnglish = (int)((object[])hostAndPort)[1];
            int portChinese = (int)((object[])hostAndPort)[2];
            int portWXTX = (int)((object[])hostAndPort)[3];

            if (status != ConnectState.Connected)
            {
                SetConnectState(ConnectState.Connecting);
                IPAddress ip = IPAddress.Parse(hostAddress);
                IPEndPoint ipEP_English = new IPEndPoint(ip, portEnglish);
                IPEndPoint ipEP_Chinese = new IPEndPoint(ip, portChinese);
                IPEndPoint ipEP_WXTX = new IPEndPoint(ip, portWXTX);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.NoDelay = true;
                _socket_Chinese = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket_Chinese.NoDelay = true;
                _socket_WXTX = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket_WXTX.NoDelay = true;

                try
                {
                    _socket.Connect(ipEP_English);
                    _socket_Chinese.Connect(ipEP_Chinese);
                    _socket_WXTX.Connect(ipEP_WXTX);
                    SetConnectState(ConnectState.Connected);
                    StartThread();
                    if (connectedInvoker != null) connectedInvoker();
                }
                catch (Exception e)
                {
                    SetConnectState(ConnectState.Error);
                    if (errorInvoker != null) errorInvoker(e);
                }
            }
        }

        protected override void ConnectedCall()
        {
            base.ConnectedCall();
            sendThread = new Thread(SendThread);
            sendThread.IsBackground = true;
            sendThread.Start();

            recThread2 = new Thread(RceThread2);
            recThread2.IsBackground = true;
            recThread2.Start();

            recThreadWXTX = new Thread(RecThreadWXTX);
            recThreadWXTX.IsBackground = true;
            recThreadWXTX.Start();

            Debuger.Log("NetworkClient Init Succeed !");
        }

        protected override void ErrorCall(Exception e)
        {
            base.ErrorCall(e);
        }

        protected override void RunThread()
        {
            while (true)
            {
                if (IsConnectOK)
                {
                    try
                    {
                        Array.Clear(mReceiveBuf, 0, mReceiveBuf.Length);
                        int length = _socket.Receive(mReceiveBuf, mReceiveBuf.Length, SocketFlags.None);
                        //lock (recLockObj)
                        {                            
                            string text = Encoding.Default.GetString(mReceiveBuf, 0, length);
                            Debuger.Log(String.Format("Receive({0}) {1}:", PortEnglish, length) + text);
                            mSingleDecodeBuf.DecodeByte(mReceiveBuf, length);
                        }
                    }
                    catch (Exception e)
                    {
                        Debuger.LogError("单字节接收数据异常！" + e);
                        Disconnect();
                    }                                   
                }
                System.Threading.Thread.Sleep(1);
            }
            Debuger.Log("NetworkClient 接收线程退出");
        }

        private void RceThread2()
        {
            while (true)
            {
                if (IsConnectOK)
                {
                    try
                    {
                        Array.Clear(mReceiveBuf2, 0, mReceiveBuf2.Length);
                        int length = _socket_Chinese.Receive(mReceiveBuf2, mReceiveBuf2.Length, SocketFlags.None);
                        //lock (recLockObj)
                        {                            
                            string text = Encoding.Unicode.GetString(mReceiveBuf2);
                            Debuger.Log(String.Format("Receive({0}) {1}:", PortChinese, length) + text);
                            mDoubleDecodeBuf.DecodeByte(mReceiveBuf2, length);
                        }                       
                    }
                    catch (Exception e)
                    {
                        Debuger.LogError("双字节接收数据异常！" + e);
                        Disconnect();
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            Debuger.Log("NetworkClient 接收线程退出");
        }

        private void RecThreadWXTX()
        {
            while(true)
            {
                if (IsConnectOK)
                {
                    try
                    {
                        Array.Clear(mReceiveBufWXTX, 0, mReceiveBufWXTX.Length);
                        int length = _socket_WXTX.Receive(mReceiveBufWXTX, mReceiveBufWXTX.Length, SocketFlags.None);
                        //lock (recLockObj)
                        {                            
                            string text = Encoding.Default.GetString(mReceiveBufWXTX, 0, length);
                            Debuger.Log(String.Format("Receive({0}) {1}:", PortWXTX, length) + text);
                            mWXTXDecodeBuf.DecodeByte(mReceiveBufWXTX, length);
                        }
                    }
                    catch (Exception e)
                    {
                        Debuger.LogError("微信头像接收数据异常！" + e);
                        Disconnect();
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        private void SendThread()
        {
            NetDataBuf netDataBuf = null;
            while(true)
            {
                if (NetDataCenter.Instance.GetQueueSend(ref netDataBuf))
                {
                    netDataBuf.Send();
                }
                System.Threading.Thread.Sleep(SEND_DELAY);
                //timeDelay(SEND_DELAY);
            }
            Debuger.Log("NetworkClient 发送线程退出");
        }

        private void timeDelay(int iInterval)
        {
            DateTime now = DateTime.Now;
            while (now.AddMilliseconds(iInterval) > DateTime.Now)
            {
            }
            return;
        }        

        //public string GetCommand(ref List<string> strList, ref NetDataBuf dataBuf)
        //{
        //    if (NetDataCenter.Instance.GetQueueReceive(ref dataBuf))
        //    {
        //        string[] arrstr = dataBuf.sb.ToString().Split('/');
        //        for (int i = 2; i < arrstr.Length; i++)
        //        {
        //            strList.Add(arrstr[i]);
        //        }
        //        return arrstr[0];
        //    }
        //    return "";
        //}

        private void SendBuf(byte[] buf, Socket socket)
        {
            if (IsConnectOK)
            {
                try
                {
                    int sendLen = socket.Send(buf, buf.Length, SocketFlags.None);
                    Debuger.Log("SendBuf: Succeed !  "+buf.Length);
                }
                catch (Exception e)
                {
                    Debuger.LogError("发送数据异常！" + e);
                }
            }
        }

        private void SendCmd(byte[] buf, ByteType byteType)
        {
             NetDataBuf netData = null;
             if (byteType == ByteType.Single)
                 netData = new NetDataBuf(_socket, buf, byteType);
             else
                 netData = new NetDataBuf(_socket_Chinese, buf, byteType);

             NetDataCenter.Instance.AddQueueSend(netData);
        }

        private void SendCmd(byte[] buf, ByteType byteType, Socket socket)
        {
            NetDataBuf netData = new NetDataBuf(socket, buf, byteType);
            NetDataCenter.Instance.AddQueueSend(netData);
        }

        private void SendCmd(string cmd, ByteType byteType)
        {
            if (GD.SIMULATE_CHUPAI) return;
            AddCmdLen(ref cmd);
            byte[] byteBuf = new byte[2048];
            if (byteType==ByteType.Single)
                byteBuf = Encoding.Default.GetBytes(cmd);
            else
                byteBuf = Encoding.Unicode.GetBytes(cmd);
            SendCmd(byteBuf, byteType);
        }

        private void AddCmdLen(ref string cmd)
        {
            int len = cmd.Length + 5;
            string[] arrstr = cmd.Split('/');
            cmd = arrstr[0] + "/" + String.Format("{0:D4}", len);
            for (int i = 1; i < arrstr.Length; i++)
            {
                cmd += "/" + arrstr[i];
            }
        }

        public override void CloseNetwork()
        {
            base.CloseNetwork();         

            if (sendThread != null)
            {
                sendThread.Abort();
                sendThread = null;
            }
            if (recThread2 != null)
            {
                recThread2.Abort();
                recThread2 = null;
            }
            if (recThreadWXTX != null)
            {
                recThreadWXTX.Abort();
                recThreadWXTX = null;
            }
            if (_socket_Chinese!=null)
            {
                if (_socket_Chinese.Connected) _socket_Chinese.Shutdown(SocketShutdown.Both);
                _socket_Chinese.Close();
                _socket_Chinese = null;
            }
            if (_socket_WXTX != null)
            {
                if (_socket_WXTX.Connected) _socket_WXTX.Shutdown(SocketShutdown.Both);
                _socket_WXTX.Close();
                _socket_WXTX = null;
            }
            mDoubleDecodeBuf.Clear();
            mWXTXDecodeBuf.Clear();
            NetDataCenter.Instance.Clear();

            Debuger.Log("CloseNetwork !");
        }

        #region 发送命令
        //版本等信息
        public void SendVersion(int iMachine, int iClientVersion)
        {
            int iPlatfrom = 0;
            if (Application.platform==RuntimePlatform.WindowsPlayer)
                iPlatfrom = 1;
            else if (Application.platform == RuntimePlatform.Android)
                iPlatfrom = 2;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                iPlatfrom = 3;

            string cmd = "VER,10001,0.001," + iPlatfrom;
            SendCmd(cmd, ByteType.Single);
        }

        //登录
        public void SendLogin(int iMachine, string password, string wxId, string wxName)
        {
            string cmd = "USER/" + iMachine + "/" + password + "/" + wxId + "/" + wxName;
            SendCmd(cmd, ByteType.Double);
        }

        private void SendWXTX(int iMachine, string password, string wxId, int packId, int packTotalLen, int packLen, byte[] dataBuf)
        {
            string cmd = "WXTX/" + iMachine + "/" + password + "/" + wxId + "/" + packId + "/" + packTotalLen + "/" + packLen + "/";

            int len = cmd.Length + 5 + dataBuf.Length;
            string[] arrstr = cmd.Split('/');
            cmd = arrstr[0] + "/" + String.Format("{0:D4}", len);
            for (int i = 1; i < arrstr.Length; i++)
            {
                cmd += "/" + arrstr[i];
            }

            byte[] cmdBuf = Encoding.Default.GetBytes(cmd);
            byte[] sendBuf = new byte[cmdBuf.Length+dataBuf.Length];
            cmdBuf.CopyTo(sendBuf, 0);
            dataBuf.CopyTo(sendBuf, cmdBuf.Length);
            SendCmd(sendBuf, ByteType.Single);
        }

        //微信头像
        public void SendWXTX(int iMachine, string password, string wxId, int dataTotalLen, byte[] dataBuf)
        {
            int packId = 0;            
            int sendedLen = 0;

            while (sendedLen < dataTotalLen)
            {
                int dataLen = 0;
                byte[] sendData = null;
                if (dataTotalLen - sendedLen >= SEND_MAX_DATA_LEN)
                    dataLen = SEND_MAX_DATA_LEN;
                else
                    dataLen = dataTotalLen - sendedLen;

                sendData = new byte[dataLen];
                Array.Copy(dataBuf, sendedLen, sendData, 0, dataLen);

                SendWXTX(iMachine, password, wxId, packId, dataTotalLen, dataLen, sendData);
                sendedLen += dataLen;
                packId++;
            }

            SendTXWB(iMachine, password, wxId);
        }

        //获取所有麻将
        public void SendGetPai(int iMachine, string password, string wxId, string randPassword, int roomId)
        {
            string cmd = "GETPAI/" + iMachine + "/" + password + "/" + wxId + "/" + randPassword + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }

        //加入房间（自由匹配）
        public void SendJoinRoom_自由匹配(int iMachine, string password, string wxId)
        {
            string cmd = "JRFJ/" + iMachine + "/" + password + "/" + wxId;
            SendCmd(cmd, ByteType.Single);
        }

        //加入房间
        public void SendJoinRoom(int iMachine, string password, string wxId, int roomId)
        {
            string cmd = "JRFJ2/" + iMachine + "/" + password + "/" + wxId + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }

        //创建房间
        public void SendCreateRoom(int iMachine, string password, string wxId, int juShu, string playRule, string strArea)
        {
            string cmd = "CJZZ/" + iMachine + "/" + password + "/" + wxId + "/" + juShu + "/" + playRule + "/" + strArea;
            SendCmd(cmd, ByteType.Single);
        }
        //出牌
        public void SendChuPai(int iMachine, string password, string wxId, string weizhi, string mj)
        {
            string cmd = "CHUPAI/" + iMachine + "/" + password + "/" + wxId + "/" + weizhi + "/" + mj;
            SendCmd(cmd, ByteType.Single);
        }
        //吃碰杠胡
        public void SendCPGH(int iMachine, string password, string wxId, string weizhi, int data, string ChiPai, string AnGangPai)
        {
            string cmd = "CPGH/" + iMachine + "/" + password + "/" + wxId + "/" + weizhi + "/" + data + "/" + ChiPai + "/" + AnGangPai;
            SendCmd(cmd, ByteType.Single);
        }

        //聊天
        public void SendLT(int iMachine, string password, string wxId, string context)
        {
            string cmd = "LT/" + iMachine + "/" + password + "/" + wxId + "/" + context;
            SendCmd(cmd, ByteType.Double);
        }
        //准备
        public void SendPrepare(int iMachine, string password, string wxId)
        {
            string cmd = "ZB/" + iMachine + "/" + password + "/" + wxId;
            SendCmd(cmd, ByteType.Single);
        }
        //心跳
        public void SendHeart(int iMachine, string password, string wxId)
        {
            string cmd = "HEART/" + iMachine + "/" + password + "/" + wxId;
            SendCmd(cmd, ByteType.Single);
        }
        //离开房间
        public void SendLKFJ(int iMachine, string password, string wxId)
        {
            string cmd = "LKFJ/" + iMachine + "/" + password + "/" + wxId;
            SendCmd(cmd, ByteType.Single);
        }
        //解散房间
        public void SendJSFJ(int iMachine, string password, string wxId)
        {
            string cmd = "JSFJ/" + iMachine + "/" + password + "/" + wxId;
            SendCmd(cmd, ByteType.Single);
        }
        //离开房间投票
        public void SendLeaveRoomTP(int iMachine, string password, string wxId, int val)
        {
            string cmd = "TP/" + iMachine + "/" + password + "/" + wxId + "/" + val;
            SendCmd(cmd, ByteType.Single);
        }
        //获取前20场战绩
        public void SendGetZJ(int iMachine, string password, string wxId)
        {
            string cmd = "GETZJ/" + iMachine + "/" + password + "/" + wxId;
            SendCmd(cmd, ByteType.Double);
        }

        //语音
        public void SendYuYing(int iMachine, string password, string wxId, int dataTotalLen, byte[] dataBuf)
        {
            int packId = 0;
            int sendedLen = 0;

            while (sendedLen < dataTotalLen)
            {
                int dataLen = 0;
                byte[] sendData = null;
                if (dataTotalLen - sendedLen >= SEND_MAX_DATA_LEN)
                    dataLen = SEND_MAX_DATA_LEN;
                else
                    dataLen = dataTotalLen - sendedLen;

                sendData = new byte[dataLen];
                Array.Copy(dataBuf, sendedLen, sendData, 0, dataLen);

                SendYuYing(iMachine, password, wxId, packId, dataTotalLen, dataLen, sendData);
                sendedLen += dataLen;
                packId++;
            }
        }
        private void SendYuYing(int iMachine, string password, string wxId, int packId, int packTotalLen, int packLen, byte[] dataBuf)
        {
            string cmd = "YUYING/" + iMachine + "/" + password + "/" + wxId + "/" + packId + "/" + packTotalLen + "/" + packLen + "/";
            int len = cmd.Length + 5 + dataBuf.Length;
            string[] arrstr = cmd.Split('/');
            cmd = arrstr[0] + "/" + String.Format("{0:D4}", len);
            for (int i = 1; i < arrstr.Length; i++)
            {
                cmd += "/" + arrstr[i];
            }
            byte[] cmdBuf = Encoding.Default.GetBytes(cmd);
            byte[] sendBuf = new byte[cmdBuf.Length + dataBuf.Length];
            cmdBuf.CopyTo(sendBuf, 0);
            dataBuf.CopyTo(sendBuf, cmdBuf.Length);
            SendCmd(sendBuf, ByteType.Single, _socket_WXTX);
        }

        //发送表情
        public void SendBQ(int iMachine, string password, string wxId, string face_name)
        {
            string cmd = "BQ/" + iMachine + "/" + password + "/" + wxId + "/" + face_name;
            SendCmd(cmd, ByteType.Single);
        }
        //发送说话
        public void SendSH(int iMachine, string password, string wxId, string clipName)
        {
            string cmd = "SH/" + iMachine + "/" + password + "/" + wxId + "/" + clipName;
            SendCmd(cmd, ByteType.Double);
        }
        //获取玩法规则
        public void SendZBCJ(int iMachine, string password, string wxId, string area)
        {
            string cmd = "ZBCJ/" + iMachine + "/" + password + "/" + area;
            SendCmd(cmd, ByteType.Double);
        }
        //回放开始
        public void SendHFKS(int iMachine, string password, string wxId, int roomId)
        {
            string cmd = "HFKS/" + iMachine + "/" + password + "/" + wxId + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }
        //回放暂停
        public void SendHFZT(int iMachine, string password, string wxId, int roomId)
        {
            string cmd = "HFZT/" + iMachine + "/" + password + "/" + wxId + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }
        //回放恢复
        public void SendHFHF(int iMachine, string password, string wxId, int roomId)
        {
            string cmd = "HFHF/" + iMachine + "/" + password + "/" + wxId + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }
        //回放上一局
        public void SendHFDEC(int iMachine, string password, string wxId, int roomId)
        {
            string cmd = "HFDEC/" + iMachine + "/" + password + "/" + wxId + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }
        //回放下一局
        public void SendHFINC(int iMachine, string password, string wxId, int roomId)
        {
            string cmd = "HFINC/" + iMachine + "/" + password + "/" + wxId + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }
        //回放离开
        public void SendHFLK(int iMachine, string password, string wxId, int roomId)
        {
            string cmd = "HFLK/" + iMachine + "/" + password + "/" + wxId + "/" + roomId;
            SendCmd(cmd, ByteType.Single);
        }
        
        public void SendME(int iMachine, string password, string wxId)
        {
            string cmd = "ME/" + iMachine + "/" + password + "/" + wxId;
            AddCmdLen(ref cmd);
            byte[] cmdBuf = Encoding.Default.GetBytes(cmd);
            SendCmd(cmdBuf, ByteType.Single, _socket_WXTX);
        }
        //头像发送完毕
        public void SendTXWB(int iMachine, string password, string wxId)
        {
            string cmd = "TXWB/" + iMachine + "/" + password + "/" + wxId;
            SendCmd(cmd, ByteType.Single);
        }
        //获取头像
        public void SendGETTX(int iMachine, string password, string wxId, string data)
        {
            string cmd = "GETTX/" + iMachine + "/" + password + "/" + wxId + "/" + data;
            SendCmd(cmd, ByteType.Single);
        }
        #endregion

    }

}
