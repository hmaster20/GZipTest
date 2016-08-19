using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    class gzip53
    {
        public static void Compress(String fileSource, String fileDestination, int buffsize)
        {
            using (var fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
            {
                using (var fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
                {
                    using (var gzipStream = new GZipStream(fsOutput, CompressionMode.Compress))
                    {
                        var buffer = new Byte[buffsize];
                        int h;
                        while ((h = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            gzipStream.Write(buffer, 0, h);
                        }
                    }
                }
            }
        }

        public static void Decompress(String fileSource, String fileDestination, int buffsize)
        {
            using (var fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
            {
                using (var fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
                {
                    using (var gzipStream = new GZipStream(fsInput, CompressionMode.Decompress))
                    {
                        var buffer = new Byte[buffsize];
                        int h;
                        while ((h = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOutput.Write(buffer, 0, h);
                        }
                    }
                }
            }
        }



        //По моему это почти один в один, то что я выложил в первом сообщении.Но в любом случае спасибо!
        //Для меня ещё оказалось и это полезным.Ну не доводилось мне IntelleSens'ом посмотреть весь список методов.

        //Path.ChangeExtension("gz");


        //А, всё понял свою ошибку.Я не освобождал gzipStream после упаковки и распаковки! 
        //Я читал где-то, что пока не освободишь, правильно не сожмётся(примерно так). Вот не заметил этого.Ещё раз спасибо. 
        //Теперь всё распаковывается нормально. Видимо проблема была с txt из-за маленького размера файла. 
        //То есть при распаковке не скидывалось на HDD, то что распаковалось, так как я не освобождал поток, короче не вызывался метод Flush();



    }
}
