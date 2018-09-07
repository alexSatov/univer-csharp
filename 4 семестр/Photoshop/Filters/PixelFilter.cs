using System;

namespace MyPhotoshop
{
    public class PixelFilter<T> : ParametrizedFilter<T>
        where T : IParameters, new()
    {
        private readonly Func<Pixel, T, Pixel> processPixel;

        public PixelFilter(string filterName, Func<Pixel, T, Pixel> processPixel) : base(filterName)
        {
            this.processPixel = processPixel;
        }

        public override Photo Process(Photo original, T parameters)
        {
            var result = new Photo(original.Width, original.Height);

            for (var x = 0; x < result.Width; x++)
                for (var y = 0; y < result.Height; y++)
                    result[x, y] = processPixel(original[x, y], parameters);

            return result;
        }
    }
}