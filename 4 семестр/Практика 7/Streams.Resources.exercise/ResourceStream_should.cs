using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streams.Resources
{
    [TestFixture]
    class ResourceStream_should
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["Key1"] = "Value1",
            ["Key2"] = "SomeValueWith \0 byte",
            ["Key3"] = new string('a', 10000)
        };


        TestStream MakeTestStream()
        {
            return new TestStream(data.SelectMany(z => new[] { z.Key, z.Value }));
        }

        void Read(string key, TestStream stream)
        {
            var reader = new ResourceReaderStream(stream, key);
            var buffer = new byte[100];
            var result = new List<byte>();
            while (true)
            {
                var count = reader.Read(buffer, 0, 100);
                if (count == 0) break;
                result.AddRange(buffer.Take(count));
            }
            var str = Encoding.ASCII.GetString(result.ToArray());
            Assert.AreEqual(data[key], str);
        }

        [Test]
        public void ReadKey1()
        {
            var testStream = MakeTestStream();
            Read("Key1", testStream);
            Assert.True(testStream.Counts.All(z => z == Constants.BufferSize), "Все чтения из нижележащего стрима должны быть с count="+Constants.BufferSize);
        }

        [Test]
        public void ReadKey2()
        {
            var testStream = MakeTestStream();
            Read("Key2", testStream);
            Assert.True(testStream.Counts.All(z => z == Constants.BufferSize), "Все чтения из нижележащего стрима должны быть с count=" + Constants.BufferSize);
        }

        [Test]
        public void ReadKey3()
        {
            var testStream = MakeTestStream();
            Read("Key3", testStream);
            Assert.True(testStream.Counts.All(z => z == Constants.BufferSize), "Все чтения из нижележащего стрима должны быть с count=" + Constants.BufferSize);

        }

    }
}
