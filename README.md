# dot-net-non-sync

Short explorations of non-synchronous execution.

# Synchronization Primitives

https://docs.microsoft.com/en-us/dotnet/standard/threading/overview-of-synchronization-primitives

```
System.Object
  System.MarshallByRefObject
    System.Threading.WaitHandle
      System.Threading.Mutex
      System.Threading.Semaphore
      System.Threading.EventWaitHandle
        System.Threading.AutoResetEvent
        System.Threading.ManualResetEvent
```

| Primitive           | Description                                                       | When is it signalled?      |
| ------------------- | ----------------------------------------------------------------- | -------------------------- |
| Object              |                                                                   |
| MarshallByRefObject |                                                                   |
| WaitHandle          |                                                                   |
| Mutex               | Grants exclusive access to a resource.                            | If no thread owns it.      |
| Semaphore           | Limits number of threads that can access a resource concurrently. | If its count exceeds zero. |
| EventWaitHandle     | Thread synchronization event.                                     | TODO                       |
| AutoResetEvent      | When signalled, resets automatically to unsignalled.              | TODO                       |
| ManualResetEvent    | When signalled, stays signalled until manually Reset().           | TODO                       |
