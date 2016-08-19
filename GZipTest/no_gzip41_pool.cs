using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    //public static class gzip41
    //{
    //    static int BufferSize = 8192;
    //    static public void Compress(string inFileName)
    //    {
    //        using (FileStream inFile = new FileStream(inFileName, FileMode.Open))
    //        {
    //            using (FileStream comp = new FileStream(inFileName + ".gz", FileMode.Append))
    //            {
    //                int read = 0;
    //                byte[] buffer = new byte[BufferSize];
    //                using (GZipStream inStream = new GZipStream(comp, CompressionMode.Compress))
    //                {
    //                    Console.Write("compressing");
    //                    while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
    //                    {
    //                        Console.Write('-');
    //                        inStream.Write(buffer, 0, read);
    //                    }
    //                    inStream.Close();
    //                }
    //                comp.Close();
    //            }
    //            inFile.Close();
    //        }
    //    }


    //    public static void Compress(string inFileName, string outFileName, bool error)
    //    {
    //        try
    //        {
    //            if (!File.Exists(outFileName))
    //                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))
    //                {
    //                    using (FileStream comp = new FileStream(outFileName + ".gz", FileMode.Create))
    //                    {
    //                        int read = 0;
    //                        byte[] buffer = new byte[BufferSize];
    //                        using (GZipStream inStream = new GZipStream(comp, CompressionMode.Compress))
    //                        {
    //                            Console.Write("packing: ");
    //                            while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
    //                            {
    //                                Console.Write('-');
    //                                inStream.Write(buffer, 0, read);
    //                            }
    //                            error = false;
    //                            inStream.Close();
    //                        }
    //                        comp.Close();
    //                    }
    //                    inFile.Close();
    //                }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("ERROR: " + ex.Message);
    //            error = true;
    //        }
    //        GC.Collect();
    //    }


    //    private static void runing()
    //    {
    //          for (j = 0; (j < multithread - 1) && ((inFile.Length - inFile.Position) > BufferSize); j++)
    //        {
    //            thread[j] = new Thread(() =>
    //            {
    //                Console.Write("- {0} -", j);
    //                read[j] = inFile.Read(buffer, 0, BufferSize);
    //                inGZip.Write(buffer, 0, read[j]);
    //            });
    //            thread[j].Start();
    //            Thread.Sleep(500);
    //            thread[j].Join();
    //            thread[j] = new Thread(() =>
    //            {
    //                inGZip.Write(buffer, 0, read[j]);
    //                Console.Write("|");
    //                Thread.Sleep(100);
    //            });
    //            thread_[j].Start();
    //            thread_[j].Join();
    //        }
    //    }


    //}
}
