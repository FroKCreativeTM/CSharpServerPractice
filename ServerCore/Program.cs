using System;
using System.Threading;

namespace ServerCore
{
    public class Test
    {
        static int num = 0;

        static void A()
        {
            for (int i = 0; i < 100000; i++)
            {
                // num++이 보장된다.
                // Interlocked
                // 성능은 좋지만 정수만 사용할 수 있다는 단점이 존재한다.
                Interlocked.Increment(ref num);
            }
        }

        static void B()
        {
            for (int i = 0; i < 100000; i++)
            {
                // num--이 보장된다.
                Interlocked.Decrement(ref num);
            }
        }

        public static void Main(string[] args)
        {
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