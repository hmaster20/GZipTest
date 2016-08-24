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
                            Zip.Read(buffer, 0, 8);//Чтение 8 байт в буфер
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
















    }

}
