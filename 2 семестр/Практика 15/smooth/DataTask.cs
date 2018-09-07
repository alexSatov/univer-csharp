using System;
using System.Collections.Generic;
using System.Linq;

namespace yield
{
    public class DataPoint
	{
		public double X;
		public double OriginalY;
		public double ExpSmoothedY;
		public double AvgSmoothedY;
		public double MinY;
		public double MaxY;
	}
	
	public static class DataTask
	{
		public static IEnumerable<DataPoint> GetData(Random random)
		{
			return GenerateOriginalData(random).SmoothExponentialy(0.1).MovingAverage(20).MinMax(200);
		}

		public static IEnumerable<DataPoint> GenerateOriginalData(Random random)
		{
			var x = 0;
			while (true)
			{
				x++;
				var y = 10 * (1 - (x / 100) % 2) + 3 * Math.Sin(x / 40.0) + 2*random.NextDouble() - 1 + 3;
				yield return new DataPoint { X = x, OriginalY = y };
			}
		}

		public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
		{
            double previous = double.NaN;            
            foreach (var d in data)
            {
                if (double.IsNaN(previous))
                {
                    previous = d.OriginalY;                                       
                }                               
                d.ExpSmoothedY = previous + alpha * (d.OriginalY - previous);
                previous = d.ExpSmoothedY;
                yield return d;                
            }                       
        }

        public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> data, int windowWidth)
		{
            var q = new Queue<double>();
            double sum = 0;
            foreach (var d in data)
            {
                sum += d.OriginalY;
                q.Enqueue(d.OriginalY);
                if (q.Count > windowWidth)
                {
                    sum -= q.Dequeue();
                }
                d.AvgSmoothedY = sum / q.Count;
                yield return d;
            }
        }
		
		public static IEnumerable<DataPoint> MinMax(this IEnumerable<DataPoint> data, int windowWidth)
		{
            var queue = new Queue<double>();
            var min = double.MaxValue;
            var max = double.MinValue;
            foreach (var d in data)
            {
                if (queue.Count < windowWidth)
                {
                    queue.Enqueue(d.OriginalY);
                    if (d.OriginalY < min)
                        min = d.OriginalY;
                    if (d.OriginalY > max)
                        max = d.OriginalY;
                    d.MinY = min;
                    d.MaxY = max;
                    yield return d;
                }
                else
                {                                      
                    if (queue.Peek() == min)
                    {
                        queue.Dequeue();
                        min = queue.Min();                        
                    }
                    else if (queue.Peek() == max)
                    {
                        queue.Dequeue();
                        max = queue.Max();                        
                    }
                    else
                        queue.Dequeue();
                    queue.Enqueue(d.OriginalY);
                    if (d.OriginalY < min)
                        min = d.OriginalY;
                    if (d.OriginalY > max)
                        max = d.OriginalY;
                    d.MinY = min;
                    d.MaxY = max;
                    yield return d;
                }
            }
                   
            //foreach (var d in data)
            //{
            //    if (queue.Count >= windowWidth)
            //        queue.Dequeue();
            //    else
            //    {
            //        queue.Enqueue(d.OriginalY);
            //        d.MaxY = queue.Max();
            //        d.MinY = queue.Min();
            //        yield return d;
            //    }
            //}
        }
    }
}