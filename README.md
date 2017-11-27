psluja.ObjectPools
===================
Thread-safe generic object pool helps sharing object instances among multiple threads. Large object that are costly to create, are state-full and are not thread safe are difficult to use in multi thread environment. This little, lightweight library will help to achieve that.

Idea
-------------
The ideas behind this library is: 

 - reduce the number of object creation and object disposal to minimum,
 - ensure that no cross-thread operation will be performed between getting the object and its return to the pool.
 

There are to implementation: **FixedObjectPool** and **DynamicObjectPool**.

The interface
-------------
This is the main interface. As you can see there are two methods. First method `GetObject` will return object from the pool if there is any. The most intresting thing is that this method return a `Task<T>` which means that if pool is currently empty you can wait until  an object will return.

Second method `PutObject` will put object back into the pool. 

    public interface IObjectPool<T> : IEnumerable<T> where T : class
    {
            Task<T> GetObject(CancellationToken token);
            void PutObject(T item);
    }


> **Note**
>
> You can use one of the extension method  `Task<Usage<T>> UseObject<T>`like this:
>
>
> using (Usage<MyHeavyObject> objUsage = await pool.UseObject())
> {
>	objUsage.Object.SomeFastMethod();
> }
>
> Notice there is no `PutObject` call - this is because `Usage<T>` will put our object into the pool for us at dispose.


FixedObjectPool
-------------
This is most basic implementation that hold fixed number of objects. User is responsible of adding more object into the pool if necessary.

### Use case
This is common scenario where you create fixed collection of object and want to share between threads. There will be always three objects in use, no more, no less.

    var myPool = new FixedObjectPool<MyHeavyObject>(new[] { new MyHeavyObject(1), new MyHeavyObject(2), new MyHeavyObject(3) });
    
	  using (Usage<MyHeavyObject> objUsage = pool.UseObjectSync())
	  {
	      var obj = objUsage.Object;
	      obj.Begin();
	      obj.SomeFastMethod();
	      obj.SomeSlowMethod();
	      obj.End();
	  }

DynamicObjectPool
-------------
This is more intresting implementation. `DynamicObjectPool` can create or dispose object for you. See constructor below:

	public DynamicObjectPool(Func<T> objectFactory, uint poolSize, uint poolMaxSize)
	
There are three parameters:
- `objectFactory` is the method that is invoked when new object is needed,
- `poolSize` defines number of objects above wchich pool stops creating new object
- `poolMaxSize` defines number of objects above which pool will start disposing objects.

> Note
> Shouldn't there be only one parameter?
> The second parameter `poolMaxSize` is only valid when user decides to put additional object instances to the pool. If user 
> decides that there is need for extra more object then is absolutly ok to call `PutObject` with newly created instance and pool will use that object. 

