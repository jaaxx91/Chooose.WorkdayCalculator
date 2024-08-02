namespace Chooose.WorkdayCalculator.Tests
{
    internal class WorkdayCalendarTests
    {
        private IWorkdayCalendar _workdayCalendar;

        [SetUp]
        public void Setup()
        {
            _workdayCalendar = new WorkdayCalendar();
            _workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
            _workdayCalendar.SetRecurringHoliday(5, 17);
            _workdayCalendar.SetHoliday(new DateTime(2004, 5, 27));
        }

        [TestCaseSource(nameof(_testCases))]
        public void ShouldReturnValidWorkDay_WhenIncrementIsNegative_Returns(TestCase testCase)
        {
            // Act
            var incrementedDate = _workdayCalendar.GetWorkdayIncrement(testCase.StartDate, testCase.Increment);

            // Assert
            Assert.That(testCase.ExpectedWorkDay == incrementedDate);
        }

        private static List<TestCase> _testCases = new List<TestCase>
        {
            new TestCase
            {
                ExpectedWorkDay = new DateTime(2004, 05, 14, 12, 0, 0),
                StartDate = new DateTime(2004, 5, 24, 18, 5, 0),
                Increment = -5.5m
            },
            new TestCase
            {
                ExpectedWorkDay = new DateTime(2004, 07, 27, 13, 47, 0),
                StartDate = new DateTime(2004, 5, 24, 19, 3, 0),
                Increment = 44.723656m
            },
            new TestCase
            {
                ExpectedWorkDay = new DateTime(2004, 5, 13, 10, 2, 0),
                StartDate = new DateTime(2004, 5, 24, 18, 3, 0),
                Increment = -6.7470217m
            },
            new TestCase
            {
                ExpectedWorkDay = new DateTime(2004, 6, 10, 14, 18, 0),
                StartDate = new DateTime(2004, 5, 24, 8, 3, 0),
                Increment = 12.782709m
            },
            new TestCase
            {
                ExpectedWorkDay = new DateTime(2004, 6, 4, 10, 12, 0),
                StartDate = new DateTime(2004, 5, 24, 7, 3, 0),
                Increment = 8.276628m
            }
        };
    }
}