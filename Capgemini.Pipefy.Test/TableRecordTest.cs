using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Capgemini.Pipefy.Table;
using Capgemini.Pipefy.TableRecord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class TableRecordTest
    {
        private static TestConfiguration testConfig;
        private static string simpleTableId, customFieldsTableId;

        [ClassInitialize]
        public static void TableRecordTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();
            var orgId = testConfig.GetCustomConfig("OrganizationID");

            var createTableQuery = "mutation {{ createTable(input: {{ organization_id: {0} name: {1} public: true }}){{ table {{ id name url }} }} }}";

            // Create simple table

            var createSimpleTable = string.Format(createTableQuery, orgId, "SimpleTable".ToQueryValue());
            var query = new PipefyQuery(createSimpleTable, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var table = resultObj["data"]["createTable"]["table"];
            simpleTableId = table.Value<string>("id");

            // Create table with custom fields

            var createCustomFieldsTable = string.Format(createTableQuery, orgId, "CustomFieldsTable".ToQueryValue());
            query = new PipefyQuery(createCustomFieldsTable, bearer);
            result = query.Execute();
            resultObj = PipefyQuery.ParseJson(result);

            table = resultObj["data"]["createTable"]["table"];
            customFieldsTableId = table.Value<string>("id");
        }

        [ClassCleanup]
        public static void TableRecordTestCleanup()
        {
            var bearer = testConfig.GetBearer();

            // Delete tables after tests

            var deleteTableQuery = "mutation {{ deleteTable(input: {{ id: {0} }}){{ success }} }}";

            if (!string.IsNullOrWhiteSpace(simpleTableId))
            {
                var deleteSimpleTableQuery = string.Format(deleteTableQuery, simpleTableId);
                var query = new PipefyQuery(deleteSimpleTableQuery, bearer);
                var result = query.Execute();
                var resultObj = PipefyQuery.ParseJson(result);
                var success = resultObj["data"]["deleteTable"].Value<bool>("success");
                Assert.IsTrue(success);
            }

            if (!string.IsNullOrWhiteSpace(customFieldsTableId))
            {
                var deleteSimpleTableQuery = string.Format(deleteTableQuery, customFieldsTableId);
                var query = new PipefyQuery(deleteSimpleTableQuery, bearer);
                var result = query.Execute();
                var resultObj = PipefyQuery.ParseJson(result);
                var success = resultObj["data"]["deleteTable"].Value<bool>("success");
                Assert.IsTrue(success);
            }
        }

        [Ignore]
        [TestMethod]
        public void TableRecord_CreateByDictionaryAndDelete_Success()
        {
            var config = TestConfiguration.Instance.Configuration;
            string title = "Title " + DateTime.Now.Ticks;

            // Create

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = config["table"].Value<string>("id");
            dict["Title"] = title;
            dict["DueDate"] = DateTime.Now.AddDays(10).Date;
            // dict["DictionaryFields"] = TestTable.Instance.GenerateRandomRecordDictionary();

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.IsTrue((long)result["TableRecordID"] > 0);

            // Delete

            dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableRecordID"] = result["TableRecordID"];

            var act2 = new DeleteTableRecord();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act2.SuccessMessage, result["Status"].ToString());
        }

        [Ignore]
        [TestMethod]
        public void TableRecord_CreateByDataRowAndDelete_Success()
        {
            var config = TestConfiguration.Instance.Configuration;
            string title = "Title " + DateTime.Now.Ticks;

            // Create

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = config["table"].Value<string>("id");
            dict["Title"] = title;
            dict["DueDate"] = DateTime.Now.AddDays(10).Date;
            // dict["DataRowFields"] = TestTable.Instance.GenerateRandomRecordDataRow();

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.IsTrue((long)result["TableRecordID"] > 0);

            // Delete

            dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableRecordID"] = result["TableRecordID"];

            var act2 = new DeleteTableRecord();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act2.SuccessMessage, result["Status"].ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TableRecord_AvoidDictionaryAndDataRow_Exception()
        {
            var config = TestConfiguration.Instance.Configuration;
            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = config["table"].Value<string>("id");
            // dict["DictionaryFields"] = TestTable.Instance.GenerateRandomRecordDictionary();
            // dict["DataRowFields"] = TestTable.Instance.GenerateRandomRecordDataRow();
            var act = new CreateTableRecord();
            WorkflowInvoker.Invoke(act, dict);
        }
    }

    internal class Table
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }

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

        public Table(string tableId)
        {
            Id = tableId;
        }

        private void LoadInfo()
        {
            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = Id;
            var act = new GetTable();
            var result = WorkflowInvoker.Invoke(act, dict);

            if (!(bool)result["Success"])
                throw new ArgumentException("Couldn't load table info for table " + dict["TableID"].ToString());
            
            _info = result["Table"] as JObject;
            Name = _info.Value<string>("name");
        }

        public Dictionary<string, object> GenerateRandomRecordDictionary()
        {
            var fields = new Dictionary<string, object>();
            var fieldsConfig = Info["table_fields"] as JArray;
            foreach (var item in fieldsConfig)
            {
                var fieldName = item.Value<string>("id");
                var fieldType = item.Value<string>("type").ToLower();
                fields.Add(fieldName, GetRandomValueByType(fieldType, fieldName));
            }
            return fields;
        }

        public DataRow GenerateRandomRecordDataRow()
        {
            var dataTable = new DataTable(Info.Value<string>("name"));
            var fields = Info["table_fields"] as JArray;
            
            foreach (var item in fields)
            {
                var fieldName = item.Value<string>("id");
                dataTable.Columns.Add(fieldName);
            }

            var row = dataTable.NewRow();
            foreach (var item in fields)
            {
                var fieldName = item.Value<string>("id");
                var fieldType = item.Value<string>("type").ToLower();
                row[fieldName] = GetRandomValueByType(fieldType, fieldName);
            }
            dataTable.Rows.Add(row);

            return row;
        }

        private object GetRandomValueByType(string type, string fieldName)
        {
            switch (type)
            {
                case "number":
                    return DateTime.Now.Millisecond;
                case "short_text":
                case "long_text":
                    return fieldName + " text " + DateTime.Now.Ticks;
                default:
                    return fieldName;
            }
        }
    }
}