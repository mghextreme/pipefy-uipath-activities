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
        private static Helper.Table table1, table2;

        [ClassInitialize]
        public static void TableTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            table1 = Helper.Table.CreateTable("Table1");
            table2 = Helper.Table.CreateTable("Table2");
        }

        [ClassCleanup]
        public static void TableTestCleanup()
        {
            table1.Delete();
            table2.Delete();
        }

        [TestMethod]
        public void Table_GetSingle_Success()
        {
            var dict = testConfig.GetDefaultActivityArguments();
            dict["TableID"] = table1.Id;

            var act = new GetTable();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Table"] as JObject;
            Assert.AreEqual(table1.Id, json.Value<string>("id"));
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
            dict["TableIDs"] = new string[]{ table1.Id, table2.Id };

            var act = new GetTables();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Tables"] as JObject[];
            Assert.AreEqual(2, json.Length);
            Assert.AreEqual(table1.Id, json[0].Value<string>("id"));
            Assert.AreEqual(table1.Name, json[0].Value<string>("name"));
            Assert.AreEqual(table2.Id, json[1].Value<string>("id"));
            Assert.AreEqual(table2.Name, json[1].Value<string>("name"));
        }
    }
}