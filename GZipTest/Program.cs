using System;
using System.Collections.Generic;
using System.Linq;



namespace GZipTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            {
                zip cc = new zip();
                cc.Compress("test3.avhdx");
                cc = null;
            }
            GC.Collect();

            // zip.Compress("test3.avhdx");
            Console.WriteLine("Архивация завершена");
            Console.ReadLine();
        }

    }
}

