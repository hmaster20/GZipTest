using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;

namespace GZipTest
{
    class no_gzip2_fs
    {
        //static void compress()
        //{
        //    var saveFile = new SaveFileDialog();
        //    saveFile.Title = "Сохранение архива";
        //    saveFile.FileName = @"c:\outTest";
        //    saveFile.Filter = "Файл ZIP|*.zip";

        //    if (saveFile.ShowDialog() == DialogResult.OK)
        //    {
        //        outFileName = saveFile.FileName;
        //    }

        //    using (FileStream fileOpen = new FileStream(@"c:\test.txt", FileMode.Open, FileAccess.Read))
        //    {
        //        using (FileStream outFile = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
        //        {
        //            using (GZipStream fileGZip = new GZipStream(outFile, CompressionMode.Compress))
        //            {
        //                fileOpen.CopyTo(fileGZip);
        //                fileGZip.Close();
        //            }
        //        }
        //    }
        //}

        //Создаю zip-архив используя GZipStream.
        //Архив создаётся и открывается великолепно, вот только внутри архива файлы теряют свои расширения.
        //В итоге создаётся нормальный читаемый архив, но файл внутри без расширения (т.е. "test" вместо "test.txt")
        //Помогите пожалуйста советом, как сделать так, что бы расширения тоже сохранялись.

        //GZipStream не предназначен для создания zip файлов.Этот класс реализует формат gzip, но не zip.В него нельзя добавлять файлы или папки, он не хранит информацию о файлах.Он просто сжимает поток данных, которые вы в него подаете.
        //Для создания zip архивов можно использовать класс ZipFile (FW 4.5 и выше).
    }
}
