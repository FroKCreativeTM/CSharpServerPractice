using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Warrior
    {
        public int hp;
        public int attack;
        public string name = "";
        public List<int> skills = new List<int>();
    }


    // 컨텐츠 단에서는 이렇게 
    // 서버 엔진의 클래스를 오버라이딩한다.
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            Warrior warrior = new Warrior() { hp = 100, attack = 10 };

            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            // byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
            byte[] buffer= BitConverter.GetBytes(warrior.hp);
            byte[] buffer2 = BitConverter.GetBytes(warrior.attack);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); 

            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"From [Client] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int nBytes)
        {
            Console.WriteLine($"Transffered bytes {nBytes}");
        }
    }

    public class Program
    {
        static Listener _listener = new Listener();

        public static void Main(string[] args)
        {
            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }
}
