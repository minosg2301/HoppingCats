using System;
using UnityEngine;

namespace BigNumbers
{
    [Serializable]
    public partial class BigNumber
    {
        public static int numDefaultPlaces = 4;

        /// <summary>
        /// BigNumber is represented in the scientific notation:
        /// 
        ///         S*Ax10^B
        ///  
        ///         S .... signum
        ///         A .... significand or mantissa
        ///         B .... exponent
        /// 
        /// </summary>
        [SerializeField] private sbyte signum = 0;
        [SerializeField] private int exponent = 0;
        [SerializeField] private byte[] mantissa = { 0 };
        [SerializeField] private int dataLength = 1;

        public BigNumber()
        {

        }

        public BigNumber(BigNumber rh)
        {
            BigNumber.Copy(rh, this);
        }

        public static implicit operator BigNumber(long value)
        {
            BigNumber A = new BigNumber();
            BigNumber.SetFromString(A, value.ToString());
            return A;
        }

        public static implicit operator BigNumber(float value)
        {
            BigNumber A = new BigNumber();
            BigNumber.SetFromString(A, value.ToString());
            return A;
        }

        public static implicit operator BigNumber(double value)
        {
            BigNumber A = new BigNumber();
            BigNumber.SetFromDouble(A, value);
            return A;
        }

        public static implicit operator BigNumber(string value)
        {
            BigNumber A = new BigNumber();
            BigNumber.SetFromString(A, value);
            return A;
        }

        public static BigNumber operator +(BigNumber c1, BigNumber c2)
        {
            BigNumber res = new BigNumber();
            BigNumber.Add(c1, c2, res);
            return res;
        }

        public static BigNumber operator +(BigNumber c1, long c2)
        {
            BigNumber res = new BigNumber();
            BigNumber C2 = c2;
            BigNumber.Add(c1, C2, res);
            return res;
        }

        public static BigNumber operator -(BigNumber c1, BigNumber c2)
        {
            BigNumber res = new BigNumber();
            BigNumber.Sub(c1, c2, res);
            return res;
        }

        public static BigNumber operator -(BigNumber c1, long c2)
        {
            BigNumber res = new BigNumber();
            BigNumber C2 = c2;
            BigNumber.Sub(c1, C2, res);
            return res;
        }

        public static BigNumber operator *(BigNumber c1, BigNumber c2)
        {
            BigNumber res = new BigNumber();
            BigNumber.Mul(c1, c2, res);
            return res;
        }

        public static BigNumber operator *(BigNumber c1, int c2)
        {
            BigNumber res = new BigNumber();
            BigNumber C2 = c2;
            BigNumber.Mul(c1, C2, res);
            return res;
        }

        public static BigNumber operator *(BigNumber c1, double c2)
        {
            BigNumber res = new BigNumber();
            BigNumber C2 = c2;
            BigNumber.Mul(c1, C2, res);
            return res;
        }

        public static BigNumber operator /(BigNumber c1, BigNumber c2)
        {
            BigNumber res = new BigNumber();
            BigNumber.Div(c1, c2, res);
            return res;
        }

        public static BigNumber operator /(BigNumber c1, float c2)
        {
            float inverseC2 = 1 / c2;
            return c1 * inverseC2;
        }

        public BigNumber Neg()
        {
            BigNumber res = 0;
            BigNumber.Neg(this, res);
            return res;
        }

        public BigNumber Abs()
        {
            BigNumber res = 0;
            BigNumber.Abs(res, this);
            return res;
        }

        public BigNumber Rez()
        {
            BigNumber res = 0;
            BigNumber.Reziprocal(this, res, numDefaultPlaces);
            return res;
        }

        public BigNumber Rez(int places)
        {
            BigNumber res = 0;
            BigNumber.Reziprocal(this, res, places);
            return res;
        }

        public BigNumber Round(int places)
        {
            BigNumber res = 0;
            BigNumber.Round(this, res, places);
            return res;
        }

        public BigNumber Sqrt(int places)
        {
            BigNumber res = 0;
            BigNumber.Sqrt(this, res, places);
            return res;
        }

        public BigNumber Sqrt()
        {
            BigNumber res = 0;
            BigNumber.Sqrt(this, res, numDefaultPlaces);
            return res;
        }

        public BigNumber Exp()
        {
            BigNumber res = 0;
            BigNumber.Exp(this, res, numDefaultPlaces);
            return res;
        }

        public BigNumber Exp(int places)
        {
            BigNumber res = 0;
            BigNumber.Exp(this, res, places);
            return res;
        }

        public BigNumber Pow(int exp)
        {
            BigNumber res = 0;
            BigNumber.IntPow(numDefaultPlaces, this, exp, res);
            return res;
        }

        public BigNumber Pow(BigNumber exp)
        {
            BigNumber res = 0;
            BigNumber.Power(this, exp, res, numDefaultPlaces);
            return res;
        }

        public BigNumber Pow(int exp, int places)
        {
            BigNumber res = 0;
            BigNumber.IntPow(places, this, exp, res);
            return res;
        }

        public BigNumber Pow(BigNumber exp, int places)
        {
            BigNumber res = 0;
            BigNumber.Power(this, exp, res, places);
            return res;
        }

        public BigNumber Log10()
        {
            BigNumber dst = new BigNumber();
            BigNumber.Log10(this, dst, numDefaultPlaces);
            return dst;
        }

        public BigNumber Log10(int places)
        {
            BigNumber dst = new BigNumber();
            BigNumber.Log10(this, dst, places);
            return dst;
        }

        public BigNumber Log()
        {
            BigNumber dst = new BigNumber();
            BigNumber.Log(this, dst, numDefaultPlaces);
            return dst;

        }
        public BigNumber Log(int places)
        {
            BigNumber dst = new BigNumber();
            BigNumber.Log(this, dst, places);
            return dst;
        }

        public BigNumber Floor()
        {
            BigNumber dst = new BigNumber();
            BigNumber.Floor(dst, this);
            return dst;
        }

        public BigNumber Ceil()
        {
            BigNumber dst = new BigNumber();
            BigNumber.Ceil(dst, this);
            return dst;
        }


        public static bool operator >(BigNumber c1, BigNumber c2)
        {
            return BigNumber.Compare(c1, c2) > 0;
        }

        public static bool operator >=(BigNumber c1, BigNumber c2)
        {
            return BigNumber.Compare(c1, c2) >= 0;
        }

        public static bool operator <(BigNumber c1, BigNumber c2)
        {
            return BigNumber.Compare(c1, c2) < 0;
        }

        public static bool operator <=(BigNumber c1, BigNumber c2)
        {
            return BigNumber.Compare(c1, c2) <= 0;
        }

        public static bool operator ==(BigNumber c1, BigNumber c2)
        {
            if (((object)c1) == null && ((object)c2) == null) return true;
            if (((object)c1) == null) return false;
            if (((object)c2) == null) return false;

            return BigNumber.Compare(c1, c2) == 0;
        }

        public static bool operator !=(BigNumber c1, BigNumber c2)
        {
            return !(c1 == c2);

        }

        public string IntString()
        {
            return BigNumber.ToIntString(this);
        }

        public string FullString()
        {
            return BigNumber.ToFullString(this);
        }

        public override string ToString()
        {
            return ToShortString(3, 2);
        }

        public string ToShortString(int digit = 4, int precision = 2)
        {
            return BigNumber.ToShortString(FullString(), digit, precision);
        }

        public string ExpString()
        {
            return BigNumber.ToExpString(this, -1);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
