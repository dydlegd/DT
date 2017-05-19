//#define MY_DEBUG
//#define HW_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace System.IO.Ports.Custom
{
#if UNITY_ANDROID
#if HW_ANDROID
    public class SerialPorts
	{
		//
		public bool IsOpen = false;
		private int fd;
		
		[DllImport("ports")]
		private extern static int TtyOpen(ref int fd);
		
		[DllImport("ports")]
		private extern static int TtyRead(int fd, byte[] buf, int buf_size);
		
		[DllImport("ports")]
		private extern static int TtyWrite(int fd, byte[] buf, int buf_size);
		
		[DllImport("ports")]
		private extern static int TtyClose(int fd);
		
		//
		public void Open()
		{
			if (TtyOpen(ref fd) < 0)
			{
				IsOpen = false;
			}
			else
			{
				IsOpen = true;
			}
		}
		
		public void Open(string name)
		{
			Open();
		}
		
		//
		public int Read(ref byte[] buf, int buf_size)
		{
			return TtyRead(fd, buf, buf_size);
		}
		
		//
		public int Write(byte[] buf, int buf_size)
		{
			return TtyWrite(fd, buf, buf_size);
		}
		
		//
		public void Close()
		{
			TtyClose(fd);
			IsOpen = false;
		}
	}
	
	#else
	
	public class SerialPorts
	{
		
		public bool IsOpen = false;
		private int handle;
		
		[DllImport("libports")]
		private extern static int ComOpen(ref int handle, byte[] portname);
		
		[DllImport ("libports")]
		private extern static int ComSetBaudRate(int handle, int baud_rate);
		
		[DllImport("libports")]
		private extern static int ComRead(int handle, byte[] buf, int buf_size);
		
		[DllImport("libports")]
		private extern static int ComWrite(int handle, byte[] buf, int buf_size);
		
		[DllImport("libports")]
		private extern static int ComClose(int handle);
		
		
		public void Open()
		{
			byte[] portname = System.Text.Encoding.ASCII.GetBytes("COM5");
			if (ComOpen(ref handle, portname) < 0)
			{
				IsOpen = false;
			}
			else
			{
				IsOpen = true;
			}
		}
		
		public void Open(string name)
		{
			byte[] portname = System.Text.Encoding.ASCII.GetBytes(name);
			if (ComOpen(ref handle, portname) < 0)
			{
				IsOpen = false;
			}
			else
			{
				IsOpen = true;
			}
		}
		
		public int SetBaudRate(int handle, int baud_rate)
		{
			return ComSetBaudRate (handle, baud_rate);
		}
		
		public int Read(ref byte[] buf, int buf_size)
		{
			return ComRead(handle, buf, buf_size);
		}
		
		
		public int Write(byte[] buf, int buf_size)
		{
			return ComWrite(handle, buf, buf_size);
		}
		
		
		public void Close()
		{
			ComClose(handle);
			IsOpen = false;
		}
	}
	#endif
	
	
	#region 串口类.
	public class CommuPort
	{
		public class SendPara {   //发送命令控制类.
			public byte[] buf = new byte[256];
			public byte cmd = 0;
			public byte len = 0;
			public int  to = 0;
			public int times = 0;
		}
		
		public class ReceivePara { //接收命令控制类.
			public byte rec = 0;
			public byte store = 0;
		}
		
		private byte[] decode_buf = new byte[256];                 //解码区.
		private byte decode_cnt;                                   //解码计数.
		private SerialPorts sp = new SerialPorts();                //串口对象.
		private Thread tReceive;                                   //接收线程.
		private Thread tSend;                                      //发送线程.
		private Queue<byte[]> rec_que = new Queue<byte[]>(1);      //接收命令队列.
		private Queue<SendPara> send_que = new Queue<SendPara>(1); //发送命令队列.
		private ReceivePara rec_cmd = new ReceivePara();           //接收到的命令类型.
		
		#region 通信译码.		
		private bool CommuCheckSum()
		{
			byte check_sum = 0;
			byte i;
			
			for (i = 1; i < decode_buf[3] + 4; i++)
			{
				check_sum += decode_buf[i];
			}
			if (check_sum == decode_buf[decode_buf[3]+4])
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		/* 命令误判或数据错误 在缓存中检索第二条命令 */
		private bool CommuDecodeMove()
		{
			byte i, j;
			
			for (i = 1; i < decode_cnt; i++)
			{
				if (0xfa == decode_buf[i])
				{
					for (j = 0; j < decode_cnt - i; j++)
					{
						decode_buf[j] = decode_buf[i + j];
					}
					decode_cnt = (byte)(decode_cnt - i);
					return true;
				}
			}
			decode_cnt = 0;
			return false;
		}
		
		private void CommuDecodeByte(byte elem)
		{
			decode_buf[decode_cnt] = elem;
			decode_cnt++;
			do
			{
				if (0xfa == decode_buf[0])
				{ 
					/* 收到命令头 */
					/*
                     * 必须判断命令长度 防止误判或数据错误造成溢出
                     * 控制字节与命令类型不需判断 如发生误判也不会丢失数据
                     */
					if (decode_cnt > 3 && decode_buf[3] <= 249)
					{
						if (decode_buf[3] + 6 <= decode_cnt)
						{ /* 收到完整数据 */
							if (0xfb == decode_buf[decode_buf[3]+5] && CommuCheckSum())
							{
								ReceiveProcess(decode_buf, (byte)(decode_buf[3]+6));
								decode_cnt -= (byte)(decode_buf[3]+6);
							}
							else
							{
								if (CommuDecodeMove())
								{
									continue;
								}
							}
						}
					}
					else
					{
						if (decode_cnt > 3)
						{
							if (CommuDecodeMove())
							{
								continue;
							}
						}
					}
				}
				else
				{
					decode_cnt = 0;
				}
				break;
			} while (true);
		}
		#endregion
		
		private void ReceiveDatasThread()
		{
			while (true) {
				if(sp.IsOpen) {
					#if MY_DEBUG
					byte[] buf = new byte[256];
					string str = "";
					int num = sp.Read(ref buf, 256);
					for(int i = 0; i < num; i++) {
						str += buf[i].ToString("X2")+" ";
						CommuDecodeByte(buf[i]);
					}
					if(num > 0) {
						Debuger.Log(str);
					}
					#else
					byte[] buf = new byte[256];
					int num = sp.Read(ref buf, 256);
                    DYD.SerialPort_Test.Instance.AddQueuCommand(buf, num);
					for(int i = 0; i < num; i++) {
						CommuDecodeByte(buf[i]);
					}
					#endif
				}
				Thread.Sleep(20); 
			}
		}
		
		private void ReceiveProcess(byte[] buf, byte len)
		{
			byte[] store_buf = new byte[256];
			//byte[] store_buf = new byte[len];
			Array.Copy(buf, store_buf, len); //动态数组只是管理数组，它指向真正的数组，所以每个数组必须有自己实际的内存空间.
			rec_que.Enqueue(store_buf);
			
			rec_cmd.rec = buf[2];
		}
		
		public byte GetCommand(ref byte[] buf)
		{	
			if(rec_que.Count > 0) {      //如果动态数组中有消息存储.
				buf = rec_que.Dequeue(); //获取最前面一条命令.
				rec_cmd.store = buf[2];
				return buf[2];
			} else {
				rec_cmd.store = 0;
				return 0;
			}
		}
		
		private void CmdPack(byte cmd, byte[] buf, byte len, ref byte[] send_buf)
		{
			byte check_sum = 0;
			
			send_buf[0] = 0xfa;
			send_buf[1] = 0x00;
			send_buf[2] = cmd;
			send_buf[3] = len;
			Array.Copy(buf, 0, send_buf, 4, len);
			for(int i = 1; i < len+4; i++) {
				check_sum += (byte)send_buf[i];
			}
			send_buf[len+4] = check_sum;
			send_buf[len+5] = 0xfb;
		}
		
		public void SendCommand(byte cmd, byte[] buf, byte len, int to, int times)
		{
			SendPara send = new SendPara();
			CmdPack(cmd, buf, len, ref send.buf);
			send.cmd = cmd;
			send.len = (byte)(len + 6);
			send.to = to;
			send.times = times;
			send_que.Enqueue(send);
			if(send.cmd == rec_cmd.rec) {    //重置rec_cmd.
				rec_cmd.rec = 0;
			}
			if(send.cmd == rec_cmd.store) {
				rec_cmd.store = 0;
			}
			tSend = new Thread(SendDatasThread);
			tSend.Start();
		}
		
		public void SendCommand(byte cmd, byte[] buf, byte len, int to)
		{
			SendCommand(cmd, buf, len, to, 3);
		}
		
		public void SendCommand(byte cmd, byte[] buf, byte len)
		{
			SendCommand(cmd, buf, len, 300);
		}
		
		public void SendCommand(byte cmd)
		{
			SendCommand(cmd, new byte[1], 0);
		}
		
		public void SendAck(byte cmd, byte[] buf, byte len)
		{
			SendCommand(cmd, buf, len, 0, 1);
		}
		
		public void SendAck(byte cmd)
		{
			SendCommand(cmd, new byte[1], 0, 0, 1);
		}
		
		public void SendTrigger(byte cmd, byte[] buf, byte len)
		{
			SendAck(cmd, buf, len);
		}
		
		private void SendDatasThread()
		{
			int i, j;
			
			SendPara send = send_que.Dequeue();
			for(i = 0; i < send.times; i++) {
				Send(send.buf, send.len);
				for(j = 0; j < send.to/2; j++) {
					Thread.Sleep(2);              //超时时间.
					if(send.cmd == rec_cmd.rec ||
					   send.cmd == rec_cmd.store ||
					   send.times == 1) { //如果已经接收到相应的命令回应，则退出重发.
						return;
					}
				}
			}
		}
		
		private void Send(byte[] buf, byte cnt)
		{	
			if(sp.IsOpen) {
				sp.Write(buf, cnt);
			}
		}
		
		protected CommuPort()
		{
			tReceive = new Thread(ReceiveDatasThread);
			if (!tReceive.IsAlive) {
				tReceive.Start();
			}
		}
		
		~CommuPort()
		{
			tReceive.Abort();
		}
		
		public bool OpenPort(string port_name)
		{
			if(!sp.IsOpen) {
				sp.Open(port_name);
			}
			if(sp.IsOpen) {
				Debuger.Log(port_name+" open success!");
				return true;
			} else {
				Debuger.Log(port_name+" open fail!");
				return false;
			}
			
			
			
		}
		
		public void ClosePort()
		{
			if(sp.IsOpen) {
				sp.Close();
			}
		}
		
	}
	#endregion
	
	
	
	#region 通信转码.
	public class CommuFun : CommuPort
	{
		public static ushort UcToU2(byte[] buf, ref byte cnt)
		{
			ushort ret;
			
			ret = (ushort)((buf[cnt] << 8) + (buf[cnt + 1]));
			cnt += 2;
			return ret;
		}
		
		public static int UcToS3(byte[] buf, ref byte cnt)
		{
			int ret;
			
			ret = (int)((buf[cnt]<<16) + (buf[cnt+1]<<8) + buf[cnt+2]);
			cnt += 3;
			return ret;
		}
		
		public static int UcToS4(byte[] buf, ref byte cnt)
		{
			int ret;
			
			ret = (int)((buf[cnt]<<24) + (buf[cnt+1]<<16) + (buf[cnt+2]<<8) + (buf[cnt+3]));
			cnt += 4;
			return ret;
		}
		
		public static long UcToS6(byte[] buf, ref byte cnt)
		{
			long ret = 0;
			int sign_flag;
			
			if((buf[cnt] & 0x80) != 0) {
				sign_flag = -1;
			} else {
				sign_flag = 1;
			}
			buf[cnt] &= 0x7f;
			ret = sign_flag*(((long)buf[cnt]<<40) + 
			                 ((long)buf[cnt+1]<<32) + 
			                 ((long)buf[cnt+2]<<24) + 
			                 ((long)buf[cnt+3]<<16) + 
			                 ((long)buf[cnt+4]<<8) + 
			                 ((long)buf[cnt+5]));
			cnt += 6;
			return ret;
		}
		
		public static void U2ToUc(ushort num, byte[] buf, ref byte cnt)
		{
			buf[cnt] = (byte)((num>>8)&0xff);
			buf[cnt+1] = (byte)(num&0xff);
			cnt += 2;
		}
		
		public static void S3ToUc(int num, byte[] buf, ref byte cnt)
		{
			buf[cnt] = (byte)((num>>16)&0xff);
			buf[cnt+1] = (byte)((num>>8)&0xff);
			buf[cnt+2] = (byte)(num&0xff);
			cnt += 3;
		}
		
		public static void S4ToUc(int num, byte[] buf, ref byte cnt)
		{
			buf[cnt] = (byte)((num>>24)&0xff);
			buf[cnt+1] = (byte)((num>>16)&0xff);
			buf[cnt+2] = (byte)((num>>8)&0xff);
			buf[cnt+3] = (byte)(num&0xff);
			cnt += 4;
		}
		
		public static void S4ToUc(uint num, byte[] buf, ref byte cnt)
		{
			buf[cnt] = (byte)((num>>24)&0xff);
			buf[cnt+1] = (byte)((num>>16)&0xff);
			buf[cnt+2] = (byte)((num>>8)&0xff);
			buf[cnt+3] = (byte)(num&0xff);
			cnt += 4;
		}
		
		public static void S6ToUc(long num, byte[] buf, ref byte cnt)
		{
			byte sign_flag;
			
			if(num >= 0) {
				sign_flag = 0;
			} else {
				sign_flag = 0x80;
			}
			
			buf[cnt] = (byte)(((num>>40)&0xff)|sign_flag);
			buf[cnt+1] = (byte)((num>>32)&0xff);
			buf[cnt+2] = (byte)((num>>24)&0xff);
			buf[cnt+3] = (byte)((num>>16)&0xff);
			buf[cnt+4] = (byte)((num>>8)&0xff);
			buf[cnt+5] = (byte)(num&0xff);
			cnt += 6;
		}
	}
	#endregion
#endif
}

