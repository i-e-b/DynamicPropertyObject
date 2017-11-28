using System;
using System.ComponentModel;

namespace DynamicPropertyObject
{
    public static class DynamicPropertyObject
    {
        /// <summary>
        /// Create a new instance of a wrapper object type, ready to be used with various extension methods
        /// </summary>
        public static PropertyTarget NewObject()
        {
            return new PropertyTarget();
        }

        public static void AddProperty<T>(this PropertyTarget target, string key, string displayName, string description, T initialValue, T[] standardValues = null)
        {
            DynTypeDescriptor.InstallTypeDescriptor(target);
            var td = DynTypeDescriptor.GetTypeDescriptor(target);
            if (td == null) throw new Exception("Could not load type descriptor");

            
            var pd = new DynPropertyDescriptor(target.GetType(), key, typeof(T), initialValue
                ,new BrowsableAttribute(true)
                ,new DisplayNameAttribute(displayName)
                ,new DescriptionAttribute(description)
            );

            if (standardValues != null && standardValues.Length > 0) {
                pd.Attributes.Add(new TypeConverterAttribute(typeof(DynStandardValueConverter)), true);
                foreach (var value in standardValues)
                {
                    var sv = new DynStandardValue(value);
                    sv.DisplayName = value.ToString();
                    pd.StandardValues.Add(sv);
                }
            }

            td.GetProperties().Add(pd);
        }
    }
}