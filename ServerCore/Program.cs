using System;
using System.Threading;

namespace ServerCore
{
    public class Test
    {
        static int num = 0;
        static object _obj = new object();

        static void A()
        {
            for (int i = 0; i < 100000; i++)
            {
                try
                {
                    Monitor.Enter(_obj);

                    // 코드 블럭
                    num++;
                }
                finally
                {
                    Monitor.Exit(_obj);
                }

            }
        }

        static void B()
        {
            for (int i = 0; i < 100000; i++)
            {
                Monitor.Enter(_obj);

                // 코드 블럭
                num--;

                Monitor.Exit(_obj);
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