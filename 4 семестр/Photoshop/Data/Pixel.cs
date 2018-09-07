using System;

namespace MyPhotoshop
{
    public struct Pixel
    {
        public double R
        {
            get { return r; }
            set { r = CheckChannel(value); }
        }

        public double G
        {
            get { return g; }
            set { g = CheckChannel(value); }
        }

        public double B
        {
            get { return b; }
            set { b = CheckChannel(value); }
        }

        private double r;
        private double g;
        private double b;

        public Pixel(double red, double green, double blue)
        {
            r = g = b = 0;
            R = red;
            G = green;
            B = blue;
        }

        public Pixel ToGrayscale()
        {
            var y = R*0.299 + G*0.587 + B*0.114;
            return new Pixel(y, y, y);
        }

        public static Pixel operator *(Pixel pixel, double number)
        {
            return new Pixel(
                Trim(pixel.R*number),
                Trim(pixel.G*number),
                Trim(pixel.B*number));
        }

        public static Pixel operator *(double number, Pixel pixel)
        {
            return pixel*number;
        }

        private static double Trim(double value)
        {
            if (value < 0) return 0;
            return value > 255 ? 255 : value;
        }

        private static double CheckChannel(double channel)
        {
            if (channel < 0 || channel > 255)
                throw new Exception($"Wrong channel channel {channel} (the channel must be between 0 and 255");
            return channel;
        }
    }
}