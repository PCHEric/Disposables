using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.Disposables
{
    /// <summary>
    /// A wrapper around an existing disposable that ensures the existing disposable is only disposed once.
    /// </summary>
    public sealed class WrappedDisposable : IDisposable
    {
        private readonly CollectionDisposable _disposable;

        /// <summary>
        /// Creates a new wrapped disposable.
        /// </summary>
        /// <param name="disposable">The disposable to wrap.</param>
        public WrappedDisposable(IDisposable disposable)
        {
            _disposable = CollectionDisposable.Create(disposable);
        }

        /// <summary>
        /// Whether the wrapped disposable is currently disposing or has been disposed.
        /// </summary>
        public bool IsDisposeStarted => _disposable.IsDisposeStarted;

        /// <summary>
        /// Whether the wrapped disposable is disposed (finished disposing).
        /// </summary>
        public bool IsDisposed => _disposable.IsDisposed;

        /// <summary>
        /// Whether the wrapped disposable is currently disposing, but not finished yet.
        /// </summary>
        public bool IsDisposing => _disposable.IsDisposing;

        /// <summary>
        /// Disposes the wrapped disposable.
        /// </summary>
        public void Dispose() => _disposable.Dispose();
    }
}
