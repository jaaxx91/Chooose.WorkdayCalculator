// See https://aka.ms/new-console-template for more information
using Chooose.WorkdayCalculator;

IWorkdayCalendar calendar = new WorkdayCalendar();

calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
calendar.SetRecurringHoliday(5, 17);
calendar.SetHoliday(new DateTime(2004, 5, 27));

var start = new DateTime(2004, 5, 24, 18, 5, 0);
decimal increment = -5.5m;
var incrementedDate = calendar.GetWorkdayIncrement(start, increment);


string format = "dd-MM-yyyy HH:mm";

Console.WriteLine(
start.ToString(format) +
" with an addition of " +
increment +
" work days is " +
incrementedDate.ToString(format));