using Chooose.WorkdayCalculator.Helpers;

namespace Chooose.WorkdayCalculator
{
    public class WorkdayCalendar : IWorkdayCalendar
    {
        private int _startHours;
        private int _startMinutes;
        private int _stopHours;
        private int _stopMinutes;
        private TimeSpan _workDayTime;
        private TimeSpan _startTime;
        private TimeSpan _stopTime;
        private List<DateTime> _holidays;
        private List<(int month, int day)> _recurringHolidays;

        public WorkdayCalendar()
        {
            _holidays = new List<DateTime>();
            _recurringHolidays = new List<(int month, int day)>();

            _startHours = WorkdayCalendarConfiguration.Default.StartHour;
            _startMinutes = WorkdayCalendarConfiguration.Default.StartMinutes;
            _stopHours = WorkdayCalendarConfiguration.Default.StopHour;
            _stopMinutes = WorkdayCalendarConfiguration.Default.StopMinutes;

            _startTime = new TimeSpan(_startHours, _startMinutes, DateTime.MinValue.Second);
            _stopTime = new TimeSpan(_stopHours, _stopMinutes, DateTime.MinValue.Second);

            _workDayTime  = _stopTime.Subtract(_startTime);
        }

        public void SetHoliday(DateTime dateTime)
            => _holidays.Add(dateTime);

        public void SetRecurringHoliday(int month, int day)
            => _recurringHolidays.Add((month, day));

        public void SetWorkdayStartAndStop(
            int startHours,
            int startMinutes,
            int stopHours,
            int stopMinutes)
        {
            if (startHours < DateTime.MinValue.Hour ||
                startHours > DateTime.MaxValue.Hour ||
                startHours >= stopHours ||
                startMinutes < DateTime.MinValue.Minute ||
                startMinutes > DateTime.MaxValue.Minute ||
                stopHours < DateTime.MinValue.Hour ||
                stopHours > DateTime.MaxValue.Hour ||
                stopMinutes < DateTime.MinValue.Minute ||
                stopMinutes > DateTime.MaxValue.Minute)
            {
                throw new ArgumentException("Invalid workday start and stop time provided.");
            }

            _startTime = new TimeSpan(startHours, startMinutes, DateTime.MinValue.Second);
            _stopTime = new TimeSpan(stopHours, stopMinutes, DateTime.MinValue.Second);

            _workDayTime = _stopTime.Subtract(_startTime);

            _startHours = startHours;
            _startMinutes = startMinutes;
            _stopHours = stopHours;
            _stopMinutes = stopMinutes;
        }

        public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
        {
            (long numberOfDays, decimal fractionalDay) = incrementInWorkdays.SplitFractionalNumber();

            var moveForward = !numberOfDays.IsNegative();

            startDate = IncrementWholeDays(startDate, numberOfDays, moveForward);

            startDate = SetStartOrStopTimeIfOutsideOfWorkingHours(startDate);

            startDate = IncrementFractionalDay(startDate, fractionalDay, moveForward);

            startDate = AdjustTimeIfOutsideOfWorkingHours(startDate, moveForward);

            return startDate;
        }

        private DateTime IncrementWholeDays(
            DateTime date,
            long numberOfDays,
            bool moveForward)
        {
            if (numberOfDays.IsNegative())
            {
                numberOfDays = numberOfDays.ToPositive();
            }

            do
            {
                date = date.AddDays(moveForward ? 1 : -1);

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                if (_holidays.Any(h => h.Day == date.Day && h.Month == date.Month && h.Year == date.Year))
                {
                    continue;
                }

                if (_recurringHolidays.Any(h => h.day == date.Day && h.month == date.Month))
                {
                    continue;
                }

                numberOfDays--;

            } while (numberOfDays > 0);

            return date;
        }

        private DateTime SetStartOrStopTimeIfOutsideOfWorkingHours(DateTime date)
        {
            if (date.TimeOfDay.Subtract(_startTime).TotalMinutes < 0)
            {
                date = date.Date.Add(new TimeSpan(_startHours, _startMinutes, 0));
            }

            if (_stopTime.Subtract(date.TimeOfDay).TotalMinutes < 0)
            {
                date = date.Date.Add(new TimeSpan(_stopHours, _stopMinutes, 0));
            }

            return date;
        }

        private DateTime IncrementFractionalDay(
            DateTime date,
            decimal fractionalDay,
            bool moveForward)
        {
            var minutesToAdd = Convert.ToDecimal(_workDayTime.TotalMinutes) * fractionalDay;

            minutesToAdd = moveForward ? Math.Floor(minutesToAdd) : Math.Ceiling(minutesToAdd);

            date = date.AddMinutes(Convert.ToDouble(minutesToAdd));

            return date;
        }

        private DateTime AdjustTimeIfOutsideOfWorkingHours(DateTime date, bool moveForward)
        {
            if (moveForward)
            {
                var timeLeft = date.TimeOfDay.Subtract(new TimeSpan(_stopHours, _stopMinutes, 0));

                if (timeLeft.Hours > 0 || timeLeft.Minutes > 0)
                {
                    date = date.AddDays(1);
                    date = date.Date.Add(new TimeSpan(_startHours, _startMinutes, 0));
                    date = date.AddMinutes(timeLeft.TotalMinutes);
                }
            }
            else
            {
                var timeLeft = new TimeSpan(_startHours, _startMinutes, 0).Subtract(date.TimeOfDay);

                if (timeLeft.Hours > 0 || timeLeft.Minutes > 0)
                {
                    date = date.AddDays(-1);
                    date = date.Date.Add(new TimeSpan(_stopHours, _stopMinutes, 0));
                    date = date.AddMinutes(-timeLeft.TotalMinutes);
                }
            }

            return date;
        }
    }
}
