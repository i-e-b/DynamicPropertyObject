using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace DynamicPropertyObject
{
    public class BooleanConverter : System.ComponentModel.BooleanConverter
    {
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(DynStandardValue) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string)) return base.ConvertFrom(context, culture, value);
            var sInpuValue = (string)value;
            sInpuValue = sInpuValue.Trim();
            foreach (var sv in GetAllPossibleValues(context))
            {
                UpdateStringFromResource(context, sv);

                if (string.Compare(sv.Value.ToString(), sInpuValue, StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(sv.DisplayName, sInpuValue, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return sv.Value;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is string)
            {
                if (destinationType == typeof(string))
                {
                    return value;
                }
                if (destinationType != typeof(DynStandardValue)) return base.ConvertTo(context, culture, value, destinationType);
                foreach (var sv in GetAllPossibleValues(context))
                {
                    UpdateStringFromResource(context, sv);

                    if (string.Compare(value.ToString(), sv.DisplayName, true, culture) == 0 ||
                        string.Compare(value.ToString(), sv.Value.ToString(), true, culture) == 0)
                    {
                        return sv;
                    }
                }
            }
            else if (value is bool)
            {
                if (destinationType == typeof(string))
                {
                    foreach (var sv in GetAllPossibleValues(context))
                    {
                        if (!sv.Value.Equals(value)) continue;
                        UpdateStringFromResource(context, sv);

                        return sv.DisplayName;
                    }
                }
                else if (destinationType == typeof(DynStandardValue))
                {
                    foreach (var sv in GetAllPossibleValues(context))
                    {
                        if (!sv.Value.Equals(value)) continue;
                        UpdateStringFromResource(context, sv);

                        return sv;
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
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

        private DynStandardValue[] GetAllPossibleValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            var list = new List<DynStandardValue>();
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor is DynPropertyDescriptor)
            {
                var pd = (DynPropertyDescriptor) context.PropertyDescriptor;
                list.AddRange(pd.StandardValues);
            }
            else
            {
                list.Add(new DynStandardValue(true));
                list.Add(new DynStandardValue(false));
            }
            return list.ToArray();
        }

        private void UpdateStringFromResource(System.ComponentModel.ITypeDescriptorContext context, DynStandardValue sv)
        {
            ResourceAttribute ra = null;

            if (context != null && context.PropertyDescriptor != null)
            {
                ra = (ResourceAttribute)context.PropertyDescriptor.Attributes.Get(typeof(ResourceAttribute));
            }
            if (ra == null)
            {
                ra = (ResourceAttribute)System.ComponentModel.TypeDescriptor.GetAttributes(typeof(bool)).Get(typeof(ResourceAttribute));
            }

            if (ra == null)
            {
                return;
            }

            ResourceManager rm = null;

            // construct the resource manager using the resInfo
            try
            {
                if (String.IsNullOrEmpty(ra.BaseName) == false && String.IsNullOrEmpty(ra.AssemblyFullName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, Assembly.ReflectionOnlyLoad(ra.AssemblyFullName));
                }
                else if (String.IsNullOrEmpty(ra.BaseName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, typeof(bool).Assembly);
                }
                else if (String.IsNullOrEmpty(ra.BaseName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, typeof(bool).Assembly);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            if (rm == null)
            {
                return;
            }

            // update the display and description string from resource using the resource manager

            string keyName = ra.KeyPrefix + sv.Value + "_Name";  // display name
            string keyDesc = ra.KeyPrefix + sv.Value + "_Desc"; // description
            string dispName = string.Empty;
            string description = string.Empty;
            try
            {
                dispName = rm.GetString(keyName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (string.IsNullOrEmpty(dispName) == false)
            {
                sv.DisplayName = dispName;
            }

            try
            {
                description = rm.GetString(keyDesc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (string.IsNullOrEmpty(description) == false)
            {
                sv.Description = description;
            }
        }
    }
}