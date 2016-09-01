using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZipTest
{

    public class GZipUnCompress : GZip
    {
        public void Decompress(string FileZip)
        {
            try
            {
                using (FileStream Zip = new FileStream(FileZip, FileMode.Open))
                using (FileStream outFile = new FileStream(FileZip.Remove(FileZip.Length - 3), FileMode.Append))// убирает расширение .gz и открывает на запись
                {
                    //int dataPortionSize;
                    int dataPortionSize = (int)Math.Pow(2, 20);
                    int compressedBlockLength;
                    Thread[] tPool;
                    Console.Write("Decompressing...");
                    byte[] buffer = new byte[8];


                    while (Zip.Position < Zip.Length)
                    {
                        tPool = new Thread[threadNumber];
                        for (int portionCount = 0; (portionCount < threadNumber) && (Zip.Position < Zip.Length); portionCount++)
                        {
                            Zip.Read(buffer, 0, 8);//Чтение 8 байт в буфер
                            compressedBlockLength = BitConverter.ToInt32(buffer, 4);
                            dataSourceZip[portionCount] = new byte[compressedBlockLength + 1];
                            buffer.CopyTo(dataSourceZip[portionCount], 0);

                            Zip.Read(dataSourceZip[portionCount], 8, compressedBlockLength - 8);
                            dataPortionSize = BitConverter.ToInt32(dataSourceZip[portionCount], compressedBlockLength - 4);
                            dataSource[portionCount] = new byte[dataPortionSize];

                            tPool[portionCount] = new Thread(DecompressBlock);
                            tPool[portionCount].Name = "Tr_" + portionCount;
                            tPool[portionCount].Start(portionCount);
                        }
                        for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);) // выяснить работу последнего блока
                        {
                            //if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                            tPool[portionCount].Join();
                            outFile.Write(dataSource[portionCount], 0, dataSource[portionCount].Length);
                            portionCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }
        }

        public void DecompressBlock(object i)
        {
            using (MemoryStream input = new MemoryStream(dataSourceZip[(int)i]))
            using (GZipStream ds = new GZipStream(input, CompressionMode.Decompress))
            {
                try
                {
                    ds.Read(dataSource[(int)i], 0, dataSource[(int)i].Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        // #############################################################################################################










































































        static int threadNumberS = Environment.ProcessorCount;
        static byte[][] dataArrayS = new byte[threadNumberS][];
        static byte[][] dataArraySZip = new byte[threadNumberS][];
        static int dataPortionSize = (int)Math.Pow(2, 20);
        //int dataSourceBlock;
        static int compressedBlockLength;

        public static void DecompressMod22(string inFileName)
        {
            try
            {
                FileStream ZipFile = new FileStream(inFileName, FileMode.Open);//Zip открывается
                FileStream File = new FileStream(inFileName.Remove(inFileName.Length - 3), FileMode.Append);
                Thread[] tPool;
                //Console.Write("Decompressing...");
                byte[] buffer = new byte[1024 * 1024];// 1048576 = 2^20

                while (ZipFile.Position < ZipFile.Length)
                {
                    tPool = new Thread[threadNumberS];
                    for (int portionCount = 0; (portionCount < threadNumberS) && (ZipFile.Position < ZipFile.Length); portionCount++)
                    {
                        if (ZipFile.Length - ZipFile.Position <= dataPortionSize)
                            compressedBlockLength = (int)(ZipFile.Length - ZipFile.Position);
                        else
                            compressedBlockLength = dataPortionSize;

                        dataArraySZip[portionCount] = new byte[compressedBlockLength];
                        ZipFile.Read(dataArraySZip[portionCount], 0, compressedBlockLength);

                        tPool[portionCount] = new Thread(DecompressBlockMod2);
                        tPool[portionCount].Name = "Tr_" + portionCount;
                        tPool[portionCount].Start(portionCount);


                        //========================================================================================================
                        //            read[j] = inFile.Read(buffer, 0, BufferSize);
                        //            inGZip.Write(buffer, 0, read[j]);
                        using (GZipStream strmUncompress = new GZipStream(ZipFile, CompressionMode.Decompress))
                        {
                            int numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                            while (numRead != 0)
                            {
                                File.Write(buffer, 0, numRead);
                                File.Flush();
                                numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                            }
                            strmUncompress.Close();     // завершение работы GZipStream strmUncompress 
                        }

                        //inFile.Read(buffer, 0, 8);
                        //compressedBlockLength = BitConverter.ToInt32(buffer, 4);
                        //compressedDataArrayS[portionCount] = new byte[compressedBlockLength + 1];
                        //buffer.CopyTo(compressedDataArrayS[portionCount], 0);

                        //inFile.Read(compressedDataArrayS[portionCount], 8, compressedBlockLength - 8);
                        //_dataPortionSize = BitConverter.ToInt32(compressedDataArrayS[portionCount], compressedBlockLength - 4);
                        //dataArrayS[portionCount] = new byte[_dataPortionSize];

                    }

                    for (int N = 0; (N < threadNumberS) && (tPool[N] != null);) // выяснить работу последнего блока
                    {
                        tPool[N].Join();
                        File.Write(dataArrayS[N], 0, dataArrayS[N].Length);
                        File.Flush();
                        N++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }
        }

        public static void DecompressBlockMod2(object i)
        {
            using (MemoryStream input = new MemoryStream(dataArraySZip[(int)i].Length))
            {
                using (GZipStream ds = new GZipStream(input, CompressionMode.Decompress))
                {
                    ds.Read(dataArraySZip[(int)i], 0, dataArraySZip[(int)i].Length);
                }
            }
        }



        private void CompressBlock(object i)
        {
            using (MemoryStream output = new MemoryStream(dataSource[(int)i].Length))
            {
                using (GZipStream cs = new GZipStream(output, CompressionMode.Compress))
                {
                    cs.Write(dataSource[(int)i], 0, dataSource[(int)i].Length); // данные записываются в output
                }
                dataSourceZip[(int)i] = output.ToArray();   //output переводим в массив и передаем в dataSourceZip
            }
        }








        //// //msdn.microsoft.com/ru-ru/library/xxwd7aah(v=vs.110).aspx
































        // #############################################################################################################




        public static void DeCompressFileS(string CompressedFile, string DeCompressedFile)
        {
            byte[] buffer = new byte[1024 * 1024];// 1048576 = 2^20
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
