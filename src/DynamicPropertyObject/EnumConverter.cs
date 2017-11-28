using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace DynamicPropertyObject
{
    public class EnumConverter : System.ComponentModel.EnumConverter
    {
        public EnumConverter(Type type)
            : base(type)
        {
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DynStandardValue))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return base.ConvertFrom(context, culture, null);
            }
            if (value is string sInpuValue)
            {
                var arrDispName = sInpuValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var sb = new StringBuilder(1000);
                foreach (string sDispName in arrDispName)
                {
                    string sTrimValue = sDispName.Trim();
                    foreach (var sv in GetAllPossibleValues(context))
                    {
                        UpdateStringFromResource(context, sv);

                        if (string.Compare(sv.Value.ToString(), sTrimValue, StringComparison.OrdinalIgnoreCase) == 0 ||
                            string.Compare(sv.DisplayName, sTrimValue, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(",");
                            }
                            sb.Append(sv.Value);
                        }
                    }
                }
                return Enum.Parse(EnumType, sb.ToString(), true);
            }
            if (value is DynStandardValue standardValue)
            {
                return standardValue.Value;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
            {
                return base.ConvertTo(context, culture, null, destinationType);
            }
            if (value is string)
            {
                if (destinationType == typeof(string))
                {
                    return value;
                }
                if (destinationType == EnumType)
                {
                    return ConvertFrom(context, culture, value);
                }
                if (destinationType == typeof(DynStandardValue))
                {
                    foreach (DynStandardValue sv in GetAllPossibleValues(context))
                    {
                        UpdateStringFromResource(context, sv);

                        if (String.Compare(value.ToString(), sv.DisplayName, true, culture) == 0 ||
                            String.Compare(value.ToString(), sv.Value.ToString(), true, culture) == 0)
                        {
                            return sv;
                        }
                    }
                }
            }
            else if (value.GetType() == EnumType)
            {
                if (destinationType == typeof(string))
                {
                    string sDelimitedValues = Enum.Format(EnumType, value, "G");
                    string[] arrValue = sDelimitedValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var sb = new StringBuilder(1000);
                    foreach (string sDispName in arrValue)
                    {
                        string sTrimValue = sDispName.Trim();
                        foreach (var sv in GetAllPossibleValues(context))
                        {
                            UpdateStringFromResource(context, sv);

                            if (string.Compare(sv.Value.ToString(), sTrimValue, StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(sv.DisplayName, sTrimValue, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if (sb.Length > 0)
                                {
                                    sb.Append(", ");
                                }
                                sb.Append(sv.DisplayName);
                            }
                        }
                    }
                    return sb.ToString();
                }
                if (destinationType == typeof(DynStandardValue))
                {
                    foreach (DynStandardValue sv in GetAllPossibleValues(context))
                    {
                        if (sv.Value.Equals(value))
                        {
                            UpdateStringFromResource(context, sv);
                            return sv;
                        }
                    }
                }
                else if (destinationType == EnumType)
                {
                    return value;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            var list = GetAllPossibleValues(context).Select(sv => sv.Value).ToList();
            var svc = new StandardValuesCollection(list);
            return svc;
        }

        private DynStandardValue[] GetAllPossibleValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            var list = new List<DynStandardValue>();
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor is DynPropertyDescriptor)
            {
                var pd = (DynPropertyDescriptor)context.PropertyDescriptor;
                list.AddRange(pd.StandardValues);
            }
            else
            {
                list.AddRange(EnumUtil.GetStandardValues(EnumType));
            }
            return list.ToArray();
        }

        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor is DynPropertyDescriptor)
            {
                return ((DynPropertyDescriptor)context.PropertyDescriptor).StandardValues.Count > 0;
            }
            return base.GetStandardValuesSupported(context);
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

        public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            ExpandEnumAttribute eea;

            if (context != null && context.PropertyDescriptor is DynPropertyDescriptor)
            {
                var pd = (DynPropertyDescriptor)context.PropertyDescriptor;
                eea = (ExpandEnumAttribute)pd.Attributes.Get(typeof(ExpandEnumAttribute), true);
            }
            else
            {
                eea = (ExpandEnumAttribute)System.ComponentModel.TypeDescriptor.GetAttributes(EnumType).Get(typeof(ExpandableIEnumerationConverter), true);
            }

            return eea != null && eea.Exapand;
        }

        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (context.PropertyDescriptor == null) { return null; }

            System.ComponentModel.DefaultValueAttribute dva = context.PropertyDescriptor.Attributes.Get(typeof(System.ComponentModel.DefaultValueAttribute)) as System.ComponentModel.DefaultValueAttribute;

            System.ComponentModel.PropertyDescriptorCollection pdc = new System.ComponentModel.PropertyDescriptorCollection(null, false);
            foreach (DynStandardValue sv in GetAllPossibleValues(context))
            {
                if (!sv.Visible) continue;

                UpdateStringFromResource(context, sv);
                var epd = new EnumChildPropertyDescriptor(context, sv.Value.ToString(), sv.Value);
                epd.Attributes.Add(new System.ComponentModel.ReadOnlyAttribute(!sv.Enabled), true);
                epd.Attributes.Add(new System.ComponentModel.DescriptionAttribute(sv.Description), true);
                epd.Attributes.Add(new System.ComponentModel.DisplayNameAttribute(sv.DisplayName), true);
                epd.Attributes.Add(new System.ComponentModel.BrowsableAttribute(sv.Visible), true);

                // setup the default value;
                if (dva != null)
                {
                    bool bHasBit = EnumUtil.IsBitsOn(dva.Value, sv.Value);
                    epd.DefaultValue = bHasBit;
                }
                pdc.Add(epd);
            }
            return pdc;
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
                ra = (ResourceAttribute)System.ComponentModel.TypeDescriptor.GetAttributes(EnumType).Get(typeof(ResourceAttribute));
            }

            if (ra == null)
            {
                return;
            }

            ResourceManager rm;

            // construct the resource manager using the resInfo
            try
            {
                if (String.IsNullOrEmpty(ra.BaseName) == false && String.IsNullOrEmpty(ra.AssemblyFullName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, Assembly.ReflectionOnlyLoad(ra.AssemblyFullName));
                }
                else if (String.IsNullOrEmpty(ra.BaseName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, EnumType.Assembly);
                }
                else if (String.IsNullOrEmpty(ra.BaseName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, EnumType.Assembly);
                }
                else
                {
                    rm = new ResourceManager(EnumType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // update the display and description string from resource using the resource manager

            string keyName = ra.KeyPrefix + sv.Value + "_Name";  // display name
            string keyDesc = ra.KeyPrefix + sv.Value + "_Desc"; // description
            string dispName = String.Empty;
            string description = String.Empty;
            try
            {
                dispName = rm.GetString(keyName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (String.IsNullOrEmpty(dispName) == false)
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
            if (String.IsNullOrEmpty(description) == false)
            {
                sv.Description = description;
            }
        }
    }
}