using System;
using System.Collections.Generic;
using System.Data;

namespace Capgemini.Pipefy
{
    public static class PipefyExtensions
    {
        public static string ToPipefyFormat(this DateTime dateTime)
        {
            return dateTime.ToString("s");
        }
        
        public static string EscapeQueryValue(this string text)
        {
            text = text.Replace(@"""", @"\\""");
            return text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", @"\n");
        }

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
    }
}