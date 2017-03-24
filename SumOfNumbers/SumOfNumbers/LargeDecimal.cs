using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SumOfNumbers
{
    public class LargeDecimal : IComparable<LargeDecimal>
    {
        private int[] _mantiss;
        private int _exponent;
        private const int MANTISS_LENGTH = 39;
        private static readonly Regex ExponentialCheck = new Regex("^\\d[\\.,]\\d+e[\\+\\-]?\\d+$", RegexOptions.Compiled);

        public static LargeDecimal Zero
        {
            get { return LargeDecimal.FromString("0.0e0"); }
        }

        private LargeDecimal(int[] mantiss, int exponent)
        {
            _exponent = exponent;
            _mantiss = new int[MANTISS_LENGTH];
            mantiss.CopyTo(_mantiss, 0);
        }

        public static LargeDecimal FromString(string numberStr)
        {
            if (numberStr.Length == 0)
            {
                throw new ArgumentException("String should be not empty and a number should be in exponential form: MeN");
            }

            if (numberStr.Length > 0 && !IsExponentialNumber(numberStr))
            {
                throw new ArgumentException("Wrong number format. Number should be in exponential form: MeN");
            }

            int[] mantiss = new int[MANTISS_LENGTH];

            numberStr = Regex.Replace(numberStr, @"[,\.]", String.Empty);
            int indexOfE = numberStr.IndexOf("e", StringComparison.CurrentCultureIgnoreCase);

            for (int i = 0; i < indexOfE; i++)
            {
                if (char.IsDigit(numberStr[i]))
                {
                    int digit = int.Parse(numberStr[i].ToString());
                    mantiss[i] = digit;
                }
            }

            var exponentStr = numberStr.Substring(indexOfE + 1);
            //exponentStr = exponentStr.Replace("+", "");

            return new LargeDecimal(mantiss, int.Parse(exponentStr));
        }

        private int FindNotZeroIndex(int startIndex = 0)
        {
            for (int i = startIndex; i < _mantiss.Length; i++)
            {
                if (_mantiss[i] > 0) return i;
            }
            return -1;
        }

        private int FindLastNotZeroIndex()
        {
            int result = -1;
            for (int i = 0; i < MANTISS_LENGTH; i++)
            {
                if (_mantiss[i] > 0) result = i;
            }
            return result;
        }

        private bool IsZero()
        {
            return FindNotZeroIndex() == -1;
        }

        public static LargeDecimal Add(LargeDecimal one, LargeDecimal two)
        {
            if (one.IsZero())
            {
                return new LargeDecimal(two._mantiss, two._exponent);
            }

            if (two.IsZero())
            {
                return new LargeDecimal(one._mantiss, one._exponent);
            }

            //make copy of two numbers
            var firstCopy = new LargeDecimal(one._mantiss, one._exponent);
            var secondCopy = new LargeDecimal(two._mantiss, two._exponent);

            Equalize(firstCopy, secondCopy);

            var result = LargeDecimal.Zero;
            result._exponent = firstCopy._exponent;

            var carry = 0;

            for (int i = MANTISS_LENGTH - 1; i >= 0; i--)
            {
                var sum = firstCopy._mantiss[i] + secondCopy._mantiss[i] + carry;
                if (sum >= 10)
                {
                    carry = sum / 10;
                    sum = sum % 10;
                }
                else carry = 0;
                result._mantiss[i] = sum;
            }

            if (carry != 0)
            {
                result.ShiftRight(1);
                result._mantiss[0] = carry;
            }

            return result;
        }

        private void ShiftLeft(int bitsCount)
        {
            if (bitsCount >= MANTISS_LENGTH)
            {
                throw new ArgumentException("Shift left cant be bigger than max mantiss length");
            }

            int newExponent = _exponent - bitsCount;
            while (_exponent != newExponent)
            {
                for (int i = 0; i < MANTISS_LENGTH - 1; i++)
                {
                    _mantiss[i] = _mantiss[i + 1];
                }

                _exponent--;
            }
        }

        private void ShiftRight(int bitsCount)
        {
            int newExponent = _exponent + bitsCount;
            while (_exponent != newExponent)
            {
                int temp = -1;
                for (int i = 0; i < MANTISS_LENGTH - 1; i++)
                {
                    if (temp == -1)
                    {
                        temp = _mantiss[i + 1];
                        _mantiss[i + 1] = _mantiss[i];
                    }
                    else
                    {
                        int temp2 = _mantiss[i + 1];
                        _mantiss[i + 1] = temp;
                        temp = temp2;
                    }
                }
                _mantiss[0] = 0;
                _exponent++;
            }
        }

        public static LargeDecimal operator >>(LargeDecimal one, int bitCount)
        {
            var copy = new LargeDecimal(one._mantiss, one._exponent);
            copy.ShiftRight(bitCount);
            return copy;
        }

        public static LargeDecimal operator <<(LargeDecimal one, int bitCount)
        {
            var copy = new LargeDecimal(one._mantiss, one._exponent);
            copy.ShiftLeft(bitCount);
            return copy;
        }

        public static LargeDecimal operator +(LargeDecimal first, LargeDecimal second)
        {
            return Add(first, second);
        }

        /// <summary>
        /// Makes two numbers exponents equal
        /// </summary>
        /// <param name="first">first number</param>
        /// <param name="second">second number</param>
        private static void Equalize(LargeDecimal first, LargeDecimal second)
        {
            if (first._exponent > second._exponent)
            {
                var diff = first._exponent - second._exponent;
                second.ShiftRight(diff);
            }
            else
            {
                var diff = first._exponent - second._exponent;
                first.ShiftRight(Math.Abs(diff));
            }
        }

        /// <summary>
        /// Checks if specified string is exponential number
        /// </summary>
        /// <param name="inputStr">input number string</param>
        /// <returns>true if string is exponential number, false otherwise</returns>
        private static bool IsExponentialNumber(string inputStr)
        {
            return ExponentialCheck.IsMatch(inputStr);
        }

        /// <summary>
        /// String representation of number
        /// </summary>
        /// <returns>string representation of number</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(_mantiss[0]);
            result.Append(".");

            var length = FindLastNotZeroIndex() + 1;

            for (int i = 1; i < length; i++)
            {
                result.Append(_mantiss[i]);
            }
            result.Append('e');
            result.Append(_exponent);
            return result.ToString();
        }

        /// <summary>
        /// Compare one number with another
        /// </summary>
        /// <param name="other">number to compare</param>
        /// <returns>1 if this is greater than other, 0 numbers are equal and -1 if other is greater than this</returns>
        public int CompareTo(LargeDecimal other)
        {
            int result = -1;
            var firstCopy = new LargeDecimal(this._mantiss, this._exponent);
            var secondCopy = new LargeDecimal(other._mantiss, other._exponent);

            int indFirst = FindNotZeroIndex();
            int indSecond = other.FindNotZeroIndex();

            if (indFirst != -1)
            {
                var newExpFirst = _exponent - indFirst;
                if (indSecond != -1)
                {
                    int newExpSecond = other._exponent - indSecond;

                    if (newExpFirst > newExpSecond)
                    {
                        result = 1;
                    }
                    else if (newExpFirst == newExpSecond)
                    {
                        Equalize(firstCopy, secondCopy);
                        result = firstCopy.CompareToByMantiss(secondCopy);
                    }
                }
                else
                {
                    result = 1;
                }
            }
            else
            {
                if (indSecond == -1)
                {
                    result = 0;
                }
            }

            return result;
        }

        private int CompareToByMantiss(LargeDecimal other)
        {
            int result = 0;

            for (int i = 0; i < MANTISS_LENGTH; i++)
            {
                result = _mantiss[i].CompareTo(other._mantiss[i]);
                if (result != 0) break;
            }
            return result;
        }
    }
}