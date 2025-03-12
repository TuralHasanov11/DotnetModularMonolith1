using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ModularMonolith.SharedKernel.UnitTests;

public class SpecificationTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class TestSpecification : Specification<TestEntity>
    {
        public TestSpecification(Expression<Func<TestEntity, bool>> criteria, bool condition = true)
            : base(criteria, condition)
        {
        }
    }

    [Fact]
    public void AddInclude_ShouldSetIncludeExpression()
    {
        // Arrange
        var spec = new TestSpecification(x => x.Id > 0);
        Func<IQueryable<TestEntity>, IIncludableQueryable<TestEntity, object>> includeExpression = q => q.Include(x => x.Name);

        // Act
        spec.AddInclude(includeExpression);

        // Assert
        Assert.Equal(includeExpression, spec.IncludeExpression);
    }

    [Fact]
    public void AddInclude_ShouldThrowException_WhenIncludeExpressionIsNull()
    {
        // Arrange
        var spec = new TestSpecification(x => x.Id > 0);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => spec.AddInclude(null));
    }

    [Fact]
    public void GetQuery_ShouldApplyCriteria_WhenConditionIsTrue()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Test1" },
            new TestEntity { Id = 2, Name = "Test2" }
        }.AsQueryable();

        var spec = new TestSpecification(x => x.Id > 1);

        // Act
        var result = SpecificationEvaluator.GetQuery(data, spec);

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result.First().Id);
    }

    [Fact]
    public void GetQuery_ShouldNotApplyCriteria_WhenConditionIsFalse()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Test1" },
            new TestEntity { Id = 2, Name = "Test2" }
        }.AsQueryable();

        var spec = new TestSpecification(x => x.Id > 1, false);

        // Act
        var result = data.GetQuery(spec);

        // Assert
        Assert.Equal(2, result.Count());
    }
}
