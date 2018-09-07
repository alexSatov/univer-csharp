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
        private static Func<Rocket, Vector, Turn> RocketAutopilot;
        private bool isUserControl;
        private ProgressBar FuelControlIndicator;
        private TableLayoutPanel controlPanel;

        public GameForm(IEnumerable<GameSpace> gameSpaces, Func<Rocket, Vector, Turn> rocketAutopilot)
		{
            RocketAutopilot = rocketAutopilot;
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
            InitControlPanel();

			this.Focus();
		}

        private void InitFuelControlIndicator()
        {
            FuelControlIndicator = new ProgressBar();
            FuelControlIndicator.Maximum = 1000;
            FuelControlIndicator.Size = new System.Drawing.Size(150, 25);
            FuelControlIndicator.Step = -1;
        }

        private void InitControlPanel()
        {
            InitFuelControlIndicator();

            var indicatorText = new Label();
            indicatorText.Text = "Fuel:";
            indicatorText.Size = new System.Drawing.Size(30, 30);
            indicatorText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            controlPanel = new TableLayoutPanel();
            controlPanel.Size = new System.Drawing.Size(Size.Width, 30);
            controlPanel.ColumnCount = 3;
            controlPanel.RowCount = 1;
            controlPanel.BackColor = Color.White;
            controlPanel.Controls.Add(spaces);
            controlPanel.Controls.Add(FuelControlIndicator, 1, 0);
            controlPanel.Controls.Add(indicatorText, 1, 0);
            Controls.Add(controlPanel);
        }

        private void ShowEndOfGameMessage()
        {
            var result = MessageBox.Show("Хотите сыграть еще?", "Ты проиграл!",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Information,
                                         MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.DefaultDesktopOnly);
            if (result == DialogResult.Yes)
            {
                FuelControlIndicator.Value = FuelControlIndicator.Maximum;
            }
            else
            {
                this.Close();
            }
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
            FuelControlIndicator.Value = 1000;
            space.Rocket = new Rocket(new Vector(600, 600), new Vector(0, 0), -Math.PI / 2);
			timer.Start();
		}

		private void TimerTick(object sender, EventArgs e)
		{
            Turn control;
			if (space == null) return;
            
            if (isUserControl)
                control = left ? Turn.Left : (right ? Turn.Right : Turn.None);
            else
                control = RocketAutopilot(space.Rocket, space.Target);

            space.Move(ClientRectangle, control);
			if ((space.Rocket.Location - space.Target).Length < 20)
				timer.Stop();

            controlPanel.Size = new System.Drawing.Size(Size.Width, 30);
            FuelControlIndicator.PerformStep();
            
            if(FuelControlIndicator.Value == 0)
            {
                ShowEndOfGameMessage();
            }

			Invalidate();
			Update();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
            if (e.KeyCode == Keys.Left)
            {
                left = true;
                isUserControl = true;
            }

			if (e.KeyCode == Keys.Right) 
            {
                right = true;
                isUserControl = true;
            }
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyDown(e);
            if (e.KeyCode == Keys.Left)    
                left = false;
          
            if (e.KeyCode == Keys.Right)
                right = false;

            if(!(left || right))
                isUserControl = false;
            
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