using NUnit.Framework;
using System.Collections.Generic;

namespace Delegates.Reports
{
    [TestFixture]
    class ReportMaker_should
    {
        List<Measurement> data = new List<Measurement>
        {
            new Measurement
            {
                 Humidity=1,
                  Temperature=-10,
            },
            new  Measurement
            {
                Humidity=2,
                Temperature=2,
            },
            new Measurement
            {
                Humidity=3,
                Temperature=14
            },
            new Measurement
            {
                Humidity=2,
                Temperature=30
            }
        };


        [Test]
        public void CheckFirstReport()
        {
            var result = @"<h1>Mean and Std</h1><ul><li><b>Temperature</b>: 9±17.0880074906351<li><b>Humidity</b>: 2±0.816496580927726</ul>";
            Assert.AreEqual(result, ReportMakerHelper.MakeFirstReport(data));
        }

        [Test]
        public void CheckSecondReport()
        {
            var result = "## Median\n\n * **Temperature**: 22\n\n * **Humidity**: 2.5\n\n";
            Assert.AreEqual(result, ReportMakerHelper.MakeSecondReport(data));
        }
    }
}
