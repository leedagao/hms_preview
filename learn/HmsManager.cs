using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;
namespace learn
{
    class HmsManager
    {
        public static HmsManager INSTANCE = new HmsManager();
        private Timer t;

        //private ConcurrentQueue<JToken> queue = new ConcurrentQueue<JToken>();
        private const int MAX_MSG = 50;
        private Queue<JToken> queue = new Queue<JToken>();
        private object myLock = new object();
        private Semaphore semaphore;

        volatile bool needLogin = true;

        HmsThread consumerThread = new ConsumerThread("ConsumerThread");
        HmsThread longPollThread = new LongPollThread("LongPollThread");

        private HmsManager()
        {
            //Console.WriteLine("Manger: "+Thread.CurrentThread.ManagedThreadId);
            //Console.WriteLine("Create HmsManager");
        }

        public void Start()
        {
            Console.WriteLine("HmsManager Start...");
            StartTimer();
        }

        public bool ConsumerQueue()
        {
            if (needLogin)
            {
                return false;
            }

            semaphore.WaitOne();

            JToken obj = null;
            lock (myLock)
            {
                queue.TryDequeue(out obj);
            }

            if (obj != null)
            {
                Console.WriteLine(Thread.CurrentThread.Name + $": Deal one message, queue.zie = {queue.Count}");
                return DealLongPollMsg(obj);
            }

            Console.WriteLine($"queue empty, but has notified! {needLogin}");
            return true;
        }

        private bool DealLongPollMsg(JToken obj)
        {

            // RegetAllData
            // 
            bool bRet = GetAllData();
            if (!bRet)
            {
                needLogin = true;
                return false;
            }

            return true;
        }

        public void Init()
        {
            Console.WriteLine("Beg Stop all Threads");
            needLogin = true;
            semaphore?.Release();
            longPollThread?.Stop();
            consumerThread?.Stop();
            semaphore?.Dispose();
            semaphore = new Semaphore(0, MAX_MSG);
            queue.Clear();
            Console.WriteLine("End Stop all Threads");
        }

        public void PrepareData()
        {
            Init();

            // 1. login
            bool bRet = Login();
            if (!bRet)
            {
                return;
            }

            // 2. subscription
            bRet = SubScription();
            if (!bRet)
            {
                return;
            }

            // 3.
            longPollThread.Start();

            // 4. getAllDAta
            bRet = GetAllData();
            if (!bRet)
            {
                needLogin = true;
                return;
            }

            // if longpoll fail, needLogin may be changed
            needLogin = false;

            // When AllDadaDone, start Thread
            consumerThread.Start();
        }

        public bool ListenLongPoll()
        {
            if (GetNotification(out JToken obj))
            {
                //
                lock (myLock)
                {
                    // more the max, dequeue first
                    /*
                    if (queue.Count == MAX_MSG)
                    {
                        queue.Dequeue();
                    }
                    */

                    queue.Enqueue(obj);
                    Console.WriteLine(Thread.CurrentThread.Name + $":add one message, queue.zie = {queue.Count}");
                }

                semaphore.Release();

                return true;

            }

            // exit thread
            needLogin = true;

            Console.WriteLine($"GetNotfi return false, needLogin = {needLogin}");
            // must signal, consumer may be wait now
            semaphore.Release();

            return false;
        }

        private bool GetAllData()
        {
            Console.WriteLine(Thread.CurrentThread.Name + ":  GetAllData");
            Thread.Sleep(3000);
            return true;
        }

        private bool SubScription()
        {
            Console.WriteLine(Thread.CurrentThread.Name + ":  SubScription");
            return true;
        }

        private bool Login()
        {
            Console.WriteLine(Thread.CurrentThread.Name + ":  Login");
            return true;
        }

        public bool GetNotification(out JToken jToken)
        {
            Console.WriteLine(Thread.CurrentThread.Name + ": GetNotification");
            bool ret = HttpUtil.SendHttp(out jToken);
            return ret;

        }


        public void StartTimer()
        {
            t = new Timer();
            t.Interval = 1;
            t.Elapsed += CheckStatus;
            t.AutoReset = false;
            t.Start();

        }

        private void CheckStatus(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(Thread.CurrentThread.Name + $":                 CheckStatus, {needLogin}");

            t.Stop();

            if (needLogin || longPollThread.IsStop())
            {
                PrepareData();
            }
            
            t.Interval = 5000;
            // for debug
           // needLogin = true; 
            t.Start();
        }
    }
}
