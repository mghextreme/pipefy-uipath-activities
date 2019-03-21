using System;
using System.Activities;
using System.Collections.Generic;
using Capgemini.Pipefy.TableRecord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class TableRecordTest
    {
        [TestMethod]
        public void TableRecord_CreateByDictionary_Success()
        {
            var config = TestConfiguration.Instance.Configuration;

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableID"] = config["table"].Value<string>("id");
            dict["Title"] = "Title " + DateTime.Now.Ticks;
            dict["DueDate"] = DateTime.Now.AddDays(10).Date;

            var fields = new Dictionary<string, object>();
            var fieldsConfig = (JArray)config["table"]["custom_fields"];
            var titleField = config["table"].Value<string>("title_field");
            for (var i = 0; i < fieldsConfig.Count; i++)
            {
                var fieldName = fieldsConfig[i].Value<string>("name");
                var fieldType = fieldsConfig[i].Value<string>("type").ToLower();
                switch (fieldType)
                {
                    case "number":
                        fields.Add(fieldName, DateTime.Now.Millisecond);
                        break;
                    default:
                        fields.Add(fieldName, fieldName + " " + DateTime.Now.Ticks);
                        break;
                }
            }

            dict["DictionaryFields"] = fields;

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.IsTrue((long)result["TableRecordID"] > 0);
        }
    }
}