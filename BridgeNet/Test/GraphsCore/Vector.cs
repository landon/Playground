using System;
using System.Collections.Generic;
using System.Text;
using GraphicsLayer;

namespace Graphs
{
    public struct Vector
    {
        public Vector(double x, double y)
        {
            _X = x;
            _Y = y;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(_X * _X + _Y * _Y);
            }
        }

        public bool Normalize()
        {
            var length = Length;

            _X = _X / length;
            _Y = _Y / length;

            return length > 0;
        }

        public double Dot(Vector vector)
        {
            return _X * vector._X + _Y * vector._Y;
        }

        public double Distance(Vector vector)
        {
            return (double)Math.Sqrt(Math.Pow(vector._X - _X, 2) + Math.Pow(vector._Y - _Y, 2));
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a._X + b._X, a._Y + b._Y);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a._X, -a._Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a._X - b._X, a._Y - b._Y);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a._X * b, a._Y * b);
        }

        public static Vector operator *(Vector a, int b)
        {
            return new Vector(a._X * b, a._Y * b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
                return false;

            Vector v = (Vector)obj;

            return Equals(v);
        }

        public bool Equals(Vector v)
        {
            return _X == v._X && _Y == v._Y;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public static implicit operator Box(Vector v)
        {
            return new Box(v.X, v.Y);
        }

        public override string ToString()
        {
            return "(" + _X + ", " + _Y + ")";
        }

        public double X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
            }
        }
        public double Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }

        double _X;
        double _Y;
    }
}
