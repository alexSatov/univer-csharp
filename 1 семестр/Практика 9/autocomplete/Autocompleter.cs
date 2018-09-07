using System;
using System.Collections.Generic;

namespace autocomplete
{
    // Внимание!
    // Есть одна распространенная ловушка при сравнении строк: строки можно сравнивать по-разному:
    // с учетом регистра, без учета, зависеть от кодировки и т.п.
    // В файле словаря все слова отсортированы методом StringComparison.OrdinalIgnoreCase.
    // Во всех функциях сравнения строк в C# можно передать способ сравнения.
    public class Autocompleter
    {
        private readonly string[] items;

        public Autocompleter(string[] loadedItems)
        {
            items = loadedItems;
        }
        public int FindLimitIndex(string prefix, string side)
        {
            var left = 0;
            var right = items.Length - 1;
            if (side == "left")
            {
                while (left < right)
                {
                    var middle = left + (right - left) / 2;
                    var item = items[middle].Substring(0, Math.Min(prefix.Length, items[middle].Length));
                    if (prefix.ToLower().CompareTo(item) <= 0)
                        right = middle;
                    else
                        left = middle + 1;
                }
            }
            if (side == "right")
            {
                while (left < right)
                {
                    var middle = right - (right - left) / 2;
                    var item = items[middle].Substring(0, Math.Min(prefix.Length, items[middle].Length));
                    if (prefix.ToLower().CompareTo(item) < 0)
                        right = middle - 1;
                    else
                        left = middle;
                }
            }
            return left;
        }
        // Найти произвольный элемент словаря, начинающийся с prefix.
        // Ускорьте эту фунцию так, чтобы она работала за O(log(n))
        public string FindByPrefix(string prefix)
        {
            var left = 0;
            var right = items.Length - 1;
            var middle = left + (right - left) / 2;
            while (!items[middle].StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && right != left)
            {
                if (prefix.CompareTo(items[middle]) <= 0)
                    right = middle;
                else
                    left = middle + 1;
                middle = left + (right - left) / 2;
            }
            if (items[middle].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return items[middle];
            return null;
        }
        // Найти первые (в порядке следования в файле) 10 (или меньше, если их меньше 10) элементов словаря, 
        // начинающийся с prefix.
        // Эта функция должна работать за O(log(n) + count)
        public string[] FindByPrefix(string prefix, int count)
        {
            var result = new List<string>();
            var leftIndex = FindLimitIndex(prefix, "left");
            if (!items[leftIndex].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return new string[0];
            else
                for (int i = 0; i < count; i++)
                    if (items[leftIndex + i].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        result.Add(items[leftIndex + i]);
            return result.ToArray();
        }
        public static string LEFT = "left";
        public static string RIGHT = "right";
        // Найти количество слов словаря, начинающихся с prefix
        // Эта функция должна работать за O(log(n))
        public int FindCount(string prefix)
        {
            var leftIndex = FindLimitIndex(prefix, LEFT);
            var rightIndex = FindLimitIndex(prefix, RIGHT);
            if (!items[leftIndex].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return -1;
            else
                return rightIndex - leftIndex + 1;
        }
    }
}
