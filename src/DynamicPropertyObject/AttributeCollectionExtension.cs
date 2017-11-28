using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicPropertyObject
{
    public static class AttributeCollectionExtension
    {
        public static void Add(this System.ComponentModel.AttributeCollection ac, Attribute attribute)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return;

            var arrAttr = (Attribute[])fi.GetValue(ac);
            var listAttr = new List<Attribute>();
            if (arrAttr != null)
            {
                listAttr.AddRange(arrAttr);
            }
            listAttr.Add(attribute);
            fi.SetValue(ac, listAttr.ToArray());
        }

        public static void AddRange(this System.ComponentModel.AttributeCollection ac, Attribute[] attributes)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return;

            var arrAttr = (Attribute[])fi.GetValue(ac);
            var listAttr = new List<Attribute>();
            if (arrAttr != null)
            {
                listAttr.AddRange(arrAttr);
            }
            listAttr.AddRange(attributes);
            fi.SetValue(ac, listAttr.ToArray());
        }

        public static void Add(this System.ComponentModel.AttributeCollection ac, Attribute attribute, bool removeBeforeAdd)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return;

            var arrAttr = (Attribute[])fi.GetValue(ac);
            var listAttr = new List<Attribute>();
            if (arrAttr != null)
            {
                listAttr.AddRange(arrAttr);
            }
            if (removeBeforeAdd)
            {
                listAttr.RemoveAll(a => a.Match(attribute));
            }
            listAttr.Add(attribute);
            fi.SetValue(ac, listAttr.ToArray());
        }

        public static void Add(this System.ComponentModel.AttributeCollection ac, Attribute attribute, Type typeToRemoveBeforeAdd)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return;

            var arrAttr = (Attribute[])fi.GetValue(ac);
            var listAttr = new List<Attribute>();
            if (arrAttr != null)
            {
                listAttr.AddRange(arrAttr);
            }
            if (typeToRemoveBeforeAdd != null)
            {
                listAttr.RemoveAll(a => a.GetType() == typeToRemoveBeforeAdd || a.GetType().IsSubclassOf(typeToRemoveBeforeAdd));
            }
            listAttr.Add(attribute);
            fi.SetValue(ac, listAttr.ToArray());
        }

        public static void Clear(this System.ComponentModel.AttributeCollection ac)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null) fi.SetValue(ac, null);
        }

        public static void Remove(this System.ComponentModel.AttributeCollection ac, Attribute attribute)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return;

            var arrAttr = (Attribute[])fi.GetValue(ac);
            var listAttr = new List<Attribute>();
            if (arrAttr != null)
            {
                listAttr.AddRange(arrAttr);
            }
            listAttr.RemoveAll(a => a.Match(attribute));
            fi.SetValue(ac, listAttr.ToArray());
        }

        public static void Remove(this System.ComponentModel.AttributeCollection ac, Type type)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return;

            var arrAttr = (Attribute[])fi.GetValue(ac);
            var listAttr = new List<Attribute>();
            if (arrAttr != null)
            {
                listAttr.AddRange(arrAttr);
            }
            listAttr.RemoveAll(a => a.GetType() == type);
            fi.SetValue(ac, listAttr.ToArray());
        }

        public static Attribute Get(this System.ComponentModel.AttributeCollection ac, Attribute attribute)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) { return null; }

            var arrAttr = (Attribute[]) fi.GetValue(ac);
            if (arrAttr == null)
            {
                return null;
            }
            var attrFound = arrAttr.FirstOrDefault(a => a.Match(attribute));
            return attrFound;
        }

        public static List<Attribute> Get(this System.ComponentModel.AttributeCollection ac, params Attribute[] attributes)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return new List<Attribute>();

            var arrAttr = (Attribute[])fi.GetValue(ac);

            if (arrAttr == null)
            {
                return null;
            }
            var listAttr = new List<Attribute>();
            listAttr.AddRange(arrAttr);
            var ac2 = new System.ComponentModel.AttributeCollection(attributes);
            var listAttrFound = listAttr.FindAll(a => ac2.Matches(a));
            return listAttrFound;
        }

        public static Attribute Get(this System.ComponentModel.AttributeCollection ac, Type attributeType)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) { return null; }
            var arrAttr = (Attribute[]) fi.GetValue(ac);
            var attrFound = arrAttr.FirstOrDefault(a => a.GetType() == attributeType);
            return attrFound;
        }

        public static Attribute Get(this System.ComponentModel.AttributeCollection ac, Type attributeType, bool derivedType)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) { return null; }
            var arrAttr = (Attribute[]) fi.GetValue(ac);
            Attribute attrFound;
            if (!derivedType)
            {
                attrFound = arrAttr.FirstOrDefault(a => a.GetType() == attributeType);
            }
            else
            {
                attrFound = arrAttr.FirstOrDefault(a => a.GetType() == attributeType || a.GetType().IsSubclassOf(attributeType));
            }
            return attrFound;
        }

        public static List<Attribute> Get(this System.ComponentModel.AttributeCollection ac, params Type[] attributeTypes)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) { return new List<Attribute>(); }
            var arrAttr = (Attribute[]) fi.GetValue(ac);

            if (arrAttr == null)
            {
                return null;
            }
            var listAttr = new List<Attribute>();
            listAttr.AddRange(arrAttr);
            // ReSharper disable once PossibleMistakenCallToGetType.2
            var listAttrFound = listAttr.FindAll(a => a.GetType() == attributeTypes.FirstOrDefault(b => b.GetType() == a.GetType()));

            return listAttrFound;
        }

        public static Attribute[] ToArray(this System.ComponentModel.AttributeCollection ac)
        {
            var fi = ac.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) { return null; }
            var arrAttr = (Attribute[]) fi.GetValue(ac);
            return arrAttr;
        }
    }
}