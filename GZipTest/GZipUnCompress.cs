using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    public class GZipUnCompress : GZip
    {
        public int Decompress(string FileZip, string FileOut)
        {
            if (FileExist(FileOut)) return 1;
            try
            {
                using (FileStream Zip = new FileStream(FileZip, FileMode.Open))
                using (FileStream outFile = new FileStream(FileOut, FileMode.Append))
                {
                    Thread[] tPool;
                    Console.WriteLine("Распаковка...");
                    while (Zip.Position < Zip.Length)
                    {
                        tPool = new Thread[threadNumber];
                        ReadInThread(Zip, tPool);
                        CreateUnzipFile(outFile, tPool);
                        if (IsStop) break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                IsStop = true;
            }
            return CheckResult(FileOut);
        }



        //чтение блоков файла
        private void ReadInThread(FileStream Zip, Thread[] tPool)
        {
            for (int N = 0; (N < threadNumber) && (Zip.Position < Zip.Length); N++)
            {
                byte[] buffer = new byte[8];//массив для хранения информации о размере и CRC

                Zip.Read(buffer, 0, 8);//Чтение 8 байт в буфер (4 байта CRC32, 4 байта ISIZE)
                int ZipBlockLength = BitConverter.ToInt32(buffer, 4);//вычисляем размер блока   

                dataSourceZip[N] = new byte[ZipBlockLength - 1];            //создание массива на основе полученного размера
                buffer.CopyTo(dataSourceZip[N], 0);                         //копирование 8 байт в массив
                Zip.Read(dataSourceZip[N], 8, dataSourceZip[N].Length - 8); //чтение потока размером в длину блока, исключая 8 прочитанных байт

                //вычисляем размер распакованного блока 
                int dataPortionSize = BitConverter.ToInt32(dataSourceZip[N], dataSourceZip[N].Length - 4);
                //создаем массив для распакованного блока
                dataSource[N] = new byte[dataPortionSize];

                tPool[N] = new Thread(DecompressBlock);
                tPool[N].Name = "Tr_" + N;
                tPool[N].Start(N);
            }
        }


        //распаковка
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


        //запись распакованных данных
        private void CreateUnzipFile(FileStream outFile, Thread[] tPool)
        {
            for (int N = 0; (N < threadNumber) && (tPool[N] != null);)
            {
                tPool[N].Join();//ожидание остановки потока
                outFile.Write(dataSource[N], 0, dataSource[N].Length);
                N++;
            }
        }
    }
}
