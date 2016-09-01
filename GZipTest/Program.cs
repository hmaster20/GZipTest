using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace GZipTest
{
    class Program
    {
        static int code;// = 0;
        public static void Main(string[] args)
        {
            //zip("test.txt", "test.txt.gz");
            //zip("test.pdf", "test.pdf.gz");
            //zip("test3.avhdx", "test3.avhdx.gz");
            //code =  zip("zip_unzip.pdf", "zip_unzip.pdf.gz");
            //unzip("test22.pdf.gz");

            unzip("test222.pdf.gz");

            //unzip("test22.doc.gz");


            //GZipUnCompress.DeCompressFile("zip_unzip.pdf.gz", "zip_unzip_u.pdf");


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





            Console.WriteLine("Result code: " + code);
            Console.ReadLine();

            // GC.Collect();
            Environment.Exit(code);
        }

        private static int zip(string file, string filezip)
        {
            GZipCompress zip = new GZipCompress();
             return zip.Compress(file, filezip);
            //zip = null;
        }

        private static void unzip(string file)
        {
            //GZipUnCompress unzip = new GZipUnCompress();
            //unzip.Decompress("");

            //GZipUnCompress.DecompressMod(file);

            // code = unzip.Decompress(file);
            // unzip.Decompress(file);
            //unzip.de.Decom(file);

            //unzip = null;
            GZipUnCompress.DecompressMod22(file);
        }

        private static void help()
        {
            Console.WriteLine("Справка по использованию программы архиватора.\n");
            Console.WriteLine("gziptest <команда> [файл 1] [файл 2]\n");
            Console.WriteLine("<команда>");
            Console.WriteLine("compress\t - выполняет сжатие [исходного файла]");
            Console.WriteLine("decompress\t - выполняет распаковку [архивного файла]");
            Console.WriteLine("\nАрхивация:\tgziptest compress document.doc document.doc.gz\t (создание архива document.doc.gz)");
            Console.WriteLine("Распаковка:\tgziptest decompress document.gz document.doc\t (распаковка архива document.doc.gz)");
        }
    }
}

