using System;
using System.Collections.Generic;

namespace func_rocket
{
    public class Tasks
    {
        public static IEnumerable<GameSpace> GetGameSpaces()
        {
            var rocket1 = new Rocket(new Vector(0, 900), new Vector(0, 0), -Math.PI / 2);
            var rocket2 = new Rocket(new Vector(900, 900), new Vector(0, 0), -Math.PI / 4);
            var rocket3 = new Rocket(new Vector(0, 450), new Vector(0, 0), Math.PI);
            var rocket4 = new Rocket(new Vector(900, 100), new Vector(0, 0), Math.PI / 2);
            var rocket5 = new Rocket(new Vector(900, 100), new Vector(0, 0), Math.PI / 2);

            var target1 = new Vector(900, 100);
            var target2 = new Vector(900, 200);
            var target3 = new Vector(1000, 400);
            var target4 = new Vector(900, 1000);
            var target5 = new Vector(900, 1000);

            yield return new GameSpace("zero-gravity", rocket1, target1, p => new Vector(0, 0));

            yield return new GameSpace("heavy-gravity", rocket2, target2, p => new Vector(0, 0.9));
           
            yield return new GameSpace("white-hole", rocket3, target3, p =>
            {
                var d = target3 - p;
                return d * (-50 / Math.Pow((d.Length + 1), 2));
            });

            var zeroTime = DateTime.Now;
            yield return new GameSpace("anomaly", rocket4, target4, p =>
            {
                var t = (DateTime.Now - zeroTime).TotalSeconds;
                return new Vector(Math.Cos(t * Math.PI), Math.Sin(t * Math.PI));
            });

            yield return new GameSpace("ULTRA", rocket5, target5, p => new Vector(rocket5.Location.X / -8000, rocket5.Location.Y / -900));
        }
       
        public static Turn ControlRocket(Rocket rocket, Vector target)
        {
            var angleToTarget = (target - rocket.Location).Angle;
            var turn1 = AngleDiff(rocket.Direction, angleToTarget);
            var turn2 = AngleDiff(rocket.Velocity.Angle, angleToTarget);
            var turn = (Math.Abs(turn1) < Math.PI / 4 || Math.Abs(turn2) < Math.PI / 4) ? (turn1 + turn2) / 2 : turn1;
            if (turn < -0.1) return Turn.Left;
            if (turn > 0.1) return Turn.Right;
            return Turn.None;
        }

        private static double AngleDiff(double a1, double a2)
        {
            var turn = a2 - a1;
            while (turn > Math.PI) turn -= 2 * Math.PI;
            while (turn < -Math.PI) turn += 2 * Math.PI;
            return turn;
        }

        //public static Turn ControlRocket(Rocket rocket, Vector target)
        //{
        //    var angleToTarget = (target - rocket.Location).Angle;
        //    var turn = AngleDiff(rocket.Direction, angleToTarget);
        //    if (turn < -0.1) return Turn.Left;
        //    if (turn > 0.1) return Turn.Right;
        //    return Turn.None;
        //}
    }
}