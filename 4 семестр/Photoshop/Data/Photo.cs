namespace MyPhotoshop
{
	public class Photo
	{
	    public readonly int Width;
	    public readonly int Height;
	    private readonly Pixel[,] data;

	    public Photo(int width, int height)
	    {
	        Width = width;
	        Height = height;
            data = new Pixel[Width, Height];
	    }

	    public Pixel this[int x, int y]
	    {
	        get { return data[x, y]; }
	        set { data[x, y] = value; }
	    }
	}
}

