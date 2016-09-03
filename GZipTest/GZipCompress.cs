using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    public class GZipCompress : GZip
    {
        public int Compress(string FileIn, string FileOut)
        {
            try
            {
                using (FileStream File = new FileStream(FileIn, FileMode.Open))
                using (FileStream FileZip = new FileStream(FileOut, FileMode.Append))
                {
                    Thread[] tPool;
                    Console.Write("Сжатие...");
                    while (File.Position < File.Length)
                    {
                        tPool = new Thread[threadNumber];
                        ReadToThread(File, tPool);
                        CreateZipFile(FileZip, tPool);
                        if (IsStop) break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                IsStop = true;
            }
            return IsStop ? 1 : 0;
        }


        // чтение блоков файла
        private void ReadToThread(FileStream File, Thread[] tPool)
        {
            int FileBlock;
            for (int N = 0; (N < threadNumber) && (File.Position < File.Length); N++)
            {
                if (File.Length - File.Position >= BlockForCompress)
                {
                    FileBlock = BlockForCompress;
                }
                else
                {
                    FileBlock = (int)(File.Length - File.Position);
                }

                dataSource[N] = new byte[FileBlock];
                File.Read(dataSource[N], 0, FileBlock); // читаем блока файла длинной FileBlock и пишем в буфер dataSource[N]

                tPool[N] = new Thread(CompressBlock);
                tPool[N].Name = "Tr_" + N;
                tPool[N].Start(N);
            }
        }

        // сжатие блоков
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


        // запись сжатых блоков
        private void CreateZipFile(FileStream FileZip, Thread[] tPool)
        {
            for (int N = 0; (N < threadNumber) && (tPool[N] != null);)
            {
                //if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                tPool[N].Join();   // ожидание потока и работа с блоком
                //BitConverter.GetBytes(dataSource[N].Length + 1)
                //            .CopyTo(dataSourceZip[N], 4);
                BitConverter.GetBytes(dataSourceZip[N].Length + 1)//получаем размер блока в байтах
                            .CopyTo(dataSourceZip[N], 4);//запись информации о размере
                FileZip.Write(dataSourceZip[N], 0, dataSourceZip[N].Length);
                N++;
            }
        }
    }
}
