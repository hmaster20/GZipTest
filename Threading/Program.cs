using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace Threading
{
    class Program
    {
       

        static void Main()
        {
            DisplayDADStats();

            Thread t1 = new Thread(WriteY);
            t1.Start();            // Выполнить WriteY в новом потоке
            Thread t2 = new Thread(WriteZ);
            t2.Start();            // Выполнить WriteZ в новом потоке
            Thread t3 = new Thread(WriteW);
            t3.Start();            // Выполнить WriteWв новом потоке

            // while (true)
            //     Console.Write("x"); // Все время печатать 'x'

     
        }



        private static void DisplayDADStats()
        {
            // Получить доступ к домену приложения для текущего потока.
            AppDomain defaultAD = AppDomain.CurrentDomain;
            // Вывести различные статистические данные об этом домене.
            Console.WriteLine("Name of this domain: {0}",
            defaultAD.FriendlyName); // Дружественное имя
            Console.WriteLine("ID of domain in this process: {0}",
            defaultAD.Id); //Идентификатор
            Console.WriteLine("Is this the default domain?: {0}",
            defaultAD.IsDefaultAppDomain()); // Является ли стандартным
            Console.WriteLine("Base directory of this domain: {0}",
            defaultAD.BaseDirectory); // Базовый каталог
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
