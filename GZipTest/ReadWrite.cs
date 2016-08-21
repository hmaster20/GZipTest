using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public static class ReadWrite
    {
        public static void TestMain()
        {
            DateTime ReadStart = DateTime.Now;
            const string Testfile = "test3.pdf";
            byte[] dataArray = new byte[0];

            using (FileStream fileRead = new FileStream(Testfile, FileMode.Open))
            {
                dataArray = new byte[fileRead.Length];          // создаем массив размером файла
                for (int i = 0; i < dataArray.Length; i++)      // проходим по массиву
                {
                    dataArray[i] = (byte)fileRead.ReadByte();   // и заполняем его
                }
                fileRead.Close();                               // закрываем поток
                DateTime ReadStop = DateTime.Now;               // фиксируем время
                Console.WriteLine("Файл " + Testfile + " прочитан и записан в массив: " + dataArray.ToString() + ". Время чтения: " +
                 Math.Round(((ReadStop - ReadStart).TotalSeconds), 3, MidpointRounding.AwayFromZero) + " секунд."); // округление до 3 символов от запятой, к тому числу, которое дальше от нуля
            }

            DateTime WriteStart = DateTime.Now;
            const string TestWfile = "test_nes.txt";
            using (FileStream fileStream = new FileStream(TestWfile, FileMode.Create))
            {
                if (dataArray.Length > 1)
                {
                    for (int i = 0;
                        i < dataArray.Length; i++)
                    {
                        fileStream.WriteByte(dataArray[i]);
                    }
                }
                fileStream.Close();
                DateTime WriteStop = DateTime.Now;
                Console.WriteLine("Файл " + TestWfile + " записан на диск. Время чтения: " +
                 Math.Round(((WriteStop - WriteStart).TotalSeconds), 3, MidpointRounding.AwayFromZero) + " секунд.");
            }

            File.Delete(TestWfile);
        }
    }
}
