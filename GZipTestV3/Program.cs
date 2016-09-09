using System;
using System.Collections.Generic;
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
                    //int FileBuffer = threadNumber * 4096;
                    int FileBuffer = threadNumber * blockForCompress;
                    //using (FileStream FileIn = new FileStream(FileInput, FileMode.Open))
                    //using (FileStream FileIn = new FileStream(FileInput, FileMode.Open, FileAccess.Read, FileShare.Read, FileBuffer, FileOptions.Asynchronous))
                    using (FileStream FileIn = new FileStream(FileInput, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, FileOptions.Asynchronous))
                    using (BufferedStream BufferedFileIn = new BufferedStream(FileIn, 64*1024))
                    {
                        using (FileStream FileOut = new FileStream(FileOutput, FileMode.Append))
                        {
                            Thread[] tPool;
                            if (CompressMethod == CompressMethod.zip)
                            {
                                Console.WriteLine("Сжатие... ");
                                //while (FileIn.Position < FileIn.Length)
                                //{
                                //    drawTextProgressBar(FileIn.Position, FileIn.Length);
                                //    tPool = new Thread[threadNumber];
                                //    ReadFile(FileIn, tPool);
                                //    CreateZipFile(FileOut, tPool);
                                //    if (isStop) break;
                                //}

                                while (BufferedFileIn.Position < BufferedFileIn.Length)
                                {
                                    drawTextProgressBar(BufferedFileIn.Position, BufferedFileIn.Length);
                                    tPool = new Thread[threadNumber];
                                    ReadBuffer(BufferedFileIn, tPool);
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка: " + e.Message);
                    isStop = true;
                }

                // var spendTime = DateTime.Now - before;
                double timepassed = Math.Round((DateTime.Now - before).TotalSeconds, 1);
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



            private static void ReadBuffer(BufferedStream Buff, Thread[] tPool)
            {
                int FileBlock;
                for (int N = 0; (N < threadNumber) && (Buff.Position < Buff.Length); N++)
                {
                    if (Buff.Length - Buff.Position >= blockForCompress)
                        FileBlock = blockForCompress;
                    else
                        FileBlock = (int)(Buff.Length - Buff.Position);

                    dataSource[N] = new byte[FileBlock];
                    Buff.Read(dataSource[N], 0, FileBlock); // читаем блока файла длинной FileBlock и пишем в буфер dataSource[N] || int i = File.Read, i сколько прочитали

                    tPool[N] = new Thread(CompressBlock);
                    tPool[N].Name = "Tr_" + N;
                    tPool[N].Start(N);
                }
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
                    File.Read(dataSource[N], 0, FileBlock); // читаем блока файла длинной FileBlock и пишем в буфер dataSource[N] || int i = File.Read, i сколько прочитали

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



            //This sounds like a fairly common requirement which can be solved by a multi-threaded producer-consumer queue.
            //The threads are kept 'alive' and are signaled to do work when new work is added to the queue.
            //The work is represented by a delegate (in your case ComputePartialDataOnThread) and the data passed to the delegate is what is queued 
            //(in your case the params to ComputePartialDataOnThread). 
            //The useful feature is that the implementation of managing worker threads and the actual algorithms are separate.Here is the p-c queue:

            //Это звучит как довольно общим требованием, которые могут быть решены с помощью многопоточной очереди на производитель-потребитель.
            //Нити поддерживаются 'живыми' и сигнализируют, чтобы сделать работу, когда новая работа добавляется в очередь.
            //Работа представлена делегатом(в вашем случае ComputePartialDataOnThread) и данные, 
            //передаваемые делегату то, что находится в очереди(в вашем случае Params к ComputePartialDataOnThread). 
            //Полезная особенность в том, что осуществление управления рабочими потоками и реальными алгоритмами отдельно.
            //Вот очередь р-с:

            //Did you use this successfully and was it faster?
            //Yes, in its final incarnation it's a bit different and more complex that that, but the same idea. 
            //The threads go into a low-priority mode after a few seconds of inactivity, then exit after 30 seconds of inactivity. 
            //This is in accordance with the type and frequency of work they will expect. It runs at close to the same speed as ThreadPool, but is less jittery. 
            //Also I found I could get better CPU caching by splitting the data up into adjacent lines as opposed to chunks, which also made a difference.

            // Использовали ли вы это успешно и был это быстрее?
            // Да, в своем последнем воплощении это немного отличается и более сложным, что это, но та же идея.
            // Потоки переходит в режим низкого приоритета через несколько секунд бездействия, а затем выйти через 30 секунд бездействия.
            // Это в соответствии с типом и частотой работы, которую они будут ожидать. Он работает на частоте, близкой к той же скоростью, как ThreadPool, 
            // но менее поволноваться.
            // Кроме того, я обнаружил, что я мог бы получить лучшее кэширование процессора путем разделения данных вверх на соседние линии, в отличие от кусков, 
            // которые также сделали разницу.




            public class SuperQueue<T> : IDisposable where T : class
            {
                readonly object _locker = new object();
                readonly List<Thread> _workers;
                readonly Queue<T> _taskQueue = new Queue<T>();
                readonly Action<T> _dequeueAction;

                /// <summary>
                /// Initializes a new instance of the <see cref="SuperQueue{T}"/> class.
                /// </summary>
                /// <param name="workerCount">The worker count.</param>
                /// <param name="dequeueAction">The dequeue action.</param>
                public SuperQueue(int workerCount, Action<T> dequeueAction)
                {
                    _dequeueAction = dequeueAction;
                    _workers = new List<Thread>(workerCount);

                    // Create and start a separate thread for each worker
                    // Создать и запустить отдельный поток для каждого работника
                    for (int i = 0; i < workerCount; i++)
                    {
                        Thread t = new Thread(Consume) { IsBackground = true, Name = string.Format("SuperQueue worker {0}", i) };
                        _workers.Add(t);
                        t.Start();
                    }
                }

                /// <summary>
                /// Enqueues the task. 
                /// Ставит в очередь задачу.
                /// </summary>
                /// <param name="task">The task.</param>
                public void EnqueueTask(T task)
                {
                    lock (_locker)
                    {
                        _taskQueue.Enqueue(task);
                        Monitor.PulseAll(_locker);
                    }
                }

                /// <summary>
                /// Consumes this instance.
                /// </summary>
                void Consume()
                {
                    while (true)
                    {
                        T item;
                        lock (_locker)
                        {
                            while (_taskQueue.Count == 0) Monitor.Wait(_locker);
                            item = _taskQueue.Dequeue();
                        }
                        if (item == null) return;

                        // run actual method
                        // запустить фактический метод
                        _dequeueAction(item);
                    }
                }

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    // Enqueue one null task per worker to make each exit.
                    _workers.ForEach(thread => EnqueueTask(null));

                    _workers.ForEach(thread => thread.Join());

                }
            }



        }
    }
}

