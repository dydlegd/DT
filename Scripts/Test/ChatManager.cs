using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ChatManager : MonoBehaviour {

    public const string IP = "127.0.0.1";
    public const int IPort = 9999;

    private Socket clientSocket;

    public UIInput textInput;
    public UILabel chatLabel;

    private Thread t;
    private byte[] recBuf = new byte[1024];

    private string message = "";

	// Use this for initialization
	void Start () {
        ConnectToServer();
	}
	
	// Update is called once per frame
	void Update () {

        //SendMessage("你好吗？");

        if (message!="")
        {
            chatLabel.text += "\n" + message;
            message = "";
        }
	}

    private void ConnectToServer()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP), IPort));

        //创建一个新的线程，用来接收消息
        t = new Thread(ReceiveMessage);
        t.Start();
    }

    void SendMessage(string text)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        clientSocket.Send(data);
    }

    public void OnSendButtonClick()
    {
        SendMessage(textInput.value);
        textInput.value = "";
    }

    void OnDestroy()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }

    private void ReceiveMessage()
    {
        while(true)
        {
            if (clientSocket.Connected==false)
            {
                break;
            }
            int length = clientSocket.Receive(recBuf);
            message = Encoding.UTF8.GetString(recBuf, 0, length);
            //chatLabel.text += "\n" + msg;
        }
    }
}
