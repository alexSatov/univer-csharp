using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clones
{
    public class StackItem
    {
        public int Value { get; set; }
        public StackItem Previous { get; set; }

        public StackItem()
        {
            Value = 0;
            Previous = null;
        }
    }

    public class Stack
    {
        public StackItem Head { get; set; }
        public StackItem Tail { get; set; }

        public Stack()
        {
            Head = null;
            Tail = null;
        }

        public void Push(int value)
        {
            if (Head == null)
                Tail = Head = new StackItem { Value = value, Previous = null };
            else
            {
                var item = new StackItem { Value = value, Previous = Tail };                
                Tail = item;
            }
        }

        public int Pop()
        {
            if (Tail == null) throw new InvalidOperationException();
            var result = Tail.Value;
            Tail = Tail.Previous;
            if (Tail == null)
                Head = null;
            return result;
        }        
    }

    public class Clone
    {
        public Stack LearnedPrograms { get; set; }
        public Stack CanceledPrograms { get; set; }

        public Clone()
        {
            CanceledPrograms = new Stack();
            LearnedPrograms = new Stack();
        }

        public void Learn(int programIndex)
        {
            LearnedPrograms.Push(programIndex);
            while (CanceledPrograms.Head != null)
                CanceledPrograms.Pop();
        }

        public void Rollback()
        {
            CanceledPrograms.Push(LearnedPrograms.Pop());
        }

        public void Relearn()
        {
            LearnedPrograms.Push(CanceledPrograms.Pop());
        }

        public Clone Cloning()
        {
            var clone = new Clone();
            clone.LearnedPrograms.Head = LearnedPrograms.Head;
            clone.LearnedPrograms.Tail = LearnedPrograms.Tail;
            clone.CanceledPrograms.Head = CanceledPrograms.Head;
            clone.CanceledPrograms.Tail = CanceledPrograms.Tail;
            return clone;
        }

        public void Check()
        {
            if (LearnedPrograms.Head == null)
                Console.WriteLine("basic");
            else
            {
                var c = LearnedPrograms.Pop();
                LearnedPrograms.Push(c);
                Console.WriteLine(c);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            string[] tokens = Console.ReadLine().Split(' ');
            var clones = new List<Clone>();
            clones.Add(new Clone());
            for (int i = 0; i < int.Parse(tokens[0]); i++)
            {
                string[] command = Console.ReadLine().Split(' ');
                int currentClone = int.Parse(command[1]) - 1;
                switch (command[0])
                {
                    case "learn":
                        clones[currentClone].Learn(int.Parse(command[2]));
                        break;
                    case "rollback":
                        clones[currentClone].Rollback();
                        break;
                    case "relearn":
                        clones[currentClone].Relearn();
                        break;
                    case "clone":
                        clones.Add(clones[currentClone].Cloning());
                        break;                  
                    case "check":
                        clones[currentClone].Check();
                        break;
                    
                }
            }
        }
    }
}