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
        [TestMethod]
        public void Table_GetSingle_Success()
        {
            var config = TestConfiguration.Instance.Configuration;

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            string tableId = config["table"].Value<string>("id");
            dict["TableID"] = tableId;

            var act = new GetTable();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Table"] as JObject;
            Assert.AreEqual(tableId, json.Value<string>("id"));
            Assert.IsTrue(json["table_fields"].Children().Count() > 0);
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
            var config = TestConfiguration.Instance.Configuration;

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            string tableId = config["table"].Value<string>("id");
            dict["TableIDs"] = new string[]{ tableId };

            var act = new GetTables();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);

            var json = result["Tables"] as JObject[];
            Assert.AreEqual(1, json.Length);
            Assert.AreEqual(tableId, json[0].Value<string>("id"));
            Assert.IsTrue(json[0].Value<string>("name").Length > 0);
        }
    }
}