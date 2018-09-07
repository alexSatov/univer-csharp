using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyPhotoshop
{
	class MainClass
	{
        [STAThread]
		public static void Main (string[] args)
		{
			var window = new MainWindow();

			window.AddFilter (new PixelFilter<LighteningParameters>("����������/����������", 
                (pixel, parameters) => pixel * parameters.Coefficient));

			window.AddFilter (new PixelFilter<GrayscaleParameters>("������� ������",
                (pixel, parameters) => pixel.ToGrayscale()));

            window.AddFilter(new TransformFilter("������� �� 90 �������� �� �.�.",
                size => new Size(size.Height, size.Width),
                (point, size) => new Point(point.Y, size.Height - point.X - 1)));

            window.AddFilter(new TransformFilter("����������� �� �����������",
                size => new Size(size.Width, size.Height),
                (point, size) => new Point(size.Width - point.X - 1, point.Y)));

            window.AddFilter(new TransformFilter<RotationParameters>("��������� ��������", new RotationTransformer()));

            Application.Run(window);
		}
	}
}
