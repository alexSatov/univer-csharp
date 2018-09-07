using System.Drawing;

namespace MyPhotoshop
{
    public interface ITransformer<in TParams>
        where TParams: IParameters, new()
    {
        Size NewSize { get; }
        Point? MapPoint(Point point);
        void Prepare(Size oldSize, TParams parameters);
    }
}