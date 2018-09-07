using System;
using System.Diagnostics;
using System.IO;

namespace FluentApi.Graph
{
	class Program
	{
		// Вместе с решением поставляется урезанная версия утилиты graphviz.
		// Полная версия доступна на сайте http://www.graphviz.org
		// Для решения этой задачи строго говоря graphviz не нужен. 
		// Однако этот Main демонстрирует ради чего всё это делается.
		// 
		// Если у вас не заработает эта урезанная версия, вы можете установить полную 
		// и поменять этот путь на путь до dot.exe в полной версии:
		private const string PATH_TO_GRAPHVIZ = @"..\..\graphviz\dot.exe";

		static void Main(string[] args)
		{
			var dot =
				DotGraphBuilder.DirectedGraph("CommentParser")
				.AddNode("START").With(a => a.Shape(NodeShape.Ellipse).Color("green"))
				.AddNode("comment").With(a => a.Shape(NodeShape.Box))
				.AddEdge("START", "slash").With(a => a.Label("'/'"))
				.AddEdge("slash", "comment").With(a => a.Label("'/'"))
				.AddEdge("comment", "comment").With(a => a.Label("other chars"))
				.AddEdge("comment", "START").With(a => a.Label("'\\\\n'"))
				.Build();
			Console.WriteLine(dot);
			ShowRenderedGraph(dot);
		}

		private static void ShowRenderedGraph(string dot)
		{
			File.WriteAllText("comment.dot", dot);
			var processStartInfo = new ProcessStartInfo(PATH_TO_GRAPHVIZ)
			{
				UseShellExecute = false,
				Arguments = "comment.dot -Tpng -o comment.png",
				RedirectStandardError = true,
				RedirectStandardOutput = true,
			};
			var p = Process.Start(processStartInfo);
			p.WaitForExit();
			Console.WriteLine(p.StandardError.ReadToEnd());
			Process.Start("comment.png");
		}
	}
}