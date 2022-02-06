using System;
using System.Threading;

namespace ServerCore
{
    public class Test
    {
        public static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        arr[y, x] = 1;
                    }
                }
                long end = DateTime.Now.Ticks;

                Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
            }

            // 공간적 지역성을 고려하지 않은 코드이다.
            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        arr[x, y] = 1;
                    }
                }
                long end = DateTime.Now.Ticks;

                Console.WriteLine($"(x, y) 순서 걸린 시간 {end - now}");
            }
        }
    }
} 