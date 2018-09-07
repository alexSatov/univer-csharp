using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Digger
{
    public class Game
    {
        public Digger digger { get; private set; }
        public ICreature[,] Map { get; set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public int Scores { get; set; }
        public bool IsOver { get; set; }

        public void KeyPressed(object sender, KeyEventArgs key)
        {
            switch (key.KeyCode.ToString())
            {
                case "D":
                    digger.MoveTo(1, 0);
                    break;

                case "A":
                    digger.MoveTo(-1, 0);
                    break;
                case "W":
                    digger.MoveTo(0, -1);
                    break;
                case "S":
                    digger.MoveTo(0, 1);
                    break;
            }
        }

        public void KeyIsNotPressed(object sender, KeyEventArgs key)
        {
            digger.MoveTo(0, 0);
        }

        public bool IsOutOfBorder(int x, int y)
        {
            return (x > MapWidth - 1) || (y > MapHeight - 1) || (x < 0) || (y < 0);
        }

        public Game()
        {
            var lines = File.ReadAllLines("Map.txt");
            for (int k = 0; k < lines.Length; k++)
            {
                var data = lines[k].Split(' ');
                if (k == 0)
                {
                    MapHeight = int.Parse(data[1]);
                    MapWidth = int.Parse(data[0]);
                    Map = new ICreature[MapWidth, MapHeight];
                    for (var i = 0; i < MapWidth; i++)
                        for (var j = 0; j < MapHeight; j++)
                            Map[i, j] = new Terrain();
                }
                if (char.IsLetter(data[0][0]))
                {
                    if (data[0] == "Digger")
                    {
                        digger = new Digger(this);
                        var coord = lines[k+1].Split(' ');
                        k++;
                        Map[int.Parse(coord[0]), int.Parse(coord[1])] = digger;
                    }
                    if (data[0] == "SackOfGold")
                    {                        
                        var coord = lines[k + 1].Split(' ');
                        k++;
                        Map[int.Parse(coord[0]), int.Parse(coord[1])] = new SackOfGold(int.Parse(coord[1]), this); 
                    }
                    if (data[0] == "TNT")
                    {
                        var coord = lines[k + 1].Split(' ');
                        k++;
                        Map[int.Parse(coord[0]), int.Parse(coord[1])] = new TNT(int.Parse(coord[1]), this, int.Parse(coord[0])); 
                    }
                }

            }
            
            //MapHeight = 20;
            //MapWidth = 30;
            //Map = new ICreature[MapWidth, MapHeight];
            //for (var i = 0; i < MapWidth; i++)
            //    for (var j = 0; j < MapHeight; j++)
            //        Map[i, j] = new Terrain();
            //digger = new Digger(this);
            //Map[15, 16] = digger;
            //Map[6, 6] = new SackOfGold(6, this);
            //Map[24, 6] = new SackOfGold(6, this);
            //Map[6, 9] = new SackOfGold(9, this);
            //Map[15, 10] = new TNT(10, this);
        }
    }

    public class Terrain : ICreature
    {
        int ICreature.GetDrawingPriority { get { return 2; } }
        string ICreature.GetImageFileName { get { return "Terrain.png"; } }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(0, 0, null);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }               
    }

    public class Digger : ICreature
    {
        private Game game;
        public int DirectionX;
        public int DirectionY;
        int ICreature.GetDrawingPriority { get { return 0; } }
        string ICreature.GetImageFileName { get { return "Digger.png"; } }

        public Digger(Game game)
        {
            this.game = game;
        }
        public void MoveTo(int x, int y)
        {
            DirectionX = x;
            DirectionY = y;           
        }

        public CreatureCommand Act(int x, int y)
        {
            if (game.IsOutOfBorder(x + DirectionX, y + DirectionY) || IsSack(x + DirectionX, y + DirectionY, game))
            {
                return new CreatureCommand(0, 0, this);
            }
            return new CreatureCommand(DirectionX, DirectionY, this);
        }

        public static bool IsSack(int x, int y, Game game)
        {
            return (game.Map[x, y] != null) && (game.Map[x, y] is SackOfGold);
        }
        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is TNT)
                return true;
            if (conflictedObject is SackOfGold)
                return true;
            if (conflictedObject is Gold)
                game.Scores += 100;
            return false;
        }        
    }

    public class SackOfGold : ICreature
    {
        private Game game;
        public int StartY;
        string ICreature.GetImageFileName { get { return "Sack.png"; } }
        int ICreature.GetDrawingPriority { get { return 1; } }

        public SackOfGold(int startY, Game game)
        {
            StartY = startY;
            this.game = game;
        }
                
        public CreatureCommand Act(int x, int y)
        {
            if (IsEmpty(x, y + 1))
            {
                return new CreatureCommand(0, 1, this);
            }
            else
            {
                if (IsEmpty(x, y - 1) && y != StartY)
                {
                    return new CreatureCommand(0, 0, new Gold(game));
                }

                return new CreatureCommand(0, 0, this);
            }
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public bool IsEmpty(int x, int y)
        {
            if (game.IsOutOfBorder(x, y)) return false;
            return (game.Map[x, y] == null) || (game.Map[x, y] is Digger) && (y - StartY > 1);
        }
    }

    public class Gold : ICreature
    {
        private Game game;
        public int StartY;
        string ICreature.GetImageFileName { get { return "Gold.png"; } }
        int ICreature.GetDrawingPriority { get { return 1; } }

        public Gold(Game game)
        {
            this.game = game;
        }
        public CreatureCommand Act(int x, int y)
        {
            if (IsEmpty(x, y + 1))
            {
                return new CreatureCommand(0, 1, this);
            }
            else
            {
                return new CreatureCommand(0, 0, this);
            }
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Digger)
            {
                return true;
            }
            return false;
        }

        public bool IsEmpty(int x, int y)
        {
            if (game.IsOutOfBorder(x, y)) return false;
            return (game.Map[x, y] == null) || (game.Map[x, y] is Digger) && (y - StartY > 1);
        }
    }

    public class TNT : ICreature
    {
        private Game game;
        public int StartY;
        public int X;
        public int Y;
        string ICreature.GetImageFileName { get { return "TNT.png"; } }
        int ICreature.GetDrawingPriority { get { return 3; } }

        public TNT(int startY, Game game, int x)
        {
            StartY = startY;
            Y = startY;
            this.game = game;
            X = x;
        }

        public CreatureCommand Act(int x, int y)
        {
            if (IsEmpty(x, y + 1) || !IsEmpty(x, y + 1) && y != StartY) 
            {
                Y += 1;
                return new CreatureCommand(0, 1, this);               
            }
            else
            {               
                return new CreatureCommand(0, 0, this);
            }
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {                       
                Explosion(game, X, Y);
                return true;                        
        }

        public void Explosion (Game game, int x, int y)
        {
             for (int i = 0; i < 3; i++)
                 for (int j = 0; j< 3; j++)
                     if (y - 1 + i >= 0 && y - 1 + i < game.MapHeight && x - 1 + j >= 0 && x - 1 + j < game.MapWidth)
                        game.Map[x - 1 + j, y - 1 + i] = null;            
        }

        public bool IsEmpty(int x, int y)
        {
            if (game.IsOutOfBorder(x, y)) return false;
            return (game.Map[x, y] == null) || (game.Map[x, y] is Digger) && (y - StartY > 1);
        }
    }
}

