using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    // 왜 서버에 커넥터?
    // 1. 서버도 리슨이 메인이긴 하지만, 
    // Connect나 Recv, Send는 공용으로 사용하면 좋다.
    // 2. MMO는 분산 처리 서버를 제작해서
    // 하나는 몬스터, 하나는 아이템 등등으로 분할해서 제작하는 경우가 있다.
    // 그 서버 간 통신에서 결국 서버끼리 연결하기 위해서는 필요하다.
    public class Connector
    {
        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;

            // 경우에 따라서 여러 대의 서버 소켓을 받아야될 경우가 있어서 토큰을 이용한다.
            args.UserToken = socket;

            RegisterConnect(args);
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;

            if (socket == null)
                return;

            bool pending = socket.ConnectAsync(args);

            if (pending == false)
                OnConnectCompleted(null, args);
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                // 어떤 세션을 받을지 모르니 팩토리를 이용한다.
                // 컨텐츠가 요구한 세션대로 생성한다.
                Session session = _sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted failed : {args.SocketError}");
            }
        }
    }
}
