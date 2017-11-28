using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace DynamicPropertyObject
{
    public class DynStandardValueConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context != null &&
                context.PropertyDescriptor != null &&
                context.PropertyDescriptor is DynPropertyDescriptor &&
                sourceType == typeof(string))
            {
                return true;
            }

            bool bOk = base.CanConvertFrom(context, sourceType);
            return bOk;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (context != null &&
                context.PropertyDescriptor != null &&
                context.PropertyDescriptor is DynPropertyDescriptor &&
                (destinationType == typeof(string) || destinationType == typeof(DynStandardValue)))
            {
                return true;
            }

            bool bOk = base.CanConvertTo(context, destinationType);
            return bOk;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object retObj;
            if (context.PropertyDescriptor == null || !(context.PropertyDescriptor is DynPropertyDescriptor) || value == null)
            {
                retObj = base.ConvertFrom(context, culture, value);
                return retObj;
            }

            var pd = (DynPropertyDescriptor) context.PropertyDescriptor;

            if (value is string)
            {
                foreach (var sv in pd.StandardValues)
                {
                    if (string.Compare(value.ToString(), sv.DisplayName, true, culture) == 0 ||
                        string.Compare(value.ToString(), sv.Value.ToString(), true, culture) == 0)
                    {
                        return sv.Value;
                    }
                }
            }
            else if (value is DynStandardValue)
            {
                return ((DynStandardValue) value).Value;
            }

            // try the native converter of the value.
            TypeConverter tc = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
            Debug.Assert(tc != null);
            retObj = tc.ConvertFrom(context, culture, value);
            return retObj;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (context == null || context.PropertyDescriptor == null || !(context.PropertyDescriptor is DynPropertyDescriptor) || value == null)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            var pd = (DynPropertyDescriptor)context.PropertyDescriptor;

            if (value is string)
            {
                if (destinationType == typeof(string))
                {
                    return value;
                }
                if (destinationType == pd.PropertyType)
                {
                    return ConvertFrom(context, culture, value);
                }
                if (destinationType == typeof(DynStandardValue))
                {
                    foreach (DynStandardValue sv in pd.StandardValues)
                    {
                        if (String.Compare(value.ToString(), sv.DisplayName, true, culture) == 0 ||
                            String.Compare(value.ToString(), sv.Value.ToString(), true, culture) == 0)
                        {
                            return sv;
                        }
                    }
                }
            }
            else if (value.GetType() == pd.PropertyType)
            {
                if (destinationType == typeof(string))
                {
                    foreach (DynStandardValue sv in pd.StandardValues)
                    {
                        if (sv.Value.Equals(value))
                        {
                            return sv.DisplayName;
                        }
                    }
                }
                else if (destinationType == typeof(DynStandardValue))
                {
                    foreach (DynStandardValue sv in pd.StandardValues)
                    {
                        if (sv.Value.Equals(value))
                        {
                            return sv;
                        }
                    }
                }
            }

            // try the native converter of the value.
            TypeConverter tc = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
            Debug.Assert(tc != null);
            return tc.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (context.PropertyDescriptor != null && context.PropertyDescriptor is DynPropertyDescriptor)
            {
                return ((DynPropertyDescriptor)context.PropertyDescriptor).StandardValues.Count > 0;
            }
            return base.GetStandardValuesSupported(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null)
            {
                ExclusiveStandardValuesAttribute psfa = (ExclusiveStandardValuesAttribute)context.PropertyDescriptor.Attributes.Get(typeof(ExclusiveStandardValuesAttribute), true);
                if (psfa != null)
                {
                    return psfa.Exclusive;
                }
            }
            return base.GetStandardValuesExclusive(context);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.PropertyDescriptor == null || !(context.PropertyDescriptor is DynPropertyDescriptor))
            {
                return base.GetStandardValues(context);
            }
            var pd = (DynPropertyDescriptor)context.PropertyDescriptor;
            var list = new List<object>();
            foreach (DynStandardValue sv in pd.StandardValues)
            {
                list.Add(sv.Value);
            }
            var svc = new StandardValuesCollection(list);

            return svc;
        }
    }
}