using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GZipTest
{

    public static class getRam
    {
        public static void GetMemory()
        {
            long count;
            Process proc = Process.GetCurrentProcess();
            count = proc.PrivateMemorySize64;

            Console.WriteLine("memory: " + (count / 1024 / 1024) + " " + proc.WorkingSet64 + " " + proc.VirtualMemorySize64);

        }


        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
        public static void Gopa()
        {
            long memKb;
            GetPhysicallyInstalledSystemMemory(out memKb);
            Console.WriteLine((memKb / 1024 / 1024) + " GB of RAM installed.");
        }



    }
}
