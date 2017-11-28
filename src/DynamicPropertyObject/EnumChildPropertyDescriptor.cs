using System;
using System.Diagnostics;
using System.Reflection;

namespace DynamicPropertyObject
{
    public class EnumChildPropertyDescriptor : BooleanPropertyDescriptor
    {
        private readonly System.ComponentModel.ITypeDescriptorContext m_context;
        private readonly object m_enumField;  // represent one of the enum field

        public EnumChildPropertyDescriptor(System.ComponentModel.ITypeDescriptorContext context, string sName, object enumFieldvalue, params Attribute[] attributes) : base(enumFieldvalue.GetType(), sName, false, attributes)
        {
            m_context = context;
            m_enumField = enumFieldvalue;
        }

        public override void SetValue(object component, object value)
        {
            Debug.Assert(component != null);
            Debug.Assert(component.GetType() == ComponentType);

            Debug.Assert(value != null);
            Debug.Assert(value.GetType() == PropertyType);

            if (m_context.PropertyDescriptor == null) return;

            object enumInstance = m_context.PropertyDescriptor.GetValue(m_context.Instance);
            bool bModified;
            if ((bool)value)
            {
                bModified = EnumUtil.TurnOnBits(ref enumInstance, m_enumField);
            }
            else
            {
                bModified = EnumUtil.TurnOffBits(ref enumInstance, m_enumField);
            }

            if (!bModified) return;

            var fi = component.GetType().GetField("value__", BindingFlags.Instance | BindingFlags.Public);
            if (fi != null) fi.SetValue(component, enumInstance);
            m_context.PropertyDescriptor.SetValue(m_context.Instance, component);
        }

        public override object GetValue(object component)
        {
            Debug.Assert(component != null);
            Debug.Assert(component.GetType() == ComponentType);

            return EnumUtil.IsBitsOn(component, m_enumField);
        }
    }
}