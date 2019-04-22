using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy
{
    /// <summary>
    /// Class for general extensions
    /// </summary>
    public static class PipefyExtensions
    {
        /// <summary>
        /// Converts a DataRow to a Dictionary, with each column beeing a key
        /// </summary>
        /// <param name="row">The DataRow to be converted</param>
        /// <returns>The Dictionary</returns>
        public static Dictionary<string, object> ToDictionary(this DataRow row)
        {
            var dict = new Dictionary<string, object>();
            if (row == null || row?.Table?.Columns.Count == 0)
                return dict;

            foreach (DataColumn column in row.Table.Columns)
            {
                string colName = column.ColumnName;
                dict.Add(colName, row[colName]);
            }

            return dict;
        }

        /// <summary>
        /// Ensures the object is correctly formatted for a JSON string, adding quotes, escape chars and brackets when necessary
        /// </summary>
        /// <param name="obj">The content to be serialized to JSON</param>
        /// <returns>The serialized object</returns>
        public static string ToQueryValue(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        internal static Dictionary<string, object> FieldsJArrayToDictionary(JArray fieldsArray)
        {
            if (fieldsArray == null)
                return null;

            var dict = new Dictionary<string, object>();

            foreach (var field in fieldsArray)
            {
                var id = field["field"].Value<string>("id");
                var type = field["field"].Value<string>("type");
                try
                {
                    switch (type.ToLower())
                    {
                        case "attachment":
                        case "checklist_horizontal":
                        case "checklist_vertical":
                        case "connector":
                            var items  = new List<string>();
                            var jArray = field["array_value"] as JArray;
                            foreach (var item in jArray)
                                items.Add(item.Value<string>());
                            dict.Add(id, items.ToArray());
                            break;
                        case "due_date":
                        case "datetime":
                            var dtValue = field.Value<string>("datetime_value");
                            bool success = DateTime.TryParse(dtValue, out DateTime value);
                            if (success)
                                dict.Add(id, value);
                            else
                                goto default;
                            break;
                        case "date":
                            var dateValue = field.Value<string>("value");
                            var value2 = DateTime.ParseExact(dateValue, new string[]{ "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None);
                            dict.Add(id, value2);
                            break;
                        default:
                            dict.Add(id, field.Value<string>("value"));
                            break;
                    }
                }
                catch (Exception)
                {
                    dict.Add(id, field.Value<string>("value"));
                }
            }

            return dict;
        }
    }
}