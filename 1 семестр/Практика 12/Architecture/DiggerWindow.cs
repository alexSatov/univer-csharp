using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Digger
{
	public class DiggerWindow : Form
    {
        private Game game; 
        const int ElementSize = 32;
        Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        static List<CreatureAnimation> animations = new List<CreatureAnimation>();


        public DiggerWindow(Game game)
        {
            ClientSize = new Size(ElementSize * game.MapWidth, ElementSize * game.MapHeight + ElementSize);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Text = "Digger";
            KeyDown += game.KeyPressed;
            KeyUp += game.KeyIsNotPressed;
            DoubleBuffered = true;
            this.game = game;

            var imagesDirectory = new DirectoryInfo("Images");
            foreach(var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name]=(Bitmap)Bitmap.FromFile(e.FullName);
            var timer = new Timer();
            timer.Interval = 1;
            timer.Tick += TimerTick;
            timer.Start();
        }

        void Act(Game game)
        {
            animations.Clear();
            for (int x = 0; x < game.MapWidth; x++)
                for (int y = 0; y < game.MapHeight; y++)
                {
                    var creature = game.Map[x, y];
                    if (creature == null) continue;
                    var command = creature.Act(x,y);
                    animations.Add(new CreatureAnimation
                    {
                        Command=command,
                        Creature = creature,
                        Location = new Point(x * ElementSize, y * ElementSize)
                    });
                }
            animations = animations.OrderByDescending(z => z.Creature.GetDrawingPriority).ToList();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0,ElementSize);
            e.Graphics.FillRectangle(Brushes.Black,0,0,ElementSize*game.MapWidth,ElementSize*game.MapHeight);
            foreach(var a in animations)
                e.Graphics.DrawImage(bitmaps[a.Creature.GetImageFileName],a.Location);
            e.Graphics.ResetTransform();
            e.Graphics.DrawString(game.Scores.ToString(), new Font("Arial", 16), Brushes.Green, 0, 0);
        }

        int tickCount = 0;

        void TimerTick(object sender, EventArgs args)
        {
            if (tickCount == 0) Act(game);
            foreach (var e in animations)
                e.Location = new Point(e.Location.X + 4*e.Command.DeltaX, e.Location.Y + 4*e.Command.DeltaY);
            if (tickCount==7)
            {
                for (int x=0;x<game.MapWidth;x++) for (int y=0;y<game.MapHeight;y++) game.Map[x,y]=null;
                foreach(var e in animations)
                {
                    var x=e.Location.X/32;
                    var y=e.Location.Y/32;
                    var nextCreature = e.Command.TransformTo == null ? e.Creature : e.Command.TransformTo;
                    if (game.Map[x, y] == null) game.Map[x, y] = nextCreature;
                    else
                    {
                        bool newDead = nextCreature.DeadInConflict(game.Map[x, y]);
                        bool oldDead = game.Map[x, y].DeadInConflict(nextCreature);
                        if (newDead && oldDead)
                            game.Map[x, y] = null;
                        else if (!newDead && oldDead)
                            game.Map[x, y] = nextCreature;
                        else if (!newDead && !oldDead)
                            throw new Exception(string.Format("Существа {0} и {1} претендуют на один и тот же участок карты", nextCreature.GetType().Name, game.Map[x, y].GetType().Name));
                    }                    
                }
            }
            tickCount++;
            if (tickCount == 8) tickCount = 0;
            Invalidate();
        }
    }
}
