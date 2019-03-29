using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            TestConfiguration testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            // Create organization for tests

            try
            {
                var createOrgQuery = "mutation { createOrganization(input: { industry: \"others\" name: \"API Test\" }){ organization { created_at id name role } } }";
                var query = new PipefyQuery(createOrgQuery, bearer);
                var result = query.Execute();
                var resultObj = PipefyQuery.ParseJson(result);

                var org = resultObj["data"]["createOrganization"]["organization"];
                var orgId = org.Value<long>("id");
                var orgName = org.Value<string>("name");
                var orgRole = org.Value<string>("role");

                testConfig.SetCustomConfig("OrganizationCreated", true);
                testConfig.SetCustomConfig("OrganizationID", orgId);
            }
            catch (Exception)
            {
                testConfig.SetCustomConfig("OrganizationCreated", false);
                throw;
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            bool orgCreated = (bool)TestConfiguration.Instance.GetCustomConfig("OrganizationCreated");
            if (!orgCreated)
                return;

            TestConfiguration testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            // Delete organization after tests

            long orgId = (long)testConfig.GetCustomConfig("OrganizationID");
            var deleteOrgQuery = string.Format("mutation {{ deleteOrganization(input: {{ id: {0} }}){{ success }} }}", orgId);
            var query = new PipefyQuery(deleteOrgQuery, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var success = resultObj["data"]["deleteOrganization"].Value<bool>("success");
            Assert.IsTrue(success);
        }
    }
}