using System.Drawing;

namespace MyPhotoshop
{
	public static class Convertors
	{
		public static Photo Bitmap2Photo(Bitmap bmp)
		{
		    var photo = new Photo(bmp.Width, bmp.Height);

		    for (var x = 0; x < bmp.Width; x++)
				for (var y = 0; y < bmp.Height; y++)
				{
				    var pixel = bmp.GetPixel(x,y);
				    photo[x, y] = new Pixel(pixel.R, pixel.G, pixel.B);
				}

			return photo;
		}
		
		public static Bitmap Photo2Bitmap(Photo photo)
		{
			var bmp = new Bitmap(photo.Width, photo.Height);
            for (var x = 0; x < bmp.Width; x++)
                for (var y = 0; y < bmp.Height; y++)
                    bmp.SetPixel(x, y, Color.FromArgb (
						(int) photo[x,y].R,
						(int) photo[x,y].G,
						(int) photo[x,y].B));
					       		
			return bmp;
		}
	}
}

