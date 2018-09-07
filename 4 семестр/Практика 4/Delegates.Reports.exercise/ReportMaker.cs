using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Delegates.Reports
{
    public static class ReportMakerHelper
    {
        public static string MakeFirstReport(IEnumerable<Measurement> data)
        {
            var reportMaker = new HtmlReportMaker();

            var content = new ReportContent("Mean and Std");

            foreach (var propertyInfo in typeof(Measurement).GetProperties())
            {
                var name = propertyInfo.Name;
                content.AddItem(new ReportItem(name,
                    Statistics.DeviationsStat(
                        data.Select(z => (double)typeof(Measurement).GetProperty(name).GetValue(z))
                    ).ToString()));
            }

            return reportMaker.MakeReport(content);
        }

        public static string MakeSecondReport(IEnumerable<Measurement> data)
        {
            var reportMaker = new MarkdownReportMaker();

            var content = new ReportContent("Median");

            foreach (var propertyInfo in typeof(Measurement).GetProperties())
            {
                var name = propertyInfo.Name;
                content.AddItem(new ReportItem(name,
                    Statistics.MedianStat(
                        data.Select(z => (double)typeof(Measurement).GetProperty(name).GetValue(z))
                    ).ToString(CultureInfo.InvariantCulture)));
            }

            return reportMaker.MakeReport(content);
        }
    }

    public struct ReportItem
    {
        public string ValueType { get; }
        public string Entry { get; }

        public ReportItem(string valueType, string entry)
        {
            ValueType = valueType;
            Entry = entry;
        }
    }

    public class ReportContent
    {
        public readonly string Caption;
        public readonly List<ReportItem> ReportItems = new List<ReportItem>();

        public ReportContent(string caption)
        {
            Caption = caption;
        }

        public void AddItem(ReportItem reportItem)
        {
            ReportItems.Add(reportItem);
        }
    }

    public interface IReportMaker
    {
        string MakeReport(ReportContent content);
    }

    public class HtmlReportMaker: IReportMaker
    {
        public string MakeReport(ReportContent content)
        {
            var result = new StringBuilder();

            result.Append($"<h1>{content.Caption}</h1>");

            result.Append("<ul>");
            foreach (var reportItem in content.ReportItems)
            {
                result.Append($"<li><b>{reportItem.ValueType}</b>: {reportItem.Entry}");
            }
            result.Append("</ul>");

            return result.ToString();
        }
    }

    public class MarkdownReportMaker: IReportMaker
    {
        public string MakeReport(ReportContent content)
        {
            var result = new StringBuilder();

            result.Append($"## {content.Caption}\n\n");

            foreach (var reportItem in content.ReportItems)
            {
                result.Append($" * **{reportItem.ValueType}**: {reportItem.Entry}\n\n");
            }

            return result.ToString();
        }
    }

    public static class Statistics
    {
        public static double MedianStat(IEnumerable<double> data)
        {
            var values = data.OrderBy(z => z).ToList();
            if (values.Count % 2 == 0)
                return (values[values.Count / 2] + values[values.Count / 2 + 1]) / 2;
            return values[values.Count / 2];
        }

        public static MeanAndStd DeviationsStat(IEnumerable<double> data)
        {
            var values = data.ToList();
            var mean = values.Average();
            var std = Math.Sqrt(values.Select(z => Math.Pow(z - mean, 2)).Sum() / (values.Count - 1));

            return new MeanAndStd
            {
                Mean = mean,
                Std = std
            };
        }
    }
}
