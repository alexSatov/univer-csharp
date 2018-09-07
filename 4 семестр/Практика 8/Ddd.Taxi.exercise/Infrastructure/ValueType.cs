using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public class ValueType<T>
        where T : class
    {
        private static readonly PropertyInfo[] properties = typeof(T)
            .GetProperties()
            .Where(p => !p.GetGetMethod().IsStatic)
            .OrderBy(p => p.Name)
            .ToArray();

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || Equals(obj as T);
        }

        public bool Equals(T value)
        {
            return value != null && properties.All(p => Equals(p.GetValue(this), p.GetValue(value)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return properties.Sum(p => p.GetValue(this).GetHashCode());
            }
        }

        public override string ToString()
        {
            var propertiesInfo = properties
                .Where(p => !p.GetGetMethod().IsStatic)
                .OrderBy(p => p.Name)
                .Select(p =>$"{p.Name}: {p.GetValue(this)}");

            return $"{GetType().Name}({string.Join("; ", propertiesInfo)})";
        }
    }
}