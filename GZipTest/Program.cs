using System;
using System.Collections.Generic;
using System.Linq;



namespace GZipTest
{
    class Program
    {
        static int code = 0;
        public static void Main(string[] args)
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




            if (args.Length == 3)
            {
                switch (args[0])
                {
                    case "compress": zip(args[1], args[2]); break;
                    case "decompress": Console.WriteLine("decompress"); break;
                    default: help(); break;
                }
            }
            else if (args.Length == 0 || args.Length > 3) help(); code = 1;








           // GC.Collect();
            Environment.Exit(code);
        }

        private static void zip(string file, string filezip)
        {
            GZipCompress zip = new GZipCompress();
            zip.Compress(file, filezip);
            zip = null;
            Console.WriteLine(" выполнено!");
         

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

        private static void help()
        {
            Console.WriteLine("Справка по использованию програмы архиватора.\n");
            Console.WriteLine("gziptest <команда> [файл 1] [файл 2]\n");
            Console.WriteLine("<команда>");
            Console.WriteLine("compress\t - выполняет сжатие [исходного файла]");
            Console.WriteLine("decompress\t - выполняет распаковку [архивного файла]");
            Console.WriteLine("\nАрхивация:\tgziptest compress document.doc document.doc.gz\t (создание архива document.doc.gz)");
            Console.WriteLine("Распаковка:\tgziptest decompress document.gz document.doc\t (распаковка архива document.doc.gz)");
        }
    }
}

