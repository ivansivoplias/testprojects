using System;
using NUnit.Framework;
using SumOfNumbers;

namespace Tests
{
    [TestFixture]
    public class LargeDecimalTest
    {
        private LargeDecimal[] _decimals;
        private LargeDecimal _summ;

        [SetUp]
        public void Setup()
        {
            _decimals = new LargeDecimal[]
            {
                LargeDecimal.FromString("1.0e10"),
                LargeDecimal.FromString("1.01212e-5"),
                LargeDecimal.FromString("2.0212e15"),
                LargeDecimal.FromString("3.12e80")
            };

            _summ = LargeDecimal.FromString("3.12e80");
        }

        [Test]
        public void Constructor_StringIsEmpty_ThrowsArgumentException()
        {
            //Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => LargeDecimal.FromString(""));
        }

        [Test]
        public void Constructor_StringIsNumber_DoesNotThrowException()
        {
            //Arrange & Act & Assert
            Assert.DoesNotThrow(() => LargeDecimal.FromString("2.05e-5"));
        }

        [Test]
        public void AddNumbers_NumbersAreValid_SumIsRight()
        {
            //Arrange
            var sum = LargeDecimal.Zero;

            //Act
            for (int i = 0; i < _decimals.Length; i++)
            {
                sum += _decimals[i];
            }

            //Assert
            Assert.AreEqual(_summ.ToString(), sum.ToString());
        }

        [Test]
        public void CompareTo_NumbersAreEqual_ReturnZero()
        {
            //Arrange
            var first = LargeDecimal.FromString("1.015e25");
            var second = LargeDecimal.FromString("1.015e25");

            //Act
            var comparisonResult = first.CompareTo(second);

            //Assert
            Assert.AreEqual(0, comparisonResult);
        }

        [Test]
        public void CompareTo_NumberGreaterThanOther_ReturnOne()
        {
            //Arrange
            var first = LargeDecimal.FromString("5.2e25");
            var second = LargeDecimal.FromString("1.015e25");

            //Act
            var comparisonResult = first.CompareTo(second);

            //Assert
            Assert.AreEqual(1, comparisonResult);
        }

        [Test]
        public void CompareTo_NumberLowerThanOther_ReturnMinusOne()
        {
            //Arrange
            var first = LargeDecimal.FromString("1.212e25");
            var second = LargeDecimal.FromString("0.12121e26");

            //Act
            var comparisonResult = first.CompareTo(second);

            //Assert
            Assert.AreEqual(-1, comparisonResult);
        }

        [Category("Parse Incorrect Tests")]
        [TestCase("12.5e0")]
        [TestCase("323e-8")]
        public void TestCreateFromString(string input)
        {
            Assert.Throws<ArgumentException>(() => LargeDecimal.FromString(input));
        }
    }
}