using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using autocomplete;

namespace Autocompliter.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void FindByPrefixTest()
        {
            var completer = new autocomplete.Autocompleter(new String[] { "abc", "abcd", "afb", "afe", "azsd" });
            var result = completer.FindByPrefix("az");
            Assert.AreEqual("azsd", result);
        }        
        [TestMethod]
        public void FindCountTest()
        {
            var completer = new autocomplete.Autocompleter(new String[] { "abc", "abcd", "bfb", "bfe", "bfzsd" });
            var result = completer.FindCount("bf");
            Assert.AreEqual(3, result);
        }
        [TestMethod]
        public void FindLefttIndex()
        {
            var completer = new autocomplete.Autocompleter(new String[] { "abc", "abcd", "bfb", "bfe", "bfzsd" });
            var result = completer.FindLimitIndex("bf", "left");
            Assert.AreEqual(2, result);
        }
        [TestMethod]
        public void FindRightttIndex()
        {
            var completer = new autocomplete.Autocompleter(new String[] { "abc", "abcd", "bfb", "bfe", "bfzsd" });
            var result = completer.FindLimitIndex("bf", "right");
            Assert.AreEqual(4, result);
        }
    }
}
