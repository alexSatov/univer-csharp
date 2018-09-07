using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WizardVSMonsters
{
    class MainWindow : Form
    {
        public Game game { get; private set; }
        private GameField gameField;
        private GamePanel gamePanel;
        public MainWindow(Size elementSize)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ClientSize = new Size(elementSize.Width * Game.MapWidth, elementSize.Height * Game.MapHeight + 50);
            KeyDown += KeyPressed;
            game = new Game();

            gameField = new GameField(elementSize, game);

            var mainLayout = new FlowLayoutPanel();
            mainLayout.FlowDirection = FlowDirection.TopDown;
            mainLayout.AutoSize = true;

            gamePanel = new GamePanel(this);
            mainLayout.Controls.Add(gamePanel);
            mainLayout.Controls.Add(gameField);
            Controls.Add(mainLayout);
        }

        public void KeyPressed(object sender, KeyEventArgs key)
        {
            switch (key.KeyCode.ToString())
            {
                case "D":
                    if (game.Wizard.CurrentState != UnitState.Attack)
                    {
                        game.Wizard.Attack();
                        gameField.WizardGraphics.StartNewAnimation();
                    }
                    break;

                case "E":
                    if (game.Wizard.CurrentSpell == SpellType.FrostBall)
                        game.Wizard.CurrentSpell = SpellType.Arrow;
                    else
                        game.Wizard.CurrentSpell += 1;
                    if(gameField.SpellGraphic !=null)
                        gameField.SpellGraphic.Destroy();
                    gamePanel.SpellSelector.UpdateCurrentChoise();
                    break;

                case "W":
                    game.Wizard.Act(-1);
                    gameField.WizardGraphics.DirectionY = -1;
                    break;

                case "S":
                    game.Wizard.Act(1);
                    gameField.WizardGraphics.DirectionY = 1;
                    break;
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MainWindow
            // 
            ClientSize = new Size(284, 262);
            Name = "MainWindow";
            ResumeLayout(false);

        }
    }

    class GamePanel : Label
    {
        private Game game;
        private GameProgressBar manaProgressBar;
        private GameProgressBar healthProgressBar;
        public SpellSelector SpellSelector { get; private set; }
        public GamePanel(MainWindow mainWindow)
        {
            game = mainWindow.game;
            Size = new Size(mainWindow.Width, 50);
            BackColor = Color.Black;


            manaProgressBar = new GameProgressBar(Brushes.MediumBlue, 0, 1000);
            healthProgressBar = new GameProgressBar(Brushes.ForestGreen, 0, 1000);
            SpellSelector = new SpellSelector(game.Wizard, new Size(50, 50));
            var layout = new TableLayoutPanel();
            layout.Size = this.Size;

            layout.Controls.Add(new PanelTextLabel("MN", Color.White), 0, 0);
            layout.Controls.Add(manaProgressBar, 1, 0);
            layout.Controls.Add(new PanelTextLabel("HP", Color.White), 2, 0);
            layout.Controls.Add(healthProgressBar, 3, 0);
            layout.Controls.Add(SpellSelector, 4, 0);


            Controls.Add(layout);
            setDefaultControlsSettings(this);
            var timer = new Timer();
            timer.Interval = 100;
            timer.Tick += TimerTick;
            timer.Start();
        }

        public static void setDefaultControlsSettings(GamePanel panel)
        {
            panel.manaProgressBar.CurrentValue = panel.game.Wizard.Mana;
            panel.manaProgressBar.Step = 2;
            panel.healthProgressBar.CurrentValue = panel.healthProgressBar.Maximum;
            panel.manaProgressBar.Size = new Size(125, 25);
            panel.healthProgressBar.Size = new Size(125, 25);
        }
        void TimerTick(object sender, EventArgs args)
        {
            manaProgressBar.PerformStep();
            healthProgressBar.PerformStep();
            game.Wizard.Mana += 2;
            manaProgressBar.CurrentValue = game.Wizard.Mana;
            healthProgressBar.CurrentValue = game.CastleHealth;
            Invalidate();
        }
    }

    class PanelTextLabel : Label
    {
        public PanelTextLabel(string text, Color textColor)
        {
            Text = text;
            Anchor = AnchorStyles.None;
            TextAlign = ContentAlignment.MiddleCenter;
            ForeColor = textColor;
            Size = new Size(25, 25);
        }
    }

    class GameProgressBar : Label
    {
        public int Step { get; set; }
        public int CurrentValue { get; set; }

        public int Maximum { get; private set; }
        public int Minimum { get; private set; }

        public Brush brush { get; set; }

        public GameProgressBar(Brush brush, int minValue = 0, int maxValue = 100)
        {
            Anchor = AnchorStyles.None;
            Maximum = maxValue;
            Minimum = minValue;
            Step = 1;
            CurrentValue = minValue;
            BorderStyle = BorderStyle.Fixed3D;
            BackColor = Color.LightSkyBlue;
            this.brush = brush;
        }

        public void PerformStep()
        {
            if (CurrentValue <= Maximum)
                CurrentValue += Step;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var len = Width * CurrentValue / Maximum;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.FillRectangle(brush, 0, 0, len, Height);
        }
    }

    class GameField : Label
    {
        private Game game;
        private Size ElementSize;
        public UnitGraphics WizardGraphics;
        public List<UnitGraphics> MonsterGraphics;
        public SpellGraphics SpellGraphic;

        public GameField(Size elementSize, Game game)
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            this.game = game;
            MonsterGraphics = new List<UnitGraphics>();
            ElementSize = elementSize;
            ClientSize = new Size(ElementSize.Width * Game.MapWidth, ElementSize.Height * Game.MapHeight + ElementSize.Width);

            WizardGraphics = new UnitGraphics(ElementSize, game.Wizard, 200, this);
            WizardGraphics.UpdateLocation();

            Controls.Add(WizardGraphics);
            BuildCastleWalls(this);
            var AnimationTimer = new Timer();
            AnimationTimer.Interval = 1;
            AnimationTimer.Tick += TimerTick;
            AnimationTimer.Start();

            var offenceTimer = new Timer();
            offenceTimer.Interval = 1000;
            offenceTimer.Tick += OffenceTick;
            offenceTimer.Start();

            var magicTimer = new Timer();
            magicTimer.Interval = 150;
            magicTimer.Tick += MagicTick;
            magicTimer.Start();
        }

        void MagicTick(object sender, EventArgs args)
        {
            game.Wizard.spells[game.Wizard.CurrentSpell].Move();
        }

        void TimerTick(object sender, EventArgs args)
        {
            WizardGraphics.UpdateLocation();
            if (SpellGraphic != null)
                SpellGraphic.UpdateLocation();
            else
            {
                SpellGraphic = new SpellGraphics(game.Wizard.spells[game.Wizard.CurrentSpell], ElementSize, this);
                Controls.Add(SpellGraphic);
            }

            foreach (var monster in game.Monsters)
            {
                var monsterGraphics = new UnitGraphics(ElementSize, monster, 200, this);
                monsterGraphics.DirectionX = -1;
                monsterGraphics.DirectionY = 0;
                if (!MonsterGraphics.Contains(monsterGraphics))
                {
                    MonsterGraphics.Add(monsterGraphics);
                    Controls.Add(monsterGraphics);
                }
            }

            try
            {
                foreach (var monsterGraph in MonsterGraphics)
                    monsterGraph.UpdateLocation();
            }
            catch
            {
                foreach (var monsterGraph in MonsterGraphics)
                    monsterGraph.UpdateLocation();
            }

            Invalidate();
        }

        void OffenceTick(object sender, EventArgs args)
        {
            try
            {
                foreach (var monster in game.Monsters)
                {
                    monster.Act();
                }
            }
            catch
            {
                foreach (var monster in game.Monsters)
                {
                    monster.Act();
                }
            }
        }

        public static void BuildCastleWalls(GameField gameField)
        {
            var img = Image.FromFile("wall.png");
            var newWall = new PictureBox();
            newWall.Size = new Size(gameField.ElementSize.Width, gameField.Height);
            newWall.Image = img;
            newWall.BackColor = Color.Transparent;
            gameField.WizardGraphics.Parent = newWall;
            gameField.Controls.Add(newWall);

        }
    }

    class SpellSelector : Label
    {
        private Wizard wizard;
        private Dictionary<SpellType, Image> spellsImages;
        public SpellSelector(Wizard wizard, Size size)
        {
            this.wizard = wizard;
            Size = size;
            spellsImages = GetSpellsImages(wizard);
            Image = spellsImages[wizard.CurrentSpell];
            TextAlign = ContentAlignment.BottomCenter;
            Text = wizard.CurrentSpell.ToString();
            ForeColor = Color.White;
        }

        public static Dictionary<SpellType, Image> GetSpellsImages(Wizard wizard)
        {
            return wizard.spells.Select(pair => new KeyValuePair<SpellType, Image>(pair.Key, Image.FromFile(pair.Key.ToString() + ".png")))
                                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void UpdateCurrentChoise()
        {
            Image = spellsImages[wizard.CurrentSpell];
            Text = wizard.CurrentSpell.ToString();
            Update();
        }
    }

    class UnitGraphics : PictureBox
    {
        private int indexOfCurrentFrame;
        private Timer animationTimer;
        public GameUnit Unit;
        private Dictionary<UnitState, List<Bitmap>> sprites;
        private GameField gameField;
        private Timer motionTimer;
        public int DirectionX;
        public int DirectionY;

        public UnitGraphics(Size spriteSize, GameUnit unit, int animationSpeed, GameField gameField)
        {
            BackColor = Color.Transparent;

            BackgroundImageLayout = ImageLayout.Stretch;
            Size = spriteSize;
            indexOfCurrentFrame = 0;
            Unit = unit;
            sprites = getSpriteDict(unit.Name, spriteSize);
            sprites.Add(UnitState.Stop, new List<Bitmap>() { sprites[UnitState.Walking][0] });
            this.gameField = gameField;

            animationTimer = new Timer();
            animationTimer.Interval = animationSpeed;
            animationTimer.Tick += NextFrame;
            animationTimer.Start();

            motionTimer = new Timer();
            motionTimer.Interval = 15;
            motionTimer.Tick += DoSmoothMovement;
            motionTimer.Start();
        }

        private void DoSmoothMovement(object sender, EventArgs args)
        {
            
            if (!Location.Equals(new Point(Unit.X * Size.Width, Unit.Y * Size.Height)))
            {
                Location = new Point(Location.X + DirectionX, Location.Y + DirectionY);
                Update();
            }
            else
            {
                if (Unit.CurrentState != UnitState.Attack)
                {
                    DirectionX = 0;
                    DirectionY = 0;
                    Unit.CurrentState = UnitState.Stop;
                }
            }
        }

        private static Dictionary<UnitState, List<Bitmap>> getSpriteDict(string unitName, Size frameSize) //new Size(48, 70)
        {
            var spritesDict = new Dictionary<UnitState, List<Bitmap>>();
            var stateList = new List<UnitState>() { UnitState.Walking, UnitState.Death, UnitState.Attack };
            return stateList.Select(state =>
            {
                var fileName = unitName + state.ToString() + ".png";
                var spritesList = UnpackSprites((Bitmap)Image.FromFile(fileName), frameSize);
                return new KeyValuePair<UnitState, List<Bitmap>>(state, spritesList);
            }).ToDictionary(pair1 => pair1.Key, pair2 => pair2.Value);
        }

        private void NextFrame(object sender, EventArgs args)
        {
            indexOfCurrentFrame++;
            if (indexOfCurrentFrame > sprites[Unit.CurrentState].Count - 1)
            {
                if (Unit.CurrentState == UnitState.Attack)
                    Unit.CurrentState = UnitState.Stop;
                indexOfCurrentFrame = 0;
            }

            Image = sprites[Unit.CurrentState][indexOfCurrentFrame];
        }

        public void Destroy(GameField gameField)
        {
            gameField.Controls.Remove(this);
            gameField.MonsterGraphics.Remove(this);
            Dispose();
        }

        public void UpdateLocation()
        {
           if (Unit.CurrentState == UnitState.Death)
           {
               gameField.Controls.Remove(this);
               gameField.MonsterGraphics.Remove(this);
           }
           Location = new Point(Unit.X * Size.Width, Unit.Y * Size.Height);
        }

        public void StartNewAnimation()
        {
            indexOfCurrentFrame = 0;
        }


        public static List<Bitmap> UnpackSprites(Bitmap inputBitmap, Size frameSize)
        {
            inputBitmap.MakeTransparent();
            var frameCount = inputBitmap.Width / frameSize.Width;
            var result = new List<Bitmap>();
            var defaultPixelFormat = System.Drawing.Imaging.PixelFormat.DontCare;
            for (var i = 0; i < frameCount; i++)
            {
                var currentFrame = inputBitmap.Clone(new Rectangle(new Point(frameSize.Width * i, 0), frameSize), defaultPixelFormat);
                currentFrame.MakeTransparent();
                result.Add(currentFrame);
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            var unitGraph = obj as UnitGraphics;
            if (unitGraph != null)
                return Unit.X == unitGraph.Unit.X && Unit.Y == unitGraph.Unit.Y && Unit.Name == unitGraph.Unit.Name;
            return false;
        }
    }

    class SpellGraphics : PictureBox
    {
        private int indexOfCurrentFrame;
        private List<Bitmap> spritesList;
        private Spell spell;
        private Timer animationTimer;
        private GameField gameField;
        public SpellGraphics(Spell spell, Size size, GameField gameField)
        {
            this.spell = spell;
            this.Size = size;
            this.gameField = gameField;
            var fileName = spell.Type.ToString() + "Flight.png";
            var inputBitmap = (Bitmap)Image.FromFile(fileName);
            spritesList = UnitGraphics.UnpackSprites(inputBitmap, size);
            Location = new Point(spell.X, spell.Y);

            animationTimer = new Timer();
            animationTimer.Interval = 60;
            animationTimer.Tick += NextFrame;
            animationTimer.Start();
        }

        public void UpdateLocation()
        {
            if (spell.IsDead)
            {
                gameField.Controls.Remove(this);
                gameField.SpellGraphic = null;
                Dispose();
            }
            Location = new Point(spell.X * Size.Width, spell.Y * Size.Height);
        }

        public void Destroy()
        {
            gameField.Controls.Remove(this);
            gameField.SpellGraphic = null;
            Dispose();
        }

        private void NextFrame(object sender, EventArgs args)
        {
            indexOfCurrentFrame++;
            if (indexOfCurrentFrame > spritesList.Count - 1)
            {
                indexOfCurrentFrame = 0;
            }

            Image = spritesList[indexOfCurrentFrame];
            Update();
        }

        public override bool Equals(object obj)
        {
            var unitGraph = obj as SpellGraphics;
            if (unitGraph == null) return false;
            return spell.X == unitGraph.spell.X && spell.Y == unitGraph.spell.Y;
        }
    }

    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow(new Size(50, 64)));
        }
    }
}
