using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nito.Disposables
{
    /// <summary>
    /// A reference-counted wrapper for a disposable.
    /// Once all references are disposed, the underlying disposable is disposed. All future reference disposals are no-ops.
    /// </summary>
    public sealed class ReferenceCountedAsyncDisposable
    {
        private readonly IAsyncDisposable _disposable;
        private int _count;

        /// <summary>
        /// Creates a reference-counted wrapper for the specified disposable.
        /// </summary>
        /// <param name="disposable">The disposable that will be disposed when the reference count reaches zero.</param>
        public ReferenceCountedAsyncDisposable(IAsyncDisposable disposable) => _disposable = new CollectionAsyncDisposable(disposable);

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
                    await _disposable.DisposeAsync().ConfigureAwait(false);
            });
        }
    }
}
