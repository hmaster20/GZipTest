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
        internal int dataPortionSize;
        internal bool isStop;

        public GZip()
        {
            threadNumber = Environment.ProcessorCount;
            dataSource = new byte[threadNumber][];
            dataSourceZip = new byte[threadNumber][];
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Handler);//срабатывает при нажатии Ctrl+C
        }

        public void Handler(object sender, ConsoleCancelEventArgs args)
        {
            isStop = true;
            args.Cancel = true;
            Console.WriteLine("Операция прервана пользователем!");
        }
    }
}
