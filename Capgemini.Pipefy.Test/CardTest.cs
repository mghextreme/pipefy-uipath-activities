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
        private static TestConfiguration testConfig;
        private static Helper.Pipe pipe;
        private static Helper.Phase simplePhase, customFieldsPhase;

        [ClassInitialize]
        public static void PipeTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            pipe = Helper.Pipe.CreatePipe("PhasesPipe");
            simplePhase = Helper.Phase.CreatePhase(pipe, "SimplePhase", false);
            customFieldsPhase = Helper.Phase.CreatePhase(pipe, "CustomFieldsPhase", false);

            // Add Custom Fields

            var numberField = new Helper.CustomField("Mileage", "number");
            customFieldsPhase.CreateField(numberField);
            var stringField = new Helper.CustomField("Chassi code", "short_text");
            customFieldsPhase.CreateField(stringField);
            var dateTimeField = new Helper.CustomField("Bought on", "datetime");
            customFieldsPhase.CreateField(dateTimeField);
        }

        [ClassCleanup]
        public static void PipeTestCleanup()
        {
            simplePhase.Delete();
            customFieldsPhase.Delete();
            pipe.Delete();
        }

        [TestMethod]
        public void Card_CreateAndDelete_Success()
        {
            // Create

            var dict = testConfig.GetDefaultActivityArguments();
            dict["PipeID"] = pipe.Id;
            dict["Title"] = "Test Card " + DateTime.Now.ToShortDateString();
            dict["DueDate"] = DateTime.Now.AddDays(6);

            var act = new CreateCard();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act.SuccessMessage, result["Status"].ToString());
            Assert.IsTrue((long)result["CardID"] > 0);

            // Delete

            dict = testConfig.GetDefaultActivityArguments();
            dict["CardID"] = result["CardID"];

            var act2 = new DeleteCard();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act2.SuccessMessage, result["Status"].ToString());
        }
    }
}