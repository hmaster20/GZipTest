using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;


namespace GZipTest
{
    public  class zip
    {
         int threadNumber = Environment.ProcessorCount;
         byte[][] dataArray ;
         byte[][] compressedDataArray;
         int dataPortionSize = (int)Math.Pow(2, 26); //2 ^ 20; // размер блока для сжатия


        public zip()
        {
            dataArray = new byte[threadNumber][];
            compressedDataArray = new byte[threadNumber][];
        }
        //static int dataPortionSize = 10000000;

        public  void Compress(string inFileName)
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))
                using (FileStream outFile = new FileStream(inFileName + ".gz", FileMode.Append))
                {
                    int _dataPortionSize;
                    Thread[] tPool;
                    Console.Write("Compressing...");

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
                            dataArray[pCount] = new byte[_dataPortionSize];
                            inFile.Read(dataArray[pCount], 0, _dataPortionSize);

                            tPool[pCount] = new Thread(CompressBlock);
                            tPool[pCount].Name = "Tred_"+ pCount;
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
                                BitConverter.GetBytes(compressedDataArray[portionCount].Length + 1)
                                            .CopyTo(compressedDataArray[portionCount], 4);
                                outFile.Write(compressedDataArray[portionCount], 0, compressedDataArray[portionCount].Length);
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
            using (MemoryStream output = new MemoryStream(dataArray[(int)i].Length))
            {
                using (GZipStream cs = new GZipStream(output, CompressionMode.Compress))
                {
                    cs.Write(dataArray[(int)i], 0, dataArray[(int)i].Length);
                }
                compressedDataArray[(int)i] = output.ToArray();
            }
        }

    }
}
