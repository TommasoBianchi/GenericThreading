using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class ThreadedWorker<U, T> {

    Queue<ThreadInfo> threadInfoQueue;

    public ThreadedWorker()
    {
        threadInfoQueue = new Queue<ThreadInfo>();
    }

    public void ClearThreadInfoQueue()
    {
        while (threadInfoQueue.Count > 0)
            threadInfoQueue.Dequeue().InvokeCallback();
    }

    public void RequestWork(U input, Func<U, T> work, UnityAction<T> callback)
    {
        Thread thread = new Thread(new ThreadStart(() =>
        {
            T result = work(input);
            lock (threadInfoQueue)
            {
                threadInfoQueue.Enqueue(new ThreadInfo(result, callback));
            }
        }));

        thread.Start();
    }

    struct ThreadInfo
    {
        T result;
        UnityAction<T> callback;

        public ThreadInfo(T result, UnityAction<T> callback)
        {
            this.result = result;
            this.callback = callback;
        }

        public void InvokeCallback()
        {
            callback(result);
        }
    }
}
