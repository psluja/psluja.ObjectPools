
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using psluja.ObjectPools.Dummies;
using psluja.ObjectPools.Utils;

namespace psluja.ObjectPools
{
    [TestClass]
    public class FixedObjectPoolTests
    {
        [TestMethod]
        public async Task FixedObjectPool_3_threads_20()
        {
            var myPool = new FixedObjectPool<MyHeavyObject>(new[] { new MyHeavyObject(1), new MyHeavyObject(2), new MyHeavyObject(3) });
            await Tester.PoolTester(myPool, 20);
        }

    }
}
