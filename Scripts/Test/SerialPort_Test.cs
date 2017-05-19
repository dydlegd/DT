using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.IO.Ports.Custom;
using System.Runtime.InteropServices;

namespace DYD
{
#if UNITY_ANDROID
    public class CommuPara : CommuFun
    {

    }
    public class SerialPort_Test : MonoBehaviour
    {

        [DllImport("linuxser")]
        private static extern int open_port(int fd, int comport);
        [DllImport("linuxser")]
        private static extern int set_opt(int fd, int nSpeed, int nBits, char nEvent, int nStop);
        [DllImport("linuxser")]
        private static extern int send_buff(int fd, byte[] data, int datalen);
        [DllImport("linuxser")]
        private static extern int read_buff(int fd, byte[] data, int datalen);



        public static SerialPort_Test Instance;
        private UITextList textList;
        private UIPopupList popup_端口号;
        private UIPopupList popup_波特率;
        private UIInput textInput;
        public Queue mQueueCommand = Queue.Synchronized(new Queue());

        private CommuPara serial = new CommuPara();

        private Thread tReceive;
        private bool bOpenPort = false;
        private int portId = 0;

        void Awake()
        {
            Instance = this;
        }


        // Use this for initialization
        void Start()
        {
            Debuger.EnableLog = GD.ENABLELOG;

            tReceive = new Thread(ReceiveDatasThread);

            textList = GetComponentInChildren<UITextList>();
            popup_端口号 = GameUtility.FindDeepChild(gameObject, "Popup_端口号").GetComponent<UIPopupList>();
            popup_波特率 = GameUtility.FindDeepChild(gameObject, "Popup_波特率").GetComponent<UIPopupList>();
            textInput = GameUtility.FindDeepChild(gameObject, "Input Field").GetComponent<UIInput>();

            EventInit();
        }

        private void EventInit()
        {
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Button_打开串口").gameObject).onClick = delegate
            {
                OpenPort();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Button_关闭串口").gameObject).onClick = delegate
            {
                ClosePort();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Button_发送").gameObject).onClick = delegate
            {
                SendBuf();
            };
            UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "Button_清除数据").gameObject).onClick = delegate
            {
                textList.Clear();
            };
        }

        private void ReceiveDatasThread()
        {
            byte[] recBuf = new byte[256];
            while (true)
            {
                int len = read_buff(portId, recBuf, 256);
                AddQueuCommand(recBuf, len);
            }
        }

        int index = 0;
        byte[] cmdBuf = new byte[256];
        // Update is called once per frame
        void Update()
        {

            if (Input.GetKey(KeyCode.Return))
            {
                AddText("asdf" + index++);
            }

            int len = GetCommand(ref cmdBuf);
            if (len > 0)
            {
                Debuger.Log("长度："+len+"-------"+cmdBuf);

                byte[] buf2 = new byte[len];
                Array.Copy(cmdBuf, buf2, len);
                AddText(byteToHexStr(buf2));


                //string context = Encoding.Default.GetString(cmdBuf, 0, len);
                //AddText(context);

                //for (int i = 0; i < len; i++)
                //{
                //    if (cmdBuf[i]==0xaa)
                //    {
                //        Debuger.Log("AA");
                //    }
                //    else if (cmdBuf[i] == 0xbb)
                //    {
                //        Debuger.Log("BB");
                //    }
                //    else if (cmdBuf[i] == 0xcc)
                //    {
                //        Debuger.Log("CC");
                //    }
                //    else if (cmdBuf[i] == 0xdd)
                //    {
                //        Debuger.Log("DD");
                //    }
                //}
            }
        }

        public static string byteToHexStr(byte[] bytes)
        {
           string StringOut = "";
           foreach (byte InByte in bytes)
           {
               StringOut = StringOut + String.Format("{0:X2} ", InByte);
           }
           return StringOut;  
        }

        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private string GetPortName()
        {
            return "COM" + popup_端口号.value;
        }

        public void AddQueuCommand(byte []buf, int len)
        {
            byte[] store_buf = new byte[len];
            Array.Copy(buf, store_buf, len);
            mQueueCommand.Enqueue(store_buf);
        }

        private int GetCommand(ref byte[] buf)
        {
            if (mQueueCommand.Count > 0)
            {
                byte[] temp = (byte[])mQueueCommand.Peek();
                buf = (byte[])mQueueCommand.Dequeue();
                return temp.Length;
            }
            return 0;
        }

        private void AddText(byte[] buf, int len)
        {
            string context = Encoding.Default.GetString(buf, 0, len);            
            //AddText(context);

            //Debuger.Log(buf);

            byte[] buf2 = new byte[len];
            Array.Copy(buf, buf2, len);
            //AddText(byteToHexStr(buf2));
        }

        private void AddText(string text)
        {
            textList.Add(text);
        }

        private int GetRate()
        {
            return Int32.Parse(popup_波特率.value);
        }

        private void OpenPort()
        {
            //serial.OpenPort(GetPortName());

            if (!bOpenPort)
            {
                int ret = open_port(1, 1);
                bOpenPort = ret > 0 ? true : false;

                if (bOpenPort)
                {
                    portId = ret;
                    set_opt(portId, GetRate(), 8, 'N', 1);
                }
                Debuger.Log(bOpenPort ? "打开串口成功" : "打开串口失败");
            }            
            else
            {
                Debuger.Log("串口成功已打开");
            }
           
        }
        private void ClosePort()
        {
            //serial.ClosePort();
        }

        private void SendBuf()
        {
            string context = textInput.value;
            byte[] buf = strToToHexByte(context);
            send_buff(portId, buf, buf.Length);

            textInput.value = "";
        }

    }
#endif
}

