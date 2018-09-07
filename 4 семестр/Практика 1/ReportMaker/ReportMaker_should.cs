using NUnit.Framework;
using System.Collections.Generic;

namespace Incapsulation.Failures
{
    [TestFixture]
    class ReportMaker_should
    {
        [Test]
        public void WorkCorrectly()
        {
            var result = ReportMaker.FindDevicesFailedBeforeDateObsolete(
                 10,
                 5,
                 2010,
                 new[] { 0, 1, 2, 3 },
                 new[] { 0, 1, 2, 3 },
                 new[]
                 {
                    new object[] { 9,4,2010 },
                    new object[] { 9,4,2010 },
                    new object[] { 11, 5, 2010 },
                    new object[] {9,4,2010 }
                 },
                 new List<Dictionary<string, object>>
                 {
                    new Dictionary<string, object>
                    {
                        ["DeviceId"]=0,
                        ["Name"]="0"
                    },
                    new Dictionary<string, object>
                    {
                        ["DeviceId"]=1,
                        ["Name"]="1"
                    },
                    new Dictionary<string, object>
                    {
                        ["DeviceId"]=2,
                        ["Name"]="2"
                    },
                    new Dictionary<string, object>
                    {
                        ["DeviceId"]=3,
                        ["Name"]="3"
                    },
                 });

            CollectionAssert.AreEqual(new[] { "0" }, result);
        }
    }
}
