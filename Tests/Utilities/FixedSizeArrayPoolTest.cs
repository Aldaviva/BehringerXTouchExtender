using BehringerXTouchExtender.Utilities;

namespace Tests.Utilities;

public class FixedSizeArrayPoolTest {

    private readonly FixedSizeArrayPool<int> _pool = new(22, 2);

    [Fact]
    public void Borrow() {
        _pool.AvailableCount.Should().Be(0);
        _pool.BorrowedCount.Should().Be(0);

        int[] borrowed = _pool.Borrow();

        borrowed.Should().HaveCount(22, "The size of the array matters. This is why ArrayPool is useless.");
        _pool.AvailableCount.Should().Be(0);
        _pool.BorrowedCount.Should().Be(1);
    }

    [Fact]
    public void Return() {
        int[] borrowed = _pool.Borrow();

        _pool.Return(borrowed);

        _pool.AvailableCount.Should().Be(1);
        _pool.BorrowedCount.Should().Be(0);
    }

    [Fact]
    public void ReturnedItemsAreReused() {
        int[] borrowed = _pool.Borrow();
        borrowed[0] = 123;
        _pool.Return(borrowed);

        int[] borrowed2 = _pool.Borrow();

        borrowed2[0].Should().Be(123);
        borrowed2.Should().BeSameAs(borrowed);
        _pool.AvailableCount.Should().Be(0);
        _pool.BorrowedCount.Should().Be(1);
    }

    [Fact]
    public void BorrowingTooManyItemsSucceeds() {
        var borrowed = new List<int[]> {
            _pool.Borrow(),
            _pool.Borrow(),
            _pool.Borrow()
        };

        borrowed[0][0] = 100;
        borrowed[1][0] = 101;
        borrowed[2][0] = 102;

        borrowed[0][0].Should().Be(100);
        borrowed[1][0].Should().Be(101);
        borrowed[2][0].Should().Be(102);

        borrowed[0].Should().NotBeSameAs(borrowed[1]);
        borrowed[0].Should().NotBeSameAs(borrowed[2]);
        borrowed[1].Should().NotBeSameAs(borrowed[2]);

        _pool.AvailableCount.Should().Be(0);
        _pool.BorrowedCount.Should().Be(3);
    }

    [Fact]
    public void ReturningExcessItemsDiscardsThem() {
        var borrowed = new List<int[]> {
            _pool.Borrow(),
            _pool.Borrow(),
            _pool.Borrow()
        };
        _pool.AvailableCount.Should().Be(0);
        _pool.BorrowedCount.Should().Be(3);

        _pool.Return(borrowed[0]);
        _pool.AvailableCount.Should().Be(0, "this was an extra item, so it should have been discarded");
        _pool.BorrowedCount.Should().Be(2);

        _pool.Return(borrowed[1]);
        _pool.AvailableCount.Should().Be(1);
        _pool.BorrowedCount.Should().Be(1);

        _pool.Return(borrowed[2]);
        _pool.AvailableCount.Should().Be(2);
        _pool.BorrowedCount.Should().Be(0);
    }

}