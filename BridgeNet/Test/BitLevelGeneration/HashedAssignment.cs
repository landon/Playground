
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public class HashedAssignment_ulong
    {
        ulong[] _assignment; 
        int _length;
		int _hashCode;

        public HashedAssignment_ulong(ulong[] assignment, int index, ulong on)
        {
            _assignment = new ulong[assignment.Length - index];
            _length = 0;

            for (int i = index; i < assignment.Length; i++)
            {
                var x = (ulong)(assignment[i] & on);
                if (x != 0)
                {
                    _assignment[_length] = x;
                    _length++;
                }
            }

            Array.Sort(_assignment, 0, _length);

            _hashCode = Hash(_assignment, _length);
        }

        int Hash(ulong[] list, int length)
        {
            ulong hash = 5381;
            for (int i = 0; i < length; i++)
                hash = (ulong)(list[i] + (hash << 5) + hash);

            return (int)hash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HashedAssignment_ulong);
        }

        public bool Equals(HashedAssignment_ulong other)
        {
            if (other == null || _length != other._length)
                return false;

            for (int i = 0; i < _length; i++)
                if (_assignment[i] != other._assignment[i])
                    return false;

			return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
namespace BitLevelGeneration
{
    public class HashedAssignment_uint
    {
        uint[] _assignment; 
        int _length;
		int _hashCode;

        public HashedAssignment_uint(uint[] assignment, int index, uint on)
        {
            _assignment = new uint[assignment.Length - index];
            _length = 0;

            for (int i = index; i < assignment.Length; i++)
            {
                var x = (uint)(assignment[i] & on);
                if (x != 0)
                {
                    _assignment[_length] = x;
                    _length++;
                }
            }

            Array.Sort(_assignment, 0, _length);

            _hashCode = Hash(_assignment, _length);
        }

        int Hash(uint[] list, int length)
        {
            ulong hash = 5381;
            for (int i = 0; i < length; i++)
                hash = (ulong)(list[i] + (hash << 5) + hash);

            return (int)hash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HashedAssignment_uint);
        }

        public bool Equals(HashedAssignment_uint other)
        {
            if (other == null || _length != other._length)
                return false;

            for (int i = 0; i < _length; i++)
                if (_assignment[i] != other._assignment[i])
                    return false;

			return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
namespace BitLevelGeneration
{
    public class HashedAssignment_ushort
    {
        ushort[] _assignment; 
        int _length;
		int _hashCode;

        public HashedAssignment_ushort(ushort[] assignment, int index, ushort on)
        {
            _assignment = new ushort[assignment.Length - index];
            _length = 0;

            for (int i = index; i < assignment.Length; i++)
            {
                var x = (ushort)(assignment[i] & on);
                if (x != 0)
                {
                    _assignment[_length] = x;
                    _length++;
                }
            }

            Array.Sort(_assignment, 0, _length);

            _hashCode = Hash(_assignment, _length);
        }

        int Hash(ushort[] list, int length)
        {
            ulong hash = 5381;
            for (int i = 0; i < length; i++)
                hash = (ulong)(list[i] + (hash << 5) + hash);

            return (int)hash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HashedAssignment_ushort);
        }

        public bool Equals(HashedAssignment_ushort other)
        {
            if (other == null || _length != other._length)
                return false;

            for (int i = 0; i < _length; i++)
                if (_assignment[i] != other._assignment[i])
                    return false;

			return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
namespace BitLevelGeneration
{
    public class HashedAssignment_byte
    {
        byte[] _assignment; 
        int _length;
		int _hashCode;

        public HashedAssignment_byte(byte[] assignment, int index, byte on)
        {
            _assignment = new byte[assignment.Length - index];
            _length = 0;

            for (int i = index; i < assignment.Length; i++)
            {
                var x = (byte)(assignment[i] & on);
                if (x != 0)
                {
                    _assignment[_length] = x;
                    _length++;
                }
            }

            Array.Sort(_assignment, 0, _length);

            _hashCode = Hash(_assignment, _length);
        }

        int Hash(byte[] list, int length)
        {
            ulong hash = 5381;
            for (int i = 0; i < length; i++)
                hash = (ulong)(list[i] + (hash << 5) + hash);

            return (int)hash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HashedAssignment_byte);
        }

        public bool Equals(HashedAssignment_byte other)
        {
            if (other == null || _length != other._length)
                return false;

            for (int i = 0; i < _length; i++)
                if (_assignment[i] != other._assignment[i])
                    return false;

			return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}

