﻿using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using psluja.ObjectPools.Dummies;

namespace psluja.ObjectPools.Utils
{
    public static class Tester
    {
        static Tester()
        {
            int minWorker, minIoc;
            ThreadPool.GetMinThreads(out minWorker, out minIoc);
            ThreadPool.SetMinThreads(20, minIoc);

        }

        public static async Task PoolTester(IObjectPool<MyHeavyObject> myPool, int threads)
        {
            ActionBlock<IObjectPool<MyHeavyObject>> actionBlock = new ActionBlock<IObjectPool<MyHeavyObject>>(pool =>
            {
                using (Usage<MyHeavyObject> objUsage = pool.UseObjectSync())
                {
                    var obj = objUsage.Object;
                    obj.Begin();
                    obj.SomeFastMethod();
                    obj.SomeSlowMethod();
                    obj.End();
                }

            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = threads
            });

            for (int i = 0; i < 200; i++)
            {
                actionBlock.Post(myPool);
            }

            actionBlock.Complete();
            await actionBlock.Completion;
        }
    }
}
