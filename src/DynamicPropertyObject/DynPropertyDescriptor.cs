using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

namespace DynamicPropertyObject
{
    public class DynPropertyDescriptor : PropertyDescriptor
    {
        private readonly Type m_compType;
        private readonly Type m_PropType;
        private readonly PropertyDescriptor m_pd;
        private readonly List<PropertyValueUIItem> m_colUIItem = new List<PropertyValueUIItem>();
        private static readonly char m_HiddenChar = '\t';
        private static ulong m_COUNT = 1;
        internal readonly ulong _ID;

        public DynPropertyDescriptor(Type componentType, string sName, Type propType, object value, params Attribute[] attributes) : base(sName, attributes)
        {
            _ID = m_COUNT++;
            m_compType = componentType;
            m_value = value;
            m_PropType = propType;
        }

        public DynPropertyDescriptor(PropertyDescriptor pd) : base(pd)
        {
            _ID = m_COUNT++;
            m_pd = pd;
        }

        public override Type ComponentType { get { return m_pd != null ? m_pd.ComponentType : m_compType; } }

        public override Type PropertyType { get { return m_pd != null ? m_pd.PropertyType : m_PropType; } }

        public override bool IsReadOnly
        {
            get
            {
                ReadOnlyAttribute attr = (ReadOnlyAttribute)Attributes.Get(typeof(ReadOnlyAttribute), true);
                return attr != null && attr.IsReadOnly;
            }
        }

        public override string Category
        {
            get
            {
                string sOut = base.Category;

                CategoryAttribute attr = (CategoryAttribute)Attributes.Get(typeof(CategoryAttribute), true);
                if (attr != null && attr.Category != null)
                {
                    sOut = attr.Category;
                }
                if (sOut == null) return null;
                sOut = sOut.PadLeft(sOut.Length + AppendCount, m_HiddenChar);
                return sOut;
            }
        }

        internal object DefaultValue
        {
            get
            {
                DefaultValueAttribute attr = (DefaultValueAttribute)Attributes.Get(typeof(DefaultValueAttribute), true);
                if (attr != null)
                {
                    return attr.Value;
                }
                return null;
            }
            set
            {
                Attributes.Add(new DefaultValueAttribute(value), true);
            }
        }

        internal int PropertyId
        {
            get
            {
                SortIDAttribute rsa = (SortIDAttribute)Attributes.Get(typeof(SortIDAttribute), true);
                if (rsa != null)
                {
                    return rsa.PropertyOrder;
                }
                return 0;
            }
            set
            {
                SortIDAttribute rsa = (SortIDAttribute)Attributes.Get(typeof(SortIDAttribute), true);
                if (rsa == null)
                {
                    rsa = new SortIDAttribute();
                    Attributes.Add(rsa);
                }
                rsa.PropertyOrder = value;
            }
        }

        internal int CategoryId
        {
            get
            {
                SortIDAttribute rsa = (SortIDAttribute)Attributes.Get(typeof(SortIDAttribute), true);

                if (rsa != null)
                {
                    return rsa.CategoryOrder;
                }
                return 0;
            }
            set
            {
                SortIDAttribute rsa = (SortIDAttribute)Attributes.Get(typeof(SortIDAttribute), true);
                if (rsa == null)
                {
                    rsa = new SortIDAttribute();
                    Attributes.Add(rsa);
                }
                rsa.CategoryOrder = value;
            }
        }

        internal string CategoryResourceKey
        {
            get
            {
                CategoryResourceKeyAttribute rsa = (CategoryResourceKeyAttribute)Attributes.Get(typeof(CategoryResourceKeyAttribute), true);
                if (rsa != null)
                {
                    return rsa.ResourceKey;
                }
                return String.Empty;
            }
            set
            {
                CategoryResourceKeyAttribute rsa = (CategoryResourceKeyAttribute)Attributes.Get(typeof(CategoryResourceKeyAttribute), true);
                if (rsa == null)
                {
                    rsa = new CategoryResourceKeyAttribute();
                    Attributes.Add(rsa);
                }
                rsa.ResourceKey = value;
            }
        }

        internal int AppendCount { get; set; }

        public override bool DesignTimeOnly { get { return false; } }

        private object m_value;

        public override object GetValue(object component)
        {
            Debug.Assert(component != null);
            Debug.Assert(component.GetType() == ComponentType);

            return m_pd != null ? m_pd.GetValue(component) : m_value;
        }

        public override void SetValue(object component, object value)
        {
            Debug.Assert(component != null);
            Debug.Assert(component.GetType() == ComponentType);

            Debug.Assert(value != null);
            Debug.Assert(value.GetType() == PropertyType);

            m_value = value;

            if (m_pd != null)
            {
                m_pd.SetValue(component, m_value);
            }
            OnValueChanged(component, new EventArgs());
        }

        /// <summary>
        /// Abstract base members
        /// </summary>
        public override void ResetValue(object component)
        {
            Debug.Assert(component != null);
            Debug.Assert(component.GetType() == ComponentType);

            if (m_pd != null)
            {
                m_pd.ResetValue(component);
            }
            else
            {
                SetValue(component, DefaultValue);
            }
        }

        public override bool CanResetValue(object component)
        {
            Debug.Assert(component != null);
            Debug.Assert(component.GetType() == ComponentType);

            if (DefaultValue == null) { return false; }

            var value = GetValue(component);
            return value != null && !value.Equals(DefaultValue);
        }

        public override bool ShouldSerializeValue(object component)
        {
            Debug.Assert(component != null);
            Debug.Assert(component.GetType() == ComponentType);

            return DefaultValue == null || CanResetValue(component);
        }

        public ICollection<PropertyValueUIItem> StateItems { get { return m_colUIItem; } }

        protected List<DynStandardValue> m_StandardValues = new List<DynStandardValue>();

        public virtual IList<DynStandardValue> StandardValues { get { return m_StandardValues; } }

        public Image ValueImage { get; set; } = null;

        internal int LCID { get; set; }
    }
}