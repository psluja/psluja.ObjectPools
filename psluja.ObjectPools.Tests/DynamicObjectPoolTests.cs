using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using psluja.ObjectPools.Dummies;
using psluja.ObjectPools.Utils;

namespace psluja.ObjectPools
{
    [TestClass]
    public class DynamicObjectPoolTests
    {
        [TestMethod]
        public async Task DynamicObjectPool_3_to_3_threads_1()
        {
            var random = new Random(1);
            var myPool = new DynamicObjectPool<MyHeavyObject>(() => new MyHeavyObject(random.Next()), 3,3);
            await Tester.PoolTester(myPool, 1);

            Assert.AreEqual(0,myPool.ObjectsDisposedCounter, 0, "ObjectsDisposedCounter");
            Assert.AreEqual(1,myPool.ObjectsCreatedCounter, 1, "ObjectsCreatedCounter");
            Assert.AreEqual(1,myPool.ObjectsMaxCounter, 1, "ObjectsMaxCounter");

            myPool.Dispose();
        }

        [TestMethod]
        public async Task DynamicObjectPool_3_to_3_threads_20()
        {
            var random = new Random(1);
            var myPool = new DynamicObjectPool<MyHeavyObject>(() => new MyHeavyObject(random.Next()), 3,3);
            await Tester.PoolTester(myPool, 20);

            Assert.AreEqual(0,myPool.ObjectsDisposedCounter, "ObjectsDisposedCounter");
            Assert.AreEqual(3,myPool.ObjectsCreatedCounter, "ObjectsCreatedCounter");
            Assert.AreEqual(3, myPool.ObjectsMaxCounter, "ObjectsMaxCounter");

            myPool.Dispose();
        }

        [TestMethod]
        public async Task DynamicObjectPool_duplicates_3_to_3()
        {
            var random = new Random(1);
            var myPool = new DynamicObjectPool<MyHeavyObject>(() => new MyHeavyObject(random.Next()), 3,3);

            var obj1 = await myPool.GetObject(CancellationToken.None);

            var obj2 = new MyHeavyObject(1);

            var obj3Task =  myPool.GetObject(CancellationToken.None);

            myPool.PutObject(obj1);

            var obj3 = await obj3Task;

            myPool.PutObject(obj2);
            myPool.PutObject(obj2);
            myPool.PutObject(obj2);
            myPool.PutObject(obj2);
            myPool.PutObject(obj2);
            myPool.PutObject(obj3);

            Assert.AreEqual(0,myPool.ObjectsDisposedCounter, "ObjectsDisposedCounter");
            Assert.AreEqual(2,myPool.ObjectsCreatedCounter, "ObjectsCreatedCounter");
            Assert.AreEqual(3,myPool.ObjectsMaxCounter, "ObjectsMaxCounter");

            myPool.Dispose();
        }

        [TestMethod]
        public async Task DynamicObjectPool_external_creation_3_to_3_threads_20()
        {
            var random = new Random(1);
            var myPool = new DynamicObjectPool<MyHeavyObject>(() => new MyHeavyObject(random.Next()), 3, 3);


            for (int i = 0; i < 10; i++)
            {
                myPool.PutObject(new MyHeavyObject(i));
            }

            await Tester.PoolTester(myPool, 20);

            Assert.AreEqual(7,myPool.ObjectsDisposedCounter, "ObjectsDisposedCounter");
            Assert.AreEqual(0,myPool.ObjectsCreatedCounter, "ObjectsCreatedCounter");
            Assert.AreEqual(3,myPool.ObjectsMaxCounter, "ObjectsMaxCounter");

            myPool.Dispose();
        }

        [TestMethod]
        public async Task DynamicObjectPool_external_creation_3_to_10_threads_20()
        {
            var random = new Random(1);
            var myPool = new DynamicObjectPool<MyHeavyObject>(() => new MyHeavyObject(random.Next()), 3, 10);

            for (int i = 0; i < 10; i++)
            {
                myPool.PutObject(new MyHeavyObject(i));
            }

            await Tester.PoolTester(myPool, 20);

            Assert.AreEqual(0, myPool.ObjectsDisposedCounter, "ObjectsDisposedCounter");
            Assert.AreEqual(0, myPool.ObjectsCreatedCounter, "ObjectsCreatedCounter");
            Assert.AreEqual(10, myPool.ObjectsMaxCounter, "ObjectsMaxCounter");

            myPool.Dispose();
        }

        [TestMethod]
        public async Task DynamicObjectPool_get_timeout()
        {
            var myPool = new DynamicObjectPool<MyHeavyObject>(() => new MyHeavyObject(1), 1, 1);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var obj1 = await myPool.GetObject(CancellationToken.None);
            var obj2 = await myPool.GetObject(cancellationTokenSource.Token);

            Assert.IsNull(obj2);

            myPool.Dispose();
        }
    }
}
