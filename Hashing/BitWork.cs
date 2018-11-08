using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashing
{
    public class BitWork
    {
        private bool[] _value;

        public BitWork(byte[] bytes)
        {
            var value = "";
            foreach (var bt in bytes)
            {
                value += Convert.ToString(bt, 2).PadLeft(8, '0');
            }
            ConvertStringToBools(value);
        }

        public BitWork(byte bt)
        {
            var value = Convert.ToString(bt, 2).PadLeft(8, '0');
            ConvertStringToBools(value);
        }

        public BitWork(string value)
        {
            ConvertStringToBools(value);
        }

        private BitWork(bool[] value)
        {
            _value = value;
        }

        private void ConvertStringToBools(string value)
        {
            _value = new bool[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                _value[i] = value[i] == '1';
            }
        }

        public int Length
        {
            get
            {
                return _value.Length;
            }
        }

        public byte[] GetBytes()
        {
            var chunks8 = Split(8);
            return chunks8
                .Select(c => Convert.ToByte(c.ToString(), 2))
                .ToArray();
        }

        public override string ToString()
        {
            return string.Join("", _value.Select(v => v ? '1' : '0'));
        }

        public BitWork AddAtEnd(byte value, int count)
        {
            var addbits = Enumerable.Repeat(value == 1, count);
            return new BitWork(_value.Concat(addbits).ToArray());
        }

        public BitWork AddAtEnd(BitWork bits)
        {
            return new BitWork(_value.Concat(bits._value).ToArray());
        }

        public BitWork AddAtBegin(byte value, int count)
        {
            var addbits = Enumerable.Repeat(value == 1, count);
            return new BitWork(addbits.Concat(_value).ToArray());
        }

        public List<BitWork> Split(int count)
        {
            return _value
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / count)
                .Select(x => new BitWork(x.Select(v => v.Value).ToArray()))
                .ToList();
        }

        public BitWork LeftRotate(int count)
        {
            var v = _value.ToList();
            var left = v.GetRange(0, count);
            v.RemoveRange(0, count);
            v.AddRange(left);
            return new BitWork(v.ToArray());
        }

        public static BitWork operator^(BitWork left, BitWork right)
        {
            var maxLength = left.Length > right.Length ? left.Length : right.Length;

            var ns = new bool[maxLength];
            for(var i = 0; i < maxLength; i++)
            {
                var l = i < left.Length ? left._value[i] : false;
                var r = i < right.Length ? right._value[i] : false;
                
                ns[i] = l ^ r;
            }

            return new BitWork(ns);
        }

        public static BitWork operator &(BitWork left, BitWork right)
        {
            var maxLength = left.Length > right.Length ? left.Length : right.Length;

            var ns = new bool[maxLength];
            for (var i = 0; i < maxLength; i++)
            {
                var l = i < left.Length ? left._value[i] : false;
                var r = i < right.Length ? right._value[i] : false;

                ns[i] = l & r;
            }

            return new BitWork(ns);
        }

        public static BitWork operator |(BitWork left, BitWork right)
        {
            var maxLength = left.Length > right.Length ? left.Length : right.Length;

            var ns = new bool[maxLength];
            for (var i = 0; i < maxLength; i++)
            {
                var l = i < left.Length ? left._value[i] : false;
                var r = i < right.Length ? right._value[i] : false;
                
                ns[i] = l | r;
            }

            return new BitWork(ns);
        }

        public static BitWork operator !(BitWork left)
        {
            var ns = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
            {
                ns[i] = !left._value[i];
            }

            return new BitWork(ns);
        }

        public static BitWork operator +(BitWork left, BitWork right)
        {
            var maxLength = left.Length > right.Length ? left.Length : right.Length;

            var vleft = left._value.Reverse().ToArray();
            var vright = right._value.Reverse().ToArray();

            var c = false;
            var ns = new List<bool>();
            for (var i = 0; i < maxLength ; i++)
            {
                var l = i < vleft.Length ? vleft[i] : false;
                var r = i < vright.Length ? vright[i] : false;

                var v = l ^ r ^ c;
                c = c ? l | r : l & r;

                ns.Add(v);
            }

            if (c)
                ns.Add(c);

            ns.Reverse();

            return new BitWork(ns.ToArray());
        }

        public BitWork TruncateLeft(int length)
        {
            if (length <= _value.Length)
            {
                var toremove = _value.Length - length;
                var v = _value.ToList();
                v.RemoveRange(0, toremove);
                return new BitWork(v.ToArray());
            }
            else
                throw new Exception();
        }

        public BitWork Clone()
        {
            return new BitWork((bool[])_value.Clone());
        }
    }
}