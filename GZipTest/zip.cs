﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;


namespace GZipTest
{
    public class GZipCompress
    {
        int threadNumber;
        byte[][] dataSource;
        byte[][] dataSourceZip;
        int dataPortionSize;
        bool isStop;

        public GZipCompress()
        {
            threadNumber = Environment.ProcessorCount;
            dataSource = new byte[threadNumber][];
            dataSourceZip = new byte[threadNumber][];
            dataPortionSize = (int)Math.Pow(2, 24); // размер блока для сжатия равен 16 777 216 байт
            isStop = false;
        }

        public int Compress(string FileIn, string FileOut)
        {
            try
            {
                using (FileStream File = new FileStream(FileIn, FileMode.Open))
                using (FileStream FileZip = new FileStream(FileOut, FileMode.Append))
                {
                    int dataSourceBlock;
                    Thread[] tPool;
                    Console.Write("Сжатие...");

                    while (File.Position < File.Length)
                    {
                        tPool = new Thread[threadNumber];//потоки
                        for (int tCount = 0; (tCount < threadNumber) && (File.Position < File.Length); tCount++)
                        {
                            if (File.Length - File.Position <= dataPortionSize)
                            {
                                dataSourceBlock = (int)(File.Length - File.Position);
                            }
                            else
                            {
                                dataSourceBlock = dataPortionSize;
                            }
                            dataSource[tCount] = new byte[dataSourceBlock];
                            File.Read(dataSource[tCount], 0, dataSourceBlock);

                            tPool[tCount] = new Thread(CompressBlock);
                            tPool[tCount].Name = "Tr_" + tCount;
                            tPool[tCount].Start(tCount);

                        }
                        for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);)
                        {
                            //if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                            tPool[portionCount].Join();
                            BitConverter.GetBytes(dataSourceZip[portionCount].Length + 1).CopyTo(dataSourceZip[portionCount], 4);
                            FileZip.Write(dataSourceZip[portionCount], 0, dataSourceZip[portionCount].Length);
                            portionCount++;
                        }
                        if (isStop) break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                isStop = true;
            }
            return isStop ? 1 : 0;
        }

        private void CompressBlock(object i)
        {
            using (MemoryStream output = new MemoryStream(dataSource[(int)i].Length))
            {
                using (GZipStream cs = new GZipStream(output, CompressionMode.Compress))
                {
                    cs.Write(dataSource[(int)i], 0, dataSource[(int)i].Length);
                }
                dataSourceZip[(int)i] = output.ToArray();
            }
        }

        public void Handler(object sender, ConsoleCancelEventArgs args)
        {
            isStop = true;
            args.Cancel = true;
            Console.WriteLine("Архивация прервана пользователем!");
        }
    }
}
