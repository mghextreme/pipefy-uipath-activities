using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test.Helper
{
    internal class TableRecord
    {
        public static Dictionary<string, JObject> FieldsJArrayToJObjectDictionary(JArray fields)
        {
            var dict = new Dictionary<string, JObject>();

            if (fields.Count == 0)
                return dict;

            foreach (var item in fields)
            {
                var id = item["field"].Value<string>("id");
                dict.Add(id, item as JObject);
            }
            return dict;
        }
    }
}