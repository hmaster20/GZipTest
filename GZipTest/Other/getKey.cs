using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest
{
    class getKey
    {
        public static void Mainen()
        {
            ConsoleKeyInfo cki;
            Console.Clear();
            //Создание обработчика событий для обработки события нажатия клавиш.
            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);//срабатывает при нажатии Ctrl+C
            while (true)
            {
                Console.Write("Press any key, or 'X' to quit, or ");
                Console.WriteLine("CTRL+C to interrupt the read operation:");

                // Start a console read operation. Do not display the input.
                // Запустите консоль операции чтения. Не показывать вход.
                cki = Console.ReadKey(true);

                // Announce the name of the key that was pressed .
                // Огласите имя ключа, которая была нажата.
                Console.WriteLine("  Key pressed: {0}\n", cki.Key);

                // Exit if the user pressed the 'X' key.
                // Выход, если пользователь нажал клавишу 'X'.
                if (cki.Key == ConsoleKey.X) break;
            }
        }


        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("\nThe read operation has been interrupted.");
            Console.WriteLine("  Key pressed: {0}", args.SpecialKey);
            Console.WriteLine("  Cancel property: {0}", args.Cancel);

            // Set the Cancel property to true to prevent the process from terminating.
            Console.WriteLine("Setting the Cancel property to true...");
            args.Cancel = true;

            // Announce the new value of the Cancel property.
            Console.WriteLine("  Cancel property: {0}", args.Cancel);
            Console.WriteLine("The read operation will resume...\n");
        }
    }



}
