using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public static class ZipStreamOne
    {
        public static void CryptCompressRun()
        {
            var cryptCompress = new CryptCompress();
            var thread = new Thread(() => cryptCompress.CompressFile("test2.doc"));
            thread.Start();

            int cnt = 0;
            while (thread.IsAlive)
            {
                cnt++;
                Console.WriteLine("Waiting... " + cnt);
                Thread.Sleep(50);
            }

            Console.WriteLine("Before Compression KBytes: {0}", cryptCompress.BeforeCompressionBytes / 1024);
            Console.WriteLine("After Compression KBytes: {0}", cryptCompress.AfterCompressionBytes / 1024);
            Console.ReadLine();
        }
    }

    //ZipMultiThreading
    public class CryptCompress
    {
        public byte[] CompressedBytes { get; set; }
        public long BeforeCompressionBytes { get; set; }
        public long AfterCompressionBytes { get; set; }

        public void CompressFile(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var SourceBytes = new byte[fileStream.Length];
                fileStream.Read(SourceBytes, 0, (int)fileStream.Length);
                CompressedBytes = CompressGzip(SourceBytes);
                BeforeCompressionBytes = fileStream.Length;
                AfterCompressionBytes = CompressedBytes.Length;
                fileStream.Close();
            }
        }

        public byte[] CompressGzip(byte[] SourceBytesIn)
        {
            using (var memory = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gZipStream.Write(SourceBytesIn, 0, SourceBytesIn.Length);
                }
                return memory.ToArray();
            }
        }
    }
}
