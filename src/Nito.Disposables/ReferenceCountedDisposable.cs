using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nito.Disposables
{
    /// <summary>
    /// A reference-counted wrapper for a disposable. The wrapper itself is the first reference.
    /// Once all references are disposed, the underlying disposable is disposed. All future reference disposals are no-ops.
    /// </summary>
    public sealed class ReferenceCountedDisposable : IDisposable
    {
#pragma warning disable CA2213 // Disposable fields should be disposed
        private readonly WrappedDisposable _wrappedDisposable;
#pragma warning restore CA2213 // Disposable fields should be disposed
        private readonly IDisposable _disposeImplementation;
        private long _count;

        /// <summary>
        /// Creates a reference-counted wrapper for the specified disposable.
        /// </summary>
        /// <param name="disposable">The disposable that will be disposed when the reference count reaches zero. May not be <c>null</c>.</param>
        public ReferenceCountedDisposable(IDisposable disposable)
        {
            // Use a wrapper to ensure that Dispose is only called once on the disposable passed in.
            _wrappedDisposable = new WrappedDisposable(disposable);
            _disposeImplementation = AddReference();
        }

        /// <summary>
        /// Whether the wrapped disposable is currently disposing or has been disposed.
        /// </summary>
        public bool IsDisposeStarted => _wrappedDisposable.IsDisposeStarted;

        /// <summary>
        /// Whether the wrapped disposable is disposed (finished disposing).
        /// </summary>
        public bool IsDisposed => _wrappedDisposable.IsDisposed;

        /// <summary>
        /// Whether the wrapped disposable is currently disposing, but not finished yet.
        /// </summary>
        public bool IsDisposing => _wrappedDisposable.IsDisposing;

        /// <summary>
        /// Increments the reference count and returns a disposable that decrements the reference count when disposed.
        /// The returned disposable may be disposed multiple times; it will only decrement the reference count once.
        /// </summary>
        public IDisposable AddReference()
        {
            Interlocked.Increment(ref _count);
            return AnonymousDisposable.Create(() =>
            {
                if (Interlocked.Decrement(ref _count) == 0)
                    _wrappedDisposable.Dispose();
            });
        }

        /// <summary>
        /// Frees the reference count held by this wrapper instance.
        /// </summary>
        public void Dispose() => _disposeImplementation.Dispose();
    }
}
