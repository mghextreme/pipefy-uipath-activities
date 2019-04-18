using System.Collections.Generic;

namespace Capgemini.Pipefy.Test.Helper
{
    internal class OptionsCustomField : CustomField
    {
        public ICollection<string> Options { get; protected set; }

        internal OptionsCustomField()
        {
            Options = new List<string>();
        }

        internal OptionsCustomField(string label, string type) : this(label, type, false, new string[0]) { }

        internal OptionsCustomField(string label, string type, string[] options) : this(label, type, false, new string[0]) { }

        internal OptionsCustomField(string label, string type, bool required) : this(label, type, required, new string[0]) { }

        internal OptionsCustomField(string label, string type, bool required, string[] options)
        {
            Label = label;
            Type = type;
            IsRequired = required;
            Options = new List<string>(options);
        }
    }
}