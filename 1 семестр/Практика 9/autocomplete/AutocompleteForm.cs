using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace autocomplete
{
	public partial class AutocompleteForm : Form
	{
		private Autocompleter autocompleter;
		private long count;
		private long sumMs;

		public AutocompleteForm()
		{
			InitializeComponent();
		}

		private void inputBox_TextChanged(object sender, EventArgs e)
		{
			autocompleteList.Items.Clear();

			// Stopwatch — это полезный инструмент для того, чтобы точно засекать время.
			// При измерении скорости работы алгоритма нужно использовать именно его.
			Stopwatch sw = Stopwatch.StartNew();
			string prefix = inputBox.Text;
			string[] foundItems = autocompleter.FindByPrefix(prefix, 10);
			int foundItemsCount = autocompleter.FindCount(prefix);
			if (foundItems.Length == 0)
			{
				string oneItem = autocompleter.FindByPrefix(prefix);
				if (oneItem != null)
					foundItems = new[] {oneItem};
			}
			sw.Stop();
			sumMs += sw.ElapsedMilliseconds;
			count++;
			statusLabel.Text = string.Format("Found: {0}; Last time: {1} ms; Average time: {2} ms", foundItemsCount, sw.ElapsedMilliseconds, sumMs / count);
			foreach (string foundItem in foundItems)
				autocompleteList.Items.Add(foundItem);
		}

		private void AutocompleteForm_Load(object sender, EventArgs e)
		{
			var phraseGenerator = CreatePhraseGenerator();
			if (phraseGenerator == null)
				Environment.Exit(-1);
			var phrases = phraseGenerator.Generate().Take(1500*1000).ToList();
			phrases.Sort();
			autocompleter = new Autocompleter(phrases.ToArray());
		}

		private static PhraseGenerator CreatePhraseGenerator()
		{
			try
			{
				return new PhraseGenerator("dic\\");
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString(), "Ошибка при загрузке файлов со словарями", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}
	}
}