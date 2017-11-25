using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace psluja.ObjectPools
{
    public abstract class ObjectPool<T> : IObjectPool<T> where T : class
    {
        protected readonly ConcurrentBag<T> _objects;

        protected ObjectPool(IEnumerable<T> objects)
        {
            _objects = new ConcurrentBag<T>(objects);
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

    }
}