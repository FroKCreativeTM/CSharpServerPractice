using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            for (int i = 0; i < 5; i++)
            {
                // 보낸다.
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World { i + 1}");
                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnReceive(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"From [Server] {recvData}");
        }

        public override void OnSend(int nBytes)
        {
            Console.WriteLine($"Transffered bytes {nBytes}");
        }
    }

    public class Program
    {

        public static void Main(string[] args)
        {
            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new GameSession(); });

            while(true)
            {
                try
                {
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