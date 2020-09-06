using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Numeric;

namespace XamarinToolbox.UnitTest.FluentExtensions
{
    public static class NumericAssertionsExtensions
    {
        public static AndConstraint<NumericAssertions<int>> BeEven(this NumericAssertions<int> num, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(num.Subject % 2 == 0)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:value} to be event, but it was odd");

            return new AndConstraint<NumericAssertions<int>>(num);
        }

        public static AndConstraint<NumericAssertions<int>> BeOdd(this NumericAssertions<int> num, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(num.Subject % 2 == 1)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:value} to be event, but it was odd");

            return new AndConstraint<NumericAssertions<int>>(num);
        }
    }
}