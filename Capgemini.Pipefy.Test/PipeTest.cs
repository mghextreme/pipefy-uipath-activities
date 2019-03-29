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
        [Ignore]
        [TestMethod]
        public void Pipe_GetSingle_Success()
        {
            var config = TestConfiguration.Instance.Configuration;

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            long pipeId = config["pipe"].Value<long>("id");
            dict["PipeID"] = pipeId;

            var act = new GetPipe();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Pipe"] as JObject;
            Assert.AreEqual(pipeId, json.Value<long>("id"));
            Assert.IsTrue(json["phases"].Children().Count() > 0);
        }
    }
}