using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.Common
{
    public class PackedIntegerBase
    {
        protected ushort GetValue(uint offset, uint count, string variable_name = "Value")
        {
            long Value = Convert.ToInt64(this.GetType().GetField(variable_name).GetValue(this));
            
            uint end_offset = offset + count;
            if (end_offset > sizeof(ushort) * 8) throw new System.Exception("Packed Integer out of range!");

            long bitmask = 0;
            for (int i = 0; i < count; i++)
                bitmask |= 1L << i;
            bitmask = bitmask << (int)offset;

            var new_value = Value & bitmask;
            new_value = new_value >> (int)offset;

            return (ushort)new_value;
        }

        protected void SetValue(uint offset, uint count, ushort value, string variable_name = "Value")
        {
            long Value = Convert.ToInt64(this.GetType().GetField(variable_name).GetValue(this));

            uint end_offset = offset + count;
            if (end_offset > sizeof(ushort) * 8) throw new System.Exception("Packed Integer out of range!");

            long bitmask = 0;
            for (int i = (int)offset; i < end_offset; i++)
                bitmask |= 1L << i;

            long new_value_masked = (long)value << (int)offset;
            new_value_masked &= bitmask;

            long old_value_masked = (long)Value & ~bitmask;

            long new_value = new_value_masked | old_value_masked;

            Value = (ushort)new_value;
        }
    }
}
