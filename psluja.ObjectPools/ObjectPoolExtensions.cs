using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace psluja.ObjectPools
{
    public static class ObjectPoolExtensions
    {
        public static Task<T> GetObject<T>(this IObjectPool<T> pool) where T : class
        {
            return pool.GetObject(CancellationToken.None);
        }
    }
}
