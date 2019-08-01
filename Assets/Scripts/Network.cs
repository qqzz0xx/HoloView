using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class Network : MonoBehaviour
{
    public static Network Inst;

    IPAddress address;
    int port;

    private Socket socket;
    private bool connected;

    private ManualResetEvent timeoutEvent = new ManualResetEvent(false);
    private int timeoutMSec = 8000;    //connect timeout count in millisecond

    private SocketAsyncEventArgs sendArgs;
    private SocketAsyncEventArgs recvArgs;

    private Transports transports;

    private float heartbeatTime = 5; //second
    private float totalDeltaTime = 0;

    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private void Awake()
    {
        Inst = this;
        Init();
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void Init()
    {
        address = GetLocalIPAddress();
        port = 5555;
        transports = new Transports();
        transports.onDispatch = Receive;

        sendArgs = new SocketAsyncEventArgs();
        recvArgs = new SocketAsyncEventArgs();

    }
    public void Connect()
    {
        var tmpSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        var ipend = new IPEndPoint(address, port);

        timeoutEvent.Reset();

        try
        {
            tmpSocket.BeginConnect(ipend, p =>
            {
                connected = true;
                socket = tmpSocket;

            }
            , tmpSocket);
        }
        finally
        {
            timeoutEvent.Set();
        }

        if (timeoutEvent.WaitOne(timeoutMSec, false))
        {
            Debug.LogError("Connect: timeout" );
        }

    }

    void Update()
    {
        if (connected)
        {
            if (socket.ReceiveAsync(recvArgs))
            {
                transports.ProBytes(recvArgs.Buffer, 0, recvArgs.Buffer.Length);
            }
        }

        totalDeltaTime += Time.deltaTime;
        if (totalDeltaTime > heartbeatTime)
        {
            totalDeltaTime -= heartbeatTime;

            SendHeartBeat();
        }
    }

    void Receive(int msgid, byte[] msgdata)
    {
        Debug.Log("Recv: " + msgid);
    }

    void Send(int msgid, byte[] msgdata)
    {
        var buffer = transports.PackBytes(msgid, msgdata);
        sendArgs.SetBuffer(buffer, 0, buffer.Length);
        socket.SendAsync(sendArgs);
    }

    void SendHeartBeat()
    {
        Send(-1, null);
    }

}
