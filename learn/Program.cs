using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace learn
{
    class Program
    {

        public static void sendHttp()
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://127.0.0.1:3000");
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string responseBody = reader.ReadToEnd();

                        Console.WriteLine(responseBody);
                    }
                }
            }

        }

        public  static void deal(object sender, ElapsedEventArgs e) {
            Console.WriteLine(DateTime.Now +": Timer :" + Thread.CurrentThread.ManagedThreadId);
            t.Stop();
            //t.Dispose();
            sendHttp();
            //Console.WriteLine("now exit...");
            t.Interval = 3000;
            t.Start();

        }

        public static void ThreadTimerRun(Object obj)
        {
            Console.WriteLine("ThreadTimer :" + Thread.CurrentThread.ManagedThreadId);
            sendHttp();

        }

        static void testTimer2()
        {
            System.Threading.Timer t = new System.Threading.Timer(ThreadTimerRun, null, 0, 1000);
        }

        static  Timer t;

        static void testTimer()
        {
            t = new Timer();
            t.Interval = 1;
            t.Elapsed += deal;
            t.AutoReset = false;
            t.Start();

        }




        static void testConcurrent()
        {
            ConcurrentQueue<JToken> queue = new ConcurrentQueue<JToken>();

            JToken obj = @"{'name':'lidg'}";
           
            queue.Enqueue(obj);
            Console.WriteLine(queue.Count);
            //queue.Clear();

            queue.TryDequeue(out JToken retObj);

            Console.WriteLine(retObj == null);

            Console.WriteLine(queue.Count);
        }


        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="args"></param>

        class A
        {
            public event EventHandler CountDozen;
            public void DoCount()
            {
                for (int i = 0; i < 100; i++)
                {
                    if (i%12 ==0 && CountDozen != null)  // 有订阅的时候，自动不为Null；否则为null
                    {
                        CountDozen(this, null);
                    }
                }
            }
        }

        class B
        {
            public int _Count = 0;
            public B(A a)
            {
                a.CountDozen += deal;
            }

            void deal(Object obj, EventArgs arg)
            {
                _Count++;
            }
        }

        static public void test()
        {
            while(true)
            {
                Console.WriteLine("ok");
                Thread.Sleep(1000);

            }
        }

       static public void myDo(Object obj)
        {
            Console.WriteLine("myDO");
            Semaphore sem = (Semaphore)obj;
            Console.WriteLine("before waiton");

            sem.WaitOne();
            Console.WriteLine("after waiton");
        }

        static void testSem()
        {
            Semaphore sem = new Semaphore(0, 10, "ABC");

            //sem.WaitOne();

            sem.Release(10);
            
            new Thread(new ParameterizedThreadStart(myDo)).Start(sem);

            Thread.Sleep(3000);
            sem.Close();
            sem.Dispose();

            sem = Semaphore.OpenExisting("ABC");  // fail
            sem.WaitOne();

            Console.WriteLine("after close , i'll here");

          

            //sem.Release();

        }


        static void Main(string[] args)
        {

            HmsManager.INSTANCE.Start();
            while (true)
            {
                //Console.WriteLine("Main:" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(1000);

            }
            return;


            testSem();
            while (true)
            {
                Console.WriteLine("Main:" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(1000);

            }

            return;

            HmsManager.INSTANCE.Start();
            while (true)
            {
                //Console.WriteLine("Main:" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(1000);

            }
            return;
           

            ConsumerThread tx = new ConsumerThread("consumer");
            tx.Start();


            tx.Stop();
            tx.Stop();
            Console.Read();
            return;
            Thread t = new Thread(test);
            t.IsBackground = false;
            t.Start();
            return;

/*
            A a = new A();
            B b = new B(a);
            a.DoCount();
            Console.WriteLine(b._Count);

            return;
*/
            Console.WriteLine("Main:" + Thread.CurrentThread.ManagedThreadId);

            new ProductConsumer();
            //HmsManager.INSTANCE.Start();
            // testConcurrent();

            // testTimer();
            //testTimer2();

            //HttpUtil.SendHttp();
            //HttpUtil.SendHttpAsync();

            while (true)
            {
                //Console.WriteLine("Main:" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(1000);

            }
        }
    }
}
