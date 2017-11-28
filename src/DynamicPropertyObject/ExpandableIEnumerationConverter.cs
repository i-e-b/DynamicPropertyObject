using System;
using System.Collections;
using System.ComponentModel;

namespace DynamicPropertyObject
{
    public class ExpandableIEnumerationConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            if (context == null || context.PropertyDescriptor == null) return base.GetPropertiesSupported(context);
            return context.PropertyDescriptor.GetValue(context.Instance) is IEnumerable;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var pdc = new PropertyDescriptorCollection(null, false);
            var nIndex = -1;

            if (!(value is IEnumerable en)) return pdc;

            var enu = en.GetEnumerator();
            enu.Reset();
            while (enu.MoveNext())
            {
                nIndex++;
                if (enu.Current == null) continue;
                string sPropName = enu.Current.ToString();

                if (enu.Current is IComponent comp && comp.Site != null && !string.IsNullOrEmpty(comp.Site.Name))
                {
                    sPropName = comp.Site.Name;
                }
                else if (value.GetType().IsArray)
                {
                    sPropName = "[" + nIndex + "]";
                }
                pdc.Add(new DynPropertyDescriptor(value.GetType(), sPropName, enu.Current.GetType(), enu.Current, TypeDescriptor.GetAttributes(enu.Current).ToArray()));
            }


            return pdc;
        }
    }
}