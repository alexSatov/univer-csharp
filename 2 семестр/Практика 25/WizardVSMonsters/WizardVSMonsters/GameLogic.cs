using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WizardVSMonsters
{
    public enum UnitState
    {
        Stop,
        Walking,
        Attack,
        Death
    }

    public enum SpellType
    {
        Arrow,
        FireBall,
        FrostBall,
    }

    public enum MonsterType
    {
        Skeleton,
        Zombie,
        Minotaur
    }

    public enum MagicEffect
    {
        None,
        Burn,
        Freeze,
    }

    public class GameUnit
    {
        public UnitState CurrentState { get; set; }
        public string Name { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GameUnit(string name, int x, int y, UnitState state = UnitState.Walking)
        {
            Name = name;
            CurrentState = state;
            X = x;
            Y = y;
        }
    }

    public class Game
    {
        public Wizard Wizard { get; private set; }
        public List<Monster> Monsters { get; set; }

        public static int MapHeight = 10;
        public static int MapWidth = 20;
        public int CastleHealth { get; set; }
        public bool IsOver { get; set; }

        public Game()
        {
            Wizard = new Wizard(this, 5);
            CastleHealth = 1000;
            Monsters = new List<Monster>();

            var enemyTimer = new Timer();
            enemyTimer.Interval = 5000;
            enemyTimer.Tick += EnemyTimerTick;
            enemyTimer.Start();
        }

        void EnemyTimerTick(object sender, EventArgs args)
        {
            AddEnemy();
        }

        public void AddEnemy()
        {
            var random = new Random();
            var enemyX = MapWidth - 1;
            var enemyY = random.Next(MapHeight);
            var enemyType = (MonsterType)random.Next(3);
            Monsters.Add(new Monster(this, enemyX, enemyY, enemyType));
        }

        public static bool IsClearWay(int x, int y)
        {
            return (x < MapWidth) && (y < MapHeight) && (y >= 0) && (x > 0);
        }
    }

    public class Spell
    {
        private Wizard wizard;

        public int Damage { get; set; }
        public MagicEffect Effect { get; set; }
        public SpellType Type { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public bool IsDead { get; set; }

        public Spell(Wizard wizard, int damage, int capacity, MagicEffect effect, SpellType type)
        {
            Type = type;
            Damage = damage;
            Effect = effect;
            this.wizard = wizard;
            IsActive = false;
            IsDead = false;
            Capacity = capacity;
        }

        public void Create()
        {
            if (!IsActive)
            {
                X = 1;
                Y = wizard.Y;
                wizard.Mana -= Capacity;
                IsActive = true;
                IsDead = false;
            }
        }

        public void Move()
        {
            CheckMonsters();
            if (IsActive)
                if (Game.IsClearWay(X + 1, Y))
                    X++;
                else
                    IsDead = true;
            CheckMonsters();
        }

        void CheckMonsters()
        {
            var isMonster = false;
            foreach (var monster in wizard.game.Monsters)
                if (monster.X == X && monster.Y == Y)
                {
                    isMonster = true;
                    monster.Health -= Damage;
                    monster.Effect = Effect;
                }
            if (isMonster)
            {
                IsDead = true;
                IsActive = false;
                X = 0;
                Y = Game.MapHeight;
            }
        }
    }

    public class Wizard : GameUnit
    {
        public Game game;
        public Dictionary<SpellType, Spell> spells;
        public int Mana { get; set; }
        public SpellType CurrentSpell { get; set; }

        public Wizard(Game game, int y)
            : base("Wizard", 0, y)
        {
            this.game = game;
            spells = new Dictionary<SpellType, Spell>();
            spells[SpellType.Arrow] = new Spell(this, 60, 40, MagicEffect.None, SpellType.Arrow);
            spells[SpellType.FireBall] = new Spell(this, 100, 100, MagicEffect.Burn, SpellType.FireBall);
            spells[SpellType.FrostBall] = new Spell(this, 80, 20, MagicEffect.Freeze, SpellType.FrostBall);
            Y = y;
            CurrentSpell = SpellType.FrostBall;
        }

        public void Act(int deltaY)
        {
            if (Y + deltaY >= 0 && Y + deltaY < Game.MapHeight)
            {
                CurrentState = UnitState.Walking;
                Y += deltaY;
            }
        }

        public void Attack()
        {
            CurrentState = UnitState.Attack;
            spells[CurrentSpell].Create();
        }
    }

    public class Monster : GameUnit
    {
        private Game game;

        public int Health { get; set; }
        public int Damage { get; set; }
        public MonsterType Type { get; set; }
        public MagicEffect Effect { get; set; }
        public bool IsDead { get { return CurrentState == UnitState.Death; } }

        public Monster(Game game, int x, int y, MonsterType type)
            : base(type.ToString(), x, y)
        {
            this.game = game;
            Type = type;

            switch (type)
            {
                case MonsterType.Skeleton:
                    Health = 110;
                    Damage = 110;
                    break;

                case MonsterType.Zombie:
                    Health = 150;
                    Damage = 75;
                    break;

                case MonsterType.Minotaur:
                    Health = 250;
                    Damage = 180;
                    break;
            }
        }

        void Move()
        {
            if (Game.IsClearWay(X - 1, Y))
            {
                CurrentState = UnitState.Walking;
                X--;
            }
            else
                Attack();
        }

        public void Act()
        {
            if (!IsDead)
            {
                CheckDeath();
                if (!IsDead)
                {
                    switch (Effect)
                    {
                        case MagicEffect.None:
                            Move();
                            break;

                        case MagicEffect.Burn:
                            Health -= 5;
                            Move();
                            break;

                        case MagicEffect.Freeze:
                            CurrentState = UnitState.Stop;
                            break;
                    }
                }
            }
        }

        void CheckDeath()
        {
            if (Health <= 0)
            {
                game.Monsters.Remove(this);
                CurrentState = UnitState.Death;
                X = 1;
                Y = Game.MapHeight;
            }
        }

        public void Attack()
        {
            CurrentState = UnitState.Attack;
            game.CastleHealth -= Damage;
        }
    }
}
