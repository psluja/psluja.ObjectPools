using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace psluja.ObjectPools
{
    public class FixedObjectPool<T> : ObjectPool<T> where T : class
    {
        private readonly ConcurrentQueue<TaskCompletionSource<T>> _consumerQueue;

        private readonly object _putLocker = new object();
        private readonly object _getLocker = new object();

        public FixedObjectPool(IEnumerable<T> objects):base(objects)
        {
            _consumerQueue = new ConcurrentQueue<TaskCompletionSource<T>>();
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
                    ((TaskCompletionSource<T>) tts).TrySetCanceled();
                },ts);

                _consumerQueue.Enqueue(ts);
                
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

                if(!_objects.Contains(item))
                    _objects.Add(item);
            }
        }
    }
}
