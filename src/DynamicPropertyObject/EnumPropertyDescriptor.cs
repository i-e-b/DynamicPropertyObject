using System.Collections.Generic;
using System.Diagnostics;

namespace DynamicPropertyObject
{
    public class EnumPropertyDescriptor : DynPropertyDescriptor
    {
        public EnumPropertyDescriptor(System.ComponentModel.PropertyDescriptor pd) : base(pd)
        {
            Debug.Assert(pd.PropertyType.IsEnum);

            m_StandardValues.Clear();
            var svaArr = EnumUtil.GetStandardValues(PropertyType);
            m_StandardValues.AddRange(svaArr);
        }

        public override IList<DynStandardValue> StandardValues { get { return m_StandardValues.AsReadOnly(); } }
    }
}