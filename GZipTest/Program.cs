using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest // GzipStream
{
    class Program
    {
        public static void Main(string[] args)
        {
            switch (args[0])
            {
                case "compress": Console.WriteLine("compress");break;
                case "decompress": Console.WriteLine("decompress"); break;
                default: break;
            }

            // foreach (string l in File.ReadLines(path, Encoding.GetEncoding(1251))) 
            // http://www.cyberforum.ru/csharp-beginners/thread757831.html


            ConsoleKeyInfo cki;

            Console.Clear();

            // Establish an event handler to process key press events.
            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
            while (true)
            {
                Console.Write("Press any key, or 'X' to quit, or ");
                Console.WriteLine("CTRL+C to interrupt the read operation:");

                // Start a console read operation. Do not display the input.
                cki = Console.ReadKey(true);

                // Announce the name of the key that was pressed .
                Console.WriteLine("  Key pressed: {0}\n", cki.Key);

                // Exit if the user pressed the 'X' key.
                if (cki.Key == ConsoleKey.X) break;
            }
        }

        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("\nThe read operation has been interrupted.");

            Console.WriteLine("  Key pressed: {0}", args.SpecialKey);

            Console.WriteLine("  Cancel property: {0}", args.Cancel);

            // Set the Cancel property to true to prevent the process from terminating.
            Console.WriteLine("Setting the Cancel property to true...");
            args.Cancel = true;

            // Announce the new value of the Cancel property.
            Console.WriteLine("  Cancel property: {0}", args.Cancel);
            Console.WriteLine("The read operation will resume...\n");
        }
    }

}
