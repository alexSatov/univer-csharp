using System;

namespace Inheritance.Geometry
{
    public interface IVisitor
    {
        double Visit(Ball ball);
        double Visit(Cube cube);
        double Visit(Cyllinder cyllinder);
    }

    public abstract class Body
    {
        public abstract double Accept(IVisitor volVisitor);
    }

    public static class BodyExtensions
    {
        public static double GetVolume(this Body body)
        {
            return body.Accept(new Visitor());
        }
    }

    public class Visitor : IVisitor
    {
        public double Visit(Ball ball)
        {
            return 4.0/3*Math.PI*Math.Pow(ball.Radius, 3);
        }

        public double Visit(Cube cube)
        {
            return Math.Pow(cube.Size, 3);
        }

        public double Visit(Cyllinder cyllinder)
        {
            return Math.PI*Math.Pow(cyllinder.Radius, 2)*cyllinder.Height;
        }
    }

    public class Ball : Body
    {
        public double Radius { get; set; }

        public override double Accept(IVisitor volVisitor)
        {
            return volVisitor.Visit(this);
        }
    }

    public class Cube : Body
    {
        public double Size { get; set; }

        public override double Accept(IVisitor volVisitor)
        {
            return volVisitor.Visit(this);
        }
    }

    public class Cyllinder : Body
    {
        public double Height { get; set; }
        public double Radius { get; set; }

        public override double Accept(IVisitor volVisitor)
        {
            return volVisitor.Visit(this);
        }
    }
}
