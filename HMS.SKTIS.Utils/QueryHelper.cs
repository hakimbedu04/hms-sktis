using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HMS.SKTIS.Utils
{
    public static class QueryHelper
    {
        public static Func<IQueryable<T>, IOrderedQueryable<T>> GetOrderByFunc<T>(this Tuple<IEnumerable<string>, string> sortCriteria)
        {
            Func<IQueryable<T>, IOrderedQueryable<T>> orderByFilter = null;

            if (sortCriteria.Item1.FirstOrDefault() != null)
            {
                //todo: need to fix for multiple order
                var selector = GetSelector<T>(sortCriteria.Item1.FirstOrDefault());
                Type[] argumentTypes = new[] { typeof(T), selector.Item2 };

                if (sortCriteria.Item2 == "DESC")
                {
                    var orderByDescMethod = typeof(Queryable).GetMethods()
                    .First(method => method.Name == "OrderByDescending"
                                     && method.GetParameters().Count() == 2)
                    .MakeGenericMethod(argumentTypes);

                    orderByFilter = query => (IOrderedQueryable<T>)
                        orderByDescMethod.Invoke(null, new object[] { query, selector.Item1 });
                }
                else
                {
                    var orderByMethod = typeof(Queryable).GetMethods()
                    .First(method => method.Name == "OrderBy"
                                     && method.GetParameters().Count() == 2)
                    .MakeGenericMethod(argumentTypes);

                    orderByFilter = query => (IOrderedQueryable<T>)
                        orderByMethod.Invoke(null, new object[] { query, selector.Item1 });
                }
            }

            return orderByFilter;
        }

        private static Tuple<Expression, Type> GetSelector<T>(string propertyName)
        {
            Type selectorResultType;
            var expression = GenerateSelector<T>(propertyName, out selectorResultType);

            return Tuple.Create((Expression)expression, selectorResultType);
        }

        private static LambdaExpression GenerateSelector<T>(string propertyName, out Type resultType)
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(T), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                String[] childProperties = propertyName.Split('.');
                property = typeof(T).GetProperty(childProperties[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (int i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }
    }
}
