using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    //class gzip52
    //{
    //    string sourcePath = "you_path";

    //    string compressed = Path.ChangeExtension(sourcePath, "gz");
    //    string decompressed = sourcePath.Insert(sourcePath.Length - 4, "_");

    //    GZip.Compress(sourcePath, compressed);
    //        GZip.Decompress(compressed, decompressed);
 
    //        string str1 = File.ReadAllText(sourcePath);
    //    string str2 = File.ReadAllText(decompressed);
 
    //        if (!str1.Equals(str2))  Debugger.Break();
        
        
    //       public static void Compress(String fileSource, String fileDestination)
    //    {
    //        using (FileStream fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
    //        {
    //            using (FileStream fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
    //            {
    //                using (GZipStream gzipStream = new GZipStream(fsOutput, CompressionMode.Compress))
    //                {
    //                    Byte[] buffer = new Byte[fsInput.Length];
    //                    int h;
    //                    while ((h = fsInput.Read(buffer, 0, buffer.Length)) > 0)
    //                    {
    //                        gzipStream.Write(buffer, 0, h);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public static void Decompress(String fileSource, String fileDestination)
    //    {
    //        using (FileStream fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
    //        {
    //            using (FileStream fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
    //            {
    //                using (GZipStream gzipStream = new GZipStream(fsInput, CompressionMode.Decompress))
    //                {
    //                    Byte[] buffer = new Byte[fsInput.Length];
    //                    int h;
    //                    while ((h = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
    //                    {
    //                        fsOutput.Write(buffer, 0, h);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
