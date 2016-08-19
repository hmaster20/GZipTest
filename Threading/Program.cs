using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Threading;


namespace Threading
{
    class Program
    {
        static void Main()
        {
            Thread t1 = new Thread(WriteY);
            t1.Start();            // Выполнить WriteY в новом потоке
            Thread t2 = new Thread(WriteZ);
            t2.Start();            // Выполнить WriteZ в новом потоке
            Thread t3 = new Thread(WriteW);
            t3.Start();            // Выполнить WriteWв новом потоке

            // while (true)
            //     Console.Write("x"); // Все время печатать 'x'
        }

        static void WriteY()
        {
            while (true)
            {
                Console.Write("y"); // Все время печатать 'y'
                Thread.Sleep(500);
            }
        }

        static void WriteZ()
        {
            while (true)
            {
                Console.Write("z"); // Все время печатать 'z'
                Thread.Sleep(500);
            }

        }

        static void WriteW()
        {
            while (true)
            {
                Console.Write("w"); // Все время печатать 'y'
                Thread.Sleep(500);
            }
        }

    }
}
