using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace psluja.ObjectPools
{
    public interface IObjectPool<T> : IEnumerable<T> where T : class
    {
        Task<T> GetObject(CancellationToken token);
        void PutObject(T item);
    }
}