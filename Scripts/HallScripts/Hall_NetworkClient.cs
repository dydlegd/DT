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
    public class Hall_NetworkClient : NetworkBase
    {
        public const string IP = "192.168.0.117";
        //public const string IP = "139.129.50.224";
        public const int PortEnglish = 6662;

        private Thread sendThread;

        public override void Init()
        {
            base.Init();

            Connect();
        }

        public virtual void UpdateLogic()
        {
            if (IsSwitchStatused)
            {
                switch (status)
                {
                    case NetworkBase.ConnectState.Disconnect:
                        Hall_ViewCenter.Instance.GetPanel<Hall_Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sNetDisconnect, "", delegate { Connect(); });
                        break;
                    case NetworkBase.ConnectState.Connecting:
                        //Hall_ViewCenter.Instance.GetPanel<Hall_Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sNetConnecting, 3);
                        break;
                    case NetworkBase.ConnectState.Connected:
                        Hall_DealCommand.Instance.SendGetVersion();
                        break;
                    case NetworkBase.ConnectState.Error:
                        Hall_ViewCenter.Instance.GetPanel<Hall_Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sNetConnectFailed, "", delegate { Connect(); });
                        break;
                    default:
                        break;
                }
                IsSwitchStatused = false;
            }
        }

        public override void Connect()
        {
            //Connect(IP, PortEnglish, PortChinese);
            ThreadedConnect(IP, PortEnglish);
        }

        private void ThreadedConnect(string hostAddress, int portEnglish)
        {
            connectThread = new Thread(new ParameterizedThreadStart(ThreadedConnect));
            connectThread.Start(new object[] { hostAddress, portEnglish });
        }

        private void ThreadedConnect(object hostAndPort)
        {
            string hostAddress = (string)((object[])hostAndPort)[0];
            int portEnglish = (int)((object[])hostAndPort)[1];

            if (status != ConnectState.Connected)
            {
                SetConnectState(ConnectState.Connecting);
                IPAddress ip = IPAddress.Parse(hostAddress);
                IPEndPoint ipEP_English = new IPEndPoint(ip, portEnglish);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.NoDelay = true;

                try
                {
                    _socket.Connect(ipEP_English);
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

            Debuger.Log("NetworkClient Init Succeed !");
        }

        protected override void RunThread()
        {
            while (true)
            {
                if (IsConnectOK)
                {
                    try
                    {
                        //lock (recLockObj)
                        {
                            Array.Clear(mReceiveBuf, 0, mReceiveBuf.Length);
                            int length = _socket.Receive(mReceiveBuf, mReceiveBuf.Length, SocketFlags.None);
                            string text = Encoding.Default.GetString(mReceiveBuf, 0, length);
                            Debuger.Log(String.Format("Receive({0}) {1}:", PortEnglish, length) + text);
                            mSingleDecodeBuf.DecodeByte(mReceiveBuf, length);
                        }
                    }
                    catch (Exception e)
                    {
                        Debuger.LogError("接收数据异常！" + e);
                        Disconnect();
                    }
                }
                System.Threading.Thread.Sleep(10);
            }
            Debuger.Log("NetworkClient 接收线程退出");
        }

        private void SendThread()
        {
            NetDataBuf netDataBuf = null;
            while (true)
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

        public override void CloseNetwork()
        {
            base.CloseNetwork();
            NetDataCenter.Instance.Clear();

            Debuger.Log("CloseNetwork !");
        }

        private void SendCmd(byte[] buf, ByteType byteType)
        {
            NetDataBuf netData = null;
            if (byteType == ByteType.Single)
                netData = new NetDataBuf(_socket, buf, byteType);
            //else
            //    netData = new NetDataBuf(_socket_Chinese, buf, byteType);

            NetDataCenter.Instance.AddQueueSend(netData);
        }

        private void SendCmd(string cmd, ByteType byteType)
        {
            AddCmdLen(ref cmd);
            byte[] byteBuf = new byte[2048];
            if (byteType == ByteType.Single)
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

        public void SendGetVersion()
        {
            string cmd = "GETVER";
            SendCmd(cmd, ByteType.Single);
        }
    }
}

