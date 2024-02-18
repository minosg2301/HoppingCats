using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace moonNest
{
    public class UnityWebRequestAwaiter : INotifyCompletion
    {
        private readonly UnityWebRequestAsyncOperation asyncOperator;
        private Action continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            asyncOperator = asyncOp;
            asyncOp.completed += OnOperatorCompleted;
        }

        public bool IsCompleted { get { return asyncOperator.isDone; } }

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        private void OnOperatorCompleted(AsyncOperation asyncOp)
        {
            continuation();
        }
    }

    public static class UnityWebRequestExtension
    {
        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}