using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZedGraph;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Slides
{
    public static class IEnumerableExtensions
    {
        public static double Median(this IEnumerable<double> items)
        {
            var itemsCopy = items.ToList();
            if (itemsCopy.Count() == 0)
                return double.NaN;

            var uniqueItems = itemsCopy                             
                              .OrderBy(i => i)
                              .ToArray();
            if (uniqueItems.Length % 2 == 0)
            {
                var median = (uniqueItems[uniqueItems.Length / 2] + uniqueItems[uniqueItems.Length / 2 - 1]) / 2;
                return median;
            }
            else
            {
                var median = uniqueItems[uniqueItems.Length / 2];
                return median;
            }
        }

        public static IEnumerable<Tuple<T, T>> GetBigrams<T>(this IEnumerable<T> items)
        {
            //if (items.Count() == 0 || items.Count() == 1)
            //    return null;

            //var conteiner = items.Take(1).ToArray()[0];
            //return items
            //       .Skip(1)
            //       .Select(i =>
            //                  {
            //                      var t = Tuple.Create(conteiner, i);
            //                      conteiner = i;
            //                      return t;
            //                  })
            //       .Distinct(); 
            bool firstElement = true;
            T conteiner = default(T);
            foreach (var item in items)
            {
                if (firstElement)
                {
                    conteiner = item;
                    firstElement = false;
                    continue;
                }
                yield return Tuple.Create(conteiner, item);
                conteiner = item;
            }                
        }
    }

    class Slide
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Topic { get; set; }

        public Slide(string id, string type, string topic)
        {
            ID = id;
            Type = type;
            Topic = topic;
        }
    }

    class Visit
    {
        public string UserID { get; set; }
        public Slide Slide { get; set; }
        public DateTime VisitTime { get; set; }

        public Visit(string userID, Slide slide, DateTime visitTime)
        {
            UserID = userID;
            Slide = slide;
            VisitTime = visitTime;
        }
    }

    class Program
    {
        public static string[][]  GetData(string fileName)
        {
            var file = File.ReadAllLines(fileName, Encoding.Default);
            return file
                   .Skip(1)
                   .Select(line => line.Split(';'))
                   .ToArray();
        }

        public static Slide[] GetSlides(string[][] slidesData)
        {
            return slidesData
                   .Select(slide => new Slide(slide[0], slide[1], slide[2]))
                   .ToArray();
        }

        public static DateTime GetVisitTime(Tuple<string, string> visitTime)
        {
            var date = visitTime.Item1.Split('-');
            var time = visitTime.Item2.Split(':', '.');
            return new DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]),
                                int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]), int.Parse(time[3]));
        }

        public static Visit[] GetVisits(string[][] visitsData, Slide[] slides)
        {
            return visitsData
                   .Select(visit => new Visit(visit[0], slides
                                                        .Where(slide => slide.ID == visit[1])
                                                        .First(),
                                                        GetVisitTime(visit.GetBigrams().ToArray()[2])))
                   .ToArray();
        }

        public static ILookup<string, Visit> GetUsers (Visit[] visits)
        {
            return visits.ToLookup(user => user.UserID, userInfo => userInfo);
        }

        public static TimeSpan GetTimeDiff (Tuple<Visit, Visit> userSlidesCouple)
        {
            return userSlidesCouple.Item2.VisitTime.Subtract(userSlidesCouple.Item1.VisitTime);
        }

        public static Dictionary<Tuple<string, Slide>, TimeSpan> GetStatistic (ILookup<string, Visit> users)
        {
            var tUserSlide = new Dictionary<Tuple<string, Slide>, TimeSpan>();
            foreach (var user in users)
            {
                var userSlidesCouples = new List<Tuple<Visit, Visit>>();
                if (users[user.Key].Count() > 1)
                    userSlidesCouples = users[user.Key].GetBigrams().ToList();
                else
                {
                    tUserSlide.Add(Tuple.Create(user.Key, users[user.Key].ToArray()[0].Slide), new TimeSpan(0));
                    continue;
                }
                int i = 0;
                foreach (var slide in users[user.Key])
                {
                    if (i != users[user.Key].Count() - 1)
                    {
                        if (!tUserSlide.ContainsKey(Tuple.Create(user.Key, slide.Slide)))
                            tUserSlide.Add(Tuple.Create(user.Key, slide.Slide), GetTimeDiff(userSlidesCouples[i]));
                    }
                    else
                    {
                        if (!tUserSlide.ContainsKey(Tuple.Create(user.Key, slide.Slide)))
                            tUserSlide.Add(Tuple.Create(user.Key, slide.Slide), new TimeSpan(0));
                    }
                    i++;
                }
            }
            return tUserSlide;
        }

        public static double TypeMedian(Dictionary<Tuple<string,Slide>, TimeSpan> tUserSlide, string type)
        {
            var correctValues = tUserSlide
                                          .Where(x => x.Key.Item2.Type == type)
                                          .Where(x => x.Value.TotalMinutes >= 1 && x.Value.TotalHours <= 2)
                                          .Select(x => x.Value.TotalMinutes)
                                          .ToArray();
            return correctValues.Median();
        }

        public static ILookup<string, double> GraphStatistic(Dictionary<Tuple<string, Slide>, TimeSpan> tUserSlide)
        {
            var graphStatistic = tUserSlide.ToLookup(key => key.Key.Item2.Topic, value => value.Value.TotalHours);
            var result = graphStatistic.ToLookup(key => key.Key, value => value.Sum());
            return result;
        }

        public static void Graph(Dictionary<Tuple<string, Slide>, TimeSpan> tUserSlide)
        {
            var graphStatistic = GraphStatistic(tUserSlide);
            var form = new Form { WindowState = FormWindowState.Maximized };
            var chart = new ZedGraphControl() { Dock = DockStyle.Fill };
            var pane = new GraphPane();
            pane.CurveList.Clear();
            var curve = pane.AddBar("Часы", null, graphStatistic.Select(hours => graphStatistic[hours.Key].First()).ToArray(), Color.Brown);
            pane.XAxis.Type = AxisType.Text;
            pane.XAxis.Scale.TextLabels = graphStatistic.Select(topic => topic.Key).ToArray();
            pane.AxisChange();
            chart.GraphPane = pane;
            chart.Invalidate();
            form.Controls.Add(chart);
            Application.Run(form);
        }

        public static void MinutesPerSlide(Dictionary<Tuple<string, Slide>, TimeSpan> tUserSlide)
        {
            var theoryMedian = TypeMedian(tUserSlide, "theory");
            var exerciseMedian = TypeMedian(tUserSlide, "exercise");
            var quizMedian = TypeMedian(tUserSlide, "quiz");
            Console.WriteLine(theoryMedian + " minutes per theory slide");
            Console.WriteLine(exerciseMedian + " minutes per exercise slide");
            Console.WriteLine(quizMedian + " minutes per quiz slide");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            var slidesData = GetData("slides.txt");
            var visitsData = GetData("visits.txt");
            var slides = GetSlides(slidesData);
            var visits = GetVisits(visitsData, slides);
            var users = GetUsers(visits);
            var tUserSlide = GetStatistic(users);            
            MinutesPerSlide(tUserSlide);
            //Graph(tUserSlide);
        }
    }
}
