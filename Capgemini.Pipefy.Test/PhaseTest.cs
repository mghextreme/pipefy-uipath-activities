using System;
using System.Activities;
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
    }
}