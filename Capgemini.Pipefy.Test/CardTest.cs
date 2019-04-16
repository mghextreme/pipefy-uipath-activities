using System;
using System.Activities;
using System.Linq;
using Capgemini.Pipefy.Card;
using Capgemini.Pipefy.Phase;
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
            pipe = Helper.Pipe.CreatePipe("CardsPipe");
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

        [TestMethod]
        public void Card_Update_Success()
        {
            // Create

            var title = "Test Card " + DateTime.Now.ToShortDateString();
            var dict = testConfig.GetDefaultActivityArguments();
            dict["PipeID"] = pipe.Id;
            dict["Title"] = title;
            dict["DueDate"] = DateTime.Now.AddDays(6);

            PipefyQueryActivity act = new CreateCard();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var cardId = (long)result["CardID"];

            // Get

            dict = testConfig.GetDefaultActivityArguments();
            dict["CardID"] = cardId;

            act = new GetCard();
            result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var cardJson = (JObject)result["Card"];
            Assert.AreEqual(title, cardJson.Value<string>("title"));

            // Update

            title = "Updated Card " + DateTime.Now.ToShortDateString();

            dict = testConfig.GetDefaultActivityArguments();
            dict["CardID"] = cardId;
            dict["Title"] = title;

            act = new UpdateCard();
            result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            // Get

            dict = testConfig.GetDefaultActivityArguments();
            dict["CardID"] = cardId;

            act = new GetCard();
            result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            cardJson = (JObject)result["Card"];
            Assert.AreEqual(title, cardJson.Value<string>("title"));

            // Delete

            dict = testConfig.GetDefaultActivityArguments();
            dict["CardID"] = cardId;

            act = new DeleteCard();
            result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
        }

        [Ignore("Current API limitations don't let me set the destination phase, meaning I can't program a test for the Move action.")]
        [TestMethod]
        public void Card_Move_Success()
        {
            // Create

            var title = "Test Card " + DateTime.Now.ToShortDateString();
            var dict = testConfig.GetDefaultActivityArguments();
            dict["PipeID"] = pipe.Id;
            dict["Title"] = title;
            dict["DueDate"] = DateTime.Now.AddDays(7);
            dict["PhaseID"] = simplePhase.Id;

            PipefyQueryActivity act = new CreateCard();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var cardId = (long)result["CardID"];

            // Move

            dict = testConfig.GetDefaultActivityArguments();
            dict["CardID"] = cardId;
            dict["PhaseID"] = customFieldsPhase.Id;

            act = new MoveCardToPhase();
            result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            // Delete

            dict = testConfig.GetDefaultActivityArguments();
            dict["CardID"] = cardId;

            act = new DeleteCard();
            result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
        }
    }
}