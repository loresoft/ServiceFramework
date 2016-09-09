﻿using System;
using System.Threading;

namespace ServiceFramework
{
    internal sealed class DisposableAction : IDisposable
    {
        public static readonly DisposableAction Empty = new DisposableAction(null);

        private Action _disposeAction;

        public DisposableAction(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            // Interlocked allows the continuation to be executed only once
            var continuation = Interlocked.Exchange(ref _disposeAction, null);
            continuation?.Invoke();
        }

    }
}