using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class Listener
    {
        Socket _listnerSocket;
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listnerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;

            _listnerSocket.Bind(endPoint);

            // 최대 대기수를 1000명
            _listnerSocket.Listen(1000);

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
                // 유저가 오면?
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args);
        }

        // public Socket Accept()
        // {
        //     // 블로킹 방식 함수
        //     return _listnerSocket.Accept();
        // }
    }
}
