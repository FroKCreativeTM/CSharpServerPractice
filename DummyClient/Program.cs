using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyClient
{
    public class Test
    {
        public static void Main(string[] args)
        {
            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            
            while(true)
            {
                // 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 연결한다.
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

                    // 보낸다.
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World");
                    int sendBytes = socket.Send(sendBuff);

                    // 받는다.
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvStr = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"From Server {recvStr}");

                    // 나간다 
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }

                Thread.Sleep(100);
            }
        }
    }
}