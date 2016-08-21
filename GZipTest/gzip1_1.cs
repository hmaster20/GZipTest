using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    class gzip1_1
    {
        public static int threadNumber = Environment.ProcessorCount;
        //static Thread[] tPool = new Thread[threadNumber];

        static byte[][] dataArray = new byte[threadNumber][];
        static byte[][] compressedDataArray = new byte[threadNumber][];

        static int dataPortionSize = 10000000;
        static int dataArraySize = dataPortionSize * threadNumber;

        //static void Main(string[] args)
        //{
        //    string fileName = "D:\\original.jpg";
        //    Compress(fileName);
        //}

        public static void Compress(string inFileName)
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
                    if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                    {
                        outFile.Write(compressedDataArray[portionCount], 0, compressedDataArray[portionCount].Length);
                        portionCount++;
                    }
                }
            }

            outFile.Close();
            inFile.Close();
        }

        static public void CompressBlock(object i)
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




        // http://www.cyberforum.ru/csharp-beginners/thread927080.html
        // Сжатие:
        //1.Читаю файл массивами блоков(массив по количеству процессов)
        //2.Архивация потоками
        //3.Полученный сжатый массив пишу в файл
        //4.Читаем следующий массив и т.д.
        //Вот в чём интерес-сжатие происходит без ошибок, распаковка делается даже Winrarом, но КАК мне реализовать распаковку самому? 
        //Ведь, когда пишу блоками, явно не указываю ни размер блока до сжатия, ни размер блока данных, записываемых в файл.

        // Что-то я не нашел в вашем коде, завершения работы потока.Где оно? 
        // Перед записью на диск вызовите Thread.Join на соответствующем потоке, чтобы убедиться что он завершился.
        // поясню: как я понял, потоки завершаются при завершении выполнения функции в нем. Завершения потоков я жду в следующей конструкции:

        //if (tPool[portionCount].ThreadState == ThreadState.Stopped)
        //* * * * * * * * * * {
        //* * * * * * * * * * * * outFile.Write(compressedDataArray[portionCount], 0, compressedDataArray[portionCount].Length);
        //* * * * * * * * * * * * portionCount++;
        //* * * * * * * * * * }

        //Завершенный процесс имеет статус Stopped, пока все не Stopped, запись не производится.
        //Немного деревянно(относительно процессорного времени), но работает хорошо.

        //Какая-то у вас странная логика.Предположим, первый поток не успел завершиться.
        //Вы сделали if (tPool[0].ThreadState == ThreadState.Stopped) - он вернул false. 
        //И что вы делаете с первым сжатым куском? Выбрасываете? Ожидания то нету.

        //итерация происходит снова - инкремент номера потока происходит только после того, как этот поток завершился.
        //Конструкция немного не прозрачна, но логику, я думаю, объяснил.

        //Да, точно, проглядел чуть-чуть.Интересная идея, только скорее всего будет тратить процессорное время, если поток запоздает.
        //Советую, все же вызвать Join, и убрать if.
        //PS.Зачем у вас вот это условие в последнем цикле (tPool[portionCount] != null) ? 
        //Вы же всегда точно создаете массив из threadNumber потоков, и всегда полностью забиваете его потоками.

        // О...это те ещё грабли! 
        //Пример 1:Если файл размером 1900 байт, потоков 8, а размер буфера под каждый поток - 1000 байт(например), то создаётся всего 2 потока.
        //Пример 2: файл размером 8900 байт - первый раз создаем 8 потоков и жмем порцию в 8000байт, затем читаем остатки и жмем в один поток.
        //Всё-таки нельзя извернуться и НЕ писать размер блока в файл? Просто так красиво получалось - эээх.Теперь архив открывается только этой же программой...
        //Можно пойти дальше - создать индексный (второй файл) и писать его в начало или просто таскать его с собой, но это уже не гуд!
        //Ещё есть фича по поводу размера блока. 
        //Размер блока, как и количество потоков, выбирается автоматически, в зависимости от машины на которой запущена программа.
        //(просто в текущей версии убрал это из-за экспериментов с записью блоков). 
        //Вот так было изначально static int dataPortionSize = Environment.SystemPageSize.
        //Это количество памяти на процесс делим на количество потоков и получаем оптимальный размер буфера под каждый поток.
        //Скорость в этом случае приростает значительно! 
        //Кстати, тестовые файлы - 1 - картинка размером 320кб, 2 - картинка размером 25 МБ, 3 - файл виртуальной машины 8ГБ =) 
        //Программа не вешает всё вокруг и закончить её можно по Ctrl+C =)



    }
}
