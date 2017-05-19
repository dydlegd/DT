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
    public enum CmdType
    {
        Null = 0,
        Login = 0xaa01,
        Logout = 0xaa02,
    }

    public enum ByteType
    {
        Single,
        Double
    }

    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public abstract class NetworkBase : ThreadBase
    {
        public static ushort UcToU2(byte[] buf, ref ushort cnt)
        {
            ushort ret;

            ret = (ushort)((buf[cnt] << 8) + (buf[cnt + 1]));
            cnt += 2;
            return ret;
        }

        public static int UcToS4(byte[] buf, ref ushort cnt)
        {
            int ret;

            ret = (int)((buf[cnt] << 24) + (buf[cnt + 1] << 16) + (buf[cnt + 2] << 8) + (buf[cnt + 3]));
            cnt += 4;
            return ret;
        }

        public static void U2ToUc(ushort num, byte[] buf, ref ushort cnt)
        {
            buf[cnt] = (byte)((num >> 8) & 0xff);
            buf[cnt + 1] = (byte)(num & 0xff);
            cnt += 2;
        }

        public static void S4ToUc(int num, byte[] buf, ref ushort cnt)
        {
            buf[cnt] = (byte)((num >> 24) & 0xff);
            buf[cnt + 1] = (byte)((num >> 16) & 0xff);
            buf[cnt + 2] = (byte)((num >> 8) & 0xff);
            buf[cnt + 3] = (byte)(num & 0xff);
            cnt += 4;
        }

        public static string UcToString(byte[] buf, ref ushort cnt)
        {
            byte[] byteArray = new byte[buf[cnt] + 1];

            for (int i = 0; i < buf[cnt]; i++)
            {
                byteArray[i] = buf[cnt + 1 + i];
            }
            cnt += (ushort)(buf[cnt] + 1);

            return Encoding.UTF8.GetString(byteArray);
        }

        public static void StringToUc(string str, byte[] buf, ref ushort cnt)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);

            buf[cnt] = (byte)(byteArray.Length);

            for (int i = 0; i < buf[cnt]; i++)
            {
                buf[cnt + 1 + i] = byteArray[i];
            }
            cnt += (ushort)(buf[cnt] + 1);
        }

        public const int SEND_DELAY = 10;
        public const int SEND_MAX_DATA_LEN = 1350;

        protected const int MAX_REC_BUF_LEN = 2048;
        protected const int MAX_DECODE_BUF_LEN = MAX_REC_BUF_LEN;
        public enum ConnectState
        {
            Null,
            Disconnect,
            Connecting,
            Connected,
            Error
        }
        public ConnectState status { get; private set; }
        protected bool IsSwitchStatused = false;//是否转换了状态

        protected byte[] mReceiveBuf = new byte[MAX_REC_BUF_LEN];
        protected int receiveByteCount = 0;

        protected byte[] mDecodeBuf = new byte[MAX_DECODE_BUF_LEN];        
        protected int mDecodeCnt = 0;

        protected NetDecodeBuf mSingleDecodeBuf = new NetDecodeBuf(ByteType.Single);

        protected Queue mQueueCommand = Queue.Synchronized(new Queue());
        protected Queue mQueueSend = Queue.Synchronized(new Queue());

        protected Socket _socket;

        protected virtual bool IsConnectOK { get { return status == ConnectState.Connected && _socket != null; } }

        protected Thread connectThread;

        public delegate void BasicEvent();

        public delegate void NetworkErrorEvent(Exception exception);

        public event BasicEvent connected
        {
            add { connectedInvoker += value; }
            remove { connectedInvoker -= value; }
        } protected BasicEvent connectedInvoker;

        public event BasicEvent disconnected
        {
            add { disconnectedInvoker += value; }
            remove { disconnectedInvoker -= value; }
        } protected BasicEvent disconnectedInvoker;

        public event NetworkErrorEvent error
        {
            add { errorInvoker += value; }
            remove { errorInvoker -= value; }
        } protected NetworkErrorEvent errorInvoker;

        public virtual void Init()
        {
            status = ConnectState.Null;            
            connected += ConnectedCall;
            error += ErrorCall;
        }     
   
        public virtual void UpdateLogic()
        {
            if (IsSwitchStatused)
            {
                switch (status)
                {
                    case NetworkBase.ConnectState.Disconnect:
                        ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sNetDisconnect, "", delegate { Application.Quit(); });
                        break;
                    case NetworkBase.ConnectState.Connecting:
                        //ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sNetConnecting, 3);
                        break;
                    case NetworkBase.ConnectState.Connected:
                        if(GameApp.Instance.gameState==GAME_STATE.HALL && !GD.RELEASE) DealCommand.Instance.SendLogin("浮夸");
                        break;
                    case NetworkBase.ConnectState.Error:
                        ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sNetConnectFailed, "", delegate { Connect(); });
                        break;
                    default:
                        break;
                }
                IsSwitchStatused = false;
            }            
        }

        protected virtual void SetConnectState(ConnectState state)
        {
            if (status == state) return;

            status = state;
            switch (status)
            {
                case ConnectState.Disconnect:                    
                    break;
                case ConnectState.Connecting:
                    break;
                case ConnectState.Connected:                    
                    break;
                case ConnectState.Error:                    
                    break;
                default:
                    break;
            }
            IsSwitchStatused = true;
        }

        protected virtual void ConnectedCall()
        {
            Debuger.Log("网络连接成功：");
            NetHeartBeat.Instance.Connected = true;
        }

        protected virtual void ErrorCall(Exception e)
        {
            Debuger.LogError("网络连接失败：" + e);
            CloseNetwork();
            //ViewCenter.Instance.GetPanel<Panel_MessageBox>(PanelType.MessageBox).ShowMessageBox(Tags.TextPrompt.sNetConnectFailed, "", delegate { Connect(); });
        }

        public virtual void Disconnect()
        {
            if (status != ConnectState.Disconnect)
            {
                if (disconnectedInvoker != null) disconnectedInvoker();
                SetConnectState(ConnectState.Disconnect);
                CloseNetwork();            
            }            
            Debuger.Log("网络断开连接，请检查网络！");
        }

        public virtual void Connect()
        {
            
        }

        protected bool Connect(string hostAddress, int port)
        {
            SetConnectState(ConnectState.Connecting);
            IPAddress ip = IPAddress.Parse(hostAddress);
            IPEndPoint ipEP = new IPEndPoint(ip, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.NoDelay = true;
            try
            {
                _socket.Connect(ipEP);
                SetConnectState(ConnectState.Connected);
                StartThread();
                thread.IsBackground = true;
                if (connectedInvoker != null) connectedInvoker();
            }
            catch (Exception e)
            {
                SetConnectState(ConnectState.Error);
                if (errorInvoker != null) errorInvoker(e);
                Debuger.LogError("网络连接失败："+e);
                return false;
            }

            return true;
        }

        protected override void RunThread()
        {
            while(true)
            {
                if (IsConnectOK)
                {
                    int length = _socket.Receive(mReceiveBuf);
                    receiveByteCount += length;

                    for (int i = 0; i < length; i++)
                    {
                        DecodeByte(mReceiveBuf[i], length);
                    }
                    mSingleDecodeBuf.DecodeByte(mReceiveBuf, length);
                }
            }
        }

        public virtual void CloseNetwork()
        {
            ExitThread();
            if (_socket!=null)
            {
                if (_socket.Connected) _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
            }
            mSingleDecodeBuf.Clear();
        }

        protected virtual void DecodeByte(byte elem, int length)
        {
            mDecodeBuf[mDecodeCnt++] = elem;
            //if (mDecodeCnt >= 4)
            //{
            //    int len = (mDecodeBuf[2] << 8) + mDecodeBuf[3];
            //    if (len > length)
            //    {
            //        Debuger.Log("接收到的长度小于命令需要的长度");
            //    }
            //    if (len + 4 <= mDecodeCnt)
            //    {
            //        AddQueueCommand(mDecodeBuf, mDecodeCnt);
            //        mDecodeCnt = 0;
            //    }
            //}
        }

        protected void AddQueueCommand(byte[] buf, int len)
        {
            byte[] store_buf = new byte[len];
            Array.Copy(buf, store_buf, len);

            mQueueCommand.Enqueue(store_buf);
            if (mQueueCommand.Count > 50)
            {  //最多存储50条消息记录
                mQueueCommand.Dequeue();       //超过后就删除最先接收到的消息
            }
        }

        public ushort GetCommand(ref byte[] buf)
        {
            if (mQueueCommand.Count > 0)
            {
                buf = (byte[])mQueueCommand.Dequeue();
                return (ushort)((buf[0] << 8) + buf[1]);
            }
            else
            {
                return 0;
            }
        }

        public string GetCommand(ref List<string> strList, ref NetDataBuf dataBuf)
        {
            if (NetDataCenter.Instance.GetQueueReceive(ref dataBuf))
            {
                string[] arrstr = dataBuf.sb.ToString().Split('/');
                for (int i = 2; i < arrstr.Length; i++)
                {
                    strList.Add(arrstr[i]);
                }
                return arrstr[0];
            }
            return "";
        }

        protected byte[] CmdPack(ushort cmd, byte[] buf, ushort len)
        {
            byte[] send_buf = new byte[len + 4];
            send_buf[0] = (byte)(cmd >> 8);
            send_buf[1] = (byte)cmd;
            send_buf[2] = (byte)(len >> 8);
            send_buf[3] = (byte)len;
            Array.Copy(buf, 0, send_buf, 4, len);

            return send_buf;
        }

        public void SendCommand(ushort cmd, byte[] buf, ushort len)
        {
            byte[] s_buf = CmdPack(cmd, buf, len);
            SendBuf(s_buf);
        }

        public void SendCommand(CmdType cmd, byte[] buf, ushort len)
        {
            byte[] s_buf = CmdPack((ushort)cmd, buf, len);
            SendBuf(s_buf);
        }

        private void SendBuf(byte[] buf)
        {
            if (IsConnectOK)
            {
                try
                {
                    int sendLen = _socket.Send(buf, buf.Length, SocketFlags.None);
                }
                catch (Exception e)
                {
                    Debuger.LogError("发送数据异常！" + e);
                }
            }            
        }
    }
}

