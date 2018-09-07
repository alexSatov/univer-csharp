using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Reflection.Differentiation
{
    public class Algebra
    {
        public static Expression<Func<double, double>> Differentiate(LambdaExpression expr)
        {
            var x = expr.Parameters[0];
            var body = expr.Body;

            if (body is ConstantExpression)
                return ConstantDerivative(0, x);
            if (body is ParameterExpression)
                return ConstantDerivative(1, x);

            if (body is MethodCallExpression)
            {
                var methodExpr = (MethodCallExpression)body;
                var methodBody = methodExpr.Arguments[0];

                return FuncDerivative(methodExpr.Method.Name, methodBody, x);
            }

            var binaryBody = (BinaryExpression) body;
            return BinaryExprDerivative(binaryBody.NodeType, binaryBody, x);
        }

        public static Expression<Func<double, double>> ConstantDerivative(
            double constant, ParameterExpression parameter)
        {
            return Expression.Lambda<Func<double, double>>(Expression.Constant(constant), parameter);
        }

        public static Expression<Func<double, double>> FuncDerivative(
            string funcName, Expression funcBody, ParameterExpression parameter)
        {
            switch (funcName)
            {
                case "Sin":
                    return Expression.Lambda<Func<double, double>>(
                        Expression.Multiply(
                            Expression.Call(typeof(Math).GetMethod("Cos"), funcBody),
                            Differentiate(Expression.Lambda(funcBody, parameter)).Body), parameter);

                case "Cos":
                    return Expression.Lambda<Func<double, double>>(
                        Expression.Multiply(
                            Expression.Negate(Expression.Call(typeof(Math).GetMethod("Sin"), funcBody)),
                            Differentiate(Expression.Lambda(funcBody, parameter)).Body), parameter);

                default:
                    throw new NotImplementedException($"Производная функции {funcName} не реализована");
            }
        }

        public static Expression<Func<double, double>> BinaryExprDerivative(
            ExpressionType type, BinaryExpression binaryBody, ParameterExpression parameter)
        {
            switch (type)
            {
                case ExpressionType.Add:
                    return Expression.Lambda<Func<double, double>>(
                        Expression.Add(
                            Differentiate(Expression.Lambda(binaryBody.Left, parameter)).Body,
                            Differentiate(Expression.Lambda(binaryBody.Right, parameter)).Body), parameter);

                case ExpressionType.Multiply:
                    return Expression.Lambda<Func<double, double>>(
                        Expression.Add(
                            Expression.Multiply(
                                Differentiate(Expression.Lambda(binaryBody.Left, parameter)).Body,
                                binaryBody.Right),
                            Expression.Multiply(
                                binaryBody.Left,
                                Differentiate(Expression.Lambda(binaryBody.Right, parameter)).Body
                                )), parameter);
                default:
                    throw new NotImplementedException($"Производная операции {type} не реализована");
            }
        }
    }
}
