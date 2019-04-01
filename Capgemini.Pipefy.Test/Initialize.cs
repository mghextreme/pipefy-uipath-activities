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

            try
            {
                var org = Helper.Organization.CreateOrganization("API Test");

                testConfig.SetCustomConfig("OrganizationCreated", true);
                testConfig.SetCustomConfig("Organization", org);
                testConfig.SetCustomConfig("OrganizationID", org.Id);
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
            TestConfiguration testConfig = TestConfiguration.Instance;
            bool orgCreated = (bool)testConfig.GetCustomConfig("OrganizationCreated");
            if (!orgCreated)
                return;

            var org = (Helper.Organization)testConfig.GetCustomConfig("Organization");
            org.Delete();
        }
    }
}