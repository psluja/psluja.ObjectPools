using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace psluja.ObjectPools.Dummies
{
    public class MyHeavyObject:IDisposable
    {
        private readonly int _id;
        private int _currentThread;

        public MyHeavyObject(int id)
        {
            _id = id;
            _currentThread = -1;
            Thread.Sleep(200);
            Console.WriteLine($"MyHeavyObject {_id} at: " + Thread.CurrentThread.ManagedThreadId);
        }

        public void Begin()
        {
            if (_currentThread == -1)
            {
                _currentThread = Thread.CurrentThread.ManagedThreadId;
            }
            else
            {
                throw new InvalidOperationException("Begin should be invoked after End or just after object creation");
            }
        }
        public void End()
        {
            if (_currentThread != -1)
            {
                _currentThread = -1;
            }
            else
            {
                throw new InvalidOperationException("End should be invoked after Begin");
            }
        }

        public void SomeFastMethod()
        {
            if (_currentThread != Thread.CurrentThread.ManagedThreadId)
            {
                throw new InvalidOperationException("Method invoked from different thread that it should be. Invoke Begin first.");
            }

            Console.WriteLine($"SomeFastMethod {_id} at: " + Thread.CurrentThread.ManagedThreadId);
        }
        public void SomeSlowMethod()
        {
            if (_currentThread != Thread.CurrentThread.ManagedThreadId)
            {
                throw new InvalidOperationException("Method invoked from different thread that it should be. Invoke Begin first.");
            }

            Thread.Sleep(50);
            Console.WriteLine($"SomeSlowMethod {_id} at: " + Thread.CurrentThread.ManagedThreadId);
        }

        public void Dispose()
        {
            Console.WriteLine($"disposed {_id} at: "+Thread.CurrentThread.ManagedThreadId);
        }
    }
}
