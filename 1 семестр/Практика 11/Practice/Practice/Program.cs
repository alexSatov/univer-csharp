using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practice
{
    class Program
    {
        static Rectangle original;
        static Rectangle processed;
        static Point rotationCenter;
        static Form form;
        static string Message;
        static double angle;

        public static void DrawRect(Pen pen, Rectangle rect, Graphics graphics)
        {
            var sides = rect.GetAllSides();
            for (var i = 0; i < sides.Length; i++)
            {
                graphics.DrawLine(pen, (int)sides[i].startPoint.x, (int)sides[i].startPoint.y, (int)sides[i].endPoint.x, (int)sides[i].endPoint.y);
            }
        }

        void Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.Beige);

            var penOfStartRect = new Pen(Color.Black, 2);
            penOfStartRect.DashStyle = DashStyle.Dash;
            DrawRect(penOfStartRect, original, graphics);

            var penOfCurrentRect = new Pen(Color.Black, 2);
            DrawRect(penOfCurrentRect, processed, graphics);

            graphics.DrawEllipse(penOfCurrentRect, (int)rotationCenter.x - 5, (int)rotationCenter.y - 5, 10, 10);
            graphics.FillEllipse(Brushes.Red, (int)rotationCenter.x - 5, (int)rotationCenter.y - 5, 10, 10);

            if (Message != null)
            {
                graphics.DrawString(Message, new Font("Arial", 14), Brushes.Blue, 0, form.ClientSize.Height - 30);
            }


        }

        static void KeyDown(object sender, KeyEventArgs key)
        {
            Message = "Вы нажали " + key.KeyCode.ToString();

            switch (key.KeyCode.ToString())
            {
                case "D":
                    processed.Rotate(angle, rotationCenter);
                    break;

                case "A":
                    processed.Rotate(-angle, rotationCenter);
                    break;

            }
            form.Invalidate();
        }


        [STAThread]
        static void Main()
        {
            form = new Form();
            form.ClientSize = new Size(600, 600);
            var program = new Program();
            form.KeyDown += KeyDown;
            original = Geometry.CreateRectangle(new Point(40, 50), new Point(90, 100), new Point(120, 70));
            processed = Geometry.CreateRectangle(new Point(40, 50), new Point(90, 100), new Point(120, 70));
            rotationCenter = new Point(100, 100);
            angle = Math.PI / 10; // 0.017;
            form.Paint += program.Paint; // Окно form будет вызывать при каждой перерисовке метод program.Paint
            Application.Run(form);
        }
    }
}
