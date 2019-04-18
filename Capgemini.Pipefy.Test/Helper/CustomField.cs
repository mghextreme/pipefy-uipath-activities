using System;
using System.Linq;

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
        public Phase Phase { get; protected set; }

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

        internal void SetParentPhase(Phase phase)
        {
            Phase = phase;
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
                    return now.AddHours(now.Ticks % 24).ToString("HH:mm:ss");
                case "datetime":
                    return now.AddDays((now.Ticks % 90) - 45);
                case "due_date":
                    return now.AddDays(now.Ticks % 60);
                case "checklist_horizontal":
                case "checklist_vertical":
                    Random rnd = new Random();
                    if (field is OptionsCustomField options)
                    {
                        if (options.Options.Count > 0)
                            return options.Options.OrderBy(x => rnd.Next()).Take(rnd.Next(1, options.Options.Count + 1)).ToArray();
                        else
                            return new string[0];
                    }
                    break;
            }
            throw new NotImplementedException($"The field type {field.Type} is not supported.");
        }

        #endregion Static
    }
}