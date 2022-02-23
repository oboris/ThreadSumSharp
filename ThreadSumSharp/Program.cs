using System;
using System.Threading;

namespace ThreadSumSharp
{
    class Program
    {
        private static readonly int dim = 10000000;
        private static readonly int threadNum = 2;

        private readonly Thread[] thread = new Thread[threadNum];

        static void Main(string[] args)
        {
            Program main = new Program();
            main.InitArr();
            Console.WriteLine(main.PartSum(0, dim));

            Console.WriteLine(main.ParallelSum());
            Console.ReadKey();
        }

        private int threadCount = 0;

        private long ParallelSum()
        {
            thread[0] = new Thread(StarterThread);
            thread[0].Start(new Bound(0, dim / 2));
            thread[1] = new Thread(StarterThread);
            thread[1].Start(new Bound(dim / 2, dim));

            lock (lockerForCount)
            {
                while (threadCount < threadNum)
                {
                    Monitor.Wait(lockerForCount);
                }
            }
            return sum;
        }

        private readonly int[] arr = new int[dim];

        private void InitArr()
        {
            for (int i = 0; i < dim; i++)
            {
                arr[i] = i;
            }
        }

        class Bound
        {
            public Bound(int startIndex, int finishIndex)
            {
                StartIndex = startIndex;
                FinishIndex = finishIndex;
            }

            public int StartIndex { get; set; }
            public int FinishIndex { get; set; }
        }

        private readonly object lockerForSum = new object();
        private void StarterThread(object param)
        {
            if (param is Bound)
            {
                long sum = PartSum((param as Bound).StartIndex, (param as Bound).FinishIndex);

                lock (lockerForSum)
                {
                    CollectSum(sum);
                }
                IncThreadCount();
            }
        }

        private readonly object lockerForCount = new object();
        private void IncThreadCount()
        {
            lock (lockerForCount)
            {
                threadCount++;
                Monitor.Pulse(lockerForCount);
            }
        }

        private long sum = 0;
        public void CollectSum(long sum)
        {
            this.sum += sum;
        }

        public long PartSum(int startIndex, int finishIndex)
        {
            long sum = 0;
            for (int i = startIndex; i < finishIndex; i++)
            {
                sum += arr[i];
            }
            return sum;
        }
    }
}
