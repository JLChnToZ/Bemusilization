using System;

namespace LitJson {
    public partial class JsonData: IJsonWrapper, IEquatable<JsonData>, IConvertible, IFormattable {
        TypeCode IConvertible.GetTypeCode() {
            switch(type) {
                case JsonType.Boolean: return TypeCode.Boolean;
                case JsonType.Int: return TypeCode.Int32;
                case JsonType.Long: return TypeCode.Int64;
                case JsonType.Double: return TypeCode.Double;
                case JsonType.String: return TypeCode.String;
                case JsonType.Object:
                case JsonType.Array: return TypeCode.Object;
                default: return TypeCode.Empty;
            }
        }

        bool IConvertible.ToBoolean(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return false;
                case JsonType.Boolean: return inst_boolean;
                case JsonType.Int: return inst_int != 0;
                case JsonType.Long: return inst_long != 0;
                case JsonType.Double: return inst_double != 0;
                case JsonType.String: return bool.Parse(inst_string);
                default: throw new InvalidCastException();
            }
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0;
                case JsonType.Int: return (sbyte)inst_int;
                case JsonType.Long: return (sbyte)inst_long;
                case JsonType.Double: return (sbyte)inst_double;
                case JsonType.Boolean: return inst_boolean ? (sbyte)1 : (sbyte)0;
                case JsonType.String: return sbyte.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        short IConvertible.ToInt16(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0;
                case JsonType.Int: return (short)inst_int;
                case JsonType.Long: return (short)inst_long;
                case JsonType.Double: return (short)inst_double;
                case JsonType.Boolean: return inst_boolean ? (short)1 : (short)0;
                case JsonType.String: return short.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        int IConvertible.ToInt32(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0;
                case JsonType.Int: return inst_int;
                case JsonType.Long: return (int)inst_long;
                case JsonType.Double: return (int)inst_double;
                case JsonType.Boolean: return inst_boolean ? 1 : 0;
                case JsonType.String: return int.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        long IConvertible.ToInt64(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0L;
                case JsonType.Long: return inst_long;
                case JsonType.Int: return inst_int;
                case JsonType.Double: return (long)inst_double;
                case JsonType.Boolean: return inst_boolean ? 1L : 0L;
                case JsonType.String: return long.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        byte IConvertible.ToByte(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0;
                case JsonType.Int: return (byte)inst_int;
                case JsonType.Long: return (byte)inst_long;
                case JsonType.Double: return (byte)inst_double;
                case JsonType.Boolean: return inst_boolean ? (byte)1 : (byte)0;
                case JsonType.String: return byte.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0;
                case JsonType.Int: return (ushort)inst_int;
                case JsonType.Long: return (ushort)inst_long;
                case JsonType.Double: return (ushort)inst_double;
                case JsonType.Boolean: return inst_boolean ? (ushort)1 : (ushort)0;
                case JsonType.String: return ushort.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        uint IConvertible.ToUInt32(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0U;
                case JsonType.Int: return (uint)inst_int;
                case JsonType.Long: return (uint)inst_long;
                case JsonType.Double: return (uint)inst_double;
                case JsonType.Boolean: return inst_boolean ? 1U : 0U;
                case JsonType.String: return uint.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0UL;
                case JsonType.Long: return (ulong)inst_long;
                case JsonType.Int: return (ulong)inst_int;
                case JsonType.Double: return (ulong)inst_double;
                case JsonType.Boolean: return inst_boolean ? 1UL : 0UL;
                case JsonType.String: return ulong.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        float IConvertible.ToSingle(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0F;
                case JsonType.Double: return (float)inst_double;
                case JsonType.Int: return inst_int;
                case JsonType.Long: return inst_long;
                case JsonType.Boolean: return inst_boolean ? 1F : 0F;
                case JsonType.String: return float.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        double IConvertible.ToDouble(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0D;
                case JsonType.Double: return inst_double;
                case JsonType.Int: return inst_int;
                case JsonType.Long: return inst_long;
                case JsonType.Boolean: return inst_boolean ? 1D : 0D;
                case JsonType.String: return double.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return 0M;
                case JsonType.Int: return inst_int;
                case JsonType.Long: return inst_long;
                case JsonType.Double: return (decimal)inst_double;
                case JsonType.Boolean: return inst_boolean ? 1M : 0M;
                case JsonType.String: return decimal.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        char IConvertible.ToChar(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return '\0';
                case JsonType.Int: return (char)inst_int;
                case JsonType.Long: return (char)inst_long;
                case JsonType.Double: return (char)inst_double;
                case JsonType.String: return char.Parse(inst_string);
                default: throw new InvalidCastException();
            }
        }

        string IConvertible.ToString(IFormatProvider provider) {
            switch(type) {
                case JsonType.None: return string.Empty;
                case JsonType.Int: return inst_int.ToString(provider);
                case JsonType.Long: return inst_long.ToString(provider);
                case JsonType.Double: return inst_double.ToString(provider);
                case JsonType.Boolean: return inst_boolean.ToString(provider);
                case JsonType.String: return inst_string;
                default: return ToString();
            }
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider) {
            switch(type) {
                case JsonType.None: return string.Empty;
                case JsonType.Int: return inst_int.ToString(format, formatProvider);
                case JsonType.Long: return inst_long.ToString(format, formatProvider);
                case JsonType.Double: return inst_double.ToString(format, formatProvider);
                case JsonType.Boolean: return inst_boolean.ToString(formatProvider);
                case JsonType.String: return inst_string;
                default: return ToString();
            }
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) {
            switch(type) {
                case JsonType.String: return DateTime.Parse(inst_string, provider);
                default: throw new InvalidCastException();
            }
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
            if(conversionType == null)
                throw new ArgumentNullException("conversionType");
            if(conversionType.IsAssignableFrom(typeof(JsonData)))
                return this;
            IConvertible rawValue;
            switch(type) {
                case JsonType.None: return null;
                case JsonType.Boolean: rawValue = inst_boolean; break;
                case JsonType.Int: rawValue = inst_int; break;
                case JsonType.Long: rawValue = inst_long; break;
                case JsonType.Double: rawValue = inst_double; break;
                case JsonType.String: rawValue = inst_string; break;
                default: throw new InvalidCastException();
            }
            return rawValue.ToType(conversionType, provider);
        }
    }
}
