using System;
using System.Collections.Generic;

namespace func_rocket
{
    public class Tasks
    {
        public static IEnumerable<GameSpace> GetGameSpaces()
        {
            var rocket = new Rocket(new Vector(600, 600), new Vector(10, 10), -Math.PI / 2);
            var center = new Vector(800, 400);

            yield return new GameSpace("zero-gravity", rocket, center, p => new Vector(0, 0));
            yield return new GameSpace("heavy-gravity", rocket, center, p => new Vector(0, 0.9));


            yield return new GameSpace("white-hole", rocket, center, p =>
            {
                var d = center - p;
                return d * (-50 / Math.Pow(d.Length + 1, 2));
            });

            var startTime = DateTime.Now;
            yield return new GameSpace("anomaly", rocket, center, p =>
            {
                var time = (DateTime.Now - startTime).TotalSeconds;
                return new Vector(Math.Cos(time * Math.PI), Math.Sin(time * Math.PI));
            });

            startTime = DateTime.Now;
            yield return new GameSpace("myGravitation", rocket, center, p =>
            {
                var time = (DateTime.Now - startTime).TotalSeconds;
                var angle = time * Math.PI;
                return new Vector(Math.Cos(angle), Math.Abs(Math.Sin(angle)));
            });
        }

        public static Turn ControlRocket(Rocket rocket, Vector target)
        {
            var angleToTarget = (target - rocket.Location).Angle;
            var firstTurn = DiffAngle(rocket.Direction, angleToTarget);
            var secondTurn = DiffAngle(rocket.Velocity.Angle, angleToTarget);
            var turn = (Math.Abs(firstTurn) < Math.PI / 4 || Math.Abs(secondTurn) < Math.PI / 4) ? (firstTurn + secondTurn) / 2 : firstTurn;
            if (turn < -0.1) return Turn.Left;
            if (turn > 0.1) return Turn.Right;

            return Turn.None;
        }

        private static double DiffAngle(double a1, double a2)
        {
            var turn = a2 - a1;
            while (turn > Math.PI) turn -= 2 * Math.PI;
            while (turn < -Math.PI) turn += 2 * Math.PI;
            return turn;
        }
    }
}