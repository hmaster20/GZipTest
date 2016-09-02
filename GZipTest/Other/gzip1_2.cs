using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    public static class gzip1_2
    {
        //После изучения спецификации GZIP, я нашел способ универсального архива без включения в архив паразитных байтов.
        //можно сжать/распаковывать на любой тачке с любым количеством процессоров.
        //Не ожидал от майкрософта такой кривой и обрезанной реализации GZIP.
        //Нет никакой программной реализации доступа к флагам и свойствам архива - всё только ручками.В общем, сделал.
        //
        //1. создаём массив байтовых массивов размерностью[количество потоков(процессоров)][длина порции данных].
        //2. читаем часть файла в этот массив.
        //3. жмем этот массив в другой массив (compressedDataArray)
        //4. пишем compressedDataArray в архивный файл
        //5. читаем новую порцию фала.

        //К слову - там, куда я это писал не заработало (сказали, что не заработало). 
        //Косяк скорее всего был в том, что размер массива (порции чтения) был динамическим и зависел от размера файла подкачки - т.е. так делать НЕ надо! 
        //просто установите размер массива фиксированным - я сделал его равным 2^20, но уже поздно было.

        static int threadNumber = Environment.ProcessorCount;
        //static Thread[] tPool = new Thread[threadNumber];

        static byte[][] dataArray = new byte[threadNumber][];
        static byte[][] compressedDataArray = new byte[threadNumber][];

        //static int dataPortionSize = 10000000;
        //static int dataArraySize = dataPortionSize * threadNumber;

        //static void Main(string[] args)
        //{
        //    string fileName = "D:\\original.jpg";
        //    Compress(fileName);
        //}

        public static void Compress(string inFileName)
        {
            //int dataPortionSize = Environment.SystemPageSize / threadNumber; // может вызывать ошибку
            //int dataPortionSize = 2 ^ 20; // размер блока для сжатия
            int dataPortionSize = 2; // размер блока для сжатия
            try
            {
                FileStream inFile = new FileStream(inFileName, FileMode.Open);
                FileStream outFile = new FileStream(inFileName + ".gz", FileMode.Append);

                int _dataPortionSize;
                Thread[] tPool;

                Console.Write("Compressing...");

                while (inFile.Position < inFile.Length)
                {
                    Console.Write(".");
                    tPool = new Thread[threadNumber];
                    for (int portionCount = 0;
                         (portionCount < threadNumber) && (inFile.Position < inFile.Length);
                         portionCount++)
                    {
                        if (inFile.Length - inFile.Position <= dataPortionSize)
                        {
                            _dataPortionSize = (int)(inFile.Length - inFile.Position);
                        }
                        else
                        {
                            _dataPortionSize = dataPortionSize;
                        }
                        dataArray[portionCount] = new byte[_dataPortionSize];
                        inFile.Read(dataArray[portionCount], 0, _dataPortionSize);

                        tPool[portionCount] = new Thread(CompressBlock);
                        tPool[portionCount].Start(portionCount);
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
                outFile.Close();
                inFile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }
        }

        public static void CompressBlock(object i)
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



        public static void Decompress(string inFileName)
        {
            try
            {
                FileStream inFile = new FileStream(inFileName, FileMode.Open);
                FileStream outFile = new FileStream(inFileName.Remove(inFileName.Length - 3), FileMode.Append);
                int _dataPortionSize;
                int compressedBlockLength;
                Thread[] tPool;
                Console.Write("Decompressing...");
                byte[] buffer = new byte[8];


                while (inFile.Position < inFile.Length)
                {
                    Console.Write(".");
                    tPool = new Thread[threadNumber];
                    for (int portionCount = 0; (portionCount < threadNumber) && (inFile.Position < inFile.Length); portionCount++)
                    {
                        inFile.Read(buffer, 0, 8);  //чтение 8 байт и запись в buffer
                        compressedBlockLength = BitConverter.ToInt32(buffer, 4);    // возваращает число на основе 4 байт начиная с позиции 4
                        compressedDataArray[portionCount] = new byte[compressedBlockLength + 1];//создаем массив размером 
                        buffer.CopyTo(compressedDataArray[portionCount], 0);


                        inFile.Read(compressedDataArray[portionCount], 8, compressedBlockLength - 8);
                        _dataPortionSize = BitConverter.ToInt32(compressedDataArray[portionCount], compressedBlockLength - 4);
                        dataArray[portionCount] = new byte[_dataPortionSize];

                        tPool[portionCount] = new Thread(DecompressBlock);
                        tPool[portionCount].Start(portionCount);
                    }

                    for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);) // выяснить работу последнего блока
                    {
                        if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                        {
                            outFile.Write(dataArray[portionCount], 0, dataArray[portionCount].Length);
                            portionCount++;
                        }
                    }
                }
                outFile.Close();
                inFile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }
        }

        public static void DecompressBlock(object i)
        {
            using (MemoryStream input = new MemoryStream(compressedDataArray[(int)i]))
            {

                using (GZipStream ds = new GZipStream(input, CompressionMode.Decompress))
                {
                    ds.Read(dataArray[(int)i], 0, dataArray[(int)i].Length);
                }
            }
        }
    }


}
