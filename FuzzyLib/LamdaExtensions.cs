using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FuzzyLib
{
    public static class LamdaExtensions
    {
        public static PropertyInfo GetPropertyInfo<T, TProperty>(this Expression<Func<T, TProperty>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            return propInfo;
        }        
    }
}