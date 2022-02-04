using System;
using System.Threading;

namespace ServerCore
{
    public class Test
    {
        static void MainThread(object state)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Hello thread!");
            }
        }

        public static void Main(string[] args)
        {

            // 첫번째 인자는 이 일을 할 스레드의 수
            // 두번째 인자는 IO 관련
            // 이 코드는 1~5개의 스레드를 가지고 있다는 뜻이다.
            // 즉 5개가 넘어가면 기다린다.
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; i++)
            {
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
                t.Start();  // 얘도 사실 스레드풀에 넣는 방식이다.
                            // 단 TaskCreationOptions.LongRunning으로 긴 일감임을 알려줄 수 있다.
                            // 이를 통해서 먹통이 되는 것을 방지할 수 있다.
            }

            //for (int i = 0; i < 4; i++)
            //{
            //    ThreadPool.QueueUserWorkItem();
            //}

            // ThreadPool는 기본적으로 static 함수로 되어있다.
            // QueueUserWorkItem는 Object를 인자로 받아줘야 한다.
            // 단 이 물건은 짧은 일감을 던지는 것이 좋다.
            // 왜냐면 긴 일감을 던져준다면 스레드가 돌아오지 않아서 
            // 영영 기다릴 것이기 때문이다.
            ThreadPool.QueueUserWorkItem(MainThread);

            while (true) { }

            //// 기본적으로 C#의 Thread는 Foreground에서 실행된다.

            // 이런 작업은 컨텍스트 스위칭 시간이 더 걸린다.
            // 그래서 애초에 스레드 양이 제한이 되어있는 ThreadPool을 사용하는 것이다.
            // for (int i = 0; i < 1000; i++)
            // {
            //     Thread t = new Thread(MainThread);
            //     t.Name = "test thread";
            //     t.IsBackground = true; // 켜져있다면 메인 스레드가 꺼지면 실행 도중이라도 같이 끝난다.
            //     t.Start();
            // }

            //Console.WriteLine("Waiting for Thread"); // 

            //// C++도 join으로 되어있음
            //t.Join();   // IsBackground가 켜져 있어도 끝날때까지 기다린다.

            // Console.WriteLine("Hello world!");
        }
    }
} 