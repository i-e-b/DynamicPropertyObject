using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace DynamicPropertyObject
{
    internal class EnumUtil
    {
        public static bool IsBitsOn(object enumInstance, object bits)
        {
            Debug.Assert(enumInstance != null);
            Debug.Assert(enumInstance.GetType().IsEnum);
            Debug.Assert(bits != null);
            Debug.Assert(bits.GetType().IsEnum);
            Debug.Assert(enumInstance.GetType() == bits.GetType());

            if (!IsFlag(enumInstance.GetType()))
            {
                return (enumInstance.Equals(bits));
            }

            if (IsZeroDefined(enumInstance.GetType()))  // special case
            {
                return (IsZero(enumInstance) && IsZero(bits));
            }

            // otherwise (!valueIsZero && !bitsIsZero)
            var enumDataType = Enum.GetUnderlyingType(enumInstance.GetType());
            if (enumDataType == typeof(Int16))
            {
                Int16 _value = Convert.ToInt16(enumInstance);
                Int16 _bits = Convert.ToInt16(bits);
                return ((_value & _bits) == _bits);
            }
            if (enumDataType == typeof(UInt16))
            {
                UInt16 _value = Convert.ToUInt16(enumInstance);
                UInt16 _bits = Convert.ToUInt16(bits);
                return ((_value & _bits) == _bits);
            }
            if (enumDataType == typeof(Int32))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                return ((_value & _bits) == _bits);
            }
            if (enumDataType == typeof(UInt32))
            {
                UInt32 _value = Convert.ToUInt32(enumInstance);
                UInt32 _bits = Convert.ToUInt32(bits);
                return ((_value & _bits) == _bits);
            }
            if (enumDataType == typeof(Int64))
            {
                Int64 _value = Convert.ToInt64(enumInstance);
                Int64 _bits = Convert.ToInt64(bits);
                return ((_value & _bits) == _bits);
            }
            if (enumDataType == typeof(UInt64))
            {
                UInt64 _value = Convert.ToUInt64(enumInstance);
                UInt64 _bits = Convert.ToUInt64(bits);
                return ((_value & _bits) == _bits);
            }
            if (enumDataType == typeof(SByte))
            {
                SByte _value = Convert.ToSByte(enumInstance);
                SByte _bits = Convert.ToSByte(bits);
                return ((_value & _bits) == _bits);
            }
            if (enumDataType == typeof(Byte))
            {
                Byte _value = Convert.ToByte(enumInstance);
                Byte _bits = Convert.ToByte(bits);
                return ((_value & _bits) == _bits);
            }
            return false;
        }

        public static bool TurnOffBits(ref object enumInstance, object bits)
        {
            Debug.Assert(enumInstance != null);
            Debug.Assert(enumInstance.GetType().IsEnum);
            Debug.Assert(bits != null);
            Debug.Assert(bits.GetType().IsEnum);
            Debug.Assert(enumInstance.GetType() == bits.GetType());

            if (!IsFlag(enumInstance.GetType()))
            {
                return false;
            }

            if (!IsBitsOn(enumInstance, bits)) // already turned off
            {
                return false;
            }
            if (IsZeroDefined(enumInstance.GetType()))  // special case
            {
                return false;
            }
            Type enumType = enumInstance.GetType();
            Type enumDataType = Enum.GetUnderlyingType(enumInstance.GetType());

            if (enumDataType == typeof(Int16))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }
            else if (enumDataType == typeof(UInt16))
            {
                UInt32 _value = Convert.ToUInt32(enumInstance);
                UInt32 _bits = Convert.ToUInt32(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }
            else if (enumDataType == typeof(Int32))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }
            else if (enumDataType == typeof(UInt32))
            {
                UInt32 _value = Convert.ToUInt32(enumInstance);
                UInt32 _bits = Convert.ToUInt32(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }
            else if (enumDataType == typeof(Int64))
            {
                Int64 _value = Convert.ToInt64(enumInstance);
                Int64 _bits = Convert.ToInt64(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }
            else if (enumDataType == typeof(UInt64))
            {
                UInt64 _value = Convert.ToUInt64(enumInstance);
                UInt64 _bits = Convert.ToUInt64(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }
            else if (enumDataType == typeof(SByte))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }
            else if (enumDataType == typeof(Byte))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value &= ~(_bits);
                enumInstance = _value;
            }

            enumInstance = Enum.ToObject(enumType, enumInstance);

            return true;
        }

        public static bool TurnOnBits(ref object enumInstance, object bits)
        {
            Debug.Assert(enumInstance != null);
            Debug.Assert(enumInstance.GetType().IsEnum);
            Debug.Assert(bits != null);
            Debug.Assert(bits.GetType().IsEnum);
            Debug.Assert(enumInstance.GetType() == bits.GetType());

            if (!IsFlag(enumInstance.GetType()))
            {
                if (enumInstance.Equals(bits)) return false;
                enumInstance = bits;
                return true;
            }

            if (IsBitsOn(enumInstance, bits)) // already turned on
            {
                return false;
            }

            if (IsZeroDefined(enumInstance.GetType()))  // special case
            {
                return !(IsZero(enumInstance) && IsZero(bits));
            }

            Type enumType = enumInstance.GetType();
            Type enumDataType = Enum.GetUnderlyingType(enumInstance.GetType());

            if (enumDataType == typeof(Int16))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            else if (enumDataType == typeof(UInt16))
            {
                UInt32 _value = Convert.ToUInt32(enumInstance);
                UInt32 _bits = Convert.ToUInt32(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            else if (enumDataType == typeof(Int32))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            else if (enumDataType == typeof(UInt32))
            {
                UInt32 _value = Convert.ToUInt32(enumInstance);
                UInt32 _bits = Convert.ToUInt32(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            else if (enumDataType == typeof(Int64))
            {
                Int64 _value = Convert.ToInt64(enumInstance);
                Int64 _bits = Convert.ToInt64(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            else if (enumDataType == typeof(UInt64))
            {
                UInt64 _value = Convert.ToUInt64(enumInstance);
                UInt64 _bits = Convert.ToUInt64(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            else if (enumDataType == typeof(SByte))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            else if (enumDataType == typeof(Byte))
            {
                Int32 _value = Convert.ToInt32(enumInstance);
                Int32 _bits = Convert.ToInt32(bits);
                _value |= _bits;
                enumInstance = _value;
            }
            enumInstance = Enum.ToObject(enumType, enumInstance);
            return true;
        }

        public static bool IsZeroDefined(Type enumType)
        {
            Debug.Assert(enumType != null);
            Debug.Assert(enumType.IsEnum);
            Debug.Assert(IsFlag(enumType));

            Type enumDataType = Enum.GetUnderlyingType(enumType);

            if (enumDataType == typeof(Int16))
            {
                Int16 zero = 0;
                return Enum.IsDefined(enumType, zero);
            }
            if (enumDataType == typeof(UInt16))
            {
                UInt16 zero = 0;
                return Enum.IsDefined(enumType, zero);
            }
            if (enumDataType == typeof(Int32))
            {
                Int32 zero = 0;
                return Enum.IsDefined(enumType, zero);
            }
            if (enumDataType == typeof(UInt32))
            {
                UInt32 zero = 0;
                return Enum.IsDefined(enumType, zero);
            }
            if (enumDataType == typeof(Int64))
            {
                Int64 zero = 0;
                return Enum.IsDefined(enumType, zero);
            }
            if (enumDataType == typeof(UInt64))
            {
                UInt64 zero = 0;
                return Enum.IsDefined(enumType, zero);
            }
            if (enumDataType == typeof(SByte))
            {
                SByte zero = 0;
                return Enum.IsDefined(enumType, zero);
            }
            if (enumDataType == typeof(Byte))
            {
                Byte zero = 0;
                return Enum.IsDefined(enumType, zero);
            }

            return false;
        }

        public static bool IsZero(object enumInstance)
        {
            Debug.Assert(enumInstance != null);
            Debug.Assert(enumInstance.GetType().IsEnum);

            if (!IsZeroDefined(enumInstance.GetType()))
            {
                return false;
            }

            Type enumDataType = Enum.GetUnderlyingType(enumInstance.GetType());

            if (enumDataType == typeof(Int16))
            {
                Int16 zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }
            if (enumDataType == typeof(UInt16))
            {
                UInt16 zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }
            if (enumDataType == typeof(Int32))
            {
                Int32 zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }
            if (enumDataType == typeof(UInt32))
            {
                UInt32 zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }
            if (enumDataType == typeof(Int64))
            {
                Int64 zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }
            if (enumDataType == typeof(UInt64))
            {
                UInt64 zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }
            if (enumDataType == typeof(SByte))
            {
                SByte zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }
            if (enumDataType == typeof(Byte))
            {
                Byte zero = 0;
                object objZero = Enum.ToObject(enumInstance.GetType(), zero);
                return objZero.Equals(enumInstance);
            }

            return false;
        }

        public static bool IsFlag(Type enumType)
        {
            Debug.Assert(enumType != null);
            Debug.Assert(enumType.IsEnum);
            return (enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0);
        }

        public static DynStandardValue[] GetStandardValues(object enumInstance)
        {
            Debug.Assert(enumInstance != null);
            Debug.Assert(enumInstance.GetType().IsEnum);
            return GetStandardValues(enumInstance.GetType(), BindingFlags.Public | BindingFlags.Instance);
        }

        public static DynStandardValue[] GetStandardValues(Type enumType)
        {
            Debug.Assert(enumType != null);
            Debug.Assert(enumType.IsEnum);

            return GetStandardValues(enumType, BindingFlags.Public | BindingFlags.Static);
        }

        private static DynStandardValue[] GetStandardValues(Type enumType, BindingFlags flags)
        {
            var arrAttr = new ArrayList();
            var fields = enumType.GetFields(flags);

            foreach (var fi in fields)
            {
                var sv = new DynStandardValue(Enum.ToObject(enumType, fi.GetValue(null)));
                sv.DisplayName = Enum.GetName(enumType, sv.Value); // by default

                if (fi.GetCustomAttributes(typeof(DynDisplayNameAttribute), false) is DynDisplayNameAttribute[] dna && dna.Length > 0)
                {
                    sv.DisplayName = dna[0].DisplayName;
                }

                if (fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false) is System.ComponentModel.DescriptionAttribute[] da && da.Length > 0)
                {
                    sv.Description = da[0].Description;
                }

                if (fi.GetCustomAttributes(typeof(System.ComponentModel.BrowsableAttribute), false) is System.ComponentModel.BrowsableAttribute[] ba && ba.Length > 0)
                {
                    sv.Visible = ba[0].Browsable;
                }

                if (fi.GetCustomAttributes(typeof(System.ComponentModel.ReadOnlyAttribute), false) is System.ComponentModel.ReadOnlyAttribute[] roa && roa.Length > 0)
                {
                    sv.Enabled = !roa[0].IsReadOnly;
                }
                arrAttr.Add(sv);
            }
            var retAttr = arrAttr.ToArray(typeof(DynStandardValue)) as DynStandardValue[];
            return retAttr;
        }
    }
}