using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Capgemini.Pipefy.Phase;
using Capgemini.Pipefy.Pipe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class PhaseTest
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

            var numberField = new Helper.CustomField("Area", "number");
            customFieldsPhase.CreateField(numberField);
            var stringField = new Helper.CustomField("Main room", "short_text");
            customFieldsPhase.CreateField(stringField);
            var dateTimeField = new Helper.CustomField("Construction started", "datetime");
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
        public void Phase_GetSingle_Success()
        {
            var dict = testConfig.GetDefaultActivityArguments();
            dict["PhaseID"] = simplePhase.Id;

            var act = new GetPhase();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Phase"] as JObject;
            Assert.AreEqual(simplePhase.Id, json.Value<long>("id"));
            Assert.AreEqual(simplePhase.Name, json.Value<string>("name"));
            Assert.AreEqual(simplePhase.Done, json.Value<bool>("done"));
        }

        [TestMethod]
        public void Phase_GetSingleWithFields_Success()
        {
            var dict = testConfig.GetDefaultActivityArguments();
            dict["PhaseID"] = customFieldsPhase.Id;

            var act = new GetPhase();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Phase"] as JObject;
            Assert.AreEqual(customFieldsPhase.Id, json.Value<long>("id"));
            Assert.AreEqual(customFieldsPhase.Name, json.Value<string>("name"));
            Assert.AreEqual(customFieldsPhase.Done, json.Value<bool>("done"));

            var expectedFields = new Dictionary<string, string>();
            expectedFields.Add("area", "number");
            expectedFields.Add("main_room", "short_text");
            expectedFields.Add("construction_started", "datetime");

            var fields = json["fields"] as JArray;
            foreach (var item in fields)
            {
                var id = item.Value<string>("id");
                var type = item.Value<string>("type");
                Assert.AreEqual(expectedFields[id], type);
                expectedFields.Remove(id);
            }
            Assert.AreEqual(0, expectedFields.Count);
        }
    }
}