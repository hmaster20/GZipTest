using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GZipTest.Other
{
    class deCompress
    {

        // #############################################################################################################



        //static int threadNumberS = Environment.ProcessorCount;
        //static byte[][] dataArrayS = new byte[threadNumberS][];
        //static byte[][] dataArraySZip = new byte[threadNumberS][];
        //static int defaultPartition = (int)Math.Pow(2, 20);
        ////int dataSourceBlock;
        //static int ZipBlock;

        //public static void DecompressMod22(string inFileName)
        //{
        //    //try
        //    //{
        //    FileStream ZipFile = new FileStream(inFileName, FileMode.Open);//Zip открывается
        //    FileStream File = new FileStream(inFileName.Remove(inFileName.Length - 3), FileMode.Append);
        //    Thread[] tPool;
        //    //Console.Write("Decompressing...");
        //    //byte[] buffer = new byte[1024 * 1024];// 1048576 = 2^20

        //    while (ZipFile.Position < ZipFile.Length)
        //    {
        //        tPool = new Thread[threadNumberS];
        //        for (int N = 0; (N < threadNumberS) && (ZipFile.Position < ZipFile.Length); N++)
        //        {
        //            if (ZipFile.Length - ZipFile.Position <= defaultPartition)
        //                ZipBlock = (int)(ZipFile.Length - ZipFile.Position);
        //            else
        //                ZipBlock = defaultPartition;


        //            dataArraySZip[N] = new byte[ZipBlock];
        //            ZipFile.Read(dataArraySZip[N], 0, ZipBlock);// размер буфера ZipBlock (1048576)
        //            //dataArrayS[N] = new byte[ZipBlock];

        //            tPool[N] = new Thread(DecompressBlockMod2);
        //            tPool[N].Name = "Tr_" + N;
        //            tPool[N].Start(N);




        //            //inFile.Read(buffer, 0, 8);
        //            //compressedBlockLength = BitConverter.ToInt32(buffer, 4);
        //            //compressedDataArrayS[portionCount] = new byte[compressedBlockLength + 1];
        //            //buffer.CopyTo(compressedDataArrayS[portionCount], 0);

        //            //inFile.Read(compressedDataArrayS[portionCount], 8, compressedBlockLength - 8);
        //            //_dataPortionSize = BitConverter.ToInt32(compressedDataArrayS[portionCount], compressedBlockLength - 4);
        //            //dataArrayS[portionCount] = new byte[_dataPortionSize];

        //        }

        //        for (int N = 0; (N < threadNumberS) && (tPool[N] != null);) // выяснить работу последнего блока
        //        {
        //            tPool[N].Join();
        //            File.Write(dataArrayS[N], 0, dataArrayS[N].Length);
        //            File.Flush();
        //            N++;
        //        }
        //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine("ERROR:" + ex.Message);
        //    //}
        //}

        //public static void DecompressBlockMod2(object i)
        //{
        //    //byte[] buffer = new byte[1024 * 1024];
        //    using (MemoryStream input = new MemoryStream(dataArraySZip[(int)i]))
        //    {
        //        using (GZipStream ds = new GZipStream(input, CompressionMode.Decompress))
        //        {
        //            //int read = ds.Read(buffer, 0, buffer.Length); // данные считываются из потока   
        //            //if (read > 0)
        //            //{
        //            //    dataArrayS[(int)i] = new byte[read];
        //            //    Array.Copy(buffer, dataArrayS[(int)i], read);
        //            //}
        //            ds.Read(dataArrayS[(int)i], 0, dataArrayS[(int)i].Length);
        //        }
        //    }





        // #############################################################################################################      


        public static void DeCompressFile(string CompressedFile, string DeCompressedFile)
        {
            byte[] buffer = new byte[1024 * 1024];// 1048576 // 2^20
            using (FileStream fstrmCompressedFile = File.OpenRead(CompressedFile)) // прочитали архив
            {
                using (FileStream fstrmDecompressedFile = File.Create(DeCompressedFile)) // создали файл
                {
                    using (GZipStream strmUncompress = new GZipStream(fstrmCompressedFile, CompressionMode.Decompress))
                    {
                        int numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                        while (numRead != 0)
                        {
                            fstrmDecompressedFile.Write(buffer, 0, numRead);
                            fstrmDecompressedFile.Flush();
                            numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                        }
                        strmUncompress.Close();     // завершение работы GZipStream strmUncompress 
                    }
                    fstrmDecompressedFile.Flush();
                    fstrmDecompressedFile.Close();  // завершение работы FileStream fstrmDecompressedFile 
                }
                fstrmCompressedFile.Close();        // End Using System.IO.FileStream fstrmCompressedFile 
            }
        }




        public static byte[] aaaaaDecompress(byte[] gzip)
        {
            byte[] baRetVal = null;
            using (MemoryStream ByteStream = new MemoryStream(gzip))
            {
                // Создание потока GZIP с режимом декомпрессии. 
                // Затем создать буфер и записи в то время как чтение из потока GZIP.
                using (GZipStream stream = new GZipStream(ByteStream, CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        count = stream.Read(buffer, 0, size);
                        while (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                            memory.Flush();
                            count = stream.Read(buffer, 0, size);
                        }
                        baRetVal = memory.ToArray();
                        memory.Close();
                    }
                    stream.Close();     // End Using System.IO.Compression.GZipStream stream 
                }
                ByteStream.Close();     // End Using System.IO.MemoryStream ByteStream
            }
            return baRetVal;
        }


    }
}
