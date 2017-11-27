using System;

namespace psluja.ObjectPools
{
    public class Usage<T> :IDisposable where T : class
    {
        private readonly IObjectPool<T> _pool;

        public T Object { get; }

        internal Usage(IObjectPool<T> pool, T o)
        {
            _pool = pool;
            Object = o;
        }

        public void Dispose()
        {
            _pool.PutObject(Object);
        }
    }
}