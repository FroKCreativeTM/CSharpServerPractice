using System;
using System.Threading;

namespace ServerCore
{
    public class Test
    {
        static volatile int num = 0;

        static void A()
        {
            // atomic = 원자성
            // 이러한 코드는 원자성을 해친다.
            for (int i = 0; i < 100000; i++)
            {
                num++;
            }
        }

        static void B()
        {
            for (int i = 0; i < 100000; i++)
            {
                num--;
            }
        }

        public static void Main(string[] args)
        {
            num++;

            // 어셈블리로 따지면 num++은
            // int temp = num;
            // temp++;
            // num = temp;와 같은 것이다

            Task t1 = new Task(A);
            Task t2 = new Task(B);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(num);
        }
    }
} 