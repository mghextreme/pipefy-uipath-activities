using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using Capgemini.Pipefy.Table;
using Capgemini.Pipefy.TableRecord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class TableRecordTest
    {
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
            dict["DictionaryFields"] = TestTable.Instance.GenerateRandomRecordDictionary();

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
        public void TableRecord_CreateByDataRowAndDelete_Success()
        {
            var config = TestConfiguration.Instance.Configuration;
            string title = "Title " + DateTime.Now.Ticks;

            // Create

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = config["table"].Value<string>("id");
            dict["Title"] = title;
            dict["DueDate"] = DateTime.Now.AddDays(10).Date;
            dict["DataRowFields"] = TestTable.Instance.GenerateRandomRecordDataRow();

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
    }

    internal class TestTable
    {
        public string TableId { get; protected set; }

        private static TestTable _instance;
        public static TestTable Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TestTable();
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

        public TestTable()
        {
            var config = TestConfiguration.Instance.Configuration;
            TableId = config["table"].Value<string>("id");
        }

        public TestTable(string tableId) : this()
        {
            TableId = tableId;
        }

        private void LoadInfo()
        {
            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = TableId;
            var act = new GetTable();
            var result = WorkflowInvoker.Invoke(act, dict);

            if (!(bool)result["Success"])
                throw new ArgumentException("Couldn't load table info for table " + dict["TableID"].ToString());
            
            _info = result["Table"] as JObject;
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