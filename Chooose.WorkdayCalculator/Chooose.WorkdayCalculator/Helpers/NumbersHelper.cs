namespace Chooose.WorkdayCalculator.Helpers
{
    public static class NumbersHelper
    {
        public static (long wholeNumber, decimal decimalPart) SplitFractionalNumber(this decimal number)
        {
            var decimalPart = number % 1;

            var wholeNumber = Convert.ToInt64(number - decimalPart);

            return (wholeNumber, decimalPart);
        }

        public static bool IsNegative(this long number)
            => number < 0;

        public static long ToPositive(this long number)
            => number * -1;
    }
}
