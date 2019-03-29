using System;

namespace Capgemini.Pipefy.Test.Helper
{
    internal class CustomField
    {
        public string Id { get; set; }
        public string Label { get; protected set; }
        public string Type { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool IsMultiple { get; set; }
        public Table Table { get; protected set; }

        internal CustomField() { }

        internal CustomField(string label, string type) : this(label, type, false) { }

        internal CustomField(string label, string type, bool required)
        {
            Label = label;
            Type = type;
            IsRequired = required;
        }

        internal void SetParentTable(Table table)
        {
            Table = table;
        }

        internal object GetRandomValue()
        {
            return GetRandomValueForField(this);
        }

        #region Static

        private static object GetRandomValueForField(CustomField field)
        {
            var now = DateTime.Now;
            switch (field.Type)
            {
                case "short_text":
                    return string.Format("String - {0} - {1}", field.Label, now.ToShortTimeString());
                case "long_text":
                    return string.Format("Text - {0} - {1} - {2}", field.Label, field.Type, now.ToString());
                case "id":
                case "number":
                    return now.Ticks % 10000;
                case "date":
                    return now.AddDays(-1 * (now.Ticks % 90)).Date;
                case "time":
                    return now.AddHours(now.Ticks % 24).ToString("hh:mm:ss");
                case "datetime":
                    return now.AddDays((now.Ticks % 90) - 45);
                case "due_date":
                    return now.AddDays(now.Ticks % 60);
            }
            throw new NotImplementedException($"The field type {field.Type} is not supported.");
        }

        #endregion Static
    }
}