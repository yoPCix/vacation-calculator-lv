using System;
using Xunit;
using FluentAssertions;

namespace VacationCalculator.Test
{
    public class CalculatorTest
    {
        [Theory]
        [InlineData(2021, 3, 1, "1.0506")]
        [InlineData(2021, 7, 26, "1.0145")]
        [InlineData(2020, 7, 6, "1.0538")]
        [InlineData(2020, 12, 28, "0.9872")]
        public void Calculate_ValidResult(int year, int month, int day, string expectedResultString)
        {
            var calculator = new Calculator();
            var result = calculator.Calculate(year);

            var expectedResult = decimal.Parse(expectedResultString);
            var dateToCheck = new DateTime(year, month, day);
            result.Should().ContainKey(dateToCheck);
            var firstMarchResult = result[dateToCheck];
            firstMarchResult.Should().BeApproximately(expectedResult, 4);
        }
    }
}
