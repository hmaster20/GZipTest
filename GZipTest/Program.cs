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
           //zip("test.pdf", "test.pdf.gz");
            zip("test3.avhdx", "test3.avhdx.gz");


            //if (args.Length == 3)
            //{
            //    switch (args[0])
            //    {
            //        case "compress": zip(args[1], args[2]); break;
            //        case "decompress": Console.WriteLine("decompress"); break;
            //        default: help(); break;
            //    }
            //}
            //else if (args.Length == 0 || args.Length > 3) help(); code = 1;
            Console.WriteLine("code:" + code);
            Console.ReadLine();

            // GC.Collect();
            Environment.Exit(code);
        }

        private static void zip(string file, string filezip)
        {
            GZipCompress zip = new GZipCompress();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(zip.Handler);//срабатывает при нажатии Ctrl+C
            code = zip.Compress(file, filezip);
            zip = null;
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

