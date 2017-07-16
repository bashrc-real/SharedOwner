## SharedOwner
A library for managing shared resources in C# almost equivalent to [shared_ptr](http://en.cppreference.com/w/cpp/memory/shared_ptr) 
# Overview
Despite having the luxury of garbage collector, items that derive from [IDisposable](https://msdn.microsoft.com/en-us/library/system.idisposable(v=vs.110).aspx) needs to be almost treated like a native resource. The owner must take the responsibility of calling `Dispose` once all the consumers of the resource are no longer using the api. But for some shared reosources their is no implicit owner and hence it becomes difficult to clean up resources in a deterministic manner. This library provides  `ref counted`  handle objects managed by a `owner`  class which has the responsibility of calling `Dispose` once the ref count of the underlying object becomes 0.

# Usage

Passing resources to other classes:
```cs
var sharedConnection = SharedOwner.SharedHandle<HttpClient>.MakeSharedHandle(() => new HttpClient());
this.m_obj = new Object1(sharedClient);
this.m_obj2 = new Object1(sharedClient);
```
Getting the object in the client side:

```cs
using (var sharedClient = this.m_sharedClient.GetHandle()) // m_sharedClient is the SharedHandle passed
{
     var httpClient = (HttpClient)sharedClient;
     // use httpClient
}
// **Do not use httpClient beyond the using scope of the intermediate handle from GetHandle**
```
Check `BasicTests.cs` for sample usage. 