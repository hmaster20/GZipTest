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
         byte[][] dataSource ;
         byte[][] dataSourceZip;
         int dataPortionSize = (int)Math.Pow(2, 24); //2 ^ 20; // размер блока для сжатия //16 777 216


        public GZipCompress()
        {
            dataSource = new byte[threadNumber][];
            dataSourceZip = new byte[threadNumber][];
        }

        public void Compress(string FileIn, string FileOut)
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
                        tPool = new Thread[threadNumber];
                       
                        for (int pCount = 0;
                             (pCount < threadNumber) && (inFile.Position < inFile.Length);
                             pCount++)
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
                            tPool[pCount].Name = "Tred_"+ pCount;
                            //tPool[pCount].Priority = ThreadPriority.AboveNormal;
                            tPool[pCount].Start(pCount);
                        }
                        for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);)
                        {
                            // поясню: потоки завершаются при завершении выполнения функции в нем. Завершения потоков я жду в следующей конструкции:
                            //Завершенный процесс имеет статус Stopped, пока все не Stopped, запись не производится.
                            //Немного деревянно(относительно процессорного времени), но работает хорошо.
                            //Перед записью на диск вызовите Thread.Join на соответствующем потоке, чтобы убедиться что он завершился.
                            //Советую, все же вызвать Join, и убрать if.

                            if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                            {   // добавлен блок обработки паразитивных байт
                                BitConverter.GetBytes(dataSourceZip[portionCount].Length + 1)
                                            .CopyTo(dataSourceZip[portionCount], 4);
                                outFile.Write(dataSourceZip[portionCount], 0, dataSourceZip[portionCount].Length);
                                portionCount++;
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }
        }

        private  void CompressBlock(object i)
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
