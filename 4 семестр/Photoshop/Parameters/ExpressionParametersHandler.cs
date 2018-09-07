using System;
using System.Linq;
using System.Linq.Expressions;

namespace MyPhotoshop
{
    public class ExpressionParametersHandler<TParams> : IParametersHandler<TParams>
        where TParams : IParameters, new()
    {
        private static ParameterInfo[] descriptions;
        private static Func<double[], TParams> parser;

        public ExpressionParametersHandler()
        {
            descriptions = typeof(TParams)
                .GetProperties()
                .Select(p => p.GetCustomAttributes(typeof(ParameterInfo), false))
                .Where(p => p.Length != 0)
                .Select(p => p[0])
                .Cast<ParameterInfo>()
                .ToArray();

            // values = new LighteningParameters { Coefficient = values[0] };

            var properties = typeof(TParams)
            .GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(ParameterInfo), false).Length > 0)
            .ToArray();

            var arg = Expression.Parameter(typeof(double[]), "values");

            var bindings = properties
                .Select(t => Expression.Bind(t, Expression.ArrayIndex(arg, Expression.Constant(0))))
                .Cast<MemberBinding>()
                .ToList();

            var body = Expression.MemberInit(
                Expression.New(typeof(TParams).GetConstructor(new Type[0])), 
                bindings);

            var lambda = Expression.Lambda<Func<double[], TParams>>(body, arg);
            parser = lambda.Compile();
        }

        public ParameterInfo[] GetDescription()
        {
            return descriptions;
        }

        public TParams CreateParams(double[] values)
        {
            return parser(values);
        }
    }
}