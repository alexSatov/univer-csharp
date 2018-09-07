using System;
using System.Linq;

namespace names
{
	internal class NamesTasks
	{
        		
		public static void ShowBirthYearsStatisticsHistogram(NameData[] names)
		{
			Console.WriteLine("Статистика рождаемости по годам");
			var minYear = int.MaxValue; 
			var maxYear = int.MinValue;
			foreach (var name in names)
			{
				minYear = Math.Min(minYear, name.BirthDate.Year);
				maxYear = Math.Max(maxYear, name.BirthDate.Year);
			}
			var years = new string[maxYear - minYear + 1];
			for (int y = 0; y < years.Length; y++)
				years[y] = (y + minYear).ToString();
			var birthsCounts = new double[maxYear - minYear + 1];
			foreach (var name in names)
				birthsCounts[name.BirthDate.Year - minYear]++;
			//Charts.ShowHistorgam("Рождаемость по годам", years, birthsCounts);
		}

		public static void ShowBirthDaysOfName(NameData[] names, string name)
		{
            var monthDays = new string [31];
            for (int i = 0; i < monthDays.Length; i++)
                monthDays[i] = i.ToString();            
            var daysCounts = new double[32];
            foreach (var n in names)
                if (n.Name == name && n.BirthDate.Day != 1)
                    daysCounts[n.BirthDate.Day]++;
            //Charts.ShowHistorgam("Рождаемость людей с именем '" + name + "'", monthDays, daysCounts);
        }

		public static void ShowBirthDateStatistics(NameData[] names)
		{
			Console.WriteLine("Статистика рождаемости по датам");         
            var cal = new double[31, 12];
            foreach (var name in names)
                cal[name.BirthDate.Day - 1, name.BirthDate.Month - 1]++;
            cal[0, 0] /= 2.5;                                  
            for (int i = 0; i < cal.GetLength(0); i++)
                for (int j = 0; j < cal.GetLength(1); j++)
                    cal[i, j] /= 100;
            Charts.ShowHeatmap("Рождаемости по датам", cal, 1, 1);
        }

		public static void ShowYourStatistics(NameData[] names)
		{          
            char mainLetter = 'а';
            var months = new string[13];
            for (int i = 1; i < months.Length; i++)
                months[i] = i.ToString();
            var namesCounts = new double[13];
            foreach (var name in names)
                if (name.Name[0] == mainLetter)
                   namesCounts[name.BirthDate.Month]++;
            //Charts.ShowHistorgam("Рождаемость по месяцам людей с заданной буквой имени", months, namesCounts);
        }
	}
}
