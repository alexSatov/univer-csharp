using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace project_text
{
    class Program
    {
        public static void Bigrams (string str3)
        {
            var word = new StringBuilder();
            string currentLine = "";
            string[] sentences ;
            var words = new List<string>();
            var bigrams = new Dictionary<string, int>();
            var halfbigram = new List<string>();
            string bigram = "";
            sentences = str3.Split(new string[] { ".", "!", "?", ";", ":", "(", ")", "-" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var e in sentences)
            {
                currentLine = e;
                for (int i = 0; i < currentLine.Length; i++)
                {
                    if (char.IsLetter(currentLine[i]) || currentLine[i] == '\'')
                        word.Append(currentLine[i]);
                    else if (word.Length > 0)
                    {
                        words.Add(word.ToString());
                        word.Clear();
                    }
                }
                
                foreach (var rightword in words)
                    halfbigram.Add(rightword);
                for (int i = 0; i < halfbigram.Count - 1; i++)
                {
                    bigram = halfbigram[i] + " " + halfbigram[i + 1];
                }
                if (!bigrams.ContainsKey(bigram.ToLower()))
                    bigrams[bigram.ToLower()] = 0;
                bigrams[bigram.ToLower()]++;
                halfbigram.Clear();
                words.Clear();
            }
            int count = 0;
            foreach (var e in bigrams.OrderByDescending(e => e.Value))
            {
                Console.WriteLine("{0}  {1}", e.Key, e.Value);
                count++;
                if (count == 20) break;
            }
        }
    

        public static void FileCleaning(string[] str2)
        {
            var a = new StringBuilder(); var word = new StringBuilder();
            var newText = new string[3378];            
            for (int j = 0; j < 3378; j++)
            {
                var currentLine = str2[j];
                word.Clear();
                a.Clear(); a.Append(currentLine);
                for (int i = 0; i < currentLine.Length; i++)
                {                    
                    if (char.IsLetter(currentLine[i]) || currentLine[i] == '\'')
                        word.Append(currentLine[i]);
                    else if (word.Length > 0)
                    {                        
                        word.Append(" "); word.Insert(0, " "); 
                        if (   word.ToString().ToLower() == " the "   || word.ToString().ToLower() == " a "
                            || word.ToString().ToLower() == " in "    || word.ToString().ToLower() == " of "
                            || word.ToString().ToLower() == " to "    || word.ToString().ToLower() == " on "
                            || word.ToString().ToLower() == " at "    || word.ToString().ToLower() == " as "
                            || word.ToString().ToLower() == " with "  || word.ToString().ToLower() == " from "
                            || word.ToString().ToLower() == " for "   || word.ToString().ToLower() == " with "
                            || word.ToString().ToLower() == " about " || word.ToString().ToLower() == " into ")
                            a.Replace(word.ToString(), " ");                                                                           
                        word.Clear();
                    }
                }
                newText[j] = a.ToString();
            }
            File.WriteAllLines(@"C: \Users\Александр\Desktop\C#\Практики\Практика 5\Text1.txt", newText);
        }

        public static void FiftyWords(string[] str1)
        {
            var words = new Dictionary<string, int>();
            var word = new StringBuilder();
            foreach (var e in str1)
            {
                var currentLine = e;
                word.Clear();
                for (int i = 0; i < currentLine.Length; i++)
                {
                    if (char.IsLetter(currentLine[i]) || currentLine[i] == '\'')
                        word.Append(currentLine[i]);
                    else if (word.Length > 0)
                    {
                        if (!words.ContainsKey(word.ToString().ToLower()))
                            words[word.ToString().ToLower()] = 0;
                        words[word.ToString().ToLower()]++;
                        word.Clear();
                    }
                }
            }
            int count = 0;
            foreach (var e in words.OrderByDescending(e => e.Value))
            {
                Console.WriteLine("{0}  {1}", e.Key, e.Value);
                count++;
                if (count == 50) break;
            }
        }
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"C: \Users\Александр\Desktop\C#\Практики\Практика 5\Text.txt");
            string text = File.ReadAllText(@"C: \Users\Александр\Desktop\C#\Практики\Практика 5\Text1.txt");
            //FiftyWords(lines);
            //FileCleaning(lines);
            Bigrams(text);
            Console.ReadKey();
        }
    }
}
