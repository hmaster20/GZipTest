using System;
using System.IO;

namespace GZipTest
{
    public class GZip
    {
        internal int threadNumber;
        internal byte[][] dataSource;
        internal byte[][] dataSourceZip;

        private int blockForCompress = (int)Math.Pow(2, 20); // размер блока 2^24 равен 16.777.216 байт, 2^20 равен 1.048.576 
        internal int BlockForCompress
        {
            get { return blockForCompress; }
            set { blockForCompress = value; }
        }

        private bool isStop = false;
        internal bool IsStop
        {
            get { return isStop; }
            set { isStop = value; }
        }
        
        public GZip()
        {
            threadNumber = Environment.ProcessorCount;
            dataSource = new byte[threadNumber][];
            dataSourceZip = new byte[threadNumber][];
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Handler);//срабатывает при нажатии Ctrl+C
        }

        public void Handler(object sender, ConsoleCancelEventArgs args)
        {
            IsStop = true;
            args.Cancel = true;
            Console.WriteLine("Операция прервана пользователем!");
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
        public int CheckResult(string FileOut)
        {
            if (IsStop && File.Exists(FileOut)) File.Delete(FileOut);//если выявлен сбой или ручная остановка то удаляем кривой файл
            return IsStop ? 1 : 0;
        }
    }
}
