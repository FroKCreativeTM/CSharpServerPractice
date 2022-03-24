using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Packet
    {
        // 보통 대부분 패킷 클래스는 
        // 패킷 사이즈와 id를 넣어준다.
        public ushort size;        
        public ushort packetid;    
    }

    // 컨텐츠 단에서는 이렇게 
    // 서버 엔진의 클래스를 오버라이딩한다.
    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            // Packet packet = new Packet() { size = 4, packetid = 10 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //// byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
            //byte[] buffer= BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetid);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); 

            //Send(sendBuff);
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketID : {id}, Size : {size}");
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
