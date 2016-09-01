using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;


namespace GZipTest
{
    public class GZipCompress : GZip
    {
        public GZipCompress()
        {
            BlockForCompress = (int)Math.Pow(2, 20); // размер блока 2^24 равен 16.777.216 байт, 2^20 равен 1.048.576 
            isStop = false;
        }

        public int Compress(string FileIn, string FileOut)
        {
            try
            {
                using (FileStream File = new FileStream(FileIn, FileMode.Open))
                using (FileStream FileZip = new FileStream(FileOut, FileMode.Append))
                {
                    int FileBlock;
                    Thread[] tPool;
                    Console.Write("Сжатие...");

                    while (File.Position < File.Length)
                    {
                        tPool = new Thread[threadNumber];
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
                            File.Read(dataSource[N], 0, FileBlock);

                            tPool[N] = new Thread(CompressBlock);
                            tPool[N].Name = "Tr_" + N;
                            tPool[N].Start(N);

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
    }
}
