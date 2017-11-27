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

        public static T GetObjectSync<T>(this IObjectPool<T> pool, CancellationToken token) where T : class
        {
            var objTask = pool.GetObject(token);
            objTask.Wait();
            return objTask.Result;
        }

        public static T GetObjectSync<T>(this IObjectPool<T> pool) where T : class
        {
            var objTask = pool.GetObject();
            objTask.Wait();
            return objTask.Result;
        }

        public static async Task<Usage<T>> UseObject<T>(this IObjectPool<T> pool) where T : class
        {
            var obj = await pool.GetObject();
            return new Usage<T>(pool, obj);
        }
        public static Usage<T> UseObjectSync<T>(this IObjectPool<T> pool) where T : class
        {
            var objTask = pool.GetObject();
            objTask.Wait();
            return new Usage<T>(pool, objTask.Result);
        }

        public static async Task<Usage<T>> UseObject<T>(this IObjectPool<T> pool, CancellationToken token) where T : class
        {
            var obj = await pool.GetObject(token);
            return new Usage<T>(pool, obj);
        }
        public static Usage<T> UseObjectSync<T>(this IObjectPool<T> pool, CancellationToken token) where T : class
        {
            var objTask = pool.GetObject(token);
            objTask.Wait();
            return new Usage<T>(pool, objTask.Result);
        }
    }
}
