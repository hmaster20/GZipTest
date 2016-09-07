using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    class Program
    {
        static int code;
        public static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(GZip.Handler);//срабатывает при нажатии Ctrl+C

            if (args.Length == 3)
            {
                switch (args[0])
                {
                    case "compress": code = GZip.FileProcessing(args[1], args[2], CompressMethod.zip); break;
                    case "decompress": code = GZip.FileProcessing(args[1], args[2], CompressMethod.unzip); break;
                    default: help(); break;
                }
            }
            else if (args.Length == 0 || args.Length > 3 || args.Length < 3)
            {
                Console.WriteLine("Для правильного указания параметров воспользуйтесь справкой ниже.\n");
                help(); code = 1;
            }

            Environment.Exit(code);
        }

        private static void help()
        {
            Console.WriteLine("Справка по использованию программы архиватора.\n");
            Console.WriteLine("GZipTest <команда> [файл 1] [файл 2]\n");
            Console.WriteLine("<команда>");
            Console.WriteLine("compress\t - выполняет сжатие [исходного файла]");
            Console.WriteLine("decompress\t - выполняет распаковку [архивного файла]");
            Console.WriteLine("\nАрхивация:\tGZipTest compress document.doc document.doc.gz\t (создание архива document.doc.gz)");
            Console.WriteLine("Распаковка:\tGZipTest decompress document.gz document.doc\t (распаковка архива document.doc.gz)");
        }

        public enum CompressMethod { zip, unzip }

        public static class GZip
        {
            public static int threadNumber = Environment.ProcessorCount;
            public static byte[][] dataSource = new byte[threadNumber][];
            public static byte[][] dataSourceZip = new byte[threadNumber][];
            public static int blockForCompress = (int)Math.Pow(2, 20);          // размер блока 2^24 равен 16.777.216 байт, 2^20 равен 1.048.576 
            public static bool isStop = false;

            public static void Handler(object sender, ConsoleCancelEventArgs args)
            {
                isStop = true;
                args.Cancel = true;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nОперация прервана пользователем!");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            public static bool FileExist(string FileOut)
            {
                if (File.Exists(FileOut))
                {
                    Console.WriteLine("Файл \"" + FileOut + "\" уже существует, укажите другое имя!");
                    return true;
                }
                return false;
            }

            //проверка завершения выполнения главного метода
            public static int CheckResult(string FileOut)
            {
                if (isStop && File.Exists(FileOut)) File.Delete(FileOut);                
                return isStop ? 1 : 0;
            }

            private static void drawTextProgressBar(long progress, long total)
            {
                //нарисовать пустой индикатор
                Console.CursorLeft = 0;
                Console.Write("["); //start
                Console.CursorLeft = 32;
                Console.Write("]"); //end
                Console.CursorLeft = 1;
                float onechunk = 30.0f / total;

                int percent = (int)(Math.Round(((double)progress / (double)total * 100), 1));

                //нарисовать заполненную часть
                int position = 1;
                for (int i = 0; i <= Math.Round((onechunk * progress), MidpointRounding.ToEven); i++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.CursorLeft = position++;
                    Console.Write(" ");
                }

                //нарисовать незаполненную часть
                for (int i = position; i <= 31; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.CursorLeft = position++;
                    Console.Write("#");
                }

                //рисовать итоги
                Console.CursorLeft = 35;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("Обработано байт {0} из {1}, {2}%    ",
                                progress.ToString(), total.ToString(), percent);
            }

            public static int FileProcessing(string FileInput, string FileOutput, CompressMethod CompressMethod)
            {
                //Stopwatch sw = new Stopwatch();
                //sw.Start();
                var before = DateTime.Now;

                if (FileExist(FileOutput)) return 1;
                try
                {
                    using (FileStream FileIn = new FileStream(FileInput, FileMode.Open))
                    using (FileStream FileOut = new FileStream(FileOutput, FileMode.Append))
                    {
                        Thread[] tPool;
                        if (CompressMethod == CompressMethod.zip)
                        {
                            Console.WriteLine("Сжатие... ");
                            while (FileIn.Position < FileIn.Length)
                            {
                                drawTextProgressBar(FileIn.Position, FileIn.Length);
                                tPool = new Thread[threadNumber];
                                ReadFile(FileIn, tPool);
                                CreateZipFile(FileOut, tPool);
                                if (isStop) break;
                            }
                        }
                        else if (CompressMethod == CompressMethod.unzip)
                        {
                            Console.WriteLine("Распаковка... ");
                            while (FileIn.Position < FileIn.Length)
                            {
                                drawTextProgressBar(FileIn.Position, FileIn.Length);
                                tPool = new Thread[threadNumber];
                                ReadZipFile(FileIn, tPool);
                                CreateUnzipFile(FileOut, tPool);
                                if (isStop) break;
                            }
                        }
                        if (!isStop) drawTextProgressBar(FileIn.Length, FileIn.Length);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка: " + e.Message);
                    isStop = true;
                }

                var spendTime = DateTime.Now - before;
                //sw.Stop();
                //TimeSpan ts = sw.Elapsed;
                //Console.WriteLine("\nТаймер засек: " + ts.TotalSeconds.ToString());
                //Console.WriteLine("\n{0:00} час. {1:D2} мин. {2:D2}.{3:00} сек.",
                //                    ts.TotalHours, ts.Minutes, ts.Seconds, ts.TotalMilliseconds);
                //sw.Reset();
                double timepassed = Math.Round(spendTime.TotalSeconds, 1);
                if (timepassed > 59)
                {
                    Console.WriteLine("\nВремя выполнения {0} мин. {1} сек.", ((int)timepassed / 60), ((int)timepassed % 60));
                }
                else
                {
                    Console.WriteLine("\nВремя выполнения {0} мин. {1} сек.", ((int)timepassed / 60), ((int)timepassed % 60));
                }
                
                return CheckResult(FileOutput);
            }


            #region Упаковка файла
            // чтение блоков файла
            private static void ReadFile(FileStream File, Thread[] tPool)
            {
                int FileBlock;
                for (int N = 0; (N < threadNumber) && (File.Position < File.Length); N++)
                {
                    if (File.Length - File.Position >= blockForCompress)
                        FileBlock = blockForCompress;
                    else
                        FileBlock = (int)(File.Length - File.Position);

                    dataSource[N] = new byte[FileBlock];
                    File.Read(dataSource[N], 0, FileBlock); // читаем блока файла длинной FileBlock и пишем в буфер dataSource[N]

                    tPool[N] = new Thread(CompressBlock);
                    tPool[N].Name = "Tr_" + N;
                    tPool[N].Start(N);
                }
            }

            // сжатие блоков
            private static void CompressBlock(object i)
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
            private static void CreateZipFile(FileStream FileZip, Thread[] tPool)
            {
                for (int N = 0; (N < threadNumber) && (tPool[N] != null);)
                {
                    tPool[N].Join();                                    // ожидание потока и работа с блоком
                    BitConverter.GetBytes(dataSourceZip[N].Length + 1)  //получаем размер блока в байтах
                                .CopyTo(dataSourceZip[N], 4);           //запись информации о размере
                    FileZip.Write(dataSourceZip[N], 0, dataSourceZip[N].Length);
                    N++;
                }
            }
            #endregion

            #region Распаковка файла

            //чтение блоков файла
            private static void ReadZipFile(FileStream Zip, Thread[] tPool)
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
            public static void DecompressBlock(object i)
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
                        Console.WriteLine("Ошибка: " + e.Message);
                    }
                }
            }


            //запись распакованных данных
            private static void CreateUnzipFile(FileStream outFile, Thread[] tPool)
            {
                for (int N = 0; (N < threadNumber) && (tPool[N] != null);)
                {
                    tPool[N].Join();//ожидание остановки потока
                    outFile.Write(dataSource[N], 0, dataSource[N].Length);
                    N++;
                }
            }
            #endregion

        }
    }
}

