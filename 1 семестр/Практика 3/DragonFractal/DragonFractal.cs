﻿using System;

// В этом пространстве имен содержатся средства для работы с изображениями. Чтобы оно стало доступно, в проект был подключен Reference на сборку System.Drawing.dll
using System.Drawing; 

// Это пространство имен содержит средства создания оконных приложений. В частности в нем находится класс Form.
// Для того, чтобы оно стало доступно, в проект был подключен на System.Windows.Forms.dll
using System.Windows.Forms;

namespace Fractals
{
	class DragonFractal
	{
		const int Size = 800;
		const int MarginSize = 100;

		static void Main()
		{
			var image = CreateDragonImage(100000);

			// При желании можно сохранить созданное изображение в файл вот так:
			// image.Save("dragon.png", ImageFormat.Png);
	
			ShowImageInWindow(image);
		}

		private static void ShowImageInWindow(Bitmap image)
		{
			// Создание нового окна заданного размера:
			var form = new Form
			{
				Text = "Harter–Heighway dragon",
				ClientSize = new Size(Size, Size)
			};

			//Добавляем специальный элемент управления PictureBox, который умеет отображать созданное нами изображение.
			form.Controls.Add(new PictureBox {Image = image, Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.CenterImage});
			form.ShowDialog();
		}


		static Bitmap CreateDragonImage(int iterationsCount)
		{
			var image = new Bitmap(Size, Size);
			var g = Graphics.FromImage(image);
			g.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);

            var random = new Random();
            double x = 1;
            double y = 0;
            double x1 = 0;
            int nextNumber = 0;
            double cosangle = Math.Cos(Math.PI / 4);
            double sinangle = Math.Sin(Math.PI / 4);
            double cosangle1 = Math.Cos(3 * Math.PI / 4);
            double sinangle1 = Math.Sin(3 * Math.PI / 4);

            for (int i = 0; i < iterationsCount; i++)
            {
                nextNumber = random.Next(2);
                if (nextNumber == 1)
                {
                    x1 = x;
                    x = (x * cosangle - y * sinangle) / Math.Sqrt(2);                    
                    y = (x1 * sinangle + y * cosangle) / Math.Sqrt(2);
                }
                else
                {
                    x1 = x;
                    x = (x * cosangle1 - y * sinangle1) / Math.Sqrt(2) + 1;                    
                    y = (x1 * sinangle1 + y * cosangle1) / Math.Sqrt(2);
                }
                SetPixel(image, x, y);
            }
                

            return image;
		}

		static void SetPixel(Bitmap image, double x, double y)
		{
			var xx = Scale(x, image.Width);
			var yy = Scale(y, image.Height);
			if (xx >=0 && xx < image.Width && yy >= 0 && yy < image.Height)
				image.SetPixel(xx, yy, Color.Yellow);
		}

		static int Scale(double x, double maxX)
		{
			return (int)Math.Round(maxX / 2.0 + (maxX / 2.0 - MarginSize) * x);
		}
	}
}
