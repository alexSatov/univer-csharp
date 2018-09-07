using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoBilliard
{
    class Program
    {
        public static string Decision (int pocketCount)
        {
            if (pocketCount != 0)
                return "Cheater";
            else
                return "Not a proof";
        }

        public static int[] GetExaminerBallsOrder()
        {
            int n = int.Parse(Console.ReadLine());
            var tokens = new int[n];
            for (int i = 0; i < n; i++)
                tokens[i] = int.Parse(Console.ReadLine());
            return tokens;
        }

        public static int ResultOfGame(int[] examinerBallsOrder)
        {
            var pocket = new Stack<int>();
            int сhichikovBallsCounter = 1;
            int examinerBallsCounter = 0;
            while (сhichikovBallsCounter <= examinerBallsOrder.Count())
            {
                if (pocket.Count == 0)
                    pocket.Push(сhichikovBallsCounter);
                if (pocket.Peek() != examinerBallsOrder[examinerBallsCounter])
                {
                    сhichikovBallsCounter++;
                    pocket.Push(сhichikovBallsCounter);
                }
                else
                {
                    pocket.Pop();
                    if (pocket.Count == 0)
                        сhichikovBallsCounter++;
                    if (examinerBallsCounter < examinerBallsOrder.Length - 1)
                        examinerBallsCounter++;
                }
            }
            return pocket.Count;
        }

        static void Main(string[] args)
        {
            var examinerBallsOrder = GetExaminerBallsOrder();            
            Console.WriteLine(Decision(ResultOfGame(examinerBallsOrder)));            
        }
    }
}
