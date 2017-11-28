using System;
using System.ComponentModel;

namespace DynamicPropertyObject
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Enum)]
    public class ExpandEnumAttribute : Attribute
    {
        public ExpandEnumAttribute(bool expand)
        {
            Exapand = expand;
        }

        public bool Exapand
        {
            get;
            set;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExclusiveStandardValuesAttribute : Attribute
    {
        public ExclusiveStandardValuesAttribute(bool exclusive)
        {
            Exclusive = exclusive;
        }

        public bool Exclusive
        {
            get;
            set;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property)]
    public class ResourceAttribute : Attribute
    {
        public ResourceAttribute() { }

        public ResourceAttribute(string baseString)
        {
            BaseName = baseString;
        }

        public ResourceAttribute(string baseString, string keyPrefix)
        {
            BaseName = baseString;
            KeyPrefix = keyPrefix;
        }

        public string BaseName { get; set; }

        public string KeyPrefix { get; set; }

        public string AssemblyFullName { get; set; }

        // Use the hash code of the string objects and xor them together.
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return (BaseName.GetHashCode() ^ KeyPrefix.GetHashCode()) ^ AssemblyFullName.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ResourceAttribute)) { return false; }
            var other = (ResourceAttribute)obj;

            return string.Compare(BaseName, other.BaseName, StringComparison.OrdinalIgnoreCase) == 0 &&
                   string.Compare(AssemblyFullName, other.AssemblyFullName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override bool Match(object obj)
        {
            if (Equals(obj, this)) return true;

            switch (obj)
            {
                case null:
                    return false;
                case ResourceAttribute attribute:
                    return attribute.GetHashCode() == GetHashCode();
                default:
                    return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SortIDAttribute : Attribute
    {
        public SortIDAttribute()
        {
            PropertyOrder = 0;
            CategoryOrder = 0;
        }

        public SortIDAttribute(int propertyId, int categoryId)
        {
            PropertyOrder = propertyId;
            CategoryOrder = categoryId;
        }

        public int PropertyOrder { get; set; }

        public int CategoryOrder { get; set; }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class DynDisplayNameAttribute : DisplayNameAttribute
    {
        public DynDisplayNameAttribute() { }
        public DynDisplayNameAttribute(string displayName) : base(displayName) { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CategoryResourceKeyAttribute : Attribute
    {
        public CategoryResourceKeyAttribute() { }
        public CategoryResourceKeyAttribute(string resourceKey) { ResourceKey = resourceKey; }
        public string ResourceKey { get; set; }
    }

    public enum SortOrder
    {
        // no custom sorting
        None,

        // sort asscending using the property name or category name
        ByNameAscending,

        // sort descending using the property name or category name
        ByNameDescending,

        // sort asscending using property id or category id
        ByIdAscending,

        // sort descending using property id or category id
        ByIdDescending
    }
}