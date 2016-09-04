using System;

namespace GZipTest
{
    class Program
    {
        static int code;// = 0;
        public static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                switch (args[0])
                {
                    case "compress": code = zip(args[1], args[2]); break;
                    case "decompress": code = unzip(args[1], args[2]); break;
                    default: help(); break;
                }
            }
            else if (args.Length == 0 || args.Length > 3)
            {
                help(); code = 1;
            }
            Console.WriteLine("Result code: " + code);
            // GC.Collect();
            Environment.Exit(code);
        }

        private static int zip(string FileIn, string FileOut)
        {
            GZipCompress zip = new GZipCompress();
            return zip.Compress(FileIn, FileOut);
        }

        private static int unzip(string FileIn, string FileOut)
        {
            GZipUnCompress unzip = new GZipUnCompress();
            return unzip.Decompress(FileIn, FileOut);
        }

        private static void help()
        {
            Console.WriteLine("Справка по использованию программы архиватора.\n");
            Console.WriteLine("GZipTest <команда> [файл 1] [файл 2]\n");
            Console.WriteLine("<команда>");
            Console.WriteLine("compress\t - выполняет сжатие [исходного файла]");
            Console.WriteLine("decompress\t - выполняет распаковку [архивного файла]");
            Console.WriteLine("\nАрхивация:\tGZipTest compress document.doc document.doc.gz\t (создание архива document.doc.gz)");
            Console.WriteLine("Распаковка:\tGZipTest decompress document.gz document.doc\t (распаковка архива document.doc.gz)");
        }
    }
}

