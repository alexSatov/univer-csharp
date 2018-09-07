using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolynomRootFinder;

namespace Root
{
    class Program
    {
        static void Main(string[] args)
        {
            double left = double.Parse(Console.ReadLine());
            double right = double.Parse(Console.ReadLine());
            var arguments = new List<double>();
            int i = 0;
            while (true)
            {
                string x = Console.ReadLine();
                if (x.Equals("")) break;
                else arguments.Add(double.Parse(x));
                i++;
            }
            Console.WriteLine(RootFinder.Solution(left, right, arguments.ToArray()));
            Console.ReadKey();
        }
    }
}
