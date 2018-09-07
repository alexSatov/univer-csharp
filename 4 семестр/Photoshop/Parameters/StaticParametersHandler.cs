using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public class StaticParametersHandler<TParams> : IParametersHandler<TParams>
        where TParams : IParameters, new()
    {
        private static PropertyInfo[] properties;
        private static ParameterInfo[] descriptions;

        public StaticParametersHandler()
        {
            properties = typeof(TParams)
            .GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(ParameterInfo), false).Length > 0)
            .ToArray();

            descriptions = properties
                .Select(p => p.GetCustomAttributes(typeof(ParameterInfo), false)[0])
                .Cast<ParameterInfo>()
                .ToArray();
        }

        public ParameterInfo[] GetDescription()
        {
            return descriptions;
        }

        public TParams CreateParams(double[] values)
        {
            var parameters = new TParams();

            for (var i = 0; i < values.Length; i++)
                properties[i].SetValue(parameters, values[i], new object[0]);

            return parameters;
        }
    }
}