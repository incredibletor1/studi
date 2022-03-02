using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Studi.Proctoring.BackOffice_Api.Models;

namespace Studi.Proctoring.BackOffice_Api.Helpers
{
    public static class LinqHelper
    {
        public static IOrderedQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyNameToSortBy, SortDirection sortDirection)
        {
            var parameter = Expression.Parameter(typeof(T), "OrderByOnPropertyName");
            var orderByExpression = Expression.Lambda(Expression.MakeMemberAccess(parameter, typeof(T).GetProperty(propertyNameToSortBy)), parameter);
            var resultExpression = Expression.Call(typeof(Queryable), sortDirection.ToString(), 
                                    new Type[] { typeof(T), typeof(T).GetProperty(propertyNameToSortBy).PropertyType },
                                    query.Expression, Expression.Quote(orderByExpression));

            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExpression);
        }
    }    
}
