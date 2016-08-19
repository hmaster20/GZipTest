using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// ReaderWriterLock 

namespace GZipTest 
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "compress": Console.WriteLine("compress"); break;
                    case "decompress": Console.WriteLine("decompress"); break;
                    default: break;
                }
            }
        }


        static void Reader(string SourceFile)
        {
            try
            {
                using (FileStream inFile = new FileStream(SourceFile, FileMode.Open))
                {
                    var SourceBytes = new byte[inFile.Length];
                    inFile.Read(SourceBytes, 0, (int)inFile.Length);
                    Console.WriteLine("Размер файла: " + inFile.Length);
                    inFile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        static void Writer()
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }


        // создание потока на чтение файла по байтам, блоками ? 
        // сохраннеие блоков в памяти
        // применение многопоточного сжатия для каждого блока
        // прогресс сжатия (сначала примитивно в процентах)
        // создание потока на запись нового файла *.gz


        //




    }







}
