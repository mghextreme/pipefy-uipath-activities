using System.Activities;
using System.Linq;
using Capgemini.Pipefy.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class TableTest
    {
        private static TestConfiguration testConfig;
        private static string table1Id, table2Id;

        [ClassInitialize]
        public static void TableTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();
            var orgId = testConfig.GetCustomConfig("OrganizationID");

            var createTableQuery = "mutation {{ createTable(input: {{ organization_id: {0} name: {1} public: true }}){{ table {{ id name url }} }} }}";

            // Create simple table

            var createTable1 = string.Format(createTableQuery, orgId, "Table1".ToQueryValue());
            var query = new PipefyQuery(createTable1, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var table = resultObj["data"]["createTable"]["table"];
            table1Id = table.Value<string>("id");

            // Create table with custom fields

            var createTable2 = string.Format(createTableQuery, orgId, "Table2".ToQueryValue());
            query = new PipefyQuery(createTable2, bearer);
            result = query.Execute();
            resultObj = PipefyQuery.ParseJson(result);

            table = resultObj["data"]["createTable"]["table"];
            table2Id = table.Value<string>("id");
        }

        [ClassCleanup]
        public static void TableTestCleanup()
        {
            var bearer = testConfig.GetBearer();

            // Delete tables after tests

            var deleteTableQuery = "mutation {{ deleteTable(input: {{ id: {0} }}){{ success }} }}";

            if (!string.IsNullOrWhiteSpace(table1Id))
            {
                var deleteTable1Query = string.Format(deleteTableQuery, table1Id);
                var query = new PipefyQuery(deleteTable1Query, bearer);
                var result = query.Execute();
                var resultObj = PipefyQuery.ParseJson(result);
                var success = resultObj["data"]["deleteTable"].Value<bool>("success");
                Assert.IsTrue(success);
            }

            if (!string.IsNullOrWhiteSpace(table2Id))
            {
                var deleteTable2Query = string.Format(deleteTableQuery, table2Id);
                var query = new PipefyQuery(deleteTable2Query, bearer);
                var result = query.Execute();
                var resultObj = PipefyQuery.ParseJson(result);
                var success = resultObj["data"]["deleteTable"].Value<bool>("success");
                Assert.IsTrue(success);
            }
        }

        [TestMethod]
        public void Table_GetSingle_Success()
        {
            var dict = testConfig.GetDefaultActivityArguments();
            dict["TableID"] = table1Id;

            var act = new GetTable();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Table"] as JObject;
            Assert.AreEqual(table1Id, json.Value<string>("id"));
        }

        [TestMethod]
        public void Table_GetSingle_NotFound()
        {
            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = "00000000";

            var act = new GetTable();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Table"] as JObject;
            Assert.IsNull(json);
        }

        [TestMethod]
        public void Table_GetMultiple_Success()
        {
            var config = testConfig.Configuration;

            var dict = testConfig.GetDefaultActivityArguments();
            dict["TableIDs"] = new string[]{ table1Id, table2Id };

            var act = new GetTables();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Tables"] as JObject[];
            Assert.AreEqual(2, json.Length);
            Assert.AreEqual(table1Id, json[0].Value<string>("id"));
            Assert.IsTrue(json[0].Value<string>("name").Length > 0);
            Assert.AreEqual(table2Id, json[1].Value<string>("id"));
            Assert.IsTrue(json[1].Value<string>("name").Length > 0);
        }
    }
}