using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace psluja.ObjectPools
{
    public class DynamicObjectPool<T> : ObjectPool<T> where T : class
    {
        private readonly Func<T> _objectGenerator;
        private readonly int _poolSize;
        private readonly int _poolMaxSize;

        private readonly ConcurrentQueue<TaskCompletionSource<T>> _consumerQueue;

        private readonly object _putLocker = new object();
        private readonly object _getLocker = new object();

        private int _objectsCreatedCounter;
        private int _objectsDisposedCounter;
        private int _objectsMaxCounter;

        public int ObjectsCreatedCounter => _objectsCreatedCounter;
        public int ObjectsDisposedCounter => _objectsDisposedCounter;
        public int ObjectsMaxCounter => _objectsMaxCounter;

        public DynamicObjectPool(Func<T> objectFactory, int poolSize, int poolMaxSize)
            :base(new List<T>())
        {
            _consumerQueue = new ConcurrentQueue<TaskCompletionSource<T>>();
            _objectGenerator = objectFactory;
            _poolSize = poolSize;
            _poolMaxSize = poolMaxSize;
        }

        public override Task<T> GetObject(CancellationToken token)
        {
            lock (_getLocker)
            {
                T item;
                if (_objects.TryTake(out item))
                {
                    return Task.FromResult(item);
                }

                var ts = new TaskCompletionSource<T>();

                token.Register(tts =>
                {
                    ((TaskCompletionSource<T>)tts).TrySetResult(null);
                }, ts);

                _consumerQueue.Enqueue(ts);

                if (Math.Max(_objectsMaxCounter, _objectsCreatedCounter) < _poolSize)
                {
                    PutObject(_objectGenerator());
                    Interlocked.Increment(ref _objectsCreatedCounter);
                }


                return ts.Task;
            }
        }

        public override void PutObject(T item)
        {
            lock (_putLocker)
            {
                TaskCompletionSource<T> consumerTaskCompletionSource;
                while (_consumerQueue.TryDequeue(out consumerTaskCompletionSource) &&
                       consumerTaskCompletionSource?.Task?.IsCompleted == false)
                {
                    consumerTaskCompletionSource.TrySetResult(item);
                    return;
                }

                // if pool is full then dispose and do not store this object
                if (_objects.Count >= _poolMaxSize)
                {
                    (item as IDisposable)?.Dispose();

                    Interlocked.Increment(ref _objectsDisposedCounter);
                    return;
                }

                if (!_objects.Contains(item))
                {
                    _objects.Add(item);
                    Interlocked.Exchange(ref _objectsMaxCounter, Math.Max(_objects.Count, _objectsMaxCounter));
                }
            }
        }

    }
}