using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class test
    {
        public static void Decompress(string inFileName)
        {
            using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream decomp = new GZipStream(inFile, CompressionMode.Decompress))
                {
                    string dir = Path.GetDirectoryName(inFileName);
                    string decompressionFileName = dir + Path.GetFileNameWithoutExtension(inFileName) + "_decompressed";
                    Console.Write("processing...");
                    int BufferSize = 8192;
                    using (FileStream outStream = new FileStream(inFileName, FileMode.Create, FileAccess.Write))
                    {
                        int read = 0;
                        byte[] buffer = new byte[BufferSize];
                        while ((read = decomp.Read(buffer, 0, BufferSize)) != 0)
                        {
                            outStream.Write(buffer, 0, read);
                        }
                        outStream.Close();
                    }
                    decomp.Close();
                }
                inFile.Close();
            }
        }

        static int BufferSize = 8192;


        static public void CompressOld(string inFileName)
        {
            using (FileStream inFile = new FileStream(inFileName, FileMode.Open))
            {
                using (FileStream comp = new FileStream(inFileName + ".gz", FileMode.Append))
                {
                    int read = 0;
                    byte[] buffer = new byte[BufferSize];
                    using (GZipStream inStream = new GZipStream(comp, CompressionMode.Compress))
                    {
                        Console.Write("compressing");
                        while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
                        {
                            Console.Write('-');
                            inStream.Write(buffer, 0, read);
                        }
                        inStream.Close();
                    }
                    comp.Close();
                }
                inFile.Close();
            }
        }


        //public static void Compress(string inFileName, string outFileName, bool error)
        public static void Compress(string inFileName, string outFileName)
        {
            try
            {
                if (!File.Exists(outFileName))
                    using (FileStream inFile = new FileStream(inFileName, FileMode.Open))
                    {
                        using (FileStream comp = new FileStream(outFileName + ".gz", FileMode.Create))
                        {
                            int read = 0;
                            byte[] buffer = new byte[BufferSize];
                            using (GZipStream inStream = new GZipStream(comp, CompressionMode.Compress))
                            {
                                Console.Write("packing: ");
                                while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
                                {
                                    Console.Write('-');
                                    inStream.Write(buffer, 0, read);
                                }
                                //error = false;
                                inStream.Close();
                            }
                            comp.Close();
                        }
                        inFile.Close();
                    }
            }

            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                //error = true;
            }
            GC.Collect();
        }


    }
}
