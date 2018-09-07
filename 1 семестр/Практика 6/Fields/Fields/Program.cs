using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldsDivider;
using System.IO;

namespace Fields
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = Console.ReadLine();
            var f = int.Parse(Console.ReadLine());
            var lines = File.ReadAllLines(fileName);
            foreach (var e in StringFields.GetRightFields(lines, f))
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
        }
    }
}
