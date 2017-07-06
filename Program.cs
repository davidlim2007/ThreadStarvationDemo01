using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadStarvationDemo01
{
    // TODO: 
    // - Comments
    // - Refine code

    // Code adapted from:
    // https://stackoverflow.com/questions/8451105/how-to-simulate-c-sharp-thread-starvation

    class Program
    {
        static void Main(string[] args)
        {
            StartThread01();
            StartThread02();

            // Put the main thread to sleep
            // for 10 seconds, letting the
            // two threads run.
            Thread.Sleep(10000);
            ToContinue = false;

            WaitThread01ToEnd();
            WaitThread02ToEnd();
        }

        static void StartThread01()
        {
            m_thread_01 = new Thread(new ThreadStart(ThreadMethod));
            m_thread_01.Priority = ThreadPriority.Highest;
            m_thread_01.Start();
        }

        static void StartThread02()
        {
            m_thread_02 = new Thread(new ThreadStart(ThreadMethod));
            m_thread_02.Priority = ThreadPriority.Lowest;
            m_thread_02.Start();
        }

        static void ThreadMethod()
        {
            int count = 0;

            try
            {
                AcquireMutex();

                while (ToContinue)
                {
                    count++;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ReleaseMutex();
                Console.WriteLine("Thread {0:D} has ended with a count of {1:D}.", 
                    Thread.CurrentThread.ManagedThreadId,
                    count);
            }
        }

        static void WaitThread01ToEnd()
        {
            m_thread_01.Join();
        }

        static void WaitThread02ToEnd()
        {
            m_thread_02.Join();
        }

        static void AcquireMutex()
        {
            m_mutex.WaitOne();
        }

        static void ReleaseMutex()
        {
            m_mutex.ReleaseMutex();
        }

        static bool ToContinue
        {
            get
            {
                return m_bToContinue;
            }
            set
            {
                m_bToContinue = value;
            }
        }

        private static Thread m_thread_01 = null;
        private static Thread m_thread_02 = null;
        private static bool m_bToContinue = true;
        private static Mutex m_mutex = new Mutex();
    }
}
