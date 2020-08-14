using System;
using System.Collections.Generic;
using System.Text;

namespace TyneTCP.Helpers
{
    public class Bitwise
    {
        public byte Value { get; private set; }

        public Bitwise(byte value)
        {
            Value = value;
        }

        public Bitwise()
            : this(0)
        {
        }

        public bool GetBool(int pos)
        {
            return (Value & (1 << pos)) > 0;
        }

        public void SetBool(int pos, bool value)
        {
            if (value)
            {
                Value |= (byte)(1 << pos);
            }
            else
            {
                Value &= (byte)(~(1 << pos));
            }
        }
    }
}