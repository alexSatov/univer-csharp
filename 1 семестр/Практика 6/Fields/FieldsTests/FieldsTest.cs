using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FieldsDivider;

namespace FieldsTests
{
    [TestClass]
    public class FieldsTest
    {
        [TestMethod]
        public void Test1()
        {
            var testText = new string[4];
            testText[0] = "acd fgh \"k\'p\'l\" \"tyo\" gdshn dsfb";
            testText[1] = "\'\'";
            testText[2] = "lpdnfjkb odfbfkbn iflkmsdf";
            testText[3] = "";
            int f = 0;

            var fields = StringFields.GetRightFields(testText, f);
            Assert.AreEqual("acd", fields[0]);
            Assert.AreEqual("", fields[1]);
            Assert.AreEqual("lpdnfjkb", fields[2]);
            Assert.AreEqual("null", fields[3]);
        }
        [TestMethod]
        public void Test2()
        {
            var testText = new string[4];
            testText[0] = "acd fgh \"k\'p\'l\" \"tyo\" gdshn dsfb";
            testText[1] = "\'\'";
            testText[2] = "lpdnfjkb \"odfbfkbn iflkmsdf";
            testText[3] = "";
            int f = 1;

            var fields = StringFields.GetRightFields(testText, f);
            Assert.AreEqual("fgh", fields[0]);
            Assert.AreEqual("null", fields[1]);
            Assert.AreEqual("odfbfkbn iflkmsdf", fields[2]);
            Assert.AreEqual("null", fields[3]);
        }
        [TestMethod]
        public void Test3()
        {
            var testText = new string[4];
            testText[0] = "acd fgh \"k\'p\'l \" \"tyo\" gdshn dsfb";
            testText[1] = "if\'lk\'\'df\'";
            testText[2] = "lpdnfjkb a\'odfbfkbn";
            testText[3] = " ";
            int f = 2;

            var fields = StringFields.GetRightFields(testText, f);
            Assert.AreEqual("k\'p\'l ", fields[0]);
            Assert.AreEqual("df", fields[1]);
            Assert.AreEqual("odfbfkbn", fields[2]);
            Assert.AreEqual("null", fields[3]);
        }
        [TestMethod]
        public void Test4()
        {
            var testText = new string[2];
            testText[0] = "acd\'fgh\"k\'p\'l\"gdshn dsfb";
            testText[1] = "p\'l\"gdshn dsfb";            
            int f = 1;

            var fields = StringFields.GetRightFields(testText, f);
            Assert.AreEqual("fgh\"k", fields[0]);
            Assert.AreEqual("l\"gdshn dsfb", fields[1]);
        }
        [TestMethod]
        public void Test5()
        {
            var testText = new string[2];
            testText[0] = "acd\\'fgh\\\\\"k\'p\'l\"gdshn dsfb";
            testText[1] = "dsf\\\\\'sdh h\'tykfg dfhh";
            int f = 0;

            var fields = StringFields.GetRightFields(testText, f);
            Assert.AreEqual("acd\'fgh\\", fields[0]);
            Assert.AreEqual("dsf\\", fields[1]);
        }
    }
}
