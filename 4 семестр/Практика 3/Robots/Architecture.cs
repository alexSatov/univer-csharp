using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public interface IRobotAI<out TCommand>
    {
        TCommand GetCommand();
    }

    public abstract class RobotAI<TCommand> : IRobotAI<TCommand>
    {
        public abstract TCommand GetCommand();
    }

    public class ShooterAI : RobotAI<ShooterCommand>
    {
        int counter = 1;
        public override ShooterCommand GetCommand()
        {
            return ShooterCommand.ForCounter(counter++);
        }
    }

    public class BuilderAI : RobotAI<BuilderCommand>
    {
        int counter = 1;
        public override BuilderCommand GetCommand()
        {
            return BuilderCommand.ForCounter(counter++);
        }
    }

    public abstract class Device<TCommand>
    {
        public abstract string ExecuteCommand(TCommand command);
    }

    public class Mover : Device<IMoveCommand>
    {
        public override string ExecuteCommand(IMoveCommand command)
        {
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }

    public class Robot<T>
    {
        private readonly IRobotAI<T> ai;
        private readonly Device<T> device;

        public Robot(IRobotAI<T> ai, Device<T> executor)
        {
            this.ai = ai;
            device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
             for (var i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }

        }
    }
    
    public static class Robot
    {
        public static Robot<IMoveCommand> Create(IRobotAI<IMoveCommand> ai, Device<IMoveCommand> executor)
        {
            return new Robot<IMoveCommand>(ai, executor);
        }
    }
}
