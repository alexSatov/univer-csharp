using System;
using System.Drawing;
using System.Linq;

namespace RoutePlanning
{
	public class PathFinder
	{
		public static int[] FindBestCheckpointsOrder(Point[] checkpoints)
		{
			// Задачи 1-3
			// Переделайте этот метод так, чтобы он находил кратчайший порядок посещения чекпоинтов.
			var bestOrder = MakeTrivialPermutation(checkpoints.Length);            
            MakePermutations(new int[checkpoints.Length], 1, bestOrder, checkpoints);           
            return bestOrder;
        }

        static void MakePermutations(int[] permutation, int position, int[] bestOrder, Point[] checkpoints)
        {
            if (position == permutation.Length)
            {
                Evaluate(permutation, bestOrder, checkpoints);
                return;
            }

            for (int i = 0; i < permutation.Length; i++)
            {
                var index = Array.IndexOf(permutation, i, 0, position);
                if (index != -1)
                    continue;
                permutation[position] = i;
                if (GetPathLen(checkpoints, permutation.Take(position + 1).ToArray()) > GetPathLen(checkpoints, bestOrder))
                    continue;
                else
                MakePermutations(permutation, position + 1, bestOrder, checkpoints);
            }
        }

        static public double PermutationDistance(int[] permutation, Point[] checkpoints)
        {
            double distance = 0;
            for (int i = 1; i < permutation.Length; i++)
                distance += Distance(checkpoints[permutation[i - 1]], checkpoints[permutation[i]]);
            return distance;
        }
        static void Evaluate(int[] permutation, int[] bestOrder, Point[] checkpoints)
        {
            var distance = GetPathLen(checkpoints, permutation);
            if (GetPathLen(checkpoints, bestOrder) > 0)
            {
                if (GetPathLen(checkpoints, bestOrder) > distance)
                   {
                       UpdateBestPermutation(permutation, bestOrder, distance);                    
                   }
            }
            else
            {
                UpdateBestPermutation(permutation, bestOrder, distance);                
            }

        }

        static void UpdateBestPermutation(int[] permutation, int[] bestOrder, double distance)
        {
            for (int i = 0; i < bestOrder.Length - 1; i++)
                bestOrder[i] = permutation[i];            
        }



        public static int[] FindBestCheckpointsOrder(Point[] checkpoints, TimeSpan timeout)
		{
			//Задача 4
			// Доделайте этот метод, чтобы он искал сначала рассматривал наиболее перспективные маршруты, и прекращал поиск через заданный timеout.

			var startTime = DateTime.Now;
			var bestFoundSolution = MakeTrivialPermutation(checkpoints.Length);
			//Ищем...
			//Если у нас кончилось время — возвращаем лучшее из того, что нашли.
			if (DateTime.Now - startTime > timeout) return bestFoundSolution;
			//Иначе, продолжаем искать.
			return bestFoundSolution;
		}


		#region Вспомогательные функции, можете использовать их, если понадобятся.

		public static int[] MakeTrivialPermutation(int size)
		{
			var bestOrder = new int[size];
			for (int i = 0; i < bestOrder.Length; i++)
				bestOrder[i] = i;
			return bestOrder;
		}

		public static double GetPathLen(Point[] checkpoints, int[] order)
		{
			Point prevPoint = checkpoints[0];
			var len = 0.0;
			foreach (int checkpointIndex in order)
			{
				len += Distance(prevPoint, checkpoints[checkpointIndex]);
				prevPoint = checkpoints[checkpointIndex];
			}
			return len;
		}

		public static double Distance(Point a, Point b)
		{
			var dx = a.X - b.X;
			var dy = a.Y - b.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		#endregion
	}
}