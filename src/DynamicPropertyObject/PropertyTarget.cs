namespace DynamicPropertyObject
{
    /// <summary>
    /// Placeholder interface for the proxy object which can store and return property values
    /// </summary>
    public class PropertyTarget
    {
        /// <summary>
        /// Retrieve a value by an added property's `key` name.
        /// </summary>
        public object this[string key]
        {
            get
            {
                var td = DynTypeDescriptor.GetTypeDescriptor(this);
                return td.GetProperties().Find(key,true)?.GetValue(this);
            }
        }
    }
}