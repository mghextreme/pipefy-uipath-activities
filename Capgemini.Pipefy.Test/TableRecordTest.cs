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
        private static Helper.Table simpleTable, customFieldsTable;
        private static Dictionary<string, string> customFields;

        [ClassInitialize]
        public static void TableRecordTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            simpleTable = Helper.Table.CreateTable("SimpleTable");
            customFieldsTable = Helper.Table.CreateTable("CustomFieldsTable");
        }

        [ClassCleanup]
        public static void TableRecordTestCleanup()
        {
            simpleTable.Delete();
            customFieldsTable.Delete();
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
}