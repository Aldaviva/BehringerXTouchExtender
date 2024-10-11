using System.Collections.Concurrent;

namespace BehringerXTouchExtender.Utilities;

internal class FixedSizeArrayPool<T> {

    private readonly int                  _arrayLength;
    private readonly int                  _preferredPoolSize;
    private readonly ConcurrentStack<T[]> _available = new();

    internal int BorrowedCount;

    public FixedSizeArrayPool(int arrayLength, int preferredPoolSize) {
        _arrayLength       = arrayLength;
        _preferredPoolSize = preferredPoolSize;
    }

    public T[] Borrow() {
        if (!_available.TryPop(out T[] toLend)) {
            toLend = new T[_arrayLength];
        }
        Interlocked.Increment(ref BorrowedCount);
        return toLend;
    }

    public void Return(T[] borrowed) {
        int borrowedCountAfterReturn = Interlocked.Decrement(ref BorrowedCount);
        while (borrowedCountAfterReturn < 0 && borrowedCountAfterReturn != Interlocked.CompareExchange(ref BorrowedCount, 0, borrowedCountAfterReturn)) {
            borrowedCountAfterReturn = BorrowedCount;
        }

        if (borrowedCountAfterReturn + AvailableCount < _preferredPoolSize) {
            _available.Push(borrowed);
        }
    }

    internal int AvailableCount => _available.Count;

}