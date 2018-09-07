using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter<TParams> : ParametrizedFilter<TParams>
        where TParams : IParameters, new()
    {
        private readonly ITransformer<TParams> transformer;

        public TransformFilter(string filterName, ITransformer<TParams> transformer)
            : base(filterName)
        {
            this.transformer = transformer;
        }

        public override Photo Process(Photo original, TParams parameters)
        {
            var originalSize = new Size(original.Width, original.Height);
            transformer.Prepare(originalSize, parameters);
            var newSize = transformer.NewSize;
            var result = new Photo(newSize.Width, newSize.Height);

            for (var x = 0; x < result.Width; x++)
                for (var y = 0; y < result.Height; y++)
                {
                    var pointFrom = transformer.MapPoint(new Point(x, y));
                    if (pointFrom.HasValue)
                        result[x, y] = original[pointFrom.Value.X, pointFrom.Value.Y];
                }

            return result;
        }
    }
}