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
            //zip("test.txt", "test_v3.gz");
            //zip("test2.doc", "test2_v2.doc.gz");
            //zip("test6.pdf", "test6.pdf.gz");
            //zip("test222__.pdf", "test222___v2.pdf.gz");
            //zip("test5.pdf", "test5_v2.pdf.gz");

            //zip("test222.pdf", "test222.pdf.gz");
            //zip("test3.avhdx", "test3.avhdx.gz");
            //code =  zip("zip_unzip.pdf", "zip_unzip.pdf.gz");
            //unzip("test66.pdf.gz");
            //unzip("test222___v2.pdf.gz");
            unzip("test5_v2.pdf.gz");

            //unzip("test_1.txt.gz");

            //MegaZipUnzip.CompressFile("test3.pdf", "test3_mega.pdf.gz");
            //MegaZipUnzip.DecompressFile("test3_mega.pdf.gz", "test3_mega.pdf");

            //unzip("zip_unzip.pdf.gz");

            //
            //gzip1_2.Decompress("test3_v3.pdf.gz");
            //GZipUnCompress.DeCompressFileS("test_1111.txt.gz", "test_1111.txt");
            //GZipUnCompress.DeCompressFileS("test222.pdf.gz", "test222.pdf");
            //GZipUnCompress.DecompressMod22("test222.pdf.gz");
            //GZipUnCompress.DecompressMod22("test_1_v2.txt.gz");
            //GZipUnCompress.DecompressMod22("test2_v2.doc.gz");
            //GZipUnCompress.DecompressMod22("test3_v2.pdf.gz");
            //unzip("test3_v2.pdf.gz");



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
            GZipUnCompress unzip = new GZipUnCompress();
            unzip.Decompress(file);
            //// code = unzip.Decompress(file);
            ////unzip = null;



            //GZipUnCompress.DecompressMod22(file);
            
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

