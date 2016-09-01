using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
