namespace MyPhotoshop
{
    public abstract class ParametrizedFilter<TParams> : IFilter
        where TParams : IParameters, new()
    {
        public readonly string FilterName;
        
        private readonly IParametersHandler<TParams> handler = new ExpressionParametersHandler<TParams>();

        protected ParametrizedFilter(string filterName)
        {
            FilterName = filterName;
        }

        public abstract Photo Process(Photo original, TParams parameters);

        public ParameterInfo[] GetParameters()
        {
            return handler.GetDescription();
        }

        public Photo Process(Photo original, double[] values)
        {
            var parameters = handler.CreateParams(values);
            return Process(original, parameters);
        }

        public override string ToString()
        {
            return FilterName;
        }
    }
}