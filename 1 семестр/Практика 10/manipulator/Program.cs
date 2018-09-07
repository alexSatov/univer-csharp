using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manipulator
{
    public static class Program
    {
        static string Message;
        static Form form;

        static double Wrist;
        static double Elbow;
        static double Shoulder;
        static ManipulatorLine[] lines;
        static double AlphaAngle;

        static void Paint(object sender, PaintEventArgs e)
        {
            //graphics - это графический контекст.
            //с его помощью вы можете рисовать
            var graphics = e.Graphics;
            //"Очистка экрана"
            graphics.Clear(Color.Beige);

            //Закомментируйте эту строку и почувствуйте разницу
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.FillRectangle(Brushes.DarkGray, 0, form.ClientSize.Height - 50, form.ClientSize.Width, form.ClientSize.Height);
            //Основные методы рисования
            var save = lines;
            DrawLines(graphics);
            DrawJoints(graphics);

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
                case "R":
                    AlphaAngle += 0.017;
                    ChangeAngles(lines[2].x2, lines[2].y2, AlphaAngle);
                    break;
                case "F":
                    AlphaAngle -= 0.017;
                    ChangeAngles(lines[2].x2, lines[2].y2, AlphaAngle);
                    break;

                case "W":
                    ChangeAngles(lines[2].x2, lines[2].y2 + 4, AlphaAngle);
                    break;
                case "S":
                    ChangeAngles(lines[2].x2, lines[2].y2 - 4, AlphaAngle);
                    break;
                case "A":
                    ChangeAngles(lines[2].x2 - 4, lines[2].y2, AlphaAngle);
                    break;
                case "D":
                    ChangeAngles(lines[2].x2 + 4, lines[2].y2, AlphaAngle);
                    break;
            }
            //Этот метод заставляет окно перерисовываться.
            //НЕ НАДО вызывать метод Paint вручную! 
            form.Invalidate();
        }

        private static void ChangeAngles(double x, double y, double angle)
        {
            var angles = RobotMathematics.MoveTo(x / 10, y / 10, angle);
            Shoulder = 2 * Math.PI - angles[0];
            Elbow = 2 * Math.PI - angles[1];
            Wrist = 2 * Math.PI - angles[2];

            lines[0].x2 = lines[0].x1 + RobotMathematics.UpperArm * Math.Cos(Shoulder) * 4;
            lines[0].y2 = lines[0].y1 + RobotMathematics.UpperArm * Math.Sin(Shoulder) * 4;

            lines[1].x1 = lines[0].x2;
            lines[1].y1 = lines[0].y2;

            lines[1].x2 = lines[1].x1 + RobotMathematics.Forearm * Math.Cos(Elbow + Shoulder - Math.PI) * 4;
            lines[1].y2 = lines[1].y1 + RobotMathematics.Forearm * Math.Sin(Elbow + Shoulder - Math.PI) * 4;

            lines[2].x1 = lines[1].x2;
            lines[2].y1 = lines[1].y2;

            lines[2].x2 = lines[2].x1 + RobotMathematics.Palm * Math.Cos(Wrist - Elbow - Shoulder) * 4;
            lines[2].y2 = lines[2].y1 + RobotMathematics.Palm * Math.Sin(Wrist - Elbow - Shoulder) * 4;
        }

        private static void LinesInit()
        {
            lines = new ManipulatorLine[3];            
            var x1 = 50;
            var y1 = form.ClientSize.Height - 50;
            var y2 = form.ClientSize.Height - 50 - RobotMathematics.UpperArm*4;

            lines[0] = new ManipulatorLine(x1, y1, x1, y2);
            lines[1] = new ManipulatorLine(x1, y2, x1, y2 - RobotMathematics.Forearm*4);
            lines[2] = new ManipulatorLine(x1, y2 - RobotMathematics.Forearm*4, x1, y2 - RobotMathematics.Forearm*4 - RobotMathematics.Palm*4);
        }

        private static void DrawLines(Graphics graphics)
        {
            var pen = new Pen(Color.Black, 2);            
            for (var i = 0; i < 3; i++)
                graphics.DrawLine(pen, (int)lines[i].x1, (int)lines[i].y1, (int)lines[i].x2, (int)lines[i].y2);
        }

        private static void DrawJoints(Graphics graphics)
        {
            for (var i = 0; i < 3; i++)
            {
                graphics.DrawEllipse(Pens.Black, (int)lines[i].x1 - 5, (int)lines[i].y1 - 5, 10, 10);
                if (i == 0) graphics.FillEllipse(Brushes.Red, (int)lines[i].x1 - 5, (int)lines[i].y1 - 5, 10, 10);
                else
                    graphics.FillEllipse(Brushes.Black, (int)lines[i].x1 - 5, (int)lines[i].y1 - 5, 10, 10);
            }

            graphics.DrawEllipse(Pens.Black, (int)lines[2].x2 - 5, (int)lines[2].y2 - 5, 10, 10);
            graphics.FillEllipse(Brushes.Green, (int)lines[2].x2 - 5, (int)lines[2].y2 - 5, 10, 10);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            form = new AntiFlickerForm(); //можете заменить AntiFlickerForm на Form, зажать какую-нибудь клавишу и почувствовать разницу
            form.Paint += Paint;
            form.KeyDown += KeyDown;
            form.ClientSize = new Size(300, 300);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.Text = "Manipulator";
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            AlphaAngle = 0;
            LinesInit();
            Application.Run(form);
        }


    }

    public class ManipulatorLine
    {
        public double x1;
        public double x2;
        public double y1;
        public double y2;

        public ManipulatorLine(double x1, double y1, double x2, double y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }       
    }
}
