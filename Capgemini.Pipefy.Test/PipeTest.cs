using System;
using System.Activities;
using System.Linq;
using Capgemini.Pipefy.Pipe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class PipeTest
    {
        private static TestConfiguration testConfig;
        private static Helper.Pipe pipe1;

        [ClassInitialize]
        public static void PipeTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            pipe1 = Helper.Pipe.CreatePipe("Pipe1");
        }

        [ClassCleanup]
        public static void PipeTestCleanup()
        {
            pipe1.Delete();
        }

        [TestMethod]
        public void Pipe_GetSingle_Success()
        {
            var dict = testConfig.GetDefaultActivityArguments();
            dict["PipeID"] = pipe1.Id;

            var act = new GetPipe();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Pipe"] as JObject;
            Assert.AreEqual(pipe1.Id, json.Value<long>("id"));
        }
    }
}