using System;
using System.Drawing;

namespace func_rocket
{
	public class GameSpace
	{
		public GameSpace(string name, Rocket rocket, Vector target, Func<Vector, Vector> gravity)
		{
			this.name = name;
			Rocket = rocket;
			Target = target;
			Gravity = gravity;
		}

		private readonly string name;
		public Rocket Rocket;
		public Vector Target;

		public override string ToString()
		{
			return name;
		}

		public Func<Vector, Vector> Gravity { get; private set; }

		public void Move(Rectangle spaceRect, Turn turnRate)
		{
			Rocket.Direction += (int)turnRate * 0.08;
			var direction = new Vector(Math.Cos(Rocket.Direction), Math.Sin(Rocket.Direction));
			var force = direction + Gravity(Rocket.Location);
			Rocket.Velocity = Rocket.Velocity + force;
			if (Rocket.Velocity.Length > 20) Rocket.Velocity = Rocket.Velocity * (10 / Rocket.Velocity.Length);
			Rocket.Location = Rocket.Location + Rocket.Velocity*0.5;
			Rocket.Location = new Vector(Math.Min(spaceRect.Width, Rocket.Location.X), Math.Min(spaceRect.Height, Rocket.Location.Y));
			Rocket.Location = new Vector(Math.Max(0, Rocket.Location.X), Math.Max(0, Rocket.Location.Y));
		}
	}
}