using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace SharedKernel;

public abstract class Specification<TEntity>(
    Expression<Func<TEntity, bool>>? criteria = null,
    bool criteriaCondition = true)
    where TEntity : class
{
    public bool IsSplitQuery { get; protected set; }

    private readonly List<Expression<Func<TEntity, bool>>> _criteria = criteria is null ? [] : [criteria];
    private readonly List<bool> _criteriaCondition = [criteriaCondition];

    public IReadOnlyList<Expression<Func<TEntity, bool>>> Criteria => _criteria.AsReadOnly();
    public IReadOnlyList<bool> CriteriaCondition => _criteriaCondition.AsReadOnly();

    public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? IncludeExpression { get; protected set; }

    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }

    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }

    public Specification<TEntity> AddInclude(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
    {
        if (includeExpression is null)
        {
            throw new InvalidOperationException("IncludeExpression is null");
        }

        IncludeExpression = includeExpression;

        return this;
    }

    protected Specification<TEntity> AddOrderBy(
        Expression<Func<TEntity, object>> orderByExpression)
    {
        if (orderByExpression is null)
        {
            throw new InvalidOperationException("OrderByExpression is null");
        }

        OrderByExpression = orderByExpression;

        return this;
    }

    protected Specification<TEntity> AddOrderByDescending(
        Expression<Func<TEntity, object>> orderByDescendingExpression)
    {
        if (orderByDescendingExpression is null)
        {
            throw new InvalidOperationException("OrderByDescendingExpression is null");
        }

        OrderByDescendingExpression = orderByDescendingExpression;

        return this;
    }

    protected Specification<TEntity> AddCriteria(
        Expression<Func<TEntity, bool>> criteria,
        bool condition = true)
    {
        if (criteria is null)
        {
            throw new InvalidOperationException("Criteria is null");
        }

        _criteria.Add(criteria);
        _criteriaCondition.Add(condition);

        return this;
    }

    public static Specification<TEntity> operator &(
        Specification<TEntity> specification1,
        Specification<TEntity> specification2)
    {
        return new AndSpecification<TEntity>(specification1, specification2);
    }
}


public class AndSpecification<TEntity> : Specification<TEntity> where TEntity : class
{
    public AndSpecification(Specification<TEntity> specification1, Specification<TEntity> specification2)
    {
        for (int i = 0; i < specification1.Criteria.Count(); i++)
        {
            AddCriteria(specification1.Criteria[i], specification1.CriteriaCondition[i]);
        }

        for (int i = 0; i < specification2.Criteria.Count(); i++)
        {
            AddCriteria(specification2.Criteria[i], specification2.CriteriaCondition[i]);
        }
    }
}
