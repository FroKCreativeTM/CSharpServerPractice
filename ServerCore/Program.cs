using System;
using System.Threading;

namespace ServerCore
{
    public class Test
    {
        // volatile : 휘발성 데이터
        // 언제 바뀔지 모르니 어셈블리 단계에서 최적화를 하지 않는다.
        // 캐시를 무시하고 최신 값을 가져와라.
        // 보통 lock을 사용한다.
        volatile static bool _stop = false;

        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작!"); ;

            while(_stop == false)
            {
                // release에선 코드 최적화 때문에 일어나면 안 되는 일이 일어나곤 한다.
            }
            Console.WriteLine("쓰레드 종료");
        }


        public static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");

            t.Wait();

            Console.WriteLine("종료 성공");
        }
    }
} 