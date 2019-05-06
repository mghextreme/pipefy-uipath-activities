using System.Activities;
using Capgemini.Pipefy.Organization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class OrganizationTest
    {
        private static TestConfiguration testConfig;

        [ClassInitialize]
        public static void PipeTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
        }

        [TestMethod]
        public void Organization_GetMultipleFromEmpty_Success()
        {
            var dict = testConfig.GetDefaultActivityArguments();
            PipefyQueryActivity act = new GetOrganizations();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var orgs = result["Organizations"] as JObject[];
            Assert.IsTrue(orgs.Length > 0);
        }

        [TestMethod]
        public void Organization_GetMultiple_Success()
        {
            var dict = testConfig.GetDefaultActivityArguments();
            dict["OrganizationIDs"] = new long[]{ (long)testConfig.GetCustomConfig("OrganizationID") };

            PipefyQueryActivity act = new GetOrganizations();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var orgs = result["Organizations"] as JObject[];
            Assert.AreEqual(1, orgs.Length);
        }

        [TestMethod]
        public void Organization_GetSingle_Success()
        {
            long orgId = (long)testConfig.GetCustomConfig("OrganizationID");
            var dict = testConfig.GetDefaultActivityArguments();
            dict["OrganizationID"] = orgId;

            PipefyQueryActivity act = new GetOrganization();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var org = result["Organization"] as JObject;
            Assert.IsNotNull(org);
            Assert.AreEqual(orgId.ToString(), org.Value<string>("id"));
        }
    }
}