using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace autocomplete
{
	public class PhraseGenerator
	{
		private readonly string[] verbs;
		private readonly string[] adjectives;
		private readonly string[] nouns;
		private static readonly Random random = new Random(1234567890);

		public PhraseGenerator(string directory)
		{
			verbs = File.ReadAllLines(Path.Combine(directory, "verbs.txt"));
			adjectives = File.ReadAllLines(Path.Combine(directory, "adjectives.txt"));
			nouns = File.ReadAllLines(Path.Combine(directory, "nouns.txt"));
		}

		public IEnumerable<string> Generate()
		{
			foreach (var word in verbs.Concat(adjectives).Concat(nouns))
				yield return word;
			while (true)
			{
				var v = verbs[random.Next(verbs.Length)];
				var a = adjectives[random.Next(adjectives.Length)];
				var n = nouns[random.Next(nouns.Length)];
				yield return v + " " + a + " " + n;
			}
		}
	}
}