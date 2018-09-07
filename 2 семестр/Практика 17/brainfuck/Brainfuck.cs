using System;
using System.Collections.Generic;
using System.Text;

namespace func.brainfuck
{
    public class Brainfuck
    {
        public static void Run(string program, Func<int> read, Action<char> write)
        {
            var runner = new ProgramRunner(program);

            runner.RegisterCommand('<', b =>
            {
                if (b.MemoryPointer == 0)
                    b.MemoryPointer = 29999;
                else
                    b.MemoryPointer--;
            });

            runner.RegisterCommand('>', b =>
            {
                if (b.MemoryPointer == 29999)
                    b.MemoryPointer = 0;
                else
                    b.MemoryPointer++;
            });

            runner.RegisterCommand('+', b => b.Memory[b.MemoryPointer]++);
            runner.RegisterCommand('-', b => b.Memory[b.MemoryPointer]--);
            runner.RegisterCommand('.', b => write((char)b.Memory[b.MemoryPointer]));
            runner.RegisterCommand(',', b => b.Memory[b.MemoryPointer] = (byte)read());

            Cycles(runner);

            GetConstans(runner);

            runner.Run();
        }

        static void Cycles(ProgramRunner runner)
        {
            var openBrackets = new int[runner.Instructions.Length];
            var closeBrackets = new int[runner.Instructions.Length];
            var bracketsStack = new Stack<int>();
            for (int i = 0; i < runner.Instructions.Length; i++)
            {
                if (runner.Instructions[i] == '[')
                    bracketsStack.Push(i);
                if (runner.Instructions[i] == ']')
                {
                    var openBracket = bracketsStack.Pop();
                    closeBrackets[openBracket] = i;
                    openBrackets[i] = openBracket;
                }
            }

            runner.RegisterCommand('[', b =>
            {
                if (b.Memory[b.MemoryPointer] == 0)
                    b.InstructionPointer = closeBrackets[b.InstructionPointer];
            });

            runner.RegisterCommand(']', b =>
            {
                if (b.Memory[b.MemoryPointer] != 0)
                    b.InstructionPointer = openBrackets[b.InstructionPointer];
            });
        }

        static void GetConstans(ProgramRunner runner)
        {
            for (char i = '0'; i <= '9'; i++)
            {
                var j = i;
                runner.RegisterCommand(j, b => b.Memory[b.MemoryPointer] = (byte)j);
            }
            for (char i = 'A'; i <= 'Z'; i++)
            {
                var j = i;
                runner.RegisterCommand(j, b => b.Memory[b.MemoryPointer] = (byte)j);
                runner.RegisterCommand(char.ToLower(j), b => b.Memory[b.MemoryPointer] = (byte)char.ToLower(j));
            }
        }
    }


    public class ProgramRunner
    {

        public byte[] Memory { get; set; }
        public int MemoryPointer { get; set; }
        public int InstructionPointer { get; set; }
        public string Instructions { get; set; }
        private Dictionary<char, Action<ProgramRunner>> Commands = new Dictionary<char, Action<ProgramRunner>>();
        

        public ProgramRunner(string program)
        {           
            Instructions = program;            
        }

        public void RegisterCommand(char name, Action<ProgramRunner> executeCommand)
        {
            Commands.Add(name, executeCommand);
        }

        public void Run()
        {
            Memory = new byte[30000];
            MemoryPointer = 0;

            for (InstructionPointer = 0; InstructionPointer < Instructions.Length; InstructionPointer++)
            {
                Action<ProgramRunner> command;
                if (Commands.TryGetValue(Instructions[InstructionPointer], out command))
                    command(this);
            }
        }
    }
}

