using System;
using System.Linq.Expressions;

namespace PO.Common.Helpers
{
    /// <summary>
    /// Exemple : PropertyHelper.GetPropertyName((RecapInfo r)=>r.Identite)
    /// </summary>
    public static class PropertyHelper
    {
        public static string GetPropertyName<T, TResult>(this T example, Expression<Func<T, TResult>> expression)
        {
            return GetPropertyName(expression);
        }

        public static string GetPropertyName<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            var memberExp = expression.Body as MemberExpression;
            if (memberExp == null) throw new ArgumentException("L'expression n'est pas un accès à une propriété", "expression");
            return memberExp.Member.Name;
        }
    }
}