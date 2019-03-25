using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace Capgemini.Pipefy
{
    public static class PipefyExtensions
    {
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

        public static string ToQueryValue(this object obj)
        {
            if (obj is DateTime dateTime)
                obj = dateTime.ToString("s");

            return JsonConvert.SerializeObject(obj);
        }
    }
}