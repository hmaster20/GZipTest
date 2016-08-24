using System;
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
        int threadNumber = Environment.ProcessorCount;
        byte[][] dataSource;
        byte[][] dataSourceZip;
        int dataPortionSize = (int)Math.Pow(2, 24); // размер блока для сжатия //16 777 216
        bool isStop;

        public GZipCompress()
        {
            dataSource = new byte[threadNumber][];
            dataSourceZip = new byte[threadNumber][];
            isStop = false;
        }

        public int Compress(string FileIn, string FileOut)
        {
            try
            {
                using (FileStream inFile = new FileStream(FileIn, FileMode.Open))
                using (FileStream outFile = new FileStream(FileOut, FileMode.Append))
                {
                    int _dataPortionSize;
                    Thread[] tPool;
                    Console.Write("Сжатие...");

                    while (inFile.Position < inFile.Length)
                    {
                        tPool = new Thread[threadNumber];//потоки
                        for (int pCount = 0; (pCount < threadNumber) && (inFile.Position < inFile.Length); pCount++)
                        {
                            if (inFile.Length - inFile.Position <= dataPortionSize)
                            {
                                _dataPortionSize = (int)(inFile.Length - inFile.Position);
                            }
                            else
                            {
                                _dataPortionSize = dataPortionSize;
                            }
                            dataSource[pCount] = new byte[_dataPortionSize];
                            inFile.Read(dataSource[pCount], 0, _dataPortionSize);

                            tPool[pCount] = new Thread(CompressBlock);
                            tPool[pCount].Name = "Tred_" + pCount;
                            tPool[pCount].Start(pCount);

                        }
                        for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);)
                        {
                            tPool[portionCount].Join();
                            BitConverter.GetBytes(dataSourceZip[portionCount].Length + 1).CopyTo(dataSourceZip[portionCount], 4);
                            outFile.Write(dataSourceZip[portionCount], 0, dataSourceZip[portionCount].Length);
                            portionCount++;

                            //if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                            //{   // добавлен блок обработки паразитивных байт
                            //    BitConverter.GetBytes(dataSourceZip[portionCount].Length + 1).CopyTo(dataSourceZip[portionCount], 4);
                            //    outFile.Write(dataSourceZip[portionCount], 0, dataSourceZip[portionCount].Length);
                            //    portionCount++;
                            //}
                        }
                        if (isStop)
                        {
                            break;
                        }

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

        public void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            isStop = true;
            args.Cancel = true;
            Console.WriteLine("Архивация прервана пользователем!");
        }
    }
}
