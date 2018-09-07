using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace func_rocket
{
	public class GameForm : Form
	{
		private Image rocket;
		private Image target;
		private Timer timer;
		private ComboBox spaces;
		private GameSpace space;
		private bool right;
		private bool left;
        private Func<Rocket, Vector, Turn> ControlRocket;   
        bool KeyPressed{ get { return left || right; } }     

		public GameForm(IEnumerable<GameSpace> gameSpaces, Func<Rocket, Vector, Turn> controlRocket)
		{
			DoubleBuffered = true;
			Text = "Use Left and Right arrows to control rocket";
			rocket = Image.FromFile("images/rocket.png");
			target = Image.FromFile("images/target.png");
			timer = new Timer();
			timer.Interval = 10;
			timer.Tick += TimerTick;
			timer.Start();
			WindowState = FormWindowState.Maximized;
			spaces = new ComboBox();
			KeyPreview = true;
			spaces.SelectedIndexChanged += SpaceChanged;
			spaces.Parent = this;
			spaces.Items.AddRange(gameSpaces.Cast<object>().ToArray());
			Controls.Add(spaces);
            ControlRocket = controlRocket; 
			this.Focus();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			if (spaces.Items.Count > 0) 
				spaces.SelectedIndex = 0;
		}

		private void SpaceChanged(object sender, EventArgs e)
		{
			space = (GameSpace) spaces.SelectedItem;
			timer.Start();
		}
        
        private void TimerTick(object sender, EventArgs e)
		{
			if (space == null) return;
            Turn control;
            if (KeyPressed)
                control = left ? Turn.Left : (right ? Turn.Right : Turn.None);
            else
                control = Tasks.ControlRocket(space.Rocket, space.Target);
            space.Move(ClientRectangle, control);
			if ((space.Rocket.Location - space.Target).Length < 20)
				timer.Stop();
			Invalidate();
			Update();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Left) left = true;
			if (e.KeyCode == Keys.Right) right = true;
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Left) left = false;
			if (e.KeyCode == Keys.Right) right = false;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillRectangle(Brushes.Beige, ClientRectangle);

			if (space == null) return;

			DrawGravity(g);
			var matrix = g.Transform;

			g.TranslateTransform((float)space.Target.X, (float)space.Target.Y);
			g.DrawImage(target, new Point(-target.Width / 2, -target.Height / 2));

			if (timer.Enabled)
			{
				g.Transform = matrix;
				g.TranslateTransform((float) space.Rocket.Location.X, (float) space.Rocket.Location.Y);
				g.RotateTransform(90 + (float) (space.Rocket.Direction*180/Math.PI));
				g.DrawImage(rocket, new Point(-rocket.Width/2, -rocket.Height/2));
			}
		}

		private void DrawGravity(Graphics g)
		{
			Action<Vector, Vector> draw = (a,b) => g.DrawLine(Pens.DeepSkyBlue, (int)a.X, (int)a.Y, (int)b.X, (int)b.Y);
			for (int x = 0; x < ClientRectangle.Width; x += 50)
				for (int y = 0; y < ClientRectangle.Height; y += 50)
				{
					var p1 = new Vector(x, y);
					var v = space.Gravity(p1);
					var p2 = p1 + 20 * v;
					var end1 = p2 - 8*v.Rotate(0.5);
					var end2 = p2 - 8*v.Rotate(-0.5);
					draw(p1, p2);
					draw(p2, end1);
					draw(p2, end2);
				}
		}
	}
}