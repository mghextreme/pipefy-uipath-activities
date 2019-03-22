using System;
using System.Activities;
using System.Linq;
using Capgemini.Pipefy.Card;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class CardTest
    {
        [TestMethod]
        public void Card_CreateAndDelete_Success()
        {
            var config = TestConfiguration.Instance.Configuration;

            // Create

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["PipeID"] = config["pipe"].Value<string>("id");
            dict["Title"] = "Test Card " + DateTime.Now.ToShortDateString();
            dict["DueDate"] = DateTime.Now.AddDays(6);

            var act = new CreateCard();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act.SuccessMessage, result["Status"].ToString());
            Assert.IsTrue((long)result["CardID"] > 0);

            // Delete

            dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["CardID"] = result["CardID"];

            var act2 = new DeleteCard();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act2.SuccessMessage, result["Status"].ToString());
        }
    }
}