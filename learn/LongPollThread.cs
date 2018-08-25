using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace learn
{
    class HmsThread 
    {
        private Thread thread;
        protected volatile bool bStop = false;
        private string name;

        public HmsThread(string name)
        {
            this.name = name;
        }

        public bool IsStop()
        {
            return bStop;
        }

        public virtual void Process()
        {
            Console.WriteLine("HmsTrhead Process...");
            
        }

        public void Start()
        {
            bStop = false;
            thread = new Thread(Process);
            thread.Name = name;
            thread.Start();
        }

        public void Stop()
        {
            if (thread == null)
            {
                return;
            }
            bStop = true;
            thread.Join();
            thread = null;
        }


    }


    class ConsumerThread : HmsThread
    {
        public ConsumerThread(string name) : base(name)
        {
        }

        public override void Process()
        {
            Console.WriteLine("ConsumerThread Process...");
            while (!bStop)
            {
                bool bRet = HmsManager.INSTANCE.ConsumerQueue();
                if (!bRet)
                {
                    bStop = true;
                }
            }
            Console.WriteLine("ConsumerThread exit");
        }
    }

    class LongPollThread :  HmsThread
    {
        public LongPollThread(string name):base(name)
        {
        }

        public override void Process()
        {
            Console.WriteLine("LongPollThread Process...");
            while (!bStop)
            {
                bool ret  = HmsManager.INSTANCE.ListenLongPoll();
                if (!ret)
                {
                    bStop = true;
                }
            }

            Console.WriteLine("LongPollThread exit");
        }

    }
}
