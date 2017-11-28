namespace DynamicPropertyObject
{
    public class DynStandardValue
    {
        public DynStandardValue(object value)
        {
            Value = value;
            Enabled = true;
            Visible = true;
        }

        public DynStandardValue(object value, string displayName)
        {
            DisplayName = displayName;
            Value = value;
            Enabled = true;
            Visible = true;
        }

        public DynStandardValue(object value, string displayName, string description)
        {
            Value = value;
            DisplayName = displayName;
            Description = description;
            Enabled = true;
            Visible = true;
        }

        public string DisplayName { get; set; }

        public bool Visible { get; set; }

        public bool Enabled { get; set; }

        public string Description { get; set; }

        public object Value { get; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(DisplayName) && (Value != null))
            {
                return Value.ToString();
            }
            return DisplayName;
        }
    }
}