using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class gzip51
    {
        private void btnStartPack_Click(object sender, RoutedEventArgs e)
        {
            FileInfo fi = new FileInfo(this.textBoxFilePath.Text);
            String compressedFile = fi.FullName + ".gz";
            String decompressedFile = GetFileNameWithoutExtension(compressedFile);
            String part2 = GetFileNameWithoutExtension(decompressedFile);

            decompressedFile = decompressedFile.Insert(part2.Length, "_");

            GZip.CompressFile(this.textBoxFilePath.Text, compressedFile, 2048);
            GZip.DecompressFile(compressedFile, decompressedFile, 2048);
        }

        private String GetFileNameWithoutExtension(String FileName)
        {
            FileInfo fi = new FileInfo(FileName);
            int realLengthFileName = fi.FullName.Length - fi.Extension.Length;
            return fi.FullName.Substring(0, realLengthFileName);
        }

        public static void Compress(String fileSource, String fileDestination, int bufferSize)
        {
            using (FileStream fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
                {
                    GZipStream gzipStream = new GZipStream(fsOutput, CompressionMode.Compress);
                    Byte[] buffer = new Byte[bufferSize];
                    int h;

                    while ((h = fsInput.Read(buffer, 0, bufferSize)) != 0)
                    {
                        gzipStream.Write(buffer, 0, h);
                    }
                }
            }
        }

        public static void Decompress(String fileSource, String fileDestination, int bufferSize)
        {
            using (FileStream fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
                {
                    GZipStream gzipStream = new GZipStream(fsInput, CompressionMode.Decompress);
                    Byte[] buffer = new Byte[bufferSize];
                    int h;

                    while ((h = gzipStream.Read(buffer, 0, bufferSize)) != 0)
                    {
                        fsOutput.Write(buffer, 0, h);
                    }
                }
            }
        }
    }
}

