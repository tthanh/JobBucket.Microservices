using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Helpers
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, object>> GetSortLambda<T>(string propertyPath)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var parts = propertyPath.Split('.');
            Expression parent = param;
            foreach (var part in parts)
            {
                parent = Expression.Property(parent, part);
            }

            if (parent.Type.IsValueType)
            {
                var converted = Expression.Convert(parent, typeof(object));
                return Expression.Lambda<Func<T, object>>(converted, param);
            }
            else
            {
                return Expression.Lambda<Func<T, object>>(parent, param);
            }
        }

        public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var body = MakeComparison(left, comparison, value);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression MakeComparison(Expression left, string comparison, string value)
        {
            switch (comparison)
            {
                case "==":
                    return MakeBinary(ExpressionType.Equal, left, value);
                case "!=":
                    return MakeBinary(ExpressionType.NotEqual, left, value);
                case ">":
                    return MakeBinary(ExpressionType.GreaterThan, left, value);
                case ">=":
                    return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
                case "<":
                    return MakeBinary(ExpressionType.LessThan, left, value);
                case "<=":
                    return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
                case "Contains":
                case "StartsWith":
                case "EndsWith":
                    return Expression.Call(MakeString(left), comparison, Type.EmptyTypes, Expression.Constant(value, typeof(string)));

                default:
                    throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
            }
        }



        private static Expression MakeString(Expression source)
        {
            return source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
        }

        private static Expression MakeBinary(ExpressionType type, Expression left, string value)
        {
            object typedValue = value;
            if (left.Type != typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    typedValue = null;
                    if (Nullable.GetUnderlyingType(left.Type) == null)
                        left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
                }
                else
                {
                    var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                    typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
                        valueType == typeof(Guid) ? Guid.Parse(value) :
                        valueType == typeof(DateTimeOffset) ? DateTimeOffset.Parse(value) :
                        Convert.ChangeType(value, valueType);
                }
            }
            var right = Expression.Constant(typedValue, left.Type);
            return Expression.MakeBinary(type, left, right);
        }

        public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, secondBody), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
        }

        public static Expression Replace(this Expression expression,
    Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }
        internal class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;
            public ReplaceVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }
            public override Expression Visit(Expression node)
            {
                return node == from ? to : base.Visit(node);
            }
        }
    }
}
