using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZipTest
{
    public class GZipUnCompress : GZip
    {

        public void Decompress(string FileZip)
        {
            try
            {
                using (FileStream Zip = new FileStream(FileZip, FileMode.Open))
                using (FileStream outFile = new FileStream(FileZip.Remove(FileZip.Length - 3), FileMode.Append))
                {
                    int dataPortionSize;
                    int compressedBlockLength;
                    Thread[] tPool;
                    Console.Write("Decompressing...");
                    byte[] buffer = new byte[8];

                    while (Zip.Position < Zip.Length)
                    {
                        tPool = new Thread[threadNumber];
                        for (int portionCount = 0; (portionCount < threadNumber) && (Zip.Position < Zip.Length); portionCount++)
                        {
                            //Zip.Read(buffer, 0, 8);//Чтение 8 байт в буфер
                            Zip.Read(buffer, 0, buffer.Length);//Чтение 8 байт из файла Zip и запись в буфер
                            compressedBlockLength = BitConverter.ToInt32(buffer, 4);
                            dataSourceZip[portionCount] = new byte[compressedBlockLength + 1];
                            buffer.CopyTo(dataSourceZip[portionCount], 0);

                            Zip.Read(dataSourceZip[portionCount], 8, compressedBlockLength - 8);
                            dataPortionSize = BitConverter.ToInt32(dataSourceZip[portionCount], compressedBlockLength - 4);
                            dataSource[portionCount] = new byte[dataPortionSize];

                            tPool[portionCount] = new Thread(DecompressBlock);
                            tPool[portionCount].Start(portionCount);
                        }
                        for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);) // выяснить работу последнего блока
                        {
                            //if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                            tPool[portionCount].Join();
                            outFile.Write(dataSource[portionCount], 0, dataSource[portionCount].Length);
                            portionCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }
        }

        public void DecompressBlock(object i)
        {
            using (MemoryStream input = new MemoryStream(dataSourceZip[(int)i]))
            {
                using (GZipStream ds = new GZipStream(input, CompressionMode.Decompress))
                {
                    ds.Read(dataSource[(int)i], 0, dataSource[(int)i].Length);
                }
            }
        }

















        // http://stackoverflow.com/questions/14422773/decompress-a-gz-file-using-gzipstream
        public static void DeCompressFile(string CompressedFile, string DeCompressedFile)
        {
            byte[] buffer = new byte[1024 * 1024];
            using (System.IO.FileStream fstrmCompressedFile = System.IO.File.OpenRead(CompressedFile)) // fi.OpenRead())
            {
                using (System.IO.FileStream fstrmDecompressedFile = System.IO.File.Create(DeCompressedFile))
                {
                    using (System.IO.Compression.GZipStream strmUncompress = new System.IO.Compression.GZipStream(fstrmCompressedFile,
                            System.IO.Compression.CompressionMode.Decompress))
                    {
                        int numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                        while (numRead != 0)
                        {
                            fstrmDecompressedFile.Write(buffer, 0, numRead);
                            fstrmDecompressedFile.Flush();
                            numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                        } // Whend
                        //int numRead = 0;
                        //while ((numRead = strmUncompress.Read(buffer, 0, buffer.Length)) != 0)
                        //{
                        //    fstrmDecompressedFile.Write(buffer, 0, numRead);
                        //    fstrmDecompressedFile.Flush();
                        //} // Whend
                        strmUncompress.Close();
                    }   // End Using System.IO.Compression.GZipStream strmUncompress 
                    fstrmDecompressedFile.Flush();
                    fstrmDecompressedFile.Close();
                }       // End Using System.IO.FileStream fstrmCompressedFile 
                fstrmCompressedFile.Close();
            }           // End Using System.IO.FileStream fstrmCompressedFile 
        }               // End Sub DeCompressFile


        // http://www.dotnetperls.com/decompress
        public static byte[] Decompress(byte[] gzip)
        {
            byte[] baRetVal = null;
            using (System.IO.MemoryStream ByteStream = new System.IO.MemoryStream(gzip))
            {

                // Create a GZIP stream with decompression mode.
                // ... Then create a buffer and write into while reading from the GZIP stream.
                using (System.IO.Compression.GZipStream stream = new System.IO.Compression.GZipStream(ByteStream
                    , System.IO.Compression.CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                    {
                        int count = 0;
                        count = stream.Read(buffer, 0, size);
                        while (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                            memory.Flush();
                            count = stream.Read(buffer, 0, size);
                        }

                        baRetVal = memory.ToArray();
                        memory.Close();
                    }

                    stream.Close();
                } // End Using System.IO.Compression.GZipStream stream 

                ByteStream.Close();
            } // End Using System.IO.MemoryStream ByteStream

            return baRetVal;
        } // End Sub Decompress








































    }

}
