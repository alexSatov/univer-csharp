using System;
using System.IO;

namespace Pluralize
{
	static class Program
	{
		static void Main(string[] args)
		{
			// Это пример ввода сложных данных из файла.
			// Циклы, строки, массивы будут рассмотрены на лекциях чуть позже, но это не должно быть препятствием вашему любопытству! :)
			string[] lines = File.ReadAllLines("rubles.txt");
			bool hasErrors = false;
			foreach (var line in lines)
			{
				string[] words = line.Split(' ');
				int count = int.Parse(words[0]);
				string rightAnswer = words[1];
				string pluralizedRubles = PluralizeRubles(count);
				if (pluralizedRubles != rightAnswer)
				{
					hasErrors = true;
					Console.WriteLine("Wrong answer: {0} {1}", count, pluralizedRubles);
				}
			}
			if (!hasErrors)
				Console.WriteLine("Correct!");
            Console.ReadKey();
        }

		private static string PluralizeRubles(int count)
        {
            // Напишите функцию склонения слова "рублей" в зависимости от предшествующего числительного n.
            if (count % 100 > 10 && count % 100 < 21 || count % 10 > 4 || count % 10 == 0)
                return "рублей";
            if (count % 10 == 1)
                return "рубль";
            
            return "рубля";
        }
	}
}
