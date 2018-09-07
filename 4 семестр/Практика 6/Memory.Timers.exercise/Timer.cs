using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Memory.Timers
{
    public static class Timer
    {
        public static string Report => report.ToString();

        private static int nesting = -1;
        private static InnerTimer deepestTimer;
        private static readonly StringBuilder report = new StringBuilder();

        public static InnerTimer Start(string name = "*")
        {
            nesting++;
            var timer = new InnerTimer(name);

            if (nesting == 0)
            {
                report.Clear();
                deepestTimer = timer;
                return timer;
            }

            timer.SuperTimer = deepestTimer;
            deepestTimer.SubTimers.Add(timer);
            deepestTimer = timer;

            return timer;
        }

        public class InnerTimer : IDisposable
        {
            public string Name { get; }
            public Stopwatch Watch { get; }
            public InnerTimer SuperTimer { get; set; }
            public List<InnerTimer> SubTimers { get; }

            private readonly StringBuilder reportEntry;

            public InnerTimer(string name)
            {
                SubTimers = new List<InnerTimer>();
                reportEntry = new StringBuilder();
                Name = name;
                Watch = new Stopwatch();
                Watch.Start();
            }

            public void Dispose()
            {
                Watch.Stop();
                FormReportEntry();

                if (nesting == 0)
                    report.Append(reportEntry);

                deepestTimer = SuperTimer;
                nesting--;
            }

            private void FormReportEntry()
            {
                var firstIndent = GetWhiteSpaces(nesting * 4);
                var secondIndent = GetWhiteSpaces(20 - Name.Length - nesting * 4);
                reportEntry.Append($"{firstIndent}{Name}{secondIndent}: {Watch.ElapsedMilliseconds}\n");

                if (SubTimers.Count > 0)
                {
                    foreach (var timer in SubTimers)
                    {
                        reportEntry.Append(timer.reportEntry);
                    }

                    var restTime = Watch.ElapsedMilliseconds - SubTimers.Sum(st => st.Watch.ElapsedMilliseconds);
                    firstIndent = GetWhiteSpaces((nesting + 1) * 4);
                    secondIndent = GetWhiteSpaces(20 - (nesting + 2) * 4);
                    reportEntry.Append($"{firstIndent}Rest{secondIndent}: {restTime}\n");
                }
            }

            private static string GetWhiteSpaces(int count)
            {
                return new string(' ', count);
            }
        }
    }
}
