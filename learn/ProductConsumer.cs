using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace learn
{
    class ProductConsumer
    {

        private Queue<string> queue = new Queue<string>();

        private Semaphore semaphore = new Semaphore(0, 3);

        private object _lock = new object();

        public bool StopFlag = false;

        public ProductConsumer()
        {
            StartProducer(1);
            Thread.Sleep(2000);
            StartConsumer(1);
        }

        private void StartProducer(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Thread t = new Thread(new ThreadStart(Producer));
                t.Name = "Producer_" + (i + 1);
                t.Start();
            }
        }

        private void StartConsumer(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Consumer));
                t.Name = "Consumer_" + (i + 1);
                t.IsBackground = true;
                t.Start(i + 1);
            }
        }


        // 生产者
        private void Producer()
        {
            int i = 0;
            while (!StopFlag)
            {
                lock (_lock)	// 任务队列为临界资源，需要锁
                {
                    queue.Enqueue("Date: " + (i++));
                    Console.WriteLine("Producer: ");
                }
                semaphore.Release(2);   // 每添加一个任务，信号量加1

                //Thread.Sleep(1000);
                break;
            }
        }

        // 消费者
        private void Consumer(object data)
        {
            int Index = (int)data;
            string taskStr = null;

            while (true)
            {
                semaphore.WaitOne();
                lock (_lock)
                {
                    taskStr = queue.Dequeue();
                }

                Console.WriteLine("consume: " + taskStr);
                Random r = new Random();
                Thread.Sleep(r.Next(1) * 1000);     // 休眠随机事件，模拟实际任务的每个任务的不同时常
                                                    // 此处应为任务处理
            }
        }


    }





}
