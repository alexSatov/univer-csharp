using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Application
{
    class Program
    {
        static double AccomAmount (double sum, double rate, int date)
        {
            return sum*(Math.Pow((rate / 100 / 12 + 1), date) - 1);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Введите сумму");
            double sum = double.Parse(Console.ReadLine());

            Console.WriteLine("Введите процентную ставку");
            int rate = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите срок вклада");
            int date = int.Parse(Console.ReadLine());

            Console.WriteLine(AccomAmount(sum,rate,date));
            Console.ReadKey();
        }
    }
}
