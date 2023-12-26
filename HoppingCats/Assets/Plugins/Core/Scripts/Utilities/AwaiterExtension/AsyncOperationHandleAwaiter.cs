using System;
using System.Runtime.CompilerServices;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace moonNest
{
    public class AsyncOperationHandleAwaiter<TObject> : INotifyCompletion
    {
        private readonly AsyncOperationHandle<TObject> asyncOperator;
        private Action continuation;

        public AsyncOperationHandleAwaiter(AsyncOperationHandle<TObject> asyncOp)
        {
            asyncOperator = asyncOp;
            asyncOperator.Completed += OnOperatorCompleted;
        }

        public bool IsCompleted { get { return asyncOperator.IsDone; } }

        public TObject GetResult() => asyncOperator.Result;

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        private void OnOperatorCompleted(AsyncOperationHandle<TObject> asyncOp)
        {
            continuation?.Invoke();
        }
    }

    public static class AsyncOperationHandleExtension
    {
        public static AsyncOperationHandleAwaiter<TObject> GetAwaiter<TObject>(this AsyncOperationHandle<TObject> asyncOp)
        {
            return new AsyncOperationHandleAwaiter<TObject>(asyncOp);
        }
    }
}