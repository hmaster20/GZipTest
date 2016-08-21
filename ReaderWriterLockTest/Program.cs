// This example shows a ReaderWriterLock protecting a shared resource that is read concurrently and written exclusively by multiple threads.
// Этот пример показывает ReaderWriterLock, защищающий общий ресурс, который читает одновременно и пишет исключительно несколькими потоками.

using System;
using System.Threading;

public class Test
{
    // Объявление ReaderWriterLock на уровне класса делает его видимым для всех потоков.
    static ReaderWriterLock rwl = new ReaderWriterLock();

    // Для этого примера, совместно используемый ресурс защищен от ReaderWriterLock просто целое число.
    static int resource = 0;

    const int numThreads = 14;//26
    static bool running = true;
    static Random rnd = new Random();

    // Statistics.
    static int readerTimeouts = 0;
    static int writerTimeouts = 0;
    static int reads = 0;
    static int writes = 0;

    public static void Main(string[] args)
    {
        // Запустите серию потоков.Каждый поток случайным образом выполняет операции чтения и записи на общем ресурсе.
        Thread[] t = new Thread[numThreads];
        for (int i = 0; i < numThreads; i++)
        {
            t[i] = new Thread(new ThreadStart(ThreadProcess));
            t[i].Name = new String(Convert.ToChar(i + 65), 1);
            t[i].Start();
            // if (i > 10)
            Thread.Sleep(3000);//300
        }

        // Скажите потокам выключиться, а затем ждите, пока они все завершаться.
        running = false;
        for (int i = 0; i < numThreads; i++)
        {
            t[i].Join();
        }

        // Display statistics.
        Console.WriteLine("\r\n{0} reads, {1} writes, {2} reader time-outs, {3} writer time-outs.",
                            reads, writes, readerTimeouts, writerTimeouts);
        Console.WriteLine("Press ENTER to exit.");
        Console.ReadLine();
    }

    static void ThreadProcess()
    {
        // As long as a thread runs, it randomly selects various ways to read and write from the shared resource. 
        // Each of the methods demonstrates one or more features of ReaderWriterLock.
        // Пока идет поток, он случайным образом выбирает различные способы чтения и записи из совместно используемого ресурса.
        // Каждый из методов демонстрирует один или несколько особенностей ReaderWriterLock.
        while (running)
        {
            double action = rnd.NextDouble();
            if (action < .8)
                ReadFromResource(10);
            else if (action < .81)
                ReleaseRestore(50);
            else if (action < .90)
                UpgradeDowngrade(100);
            else
                WriteToResource(100);
        }
    }

    // Shows how to request and release a reader lock, and how to handle time-outs.
    // Показывает, как запросить и освободить блокировку ЧТЕНИЯ, и как обращаться с тайм-аутом.
    static void ReadFromResource(int timeOut)
    {
        try
        {
            rwl.AcquireReaderLock(timeOut);
            try
            {
                // It is safe for this thread to read from the shared resource.
                // Он безопасен для этого потока, чтобы читать из общего ресурса.
                Display("reads resource value " + resource);
                Interlocked.Increment(ref reads);
            }
            finally
            {
                // Ensure that the lock is released.
                // Убедитесь в том, что блокировка будет снята.
                rwl.ReleaseReaderLock();
            }
        }
        catch (ApplicationException)
        {
            // The reader lock request timed out.
            // Запрос блокировки считыватель тайм-аут.
            Interlocked.Increment(ref readerTimeouts);
        }
    }

    // Shows how to request and release the writer lock, and how to handle time-outs.
    // Показывает, как запросить и освободить блокировку записи и как обращаться с тайм-аутом.
    static void WriteToResource(int timeOut)
    {
        try
        {
            rwl.AcquireWriterLock(timeOut);
            try
            {
                // It is safe for this thread to read or write from the shared resource.
                // Он безопасен для этого нить для чтения или записи из совместно используемого ресурса.
                resource = rnd.Next(500);
                Display("writes resource value " + resource);
                Interlocked.Increment(ref writes);
            }
            finally
            {
                // Ensure that the lock is released.
                // Убедитесь в том, что блокировка будет снята.
                rwl.ReleaseWriterLock();
            }
        }
        catch (ApplicationException)
        {
            // The writer lock request timed out.
            // Запрос блокировки писатель тайм-аут.
            Interlocked.Increment(ref writerTimeouts);
        }
    }

    // Shows how to request a reader lock, upgrade the reader lock to the writer lock, and downgrade to a reader lock again.
    // Показывает, как запросить блокировку чтения, обновить блокировку чтения на блокировку записи, и снова вернуть к блокировке чтения.
    static void UpgradeDowngrade(int timeOut)
    {
        try
        {
            rwl.AcquireReaderLock(timeOut);
            try
            {
                // It is safe for this thread to read from the shared resource.
                // Он безопасен для этой темы, чтобы читать из общего ресурса.
                Display("reads resource value " + resource);
                Interlocked.Increment(ref reads);

                // If it is necessary to write to the resource, you must either release the reader lock and 
                // then request the writer lock, or upgrade the reader lock. Note that upgrading the reader lock
                // puts the thread in the write queue, behind any other threads that might be waiting for the writer lock.
                try
                {
                    LockCookie lc = rwl.UpgradeToWriterLock(timeOut);
                    try
                    {
                        // It is safe for this thread to read or write from the shared resource.
                        resource = rnd.Next(500);
                        Display("writes resource value " + resource);
                        Interlocked.Increment(ref writes);
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        rwl.DowngradeFromWriterLock(ref lc);
                    }
                }
                catch (ApplicationException)
                {
                    // The upgrade request timed out.
                    Interlocked.Increment(ref writerTimeouts);
                }

                // When the lock has been downgraded, it is still safe to read from the resource.
                // Когда замок был понижен, он по-прежнему безопасно прочитать из ресурса.
                Display("reads resource value " + resource);
                Interlocked.Increment(ref reads);
            }
            finally
            {
                // Ensure that the lock is released.
                // Убедитесь в том, что блокировка будет снята.
                rwl.ReleaseReaderLock();
            }
        }
        catch (ApplicationException)
        {
            // The reader lock request timed out.
            Interlocked.Increment(ref readerTimeouts);
        }
    }

    // Shows how to release all locks and later restore
    // the lock state. Shows how to use sequence numbers
    // to determine whether another thread has obtained
    // a writer lock since this thread last accessed the
    // resource.
    static void ReleaseRestore(int timeOut)
    {
        int lastWriter;

        try
        {
            rwl.AcquireReaderLock(timeOut);
            try
            {
                // It is safe for this thread to read from
                // the shared resource. Cache the value. (You
                // might do this if reading the resource is
                // an expensive operation.)
                int resourceValue = resource;
                Display("reads resource value " + resourceValue);
                Interlocked.Increment(ref reads);

                // Save the current writer sequence number.
                // Сохранить текущий порядковый номер писатель.
                lastWriter = rwl.WriterSeqNum;

                // Release the lock, and save a cookie so the lock can be restored later.
                LockCookie lc = rwl.ReleaseLock();

                // Wait for a random interval (up to a 
                // quarter of a second), and then restore
                // the previous state of the lock. Note that
                // there is no time-out on the Restore method.
                Thread.Sleep(rnd.Next(250));
                rwl.RestoreLock(ref lc);

                // Check whether other threads obtained the
                // writer lock in the interval. If not, then
                // the cached value of the resource is still
                // valid.
                if (rwl.AnyWritersSince(lastWriter))
                {
                    resourceValue = resource;
                    Interlocked.Increment(ref reads);
                    Display("resource has changed " + resourceValue);
                }
                else
                {
                    Display("resource has not changed " + resourceValue);
                }
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ReleaseReaderLock();
            }
        }
        catch (ApplicationException)
        {
            // The reader lock request timed out.
            Interlocked.Increment(ref readerTimeouts);
        }
    }

    // Helper method briefly displays the most recent thread action. Comment out calls to Display to get a better idea of throughput.
    // Вспомогательный метод кратко отображает последнее действие потока. 
    // Закомментируйте вызовы для отображения, чтобы получить более полное представление о пропускной способности.
    static void Display(string msg)
    {
        Console.Write("Thread {0} {1}.       \r", Thread.CurrentThread.Name, msg);
    }
}
