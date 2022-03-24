using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _listnerSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listnerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            _listnerSocket.Bind(endPoint);

            _listnerSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptComleted); // 델리게이트
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 주의!!!!!!!!!!!!
            // OnAcceptComleted가 끝나면 AcceptSocket가 null이 되야한다.
            // null이 아니라면 args가 dirty하기 때문
            args.AcceptSocket = null;

            bool pending = _listnerSocket.AcceptAsync(args);

            // 클라이언트 요청이 와서 바로 처리할 수 있는 경우
            if (pending == false)
                OnAcceptComleted(null, args);

        }

        // MULTY-THREAD!!! RED ZONE
        void OnAcceptComleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                // 받고 
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args);
        }
    }
}
