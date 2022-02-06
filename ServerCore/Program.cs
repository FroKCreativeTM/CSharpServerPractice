using System;
using System.Threading;

namespace ServerCore
{
    public class Test
    {
        int _answer;
        bool _complete;

        void A()
        {
            _answer = 123;
            Thread.MemoryBarrier();
            _complete = true;
            Thread.MemoryBarrier();
        }

        void B()
        {
            Thread.MemoryBarrier(); // 얘는 store -> store -> read이기 때문에
                                    // 명시를 해주는 것이다.(가시성을 위해서)
            if(_complete)
            {
                Thread.MemoryBarrier();
                Console.WriteLine(_answer);
            }
        }

        public static void Main(string[] args)
        {

        }
    }
} 