using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
//using System.Threading.Tasks;

namespace GZipTest
{
    public static class MegaZipUnzip
    {
        public enum CompressionType
        {
            Deflate,
            GZip
        }

        /// <summary>
        /// Compress the source file to the destination file.
        /// This is done in 1MB chunks to not overwhelm the memory usage.
        /// </summary>
        /// <param name="sourceFile">the uncompressed file</param>
        /// <param name="destinationFile">the compressed file</param>
        /// <param name="compressionType">the type of compression to use</param>
        //public static async Task CompressFileAsync(string sourceFile, string destinationFile, CompressionType compressionType)
        //{
        //    if (string.IsNullOrWhiteSpace(sourceFile))
        //        throw new ArgumentNullException(nameof(sourceFile));
        //    if (string.IsNullOrWhiteSpace(destinationFile))
        //        throw new ArgumentNullException(nameof(destinationFile));
        //    FileStream streamSource = null;
        //    FileStream streamDestination = null;
        //    Stream streamCompressed = null;
        //    int bufferSize = 4096;
        //    using (streamSource = new FileStream(sourceFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize, useAsync: true))
        //    {
        //        using (streamDestination = new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
        //        {
        //            // read 1MB chunks and compress them
        //            long fileLength = streamSource.Length;
        //            // write out the fileLength size
        //            byte[] size = BitConverter.GetBytes(fileLength);
        //            await streamDestination.WriteAsync(size, 0, size.Length);
        //            long chunkSize = 1048576; // 1MB
        //            while (fileLength > 0)
        //            {
        //                // read the chunk
        //                byte[] data = new byte[chunkSize];
        //                await streamSource.ReadAsync(data, 0, data.Length);
        //                // compress the chunk
        //                MemoryStream compressedDataStream = new MemoryStream();
        //                if (compressionType == CompressionType.Deflate)
        //                    streamCompressed =
        //                    new DeflateStream(compressedDataStream, CompressionMode.Compress);
        //                else
        //                    streamCompressed = new GZipStream(compressedDataStream, CompressionMode.Compress);
        //                using (streamCompressed)
        //                {
        //                    // write the chunk in the compressed stream
        //                    await streamCompressed.WriteAsync(data, 0, data.Length);
        //                }
        //                // get the bytes for the compressed chunk
        //                byte[] compressedData = compressedDataStream.GetBuffer();
        //                // write out the chunk size
        //                size = BitConverter.GetBytes(chunkSize);
        //                await streamDestination.WriteAsync(size, 0, size.Length);
        //                // write out the compressed size
        //                size = BitConverter.GetBytes(compressedData.Length);
        //                await streamDestination.WriteAsync(size, 0, size.Length);
        //                // write out the compressed chunk
        //                await streamDestination.WriteAsync(compressedData, 0, compressedData.Length);
        //                // subtract the chunk size from the file size
        //                fileLength -= chunkSize;
        //                // if chunk is less than remaining file use
        //                // remaining file
        //                if (fileLength < chunkSize)
        //                    chunkSize = fileLength;
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// This function will decompress the chunked compressed file
        /// created by the CompressFile function.
        /// </summary>
        /// <param name="sourceFile">the compressed file</param>
        /// <param name="destinationFile">the destination file</param>
        /// <param name="compressionType">the type of compression to use</param>
        //public static async Task DecompressFileAsync(string sourceFile, string destinationFile, CompressionType compressionType)
        //{
        //    if (string.IsNullOrWhiteSpace(sourceFile))
        //        throw new ArgumentNullException(nameof(sourceFile));
        //    if (string.IsNullOrWhiteSpace(destinationFile))
        //        throw new ArgumentNullException(nameof(destinationFile));
        //    FileStream streamSource = null;
        //    FileStream streamDestination = null;
        //    Stream streamUncompressed = null;
        //    int bufferSize = 4096;
        //    using (streamSource = new FileStream(sourceFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize, useAsync: true))
        //    {
        //        using (streamDestination = new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
        //        {
        //            // read the fileLength size
        //            // read the chunk size
        //            byte[] size = new byte[sizeof(long)];
        //            await streamSource.ReadAsync(size, 0, size.Length);
        //            // convert the size back to a number
        //            long fileLength = BitConverter.ToInt64(size, 0);
        //            long chunkSize = 0;
        //            int storedSize = 0;
        //            long workingSet = Process.GetCurrentProcess().WorkingSet64;
        //            while (fileLength > 0)
        //            {
        //                // read the chunk size
        //                size = new byte[sizeof(long)];
        //                await streamSource.ReadAsync(size, 0, size.Length);
        //                // convert the size back to a number
        //                chunkSize = BitConverter.ToInt64(size, 0);
        //                if (chunkSize > fileLength ||
        //                chunkSize > workingSet)
        //                    throw new InvalidDataException();
        //                // read the compressed size
        //                size = new byte[sizeof(int)];
        //                await streamSource.ReadAsync(size, 0, size.Length);
        //                // convert the size back to a number
        //                storedSize = BitConverter.ToInt32(size, 0);
        //                if (storedSize > fileLength ||
        //                storedSize > workingSet)
        //                    throw new InvalidDataException();
        //                if (storedSize > chunkSize)
        //                    throw new InvalidDataException();
        //                byte[] uncompressedData = new byte[chunkSize];
        //                byte[] compressedData = new byte[storedSize];
        //                await streamSource.ReadAsync(compressedData, 0, compressedData.Length);
        //                // uncompress the chunk
        //                MemoryStream uncompressedDataStream = new MemoryStream(compressedData);
        //                if (compressionType == CompressionType.Deflate)
        //                    streamUncompressed = new DeflateStream(uncompressedDataStream, CompressionMode.Decompress);
        //                else
        //                    streamUncompressed = new GZipStream(uncompressedDataStream, CompressionMode.Decompress);
        //                using (streamUncompressed)
        //                {
        //                    // read the chunk in the compressed stream
        //                    await streamUncompressed.ReadAsync(uncompressedData, 0, uncompressedData.Length);
        //                }
        //                // write out the uncompressed chunk
        //                await streamDestination.WriteAsync(uncompressedData, 0, uncompressedData.Length);
        //                // subtract the chunk size from the file size
        //                fileLength -= chunkSize;
        //                // if chunk is less than remaining file use remaining file
        //                if (fileLength < chunkSize)
        //                    chunkSize = fileLength;
        //            }
        //        }
        //    }
        //}


        //public static async void TestCompressNewFileAsync()
        //{
        //    byte[] data = new byte[10000000];
        //    for (int i = 0; i < 10000000; i++)
        //        data[i] = (byte)i;
        //    using (FileStream fs = new FileStream(@"C:\NewNormalFile.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, useAsync: true))
        //    {
        //        await fs.WriteAsync(data, 0, data.Length);
        //    }
        //    await CompressFileAsync(@"C:\NewNormalFile.txt", @"C:\NewCompressedFile.txt", CompressionType.Deflate);
        //    await DecompressFileAsync(@"C:\NewCompressedFile.txt", @"C:\NewDecompressedFile.txt", CompressionType.Deflate);
        //    await CompressFileAsync(@"C:\NewNormalFile.txt", @"C:\NewGZCompressedFile.txt", CompressionType.GZip);
        //    await DecompressFileAsync(@"C:\NewGZCompressedFile.txt", @"C:\NewGZDecompressedFile.txt", CompressionType.GZip);
        //    //Normal file size == 10,000,000 bytes
        //    //GZipped file size == 84,362
        //    //Deflated file size == 42,145
        //    //Pre .NET 4.5 GZipped file size == 155,204
        //    //Pre .NET 4.5 Deflated file size == 155,168
        //    // 36 bytes are related to the GZip CRC
        //}






        //public static void CompressFile(string sourceFile, string destinationFile, CompressionType compressionType) : this ( sourceFile, destinationFile, CompressionType.GZip) { }





        private static Stream ZipType(CompressionType compressionType, MemoryStream compressedStream)
        {
            Stream streamCompressed;
            if (compressionType == CompressionType.Deflate)
                streamCompressed = new DeflateStream(compressedStream, CompressionMode.Compress);
            else
                streamCompressed = new GZipStream(compressedStream, CompressionMode.Compress);
            return streamCompressed;
        }


        public static void CompressFile(string sourceFile, string destinationFile)
        {
            CompressFile(sourceFile, destinationFile, CompressionType.GZip);
        }

        /// <summary>
        /// Compress the source file to the destination file.
        /// This is done in 1MB chunks to not overwhelm the memory usage.
        /// </summary>
        /// <param name="sourceFile">the uncompressed file</param>
        /// <param name="destinationFile">the compressed file</param>
        /// <param name="compressionType">the type of compression to use</param>
        /// 

        // for old (Pre–.NET 4.5 version of the CompressFile and DecompressFile methods)
        // ==========================================================================================================================
        public static void CompressFile(string sourceFile, string destinationFile, CompressionType compressionType)
        {
            if (sourceFile != null)
            {
                FileStream streamRead = null;
                FileStream streamWrite = null;
                Stream streamCompressed = null;
                using (streamRead = File.OpenRead(sourceFile))
                {
                    using (streamWrite = File.OpenWrite(destinationFile))
                    {                        
                        long fileLength = streamRead.Length;// ============= прочитать 1 MB кусок и сжать его                        
                        byte[] size = BitConverter.GetBytes(fileLength);// = записать размер длинны файла
                        streamWrite.Write(size, 0, size.Length);
                        long BlockSize = 1048576; // 1MB
                        while (fileLength > 0)
                        {
                            byte[] data = new byte[BlockSize];// =========== read the chunk
                            streamRead.Read(data, 0, data.Length);
                            MemoryStream compressedDataStream = new MemoryStream();// compress the chunk
                            streamCompressed = ZipType(compressionType, compressedDataStream);

                            using (streamCompressed)
                            {                               
                                streamCompressed.Write(data, 0, data.Length); // ========== write the chunk in the compressed stream
                            }                            
                            byte[] compressedData = compressedDataStream.GetBuffer();// === get the bytes for the compressed chunk                            
                            size = BitConverter.GetBytes(BlockSize);// ==================== write out the chunk size
                            streamWrite.Write(size, 0, size.Length);                            
                            size = BitConverter.GetBytes(compressedData.Length);// ======== write out the compressed size
                            streamWrite.Write(size, 0, size.Length);                            
                            streamWrite.Write(compressedData, 0, compressedData.Length);// = write out the compressed chunk                            
                            fileLength -= BlockSize;// ===================================== subtract the chunk size from the file size                            
                            if (fileLength < BlockSize) BlockSize = fileLength;// ========== if chunk is less than remaining file use remaining file
                        }
                    }
                }
            }
        }
        


        //public static void DecompressFile(string sourceFile)
        //{
        //    DecompressFile(sourceFile, sourceFile, CompressionType.GZip);
        //}

        public static void DecompressFile(string sourceFile, string destinationFile)
        {
            DecompressFile(sourceFile, destinationFile, CompressionType.GZip);
        }


        /// <summary>
        /// This function will decompress the chunked compressed file created by the CompressFile function.
        /// </summary>
        /// <param name="sourceFile">the compressed file</param>
        /// <param name="destinationFile">the destination file</param>
        /// <param name="compressionType">the type of compression to use</param>

        public static void DecompressFile(string sourceFile, string destinationFile, CompressionType compressionType)
        {
            FileStream streamSource = null;
            FileStream streamDestination = null;
            Stream streamUncompressed = null;
            using (streamSource = File.OpenRead(sourceFile))
            {
                using (streamDestination = File.OpenWrite(destinationFile))
                {
                    // read the fileLength size = читать размер файла Длина
                    // read the chunk size = читать размер куска
                    byte[] size = new byte[sizeof(long)];
                    streamSource.Read(size, 0, size.Length);
                    // convert the size back to a number = преобразовать размер обратно в число
                    long fileLength = BitConverter.ToInt64(size, 0);
                    long chunkSize = 0;
                    int storedSize = 0;
                    long workingSet = Process.GetCurrentProcess().WorkingSet64;
                    while (fileLength > 0)
                    {
                        // читать размер куска
                        size = new byte[sizeof(long)];
                        streamSource.Read(size, 0, size.Length);                        
                        chunkSize = BitConverter.ToInt64(size, 0);// = преобразовать размер обратно в число                     
                        size = new byte[sizeof(int)]; //============== читать размер сжатого
                        streamSource.Read(size, 0, size.Length);                       
                        storedSize = BitConverter.ToInt32(size, 0); // convert the size back to a number = преобразовать размер обратно в число
                        byte[] uncompressedData = new byte[chunkSize];
                        byte[] compressedData = new byte[storedSize];
                        streamSource.Read(compressedData, 0, compressedData.Length);                        
                        MemoryStream uncompressedDataStream = new MemoryStream(compressedData);// uncompress the chunk
                        streamUncompressed = ZipType(compressionType, uncompressedDataStream);

                        using (streamUncompressed)
                        {                            
                            streamUncompressed.Read(uncompressedData, 0, uncompressedData.Length);// read the chunk in the compressed stream
                        }
                        streamDestination.Write(uncompressedData, 0, uncompressedData.Length); // = write out the uncompressed chunk                        
                        fileLength -= chunkSize;//================================================= subtract the chunk size from the file size
                        // if chunk is less than remaining file use remaining file
                        if (fileLength < chunkSize)
                            chunkSize = fileLength;
                    }
                }
            }









        }


    }
}
