using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DynamicPropertyObject
{
    public class BooleanPropertyDescriptor : DynPropertyDescriptor
    {
        public BooleanPropertyDescriptor(System.ComponentModel.PropertyDescriptor pd) : base(pd)
        {
            Debug.Assert(pd.PropertyType == typeof(bool));

            m_StandardValues.Clear();
            m_StandardValues.Add(new DynStandardValue(true));
            m_StandardValues.Add(new DynStandardValue(false));
        }

        public BooleanPropertyDescriptor(Type componentType, string sName, bool value, params Attribute[] attributes)
            : base(componentType, sName, typeof(bool), value, attributes)
        {
            m_StandardValues.Clear();
            m_StandardValues.Add(new DynStandardValue(true));
            m_StandardValues.Add(new DynStandardValue(false));
        }

        public override IList<DynStandardValue> StandardValues
        {
            get
            {
                return m_StandardValues.AsReadOnly();
            }
        }
    }
}