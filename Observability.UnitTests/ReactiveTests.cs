using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using AwesomeAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Observability.UnitTests;

public static class ReactiveTests
{
    [Theory]
    [InlineData(0, -10)]
    [InlineData(0, 10)]
    [InlineData(0, int.MaxValue)]
    [InlineData(0, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MaxValue)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithAddExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => unchecked(target.Value + offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(offset).Only();
        target.Value = value;
        subscription.ShouldBe(unchecked(value + offset)).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0, -10)]
    [InlineData(0, 10)]
    [InlineData(0, int.MaxValue)]
    [InlineData(0, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MaxValue)]
    public static void Observe_WorksWithAddCheckedExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => checked(target.Value + offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(offset).Only();
        target.Value = value;
        subscription.ShouldBe(checked(value + offset)).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithAddCheckedExpressions_WhenOverflow(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => checked(target.Value + offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(offset).Only();
        target.Value = value;
        subscription
            .ShouldBeError(new OverflowException("Arithmetic operation resulted in an overflow."))
            .Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0, -10)]
    [InlineData(0, 10)]
    [InlineData(0, int.MaxValue)]
    [InlineData(0, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MaxValue)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithSubtractExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => unchecked(target.Value - offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(-offset).Only();
        target.Value = value;
        subscription.ShouldBe(unchecked(value - offset)).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0, -10)]
    [InlineData(0, 10)]
    [InlineData(0, int.MaxValue)]
    [InlineData(0, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MaxValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithSubtractCheckedExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => checked(target.Value - offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(-offset).Only();
        target.Value = value;
        subscription.ShouldBe(checked(value - offset)).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, int.MaxValue)]
    public static void Observe_WorksWithSubtractCheckedExpressions_WhenOverflow(
        int offset,
        int value
    )
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => checked(target.Value - offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(-offset).Only();
        target.Value = value;
        subscription
            .ShouldBeError(new OverflowException("Arithmetic operation resulted in an overflow."))
            .Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0, -10)]
    [InlineData(0, 10)]
    [InlineData(0, int.MaxValue)]
    [InlineData(0, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MaxValue)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithMultiplyExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 1 };
        var sut = Reactive.Observe(() => unchecked(target.Value * offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(offset).Only();
        target.Value = value;
        subscription.ShouldBe(unchecked(value * offset)).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0, -10)]
    [InlineData(0, 10)]
    [InlineData(0, int.MaxValue)]
    [InlineData(0, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    public static void Observe_WorksWithMultiplyCheckedExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 1 };
        var sut = Reactive.Observe(() => checked(target.Value * offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(offset).Only();
        target.Value = value;
        subscription.ShouldBe(checked(value * offset)).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(100, int.MaxValue)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithMultiplyCheckedExpressions_WithOverflow(
        int offset,
        int value
    )
    {
        // arrange
        var target = new TestReactive<int> { Value = 1 };
        var sut = Reactive.Observe(() => checked(target.Value * offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(offset).Only();
        target.Value = value;
        subscription
            .ShouldBeError(new OverflowException("Arithmetic operation resulted in an overflow."))
            .Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(1, -10)]
    [InlineData(1, 10)]
    [InlineData(1, int.MaxValue)]
    [InlineData(1, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MaxValue)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithDivideExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => target.Value / offset);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        target.Value = value;
        subscription.ShouldBe(value / offset).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(1, -10)]
    [InlineData(1, 10)]
    [InlineData(1, int.MaxValue)]
    [InlineData(1, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MaxValue)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithModuloExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => target.Value % offset);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        target.Value = value;
        subscription.ShouldBe(value % offset).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(1, -10)]
    [InlineData(1, 10)]
    [InlineData(1, int.MaxValue)]
    [InlineData(1, int.MinValue)]
    [InlineData(100, -10)]
    [InlineData(100, 10)]
    [InlineData(100, int.MaxValue)]
    [InlineData(100, int.MinValue)]
    [InlineData(-100, -10)]
    [InlineData(-100, 10)]
    [InlineData(-100, int.MaxValue)]
    [InlineData(-100, int.MinValue)]
    public static void Observe_WorksWithPowerExpressions(int offset, int value)
    {
        // arrange
        var target = new TestReactive<int> { Value = 1 };
        var sut = Reactive.Observe(PowExpression(() => target.Value, () => offset));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(1).Only();
        target.Value = value;
        subscription.ShouldBe(Math.Pow(value, offset)).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithAndExpressions()
    {
        // arrange
        var a = new TestReactive<bool> { Value = false };
        var b = new TestReactive<bool> { Value = false };
        var sut = Reactive.Observe(() => a.Value & b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        a.Value = true;
        subscription.ShouldBe(false).Only();
        b.Value = true;
        subscription.ShouldBe(true).Only();
        a.Value = false;
        subscription.ShouldBe(false).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithOrExpressions()
    {
        // arrange
        var a = new TestReactive<bool> { Value = false };
        var b = new TestReactive<bool> { Value = false };
        var sut = Reactive.Observe(() => a.Value | b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        a.Value = true;
        subscription.ShouldBe(true).Only();
        b.Value = true;
        subscription.ShouldBe(true).Only();
        a.Value = false;
        subscription.ShouldBe(true).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithXOrExpressions()
    {
        // arrange
        var a = new TestReactive<bool> { Value = false };
        var b = new TestReactive<bool> { Value = false };
        var sut = Reactive.Observe(() => a.Value ^ b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        a.Value = true;
        subscription.ShouldBe(true).Only();
        b.Value = true;
        subscription.ShouldBe(false).Only();
        a.Value = false;
        subscription.ShouldBe(true).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithLessThanExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value < b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        a.Value = 10;
        subscription.ShouldBe(false).Only();
        b.Value = 100;
        subscription.ShouldBe(true).Only();
        a.Value = 0;
        subscription.ShouldBe(true).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithLessThanOrEqualExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value <= b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(true).Only();
        a.Value = 10;
        subscription.ShouldBe(false).Only();
        b.Value = 100;
        subscription.ShouldBe(true).Only();
        a.Value = 0;
        subscription.ShouldBe(true).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithGreaterThanExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value > b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        a.Value = 10;
        subscription.ShouldBe(true).Only();
        b.Value = 100;
        subscription.ShouldBe(false).Only();
        a.Value = 0;
        subscription.ShouldBe(false).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithGreaterThanOrEqualExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value >= b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(true).Only();
        a.Value = 10;
        subscription.ShouldBe(true).Only();
        b.Value = 100;
        subscription.ShouldBe(false).Only();
        a.Value = 0;
        subscription.ShouldBe(false).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithEqualExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value == b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(true).Only();
        a.Value = 10;
        subscription.ShouldBe(false).Only();
        b.Value = 100;
        subscription.ShouldBe(false).Only();
        a.Value = 0;
        subscription.ShouldBe(false).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithNotEqualExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value != b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        a.Value = 10;
        subscription.ShouldBe(true).Only();
        b.Value = 100;
        subscription.ShouldBe(true).Only();
        a.Value = 0;
        subscription.ShouldBe(true).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithLeftShiftExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value << b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        a.Value = 1;
        subscription.ShouldBe(1).Only();
        b.Value = 5;
        subscription.ShouldBe(1 << 5).Only();
        a.Value = 12;
        subscription.ShouldBe(12 << 5).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithRightShiftExpressions()
    {
        // arrange
        var a = new TestReactive<int> { Value = 0 };
        var b = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => a.Value >> b.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        a.Value = 100;
        subscription.ShouldBe(100).Only();
        b.Value = 5;
        subscription.ShouldBe(100 >> 5).Only();
        a.Value = 12324324;
        subscription.ShouldBe(12324324 >> 5).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithArrayIndexExpressions()
    {
        // arrange
        var target = new TestReactive<int[]> { Value = [1, 2, 3, 4, 5, 6, 7, 8] };
        var index = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => target.Value[index.Value]);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(1).Only();
        index.Value = 4;
        subscription.ShouldBe(5).Only();
        target.Value = [7, 6, 4, 3, 2, 1];
        subscription.ShouldBe(2).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    private sealed class Counter
    {
        public int Count { get; private set; }

        public T Increment<T>(T value)
        {
            Count++;
            return value;
        }
    }

    [Fact]
    public static void Observe_WorksWithAndAlsoExpressions()
    {
        // arrange
        var a = new TestReactive<bool> { Value = false };
        var b = new TestReactive<bool> { Value = false };
        var counter = new Counter();
        var sut = Reactive.Observe(() => a.Value && counter.Increment(b.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        _ = counter.Count.Should().Be(0);
        b.Value = true;
        subscription.ShouldBeEmpty();
        _ = counter.Count.Should().Be(0);
        a.Value = true;
        subscription.ShouldBe(true).Only();
        _ = counter.Count.Should().Be(1);
        b.Value = false;
        subscription.ShouldBe(false).Only();
        _ = counter.Count.Should().Be(2);
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithOrElseExpressions()
    {
        // arrange
        var a = new TestReactive<bool> { Value = false };
        var b = new TestReactive<bool> { Value = false };
        var counter = new Counter();
        var sut = Reactive.Observe(() => a.Value || counter.Increment(b.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        _ = counter.Count.Should().Be(1);
        b.Value = true;
        subscription.ShouldBe(true).Only();
        _ = counter.Count.Should().Be(2);
        a.Value = true;
        subscription.ShouldBe(true).Only();
        _ = counter.Count.Should().Be(2);
        b.Value = false;
        subscription.ShouldBeEmpty();
        _ = counter.Count.Should().Be(2);
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithCoalesceExpressions()
    {
        // arrange
        var a = new TestReactive<string?> { Value = null };
        var b = new TestReactive<string> { Value = "Success" };
        var counter = new Counter();
        var sut = Reactive.Observe(() => a.Value ?? counter.Increment(b.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("Success").Only();
        _ = counter.Count.Should().Be(1);
        b.Value = "B value";
        subscription.ShouldBe("B value").Only();
        _ = counter.Count.Should().Be(2);
        a.Value = "A value";
        subscription.ShouldBe("A value").Only();
        _ = counter.Count.Should().Be(2);
        b.Value = "Invalid";
        subscription.ShouldBeEmpty();
        _ = counter.Count.Should().Be(2);
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithCoalesceNullableExpressions()
    {
        // arrange
        var a = new TestReactive<int?> { Value = null };
        var b = new TestReactive<int> { Value = 123 };
        var counter = new Counter();
        var sut = Reactive.Observe(() => a.Value ?? counter.Increment(b.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(123).Only();
        _ = counter.Count.Should().Be(1);
        b.Value = 456;
        subscription.ShouldBe(456).Only();
        _ = counter.Count.Should().Be(2);
        a.Value = 111;
        subscription.ShouldBe(111).Only();
        _ = counter.Count.Should().Be(2);
        b.Value = 999;
        subscription.ShouldBeEmpty();
        _ = counter.Count.Should().Be(2);
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithCoalesceConversionNullableExpressions()
    {
        // arrange
        var a = new TestReactive<byte[]?> { Value = null };
        var b = new TestReactive<ReadOnlyMemory<byte>> { Value = new byte[] { 1, 2, 3, 4, 5 } };
        var counter = new Counter();
        var sut = Reactive.Observe(() => a.Value ?? counter.Increment(b.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(b.Value).Only();
        _ = counter.Count.Should().Be(1);
        b.Value = new byte[] { 6, 7, 8, 9 };
        subscription.ShouldBe(b.Value).Only();
        _ = counter.Count.Should().Be(2);
        a.Value = [5, 4, 3, 2];
        subscription.ShouldBe(a.Value).Only();
        _ = counter.Count.Should().Be(2);
        b.Value = new byte[] { 8, 234, 3, 23 };
        subscription.ShouldBeEmpty();
        _ = counter.Count.Should().Be(2);
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithCoalesceConversionReferenceExpressions()
    {
        // arrange
        var a = new TestReactive<Memory<byte>?> { Value = null };
        var b = new TestReactive<ReadOnlyMemory<byte>> { Value = new byte[] { 1, 2, 3, 4, 5 } };
        var counter = new Counter();
        var sut = Reactive.Observe(() => a.Value ?? counter.Increment(b.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(b.Value).Only();
        _ = counter.Count.Should().Be(1);
        b.Value = new byte[] { 6, 7, 8, 9 };
        subscription.ShouldBe(b.Value).Only();
        _ = counter.Count.Should().Be(2);
        a.Value = new byte[] { 5, 4, 3, 2 };
        subscription.ShouldBe(a.Value.Value).Only();
        _ = counter.Count.Should().Be(2);
        b.Value = new byte[] { 8, 234, 3, 23 };
        subscription.ShouldBeEmpty();
        _ = counter.Count.Should().Be(2);
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithConditionExpressions()
    {
        // arrange
        var test = new TestReactive<bool> { Value = false };
        var ifTrue = new TestReactive<int> { Value = 1 };
        var ifFalse = new TestReactive<int> { Value = 2 };
        var trueCounter = new Counter();
        var falseCounter = new Counter();
        var sut = Reactive.Observe(() =>
            test.Value ? trueCounter.Increment(ifTrue.Value) : falseCounter.Increment(ifFalse.Value)
        );

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(2).Only();
        _ = trueCounter.Count.Should().Be(0);
        _ = falseCounter.Count.Should().Be(1);
        ifTrue.Value = 123;
        ifFalse.Value = 456;
        subscription.ShouldBe(456).Only();
        _ = trueCounter.Count.Should().Be(0);
        _ = falseCounter.Count.Should().Be(2);
        test.Value = true;
        subscription.ShouldBe(123).Only();
        _ = trueCounter.Count.Should().Be(1);
        _ = falseCounter.Count.Should().Be(2);
        ifTrue.Value = 321;
        ifFalse.Value = 654;
        subscription.ShouldBe(321).Only();
        _ = trueCounter.Count.Should().Be(2);
        _ = falseCounter.Count.Should().Be(2);
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithNestedDelegates()
    {
        // arrange
        var source = new TestReactive<int[]?> { Value = null };
        var divisor = new TestReactive<int> { Value = 2 };
        var sut = Reactive.Observe(() =>
            source.Value!.Where(v => v % divisor.Value == 0).ToArray()
        );

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(null).Only();
        source.Value = [1, 2, 3, 4, 5, 6, 7, 8, 9];
        subscription.ShouldBe([2, 4, 6, 8]).Only();
        divisor.Value = 3;
        subscription.ShouldBe([3, 6, 9]).Only();
        source.Value = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
        subscription.ShouldBe([3, 6, 9, 12, 15]).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithNotExpressions()
    {
        // arrange
        var source = new TestReactive<bool> { Value = false };
        var sut = Reactive.Observe(() => !source.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(true).Only();
        source.Value = true;
        subscription.ShouldBe(false).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithDecrementExpressions()
    {
        // arrange
        var source = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(Wrap<int>(() => source.Value, Expression.Decrement));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(-1).Only();
        source.Value = 10;
        subscription.ShouldBe(9).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithIncrementExpressions()
    {
        // arrange
        var source = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(Wrap<int>(() => source.Value, Expression.Increment));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(1).Only();
        source.Value = 10;
        subscription.ShouldBe(11).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithDefaultExpressions()
    {
        // arrange
        var sut = Reactive.Observe(Expression.Lambda<Func<int>>(Expression.Default(typeof(int))));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).ThenBeCompleted().Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithUnboxExpressions()
    {
        // arrange
        var source = new TestReactive<object> { Value = 0 };
        var sut = Reactive.Observe(
            Wrap<int, Type>(() => source.Value, typeof(int), Expression.Unbox)
        );

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = 10;
        subscription.ShouldBe(10).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithUnaryPlusExpressions()
    {
        // arrange
        var source = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(Wrap<int>(() => source.Value, Expression.UnaryPlus));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = 10;
        subscription.ShouldBe(10).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithAsExpressions()
    {
        // arrange
        var source = new TestReactive<object> { Value = 0 };
        var sut = Reactive.Observe(() => source.Value as int?);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = 10;
        subscription.ShouldBe(10).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithConvertExpressions()
    {
        // arrange
        var source = new TestReactive<object> { Value = 0 };
        var sut = Reactive.Observe(() => (int)source.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = 10;
        subscription.ShouldBe(10).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithConvertCheckedExpressions()
    {
        // arrange
        var source = new TestReactive<float> { Value = 0 };
        var sut = Reactive.Observe(() => checked((int)source.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = 10;
        subscription.ShouldBe(10).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithArrayLengthExpressions()
    {
        // arrange
        var source = new TestReactive<int[]?> { Value = null };
        var sut = Reactive.Observe(() => source.Value!.Length);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = [0, 0, 0, 0, 0, 0, 0, 0];
        subscription.ShouldBe(8).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithOnesComplimentExpressions()
    {
        // arrange
        var source = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => ~source.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(-1).Only();
        source.Value = 10;
        subscription.ShouldBe(-11).Only();
        source.Value = -10;
        subscription.ShouldBe(9).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithIsTrueExpressions()
    {
        // arrange
        var source = new TestReactive<bool> { Value = false };
        var sut = Reactive.Observe(Wrap<bool>(() => source.Value, Expression.IsTrue));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(false).Only();
        source.Value = true;
        subscription.ShouldBe(true).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithIsFalseExpressions()
    {
        // arrange
        var source = new TestReactive<bool> { Value = false };
        var sut = Reactive.Observe(Wrap<bool>(() => source.Value, Expression.IsFalse));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(true).Only();
        source.Value = true;
        subscription.ShouldBe(false).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithNegateExpressions()
    {
        // arrange
        var source = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => -source.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = 10;
        subscription.ShouldBe(-10).Only();
        source.Value = -10;
        subscription.ShouldBe(10).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithNegateCheckedExpressions()
    {
        // arrange
        var source = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => checked(-source.Value));

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(0).Only();
        source.Value = 10;
        subscription.ShouldBe(-10).Only();
        source.Value = -10;
        subscription.ShouldBe(10).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithArraySizeExpressions()
    {
        // arrange
        var source = new TestReactive<int> { Value = 0 };
        var sut = Reactive.Observe(() => new int[source.Value]);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe([]).Only();
        source.Value = 5;
        subscription.ShouldBe([0, 0, 0, 0, 0]).Only();
        source.Value = 1;
        subscription.ShouldBe([0]).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithArrayElementsExpressions()
    {
        // arrange
        var source = new TestMultipleReactive<int, int, int, int, int>
        {
            V1 = 0,
            V2 = 1,
            V3 = 2,
            V4 = 3,
            V5 = 4,
        };
        var sut = Reactive.Observe(() =>
            new[] { source.V1, source.V2, source.V3, source.V4, source.V5 }
        );

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe([0, 1, 2, 3, 4]).Only();
        source.V1 = 6;
        subscription.ShouldBe([6, 1, 2, 3, 4]).Only();
        source.V2 = 9;
        source.V3 = 8;
        source.V4 = 7;
        source.V5 = 6;
        source.V1 = 5;
        subscription
            .ShouldBe([6, 9, 2, 3, 4])
            .ThenBe([6, 9, 8, 3, 4])
            .ThenBe([6, 9, 8, 7, 4])
            .ThenBe([6, 9, 8, 7, 6])
            .ThenBe([5, 9, 8, 7, 6])
            .Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithListInitExpressions()
    {
        // arrange
        var source = new TestMultipleReactive<int, int, int, int, int>
        {
            V1 = 0,
            V2 = 1,
            V3 = 2,
            V4 = 3,
            V5 = 4,
        };
        var sut = Reactive.Observe(() =>
            new List<int> { source.V1, source.V2, source.V3, source.V4, source.V5 }
        );

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe([0, 1, 2, 3, 4]).Only();
        source.V1 = 6;
        subscription.ShouldBe([6, 1, 2, 3, 4]).Only();
        source.V2 = 9;
        source.V3 = 8;
        source.V4 = 7;
        source.V5 = 6;
        source.V1 = 5;
        subscription
            .ShouldBe([6, 9, 2, 3, 4])
            .ThenBe([6, 9, 8, 3, 4])
            .ThenBe([6, 9, 8, 7, 4])
            .ThenBe([6, 9, 8, 7, 6])
            .ThenBe([5, 9, 8, 7, 6])
            .Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_WorksWithMemberInitExpressions()
    {
        // arrange
        var source = new TestMultipleReactive<int, int, int, int, int>
        {
            V1 = 0,
            V2 = 1,
            V3 = 2,
            V4 = 3,
            V5 = 4,
        };
        var sut = Reactive.Observe(() =>
            new ComplexResult
            {
                V1 = source.V1,
                V2 = source.V2,
                Inner =
                {
                    V3 = source.V3,
                    V4 = source.V4,
                    V5 = source.V5,
                },
                All = { source.V1, source.V2, source.V3, source.V4, source.V5 },
            }
        );

        // act
        using var subscription = sut.Test();

        // assert
        ComplexResult? prev = null;
        Assert(0, 1, 2, 3, 4, ref prev);
        subscription.ShouldBeEmpty();
        source.V1 = 6;
        Assert(6, 1, 2, 3, 4, ref prev);
        subscription.ShouldBeEmpty();
        source.V2 = 9;
        source.V3 = 8;
        source.V4 = 7;
        source.V5 = 6;
        source.V1 = 5;
        Assert(6, 9, 2, 3, 4, ref prev);
        Assert(6, 9, 8, 3, 4, ref prev);
        Assert(6, 9, 8, 7, 4, ref prev);
        Assert(6, 9, 8, 7, 6, ref prev);
        Assert(5, 9, 8, 7, 6, ref prev);
        subscription.ShouldBeEmpty();
        subscription.Dispose();
        subscription.ShouldBeEmpty();

        void Assert(int v1, int v2, int v3, int v4, int v5, ref ComplexResult? value)
        {
            _ = subscription.ShouldBe(
                new ComplexResult
                {
                    V1 = v1,
                    V2 = v2,
                    Inner =
                    {
                        V3 = v3,
                        V4 = v4,
                        V5 = v5,
                    },
                    All = { v1, v2, v3, v4, v5 },
                },
                out var current
            );
            _ = current.Should().NotBeSameAs(value);
            value = current;
        }
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForAConstantExpression()
    {
        // arrange
        var sut = Reactive.Observe(() => "123");

        // act
        using var subscription = sut.Test();

        // assert
        _ = subscription.ShouldBe("123").ThenBeCompleted();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForAnUnreactiveExpression()
    {
        // arrange
        var ctx = new TestUnreactive { Id = "123" };
        var sut = Reactive.Observe(() => ctx.Id);

        // act
        using var subscription = sut.Test();

        // assert
        _ = subscription.ShouldBe("123").ThenBeCompleted();
        ctx.Id = "456";
        subscription.ShouldBeEmpty();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForAReactiveProperty()
    {
        // arrange
        var ctx = new TestReactive { Property = "123" };
        var sut = Reactive.Observe(() => ctx.Property);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("123").Only();
        ctx.Property = "456";
        ctx.SetField("FAIL");
        ctx.SetMethod("FAIL");
        subscription.ShouldBe("456").Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
        ctx.Property = "789";
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForAReactiveField()
    {
        // arrange
        var ctx = new TestReactive { Field = "123" };
        var sut = Reactive.Observe(() => ctx.Field);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("123").Only();
        ctx.Property = "FAIL";
        ctx.SetField("456");
        ctx.SetMethod("FAIL");
        subscription.ShouldBe("456").Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
        ctx.SetField("789");
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForAReactiveMethod()
    {
        // arrange
        var ctx = new TestReactive();
        ctx.SetMethod("123");
        var sut = Reactive.Observe(() => ctx.Method());

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("123").Only();
        ctx.Property = "FAIL";
        ctx.SetField("FAIL");
        ctx.SetMethod("456");
        subscription.ShouldBe("456").Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
        ctx.SetMethod("789");
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForANullableStructUsingValueToString()
    {
        // arrange
        var source = new TestReactive<Guid?> { Value = Guid.Empty };
        var sut = Reactive.Observe(() => source.Value.Value.ToString());

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("00000000-0000-0000-0000-000000000000").Only();
        source.Value = null;
        subscription.ShouldBe(null).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForANullableStructUsingValue()
    {
        // arrange
        var source = new TestReactive<Guid?> { Value = Guid.Empty };
        var sut = Reactive.Observe(() => source.Value.Value);

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe(Guid.Empty).Only();
        source.Value = null;
        subscription.ShouldBe(Guid.Empty).Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForANullableStructUsingHasValue()
    {
        // arrange
        var source = new TestReactive<Guid?> { Value = Guid.Empty };
        var sut = Reactive.Observe(() => source.Value.HasValue.ToString());

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("True").Only();
        source.Value = null;
        subscription.ShouldBe("False").Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForANullableStructUsingGetValueOrDefault()
    {
        // arrange
        var source = new TestReactive<Guid?> { Value = Guid.Empty };
        var sut = Reactive.Observe(() => source.Value.GetValueOrDefault().ToString());

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("00000000-0000-0000-0000-000000000000").Only();
        source.Value = null;
        subscription.ShouldBe("00000000-0000-0000-0000-000000000000").Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForANullableNestedReactiveObject()
    {
        // arrange
        var ctx = new NullableInnerReactive();
        var inner = new TestReactive();
        var property = Reactive.Observe(() => ctx.Inner!.Property);
        var field = Reactive.Observe(() => ctx.Inner!.Field);
        var method = Reactive.Observe(() => ctx.Inner!.Method());

        // act
        using var propertySub = property.Test();
        using var fieldSub = field.Test();
        using var methodSub = method.Test();

        // assert
        propertySub.ShouldBe(null).Only();
        fieldSub.ShouldBe(null).Only();
        methodSub.ShouldBe(null).Only();
        ctx.Inner = inner;
        propertySub.ShouldBe("Property").Only();
        fieldSub.ShouldBe("Field").Only();
        methodSub.ShouldBe("Method").Only();
        inner.Property = "abc";
        inner.SetField("def");
        inner.SetMethod("ghi");
        propertySub.ShouldBe("abc").Only();
        fieldSub.ShouldBe("def").Only();
        methodSub.ShouldBe("ghi").Only();
        ctx.Inner = null;
        propertySub.ShouldBe(null).Only();
        fieldSub.ShouldBe(null).Only();
        methodSub.ShouldBe(null).Only();
        inner.Property = "123";
        inner.SetField("456");
        inner.SetMethod("789");
        propertySub.ShouldBeEmpty();
        fieldSub.ShouldBeEmpty();
        methodSub.ShouldBeEmpty();
        ctx.Inner = inner;
        propertySub.ShouldBe("123").Only();
        fieldSub.ShouldBe("456").Only();
        methodSub.ShouldBe("789").Only();
        propertySub.Dispose();
        fieldSub.Dispose();
        methodSub.Dispose();
        propertySub.ShouldBeEmpty();
        fieldSub.ShouldBeEmpty();
        methodSub.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForAComputedReactiveProperty()
    {
        // arrange
        var ctx = new TestReactive { Property = "123" };
        var sut = Reactive.Observe(() => $"The value is currently: {ctx.Property}");

        // act
        using var subscription = sut.Test();

        // assert
        subscription.ShouldBe("The value is currently: 123").Only();
        ctx.Property = "456";
        ctx.SetField("FAIL");
        ctx.SetMethod("FAIL");
        subscription.ShouldBe("The value is currently: 456").Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
        ctx.Property = "789";
        subscription.ShouldBeEmpty();
    }

    [Fact]
    public static void Observe_ShouldReturnTheCurrentValueForAComputedAnonymousType()
    {
        // arrange
        var ctx = new TestReactive();
        var sut = Reactive.Observe(() =>
            new
            {
                ctx.Property,
                ctx.Field,
                Method = ctx.Method(),
            }
        );

        // act
        using var subscription = sut.Test();

        // assert
        subscription
            .ShouldBe(
                new
                {
                    Property = "Property",
                    Field = "Field",
                    Method = "Method",
                }
            )
            .Only();
        ctx.Property = "123";
        ctx.SetField("456");
        ctx.SetMethod("789");
        subscription
            .ShouldBe(
                new
                {
                    Property = "123",
                    Field = "Field",
                    Method = "Method",
                }
            )
            .ThenBe(
                new
                {
                    Property = "123",
                    Field = "456",
                    Method = "Method",
                }
            )
            .ThenBe(
                new
                {
                    Property = "123",
                    Field = "456",
                    Method = "789",
                }
            )
            .Only();
        subscription.Dispose();
        subscription.ShouldBeEmpty();
        ctx.Property = "789";
        subscription.ShouldBeEmpty();
    }

    private static Expression<Func<double>> PowExpression(
        Expression<Func<double>> left,
        Expression<Func<double>> right
    )
    {
        return Expression.Lambda<Func<double>>(Expression.Power(left.Body, right.Body));
    }

    private static Expression<Func<T>> Wrap<T>(
        Expression<Func<object>> value,
        Func<Expression, Expression> wrap
    )
    {
        return Expression.Lambda<Func<T>>(wrap(StripConversion(value.Body)));
    }

    private static Expression<Func<T>> Wrap<T, TArg>(
        Expression<Func<object>> value,
        TArg arg,
        Func<Expression, TArg, Expression> wrap
    )
    {
        return Expression.Lambda<Func<T>>(wrap(StripConversion(value.Body), arg));
    }

    private static Expression StripConversion(Expression node)
    {
        if (
            node is UnaryExpression
            {
                NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked
            } unary
        )
        {
            return unary.Operand;
        }
        return node;
    }
}

internal sealed class TestUnreactive
{
    public required string Id { get; set; }
}

internal sealed class ComplexResult
{
    public int V1 { get; set; }
    public int V2 { get; set; }
    public ComplexInnerResult Inner { get; } = new();
    public List<int> All { get; } = [];
}

internal sealed class ComplexInnerResult
{
    public int V3 { get; set; }
    public int V4 { get; set; }
    public int V5 { get; set; }
}

internal sealed class TestReactive<T> : ReactiveObject
{
    public required T Value
    {
        get;
        set => Set(ref field, value);
    }
}

internal sealed class TestMultipleReactive<T1, T2, T3, T4, T5> : ReactiveObject
{
    public required T1 V1
    {
        get;
        set => Set(ref field, value);
    }
    public required T2 V2
    {
        get;
        set => Set(ref field, value);
    }
    public required T3 V3
    {
        get;
        set => Set(ref field, value);
    }
    public required T4 V4
    {
        get;
        set => Set(ref field, value);
    }
    public required T5 V5
    {
        get;
        set => Set(ref field, value);
    }
}

internal sealed class TestReactive : ReactiveObject
{
    private string _current = "Method";
    public string Property
    {
        get;
        set => Set(ref field, value);
    } = "Property";
    public string Field = "Field";

    public string Method()
    {
        return _current;
    }

    public void SetField(string value)
    {
        Field = value;
        OnMemberChanged(nameof(Field));
    }

    public void FieldHasChanged()
    {
        OnMemberChanged(nameof(Field));
    }

    public void SetMethod(string value)
    {
        _current = value;
        OnMemberChanged(nameof(Method));
    }
}

internal sealed class NullableInnerReactive : ReactiveObject
{
    public TestReactive? Inner
    {
        get;
        set => Set(ref field, value);
    }
}

internal static class ObservableTest
{
    public static ObservableTest<T> Test<T>(this IObservable<T> source)
    {
        return new(source);
    }
}

internal sealed class ObservableTest<T> : IDisposable
{
    private readonly IDisposable _subscription;
    private readonly ConcurrentQueue<Notification<T>> _messages = [];
    private readonly Assertion _assertion;

    public ObservableTest(IObservable<T> source)
    {
        _assertion = new(_messages);
        _subscription = source.Materialize().Subscribe(OnNext);
    }

    private void OnNext(Notification<T> notification)
    {
        _messages.Enqueue(notification);
    }

    public void ShouldBeEmpty()
    {
        _assertion.Only();
    }

    public Assertion ShouldBe(T value)
    {
        return _assertion.ThenBe(value);
    }

    public Assertion ShouldBe(T value, out T actual)
    {
        return _assertion.ThenBe(value, out actual);
    }

    public Assertion ShouldBeCompleted()
    {
        return _assertion.ThenBeCompleted();
    }

    public Assertion ShouldBeError(Exception exception)
    {
        return _assertion.ThenBeError(exception);
    }

    public sealed class Assertion(ConcurrentQueue<Notification<T>> messages)
    {
        public void Only()
        {
            _ = messages.TryDequeue(out var notification);
            _ = notification.Should().BeNull();
        }

        public Assertion ThenBe(T value)
        {
            return ThenBe(value, out _);
        }

        public Assertion ThenBe(T value, out T actual)
        {
            _ = messages.TryDequeue(out var notification).Should().BeTrue();
            _ = notification
                .Should()
                .BeEquivalentTo(
                    Notification.CreateOnNext(value),
                    op => op.ComparingByMembers<Notification<T>>()
                );
            actual = notification.Value;
            return this;
        }

        public Assertion ThenBeCompleted()
        {
            _ = messages.TryDequeue(out var notification).Should().BeTrue();
            _ = notification
                .Should()
                .BeEquivalentTo(
                    Notification.CreateOnCompleted<T>(),
                    op => op.ComparingByMembers<Notification<T>>()
                );
            return this;
        }

        public Assertion ThenBeError(Exception exception)
        {
            _ = messages.TryDequeue(out var notification).Should().BeTrue();
            _ = notification
                .Should()
                .BeEquivalentTo(
                    Notification.CreateOnError<T>(exception),
                    op =>
                        op.ComparingByMembers<Notification<T>>()
                            .Excluding(x => x.Value)
                            .Excluding(x => x.Exception!.StackTrace)
                            .Excluding(x => x.Exception!.Source)
                            .Excluding(x => x.Exception!.TargetSite)
                );
            return this;
        }
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}
