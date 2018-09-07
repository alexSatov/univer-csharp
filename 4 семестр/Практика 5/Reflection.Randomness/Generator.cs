using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Reflection.Randomness
{
    public class FromDistribution : Attribute
    {
        public Type DistributionType { get; }
        public double[] Values { get; }

        public FromDistribution(Type type, params double[] values)
        {
            if (type.GetFields().Length != values.Length)
                throw new ArgumentException(
                    $"Количество полей класса {type} не совпадает с количеством переданных значений.");

            DistributionType = type;
            Values = values;
        }
    }

    public class Generator<T>
    {
        private static readonly Dictionary<Type, Func<double[], IContinousDistribution>> distributionsInitializers =
            new Dictionary<Type, Func<double[], IContinousDistribution>>
            {
                [typeof(NormalDistribution)] = CreateDistributionInitializerLambda<NormalDistribution>(),
                [typeof(ExponentialDistribution)] = CreateDistributionInitializerLambda<ExponentialDistribution>()
            };

        private readonly Dictionary<string, IContinousDistribution> customizableProperties 
            = new Dictionary<string, IContinousDistribution>();
        
        public T Generate(Random rnd)
        {
            var properties = typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(FromDistribution), false).Length > 0)
                .ToArray();

            var distributions = properties
                .Select(p => p.GetCustomAttributes(typeof(FromDistribution), false))
                .Where(d => d.Length != 0)
                .Select(d => d[0])
                .Cast<FromDistribution>()
                .Select(d => distributionsInitializers[d.DistributionType](d.Values))
                .ToArray();

            var values = Enumerable.Range(0, properties.Length)
                .Select(i => customizableProperties.ContainsKey(properties[i].Name)
                    ? customizableProperties[properties[i].Name].Generate(rnd)
                    : distributions[i].Generate(rnd))
                .ToArray();

            var instance = CreateClassInitializerLambda(properties)(values);
            return instance;
        }

        public CustomizableGenerator For(Expression<Func<T, double>> expr)
        {
            if (expr.Body is MemberExpression)
            {
                var memberExp = (MemberExpression)expr.Body;
                if (memberExp.Member.MemberType == MemberTypes.Property)
                    return new CustomizableGenerator(memberExp.Member.Name, this);
            }

            throw new ArgumentException($"Выражение-делегат должно быть обращением к свойству класса {typeof(T)}");
        }

        private static Func<double[], T> CreateClassInitializerLambda(PropertyInfo[] properties)
        {
            var values = Expression.Parameter(typeof(double[]), "values");
            var bindings = new MemberBinding[properties.Length];
            for (var i = 0; i < properties.Length; i++)
                bindings[i] = Expression.Bind(properties[i], Expression.ArrayIndex(values, Expression.Constant(i)));

            var body = Expression.MemberInit(
                Expression.New(typeof(T).GetConstructor(new Type[0])),
                bindings);

            return Expression.Lambda<Func<double[], T>>(body, values).Compile();
        }

        private static Func<double[], TDist> CreateDistributionInitializerLambda<TDist>()
            where TDist : IContinousDistribution
        {
            var values = Expression.Parameter(typeof(double[]), "values");
            var fields = typeof(TDist).GetFields();

            var args = new Expression[fields.Length];
            for (var i = 0; i < fields.Length; i++)
                args[i] = Expression.ArrayIndex(values, Expression.Constant(i));

            var constructorTypes = Enumerable.Range(0, fields.Length).Select(i => typeof(double)).ToArray();
            var body = Expression.MemberInit(Expression.New(typeof(TDist).GetConstructor(constructorTypes), args));

            var distributionInit = Expression.Lambda<Func<double[], TDist>>(body, values);
            return distributionInit.Compile();
        }

        public class CustomizableGenerator
        {
            private readonly string propertyName;
            private readonly Generator<T> mainGenerator;

            public CustomizableGenerator(string property, Generator<T> mainGenerator)
            {
                propertyName = property;
                this.mainGenerator = mainGenerator;
            }

            public Generator<T> Set(IContinousDistribution distribution)
            {
                mainGenerator.customizableProperties[propertyName] = distribution;
                return mainGenerator;
            }
        }

        //private static Dictionary<Type, Func<double[], IContinousDistribution>> CreateDistributionsInitializers()
        //{
        //    return typeof(IContinousDistribution).Assembly.DefinedTypes
        //        .Where(t => t.ImplementedInterfaces.Any(i => i.FullName == typeof(IContinousDistribution).FullName))
        //        .Cast<Type>()
        //        .Select(t =>
        //            Tuple.Create(t,
        //                typeof(Generator<T>).GetMethod("CreateDistributionInitializerLambda").MakeGenericMethod(t)))
        //        .ToDictionary(t => t.Item1,
        //            t =>
        //                (Func<double[], IContinousDistribution>)
        //                t.Item2.CreateDelegate(typeof(Func<double[], IContinousDistribution>)));

        //}
    }
}
