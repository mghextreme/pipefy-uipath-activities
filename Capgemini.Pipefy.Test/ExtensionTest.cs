using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void Extension_ToQueryValueString_Success()
        {
            string original = @"You look ""hasty"".";
            string expected = @"""You look \""hasty\"".""";

            string result = original.ToQueryValue();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Extension_ToQueryValueInteger_Success()
        {
            long original = 98714;
            string expected = "98714";

            string result = original.ToQueryValue();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Extension_ToQueryValueArray_Success()
        {
            int[] original = new int[]{ 123, -813, 1995, 5, 105 };
            string expected = "[123,-813,1995,5,105]";

            string result = original.ToQueryValue();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Extension_ToQueryValueStringArray_Success()
        {
            string[] original = new string[]{ "One", "Two", "Forty five", "~\"Sure\"~" };
            string expected = "[\"One\",\"Two\",\"Forty five\",\"~\\\"Sure\\\"~\"]";

            string result = original.ToQueryValue();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Extension_ToQueryValueDateTime_Success()
        {
            DateTime original = new DateTime(2019, 03, 25, 12, 56, 12, DateTimeKind.Local);
            var utcDiff = TimeZoneInfo.Local.GetUtcOffset(original);
            string expected = string.Format("\"2019-03-25T12:56:12{2}{0}:{1}\"", utcDiff.Hours.ToString("00"), utcDiff.Minutes.ToString("00"), utcDiff.Hours >= 0 ? "+" : string.Empty);

            string result = original.ToQueryValue();
            Assert.AreEqual(expected, result);
        }
    }
}