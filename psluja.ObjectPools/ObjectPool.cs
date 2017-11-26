using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace psluja.ObjectPools
{
    public abstract class ObjectPool<T> : IDisposable, IObjectPool<T> where T : class
    {
        protected readonly ConcurrentBag<T> _objects;
        protected readonly ConcurrentQueue<TaskCompletionSource<T>> _consumerQueue;

        protected ObjectPool(IEnumerable<T> objects)
        {
            _objects = new ConcurrentBag<T>(objects);
            _consumerQueue = new ConcurrentQueue<TaskCompletionSource<T>>();
        }

        public abstract Task<T> GetObject(CancellationToken token);
        public abstract void PutObject(T item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects?.GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _objects?.GetEnumerator();
        }

        public virtual void Dispose()
        {
            TaskCompletionSource<T> consumerTaskCompletionSource;
            while (_consumerQueue.TryDequeue(out consumerTaskCompletionSource))
            {
                consumerTaskCompletionSource.TrySetResult(null);
            }

            T obj;
            while (_objects.TryTake(out obj))
            {
                (obj as IDisposable)?.Dispose();
            }

        }
    }
}