#if NETSTANDARD2_1
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nito.Disposables
{
    /// <summary>
    /// A reference-counted wrapper for a disposable. The wrapper itself is the first reference.
    /// Once all references are disposed, the underlying disposable is disposed. All future reference disposals are no-ops.
    /// </summary>
    public sealed class ReferenceCountedAsyncDisposable : IAsyncDisposable
    {
#pragma warning disable CA2213 // Disposable fields should be disposed
        private readonly WrappedAsyncDisposable _wrappedDisposable;
#pragma warning restore CA2213 // Disposable fields should be disposed
        private readonly IAsyncDisposable _disposeImplementation;
        private long _count;

        /// <summary>
        /// Creates a reference-counted wrapper for the specified disposable.
        /// </summary>
        /// <param name="disposable">The disposable that will be disposed when the reference count reaches zero. May not be <c>null</c>.</param>
        public ReferenceCountedAsyncDisposable(IAsyncDisposable disposable)
        {
            _wrappedDisposable = new WrappedAsyncDisposable(disposable);
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
        public IAsyncDisposable AddReference()
        {
            Interlocked.Increment(ref _count);
            return AnonymousAsyncDisposable.Create(async () =>
            {
                if (Interlocked.Decrement(ref _count) == 0)
                    await _wrappedDisposable.DisposeAsync().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Frees the reference count held by this wrapper instance.
        /// </summary>
        public ValueTask DisposeAsync() => _disposeImplementation.DisposeAsync();
    }
}
#endif