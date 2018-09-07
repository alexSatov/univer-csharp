using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognizer
{
    class Tasks
    {
        /* Задача #1 (1 балл)
		 * Переведите изображение в серую гамму.
		 * 
		 * original[x,y,i] - это красный (i=0), зеленый (1) и синий (2) 
		 * каналы пикселя с координатами x,y. Каждый канал лежит в диапазоне от 0 до 255.
		 * 
		 * Получившийся массив должен иметь тот же размер, 
		 * grayscale[x,y] - яркость от 0 до 1 пикселя в координатах x,y
		 *
		 * Используйте формулу http://ru.wikipedia.org/wiki/Оттенки_серого
		 */
        public static double[,] Grayscale(byte[,,] original)
        {
            var grayscale = new double[original.GetLength(0), original.GetLength(1)];
            for (int y = 0; y < original.GetLength(1); y++)
                for (int x = 0; x < original.GetLength(0); x++)
                {
                    grayscale[x, y] = (original[x, y, 0] * 0.299 + original[x, y, 1] * 0.587 + original[x, y, 2] * 0.114)/255;
                }
            return grayscale;
        }


        /* Задача #2 (1 балл)
		 * Очистите переданное изображение от шума.
		 * 
		 * Пиксели шума (и только они) имеют черный цвет (0,0,0) или белый цвет (255,255,255)
		 * Вам нужно заменить эти цвета на средний цвет соседних пикселей.
		 * Соседними считаются 4 ближайших пиксела, если пиксел не у края изображения.
		 */
        public static void ChangeToAverageColor (byte[,,] original, int X, int Y)
        {
            
            int redAverageCanal = 0, greenAverageCanal = 0, blueAverageCanal = 0, count = 0;
            foreach (var i in new int[] { -1, 0, 1})
            {
                foreach (var j in new int[] { -1, 0, 1 })
                {
                    if (i != j && i != -j)
                    {
                       if (X + i >= 0 && X + i < original.GetLength(0) && Y + j >= 0 && Y + j < original.GetLength(1) && !IsNoise(original[X + i, Y + j, 0], original[X + i, Y + j, 1], original[X + i, Y + j, 2]))
                        {
                            redAverageCanal += original[X + i, Y + j, 0];
                            greenAverageCanal += original[X + i, Y + j, 1];
                            blueAverageCanal += original[X + i, Y + j, 2];
                            count++;
                        }
                    }
                }

            }          
            original[X, Y, 0] = (byte)Math.Round((double)redAverageCanal / count);
            original[X, Y, 1] = (byte)Math.Round((double)greenAverageCanal / count);
            original[X, Y, 2] = (byte)Math.Round((double)blueAverageCanal / count);
        }

        public static bool IsNoise (byte redCanal, byte greenCanal, byte blueCanal)
        {
            return    (redCanal == 0 && greenCanal == 0 && blueCanal == 0) 
                   || (redCanal == 255 && greenCanal == 255 && blueCanal == 255);
        }

        public static void ClearNoise(byte[,,] original)
        {            
            for (int y = 0; y < original.GetLength(1); y++)
                for (int x = 0; x < original.GetLength(0); x++)
                {
                    if (IsNoise(original[x, y, 0], original[x, y, 1], original[x, y, 2]))
                        ChangeToAverageColor(original, x, y);
                }
        }

        /* Задача #3 (1 балл)
		* Замените все цвета, встречающиеся у 10% самых ярких пикселей на белый, а остальные — на черный цвет. 
		* Этот процесс называется пороговая бинаризация изображения.
		*/
        public static void ThresholdFiltering(double[,] original)
        {
            int verge = 0;
            double value = 0;
            var difPixels = new List<double>();
            for (int y = 0; y < original.GetLength(1); y++)
                for (int x = 0; x < original.GetLength(0); x++)
                {
                    if (!difPixels.Contains(original[x, y]))
                        difPixels.Add(original[x, y]);
                }
            difPixels.Sort();
            difPixels.Reverse();
            verge = (int)Math.Round(difPixels.Count * 0.1);
            value = difPixels[verge];
            for (int y = 0; y < original.GetLength(1); y++)
                for (int x = 0; x < original.GetLength(0); x++)
                {
                    if (original[x, y] >= value)
                        original[x, y] = 1;
                    else
                        original[x, y] = 0;
                }
        }


        /* Задача #4 (1 балл)
		Разберитесь, как работает нижеследующий код (называемый фильтрацией Собеля), 
		и какое отношение к нему имеют эти матрицы:
		
		   | -1 -2 -1 |         | -1  0  1 |  
		Gx=|  0  0  0 |      Gy=| -2  0  2 |       
		   |  1  2  1 |         | -1  0  1 |    
		
		https://ru.wikipedia.org/wiki/%D0%9E%D0%BF%D0%B5%D1%80%D0%B0%D1%82%D0%BE%D1%80_%D0%A1%D0%BE%D0%B1%D0%B5%D0%BB%D1%8F
		
		Попробуйте заменить фильтр Собеля 3x3 на фильтр Собеля 5x5 и сравните результаты. 
		http://www.cim.mcgill.ca/~image529/TA529/Image529_99/assignments/edge_detection/references/sobel.htm

		Выберите фильтр, который лучше выделяет границы на изображении.

		Обобщите код применения фильтра так, чтобы можно было передавать ему любые матрицы, любого нечетного размера.
		Фильтры Собеля размеров 3 и 5 должны быть частным случаем. 
		После такого обобщения менять фильтр Собеля одного размера на другой будет легко.
		*/
        public static double[,] SobelFiltering(double[,] g)
        {
            var width = g.GetLength(0);
            var height = g.GetLength(1);
            var result = new double[width, height];
            for (int x = 2; x < width - 2; x++)
                for (int y = 2; y < height - 2; y++)
                {
                    var gx = -g[x - 2, y - 2] - 4 * g[x - 1, y - 2] - 6 * g[x, y - 2] - 4 * g[x + 1, y - 2] - g[x + 2, y - 2]
                             - 2 * g[x - 2, y - 1] - 8 * g[x - 1, y - 1] - 12 * g[x, y - 1] - 8 * g[x + 1, y - 1] - 2 * g[x + 2, y - 1]
                             + 2 * g[x - 2, y + 1] + 8 * g[x - 1, y + 1] + 12 * g[x, y + 1] + 8 * g[x + 1, y + 1] + 2 * g[x + 2, y + 1]
                             + g[x - 2, y + 2] + 4 * g[x - 1, y + 2] + 6 * g[x, y + 2] + 4 * g[x + 1, y + 2] + g[x + 2, y + 2];

                    var gy = -g[x - 2, y - 2] - 4 * g[x - 2, y - 1] - 6 * g[x - 2, y] - 4 * g[x - 2, y + 1] - g[x - 2, y + 2]
                             - 2 * g[x - 1, y - 2] - 8 * g[x - 1, y - 1] - 12 * g[x - 1, y] - 8 * g[x - 1, y + 1] - 2 * g[x - 1, y + 2]
                             + 2 * g[x + 1, y - 2] + 8 * g[x + 1, y - 1] + 12 * g[x + 1, y] + 8 * g[x + 1, y + 1] + 2 * g[x + 1, y + 2]
                             + g[x + 2, y - 2] + 4 * g[x + 2, y - 1] + 6 * g[x + 2, y] + 4 * g[x + 2, y + 1] + g[x + 2, y + 2];
                    result[x, y] = Math.Sqrt(gx * gx + gy * gy);
                }
            return result;
        }



        /* Задача #5 (без баллов): 
		Реализуйте или используйте готовый алгоритм Хафа для поиска аналитических координат прямых на изображений
		http://ru.wikipedia.org/wiki/Преобразование_Хафа
		*/
        public static Line[] HoughAlgorithm(double[,] original)
        {
            var width = original.GetLength(0);
            var height = original.GetLength(1);
            return new[] { new Line(0, 0, width, height), new Line(0, height, width, 0) };
        }


    }
}

