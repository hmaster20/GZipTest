// Немного подсмотрел на MSDN + Parallel.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;
//using System.Threading.Tasks; // .Net 4

namespace GZipTest
{
    //class no_gzip42_pl
    //{
    //    static void Main(string[] args)
    //    {
    //        string path = @"C:\Test";
    //        Compress(path);
    //        Decompress(path);
    //        Console.WriteLine("Done!");
    //        Console.ReadLine();
    //    }

    //    static void Decompress(string path)
    //    {
    //        IEnumerable<FileInfo> files = new DirectoryInfo(path).GetFiles().Where(i => 
    //        (i.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && i.Extension == ".gz");

    //        Parallel.ForEach(files, (info) =>
    //        {
    //            using (FileStream oldStream = File.OpenRead(info.FullName))
    //            {
    //                string newPath = Path.Combine(info.DirectoryName, Path.GetFileNameWithoutExtension(info.FullName) + "_new.jpg");
    //                using (FileStream fStream = File.Create(newPath))
    //                {
    //                    using (GZipStream gzStream = new GZipStream(oldStream, CompressionMode.Decompress))
    //                    {
    //                        gzStream.CopyTo(fStream);
    //                    }
    //                }
    //            }
    //        });
    //    }

    //    static void Compress(string path)
    //    {
    //        IEnumerable<FileInfo> files = new DirectoryInfo(path).GetFiles().Where(i => (i.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden);
    //        Parallel.ForEach(files, (info) =>
    //        {
    //            using (FileStream fStream = File.OpenRead(info.FullName))
    //            {
    //                string newPath = Path.Combine(info.DirectoryName, Path.GetFileNameWithoutExtension(info.FullName) + ".gz");
    //                using (FileStream wStream = File.Create(newPath))
    //                {
    //                    using (GZipStream gzStream = new GZipStream(wStream, CompressionMode.Compress))
    //                    {
    //                        fStream.CopyTo(gzStream);
    //                    }
    //                }
    //            }
    //        });
    //    }
    //}
}
