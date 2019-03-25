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
    }
}