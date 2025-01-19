namespace SharedKernel;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(
        this IQueryable<TEntity> inputQueryable,
        Specification<TEntity> specification)
        where TEntity : class
    {
        IQueryable<TEntity> queryable = inputQueryable;

        if (specification.Criteria is not null)
        {
            for (int i = 0; i < specification.Criteria.Count; i++)
            {
                queryable = queryable.WhereIf(
                    specification.CriteriaCondition[i],
                    specification.Criteria[i]);
            }
        }

        if (specification.IncludeExpression is not null)
        {
            queryable = specification.IncludeExpression(queryable);
        }

        if (specification.OrderByExpression is not null)
        {
            queryable = queryable.OrderBy(specification.OrderByExpression);
        }
        else if (specification.OrderByDescendingExpression is not null)
        {
            queryable = queryable.OrderByDescending(specification.OrderByDescendingExpression);
        }

        return queryable;
    }
}
