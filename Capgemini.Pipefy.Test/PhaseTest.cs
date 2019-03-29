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
        [Ignore]
        [TestMethod]
        public void Phase_GetSingle_Success()
        {
            var phases = TestPipe.Instance.Info["phases"] as JArray;
            Assert.IsTrue(phases.Count > 0);

            long phaseId = phases.First.Value<long>("id");
            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["PhaseID"] = phaseId;

            var act = new GetPhase();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Phase"] as JObject;
            Assert.AreEqual(phaseId, json.Value<long>("id"));
            Assert.IsTrue(json["cards_can_be_moved_to_phases"].Children().Count() > 0);
        }
    }

    internal class TestPipe
    {
        public long PipeId { get; protected set; }

        private static TestPipe _instance;
        public static TestPipe Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TestPipe();
                return _instance;
            }
        }

        private JObject _info;
        public JObject Info
        {
            get
            {
                if (_info == null)
                    LoadInfo();
                return _info;
            }
        }

        public TestPipe()
        {
            var config = TestConfiguration.Instance.Configuration;
            PipeId = config["pipe"].Value<long>("id");
        }

        public TestPipe(long pipeId) : this()
        {
            PipeId = pipeId;
        }

        private void LoadInfo()
        {
            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["PipeID"] = PipeId;
            var act = new GetPipe();
            var result = WorkflowInvoker.Invoke(act, dict);

            if (!(bool)result["Success"])
                throw new ArgumentException("Couldn't load table info for pipe " + dict["PipeID"].ToString());
            
            _info = result["Pipe"] as JObject;
        }
    }
}