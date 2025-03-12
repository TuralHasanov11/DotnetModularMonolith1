using System.Linq.Expressions;

namespace ModularMonolith.SharedKernel.UnitTests;

public class QueryableExtensionsTests
{
    [Fact]
    public void WhereIf_ShouldApplyPredicate_WhenConditionIsTrue()
    {
        // Arrange
        var data = new List<int> { 1, 2, 3, 4, 5 }.AsQueryable();
        bool condition = true;
        Expression<Func<int, bool>> predicate = x => x > 3;

        // Act
        var result = data.WhereIf(condition, predicate);

        // Assert
        Assert.Equal([4, 5], result.ToList());
    }

    [Fact]
    public void WhereIf_ShouldNotApplyPredicate_WhenConditionIsFalse()
    {
        // Arrange
        var data = new List<int> { 1, 2, 3, 4, 5 }.AsQueryable();
        bool condition = false;
        Expression<Func<int, bool>> predicate = x => x > 3;

        // Act
        var result = data.WhereIf(condition, predicate);

        // Assert
        Assert.Equal(data.ToList(), result.ToList());
    }

    [Fact]
    public void WhereIf_ShouldHandleEmptyQueryable()
    {
        // Arrange
        var data = new List<int>().AsQueryable();
        bool condition = true;
        Expression<Func<int, bool>> predicate = x => x > 3;

        // Act
        var result = data.WhereIf(condition, predicate);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void WhereIf_ShouldHandleNullPredicate()
    {
        // Arrange
        var data = new List<int> { 1, 2, 3, 4, 5 }.AsQueryable();
        const bool condition = true;
        Expression<Func<int, bool>> predicate = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => data.WhereIf(condition, predicate));
    }
}
