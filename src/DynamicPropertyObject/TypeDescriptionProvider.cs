using System;

namespace DynamicPropertyObject
{
    internal class TypeDescriptionProvider : System.ComponentModel.TypeDescriptionProvider
    {
        private readonly System.ComponentModel.ICustomTypeDescriptor m_ctd;

        public TypeDescriptionProvider() { } 
        public TypeDescriptionProvider(System.ComponentModel.TypeDescriptionProvider parent) : base(parent) { }

        public TypeDescriptionProvider(System.ComponentModel.TypeDescriptionProvider parent, System.ComponentModel.ICustomTypeDescriptor ctd) : base(parent) { m_ctd = ctd; }

        public override System.ComponentModel.ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) { return m_ctd; }
    }
}