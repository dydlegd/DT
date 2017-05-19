using UnityEngine;
using System.Collections;
using System;
using System.Text;

namespace DYD
{
    public class NetDecodeBuf
    {
        private const int DECODE_BUF_LEN = 2048;
        public ByteType byteType { get; private set; }
        public byte[] mDecodeBuf = new byte[DECODE_BUF_LEN];
        public int mDecodeCnt;
        private NetDataBuf LastNetBuf{get;set;}

        public NetDecodeBuf(ByteType byteType)
        {
            this.byteType = byteType;
        }

        public void Clear()
        {
            mDecodeCnt = 0;
        }

        public void DecodeByte(byte[] buf, int len)
        {
            for (int i = 0; i < len; i++)
            {
                DecodeByte(buf[i]);
            }
        }

        private void DecodeByte(byte elem)
        {
            mDecodeBuf[mDecodeCnt++] = elem;

            int packTotalLen = GetPackTotalLen();

            if (mDecodeCnt >= packTotalLen)
            {
                ParseDecodeBuf(mDecodeBuf, mDecodeCnt);
                Array.Clear(mDecodeBuf, 0, mDecodeBuf.Length);
                mDecodeCnt = 0;
            }
        }
        

        private void ParseDecodeBuf(byte[] buf, int len)
        {
            byte[] store_buf = new byte[len];
            Array.Copy(buf, store_buf, len);

            string context = GetStringContext(store_buf, len);
            string[] arrstr = context.Split('/');
            Debuger.Log(string.Format("AddOneCommand:{0}", arrstr[0]));
            if (arrstr[0] == "WXTX" || arrstr[0] == "YUYING")
            {
                int packTotalLen = Int32.Parse(arrstr[4]);
                int packId = Int32.Parse(arrstr[3]);
                int packLen = Int32.Parse(arrstr[5]);
                int dataStartIndex = len - packLen;
               

                if (packId == 0)
                {
                    ParseQueueOneCommand(store_buf, packTotalLen, dataStartIndex);
                }
                else
                {
                    byte[] dataAry = new byte[packLen];
                    Array.Copy(store_buf, dataStartIndex, dataAry, 0, packLen);

                    LastNetBuf.AddData(dataAry, byteType);
                    if (LastNetBuf.IsFullDataPacket==true)
                    {
                        NetDataCenter.Instance.AddQueueReceive(LastNetBuf.Clone());
                        LastNetBuf = null;
                    }
                }             
            }
            else
            {
                ParseQueueOneCommand(store_buf, len, 0);
            }            
        }

        private void ParseQueueOneCommand(byte[] buf, int iTotalDataLen, int iDataOffset)
        {
            if (buf.Length >= iTotalDataLen)
            {
                LastNetBuf = AddQueueOneCompleteCommand(buf);
            }
            else
            {
                NetDataBuf netBuf = new NetDataBuf(null, buf, byteType);
                netBuf.FullDataLen = iTotalDataLen;
                netBuf.DataOffset = iDataOffset;
                LastNetBuf = netBuf;
            }
        }

        private NetDataBuf AddQueueOneCompleteCommand(byte[] buf)
        {
            NetDataBuf netBuf = new NetDataBuf(null, buf, byteType);
            NetDataCenter.Instance.AddQueueReceive(netBuf);
            return netBuf;
        }

        private int GetPackTotalLen()
        {
            string text = GetStringContent();
            string[] arrstr = text.Split('/');
            int packTotalLen = DECODE_BUF_LEN;//整包长度

            if (arrstr.Length>=2 && arrstr[1].Length==4)
            {
                packTotalLen = Int32.Parse(arrstr[1]);
            }

            //switch (arrstr[0])
            //{
            //    case "VERED":
            //    case "USERED":
            //    case "USERZX":
            //        packTotalLen = text.Length;
            //        break;
            //    case "HEART":
            //        packTotalLen = 10;
            //        break;
            //    case "VER":
            //        {
            //            if (arrstr.Length >= 7)
            //            {
            //                packTotalLen = Int32.Parse(arrstr[4]);
            //                for (int i = 0; i < 6; i++)
            //                {
            //                    packTotalLen += arrstr[i].Length + 1;
            //                }
            //            }
            //        }
            //        break;
            //    case "WXTX":
            //        {
            //            if (arrstr.Length >= 3)
            //            {
            //                packTotalLen = Int32.Parse(arrstr[1]);
            //            }
            //        }
            //        break;
            //    default:
            //        {
            //            if (arrstr.Length >= 3)
            //            {
            //                packTotalLen = Int32.Parse(arrstr[1]);
            //            }
            //        }
            //        break;
            //}

            if (byteType==ByteType.Double)
            {
                packTotalLen *= 2;
            }

            return packTotalLen;
        }

        private string GetStringContent()
        {
            string text = "";
            if (byteType == ByteType.Single)
                text = Encoding.Default.GetString(mDecodeBuf, 0, mDecodeCnt);
            else if (byteType == ByteType.Double)
                text = Encoding.Unicode.GetString(mDecodeBuf, 0, mDecodeCnt);
            return text;
        }
        private string GetStringContext(byte[] buf, int len)
        {
            string text = "";
            if (byteType == ByteType.Single)
                text = Encoding.Default.GetString(buf, 0, len);
            else if (byteType == ByteType.Double)
                text = Encoding.Unicode.GetString(buf, 0, len);
            return text;
        }
    }

}
