using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace psluja.ObjectPools.Dummies
{
    public class MyHeavyObject:IDisposable
    {
        private readonly int _id;
        public MyHeavyObject(int id)
        {
            _id = id;
            Thread.Sleep(200);
            Console.WriteLine($"MyHeavyObject {_id} at: " + Thread.CurrentThread.ManagedThreadId);
        }

        public void SomeFastMethod()
        {
            Console.WriteLine($"SomeFastMethod {_id} at: " + Thread.CurrentThread.ManagedThreadId);
        }
        public void SomeSlowMethod()
        {
            Thread.Sleep(50);
            Console.WriteLine($"SomeSlowMethod {_id} at: " + Thread.CurrentThread.ManagedThreadId);
        }

        public void Dispose()
        {
            Console.WriteLine($"disposed {_id} at: "+Thread.CurrentThread.ManagedThreadId);
        }
    }
}
