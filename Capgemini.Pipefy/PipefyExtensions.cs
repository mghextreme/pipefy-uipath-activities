using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

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
    }
}