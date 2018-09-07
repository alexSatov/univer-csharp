using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Memory.API
{
    [TestFixture]
    class APIObject_should
    {
        [Test]
        public void FreeResourcesWhenDisposeCalled()
        {
            var q = new APIObject(1);
            Assert.True(MagicAPI.Contains(1));
            q.Dispose();
            Assert.False(MagicAPI.Contains(1));
        }

        [Test]
        public void FreeResourcesInUsing()
        {
            using (var q = new APIObject(2))
            {
                Assert.True(MagicAPI.Contains(2));

            }
            Assert.False(MagicAPI.Contains(2));
        }

        [Test]
        public void DontFailWhenTwoDisposeAreCalled()
        {
            var q = new APIObject(3);
            Assert.True(MagicAPI.Contains(3));
            q.Dispose();
            q.Dispose();
            Assert.False(MagicAPI.Contains(3));
        }

        void M()
        {
            var q = new APIObject(4);
        }

        [Test]
        public void HaveFinalizer()
        {
            M();
            GC.Collect();
            Thread.Sleep(1000);
            Assert.False(MagicAPI.Contains(4));
        }

        [Test]
        public void NotFailWhenMagicAPIErrorOccured()
        {
            var q = new APIObject(5);
            MagicAPI.Free(5);
            q.Dispose();
        }
    }
}
