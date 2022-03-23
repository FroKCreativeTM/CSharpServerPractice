using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class Session
    {
        Socket _socket;
        int _disconnected = 0;

        object _lock = new object();    
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        bool _pending = false;
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // 버퍼를 0번부터 1024부터 가지는 byte 버퍼를 생성해서 이를 이용해서 세팅한다.
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            // 서버 부하를 막기 위해서 먼저 등록해둔다.
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                // 내가 첫번째로 send를 호출해서 전공 가능
                if (_pendingList.Count == 0)
                    RegisterSend();
            }

            // _socket.Send(sendBuff);
            // 버퍼를 0번부터 1024부터 가지는 byte 버퍼를 생성해서 이를 이용해서 세팅한다.
            // 문제는 이렇게 생성하면 버퍼에 대한 args를 공유하기 때문에 멀티 스레드 환경에서 문제가 발생
            // _sendArgs.SetBuffer(sendBuff, 0, sendBuff.Length);
            // RegisterSend(_sendArgs);
        }

        public void Disconnect()
        {
            // 1이되면 끊겼다.
            if(Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신
        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);

            // 지금 바로 받을 수 있다
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        // 얘는 언제할 지 모름
        void RegisterSend()
        {
            // 다른 스레드가 껴들면 기다리게 
            _pending = true;
            // byte[] buff = _sendQueue.Dequeue();
            // _sendArgs.SetBuffer(buff, 0, buff.Length);


            _pendingList.Clear();
            // 버퍼 리스트를 한 번에 보내준다.
            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                // _sendArgs.BufferList에 바로 넣으면 안 된다!
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);
            // 지금 바로 보낼 수 있다.
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 컬백 방식으로 이 함수가 실행될 수 있으니
            // 이를 락으로 다른 스레드가 간섭하지 못하게 만든다.
            lock(_lock)
            {
                // 몇 바이트를 보냈고 그리고 소켓 에러가 없는 경우
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        Console.WriteLine($"Transffered bytes {_sendArgs.BytesTransferred}");

                        // 만약 보내는 동안 또 큐에 내용이 들어왔다면
                        if(_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                        else
                        {
                            // 여기서는 RegisterSend는 재사용할 수 없다.
                            _pending = false;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
                else
                {
                    // TODO Disconnect
                    Disconnect();
                }
            }
        }

            private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 몇 바이트를 받았고 그리고 소켓 에러가 없는 경우
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // TODO
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"From client {recvData}");

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            else
            {
                // TODO Disconnect
                Disconnect();
            }
        }
        #endregion
    }
}
