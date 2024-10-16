using System;
using System.Collections.Generic;

namespace VacationCalculator;
 
public class Calculator
{
    private const int HoursPerDay = 8;
    private const int HoursPerShortDay = 7;

    public IDictionary<DateTime, decimal> Calculate(int year, int vacationDurationDays)
    {
        var firstDay = new DateTime(year, 1, 1);
        while (firstDay.DayOfWeek != DayOfWeek.Monday)
        {
            firstDay = firstDay.AddDays(1);
        }

        var vacationPeriods = new Dictionary<DateTime, decimal>();

        while (firstDay <= new DateTime(year, 12, 31))
        {
            var averageHourlySalary = GetAverageHourlySalaryFor6PreviousCalendarMonths(firstDay);
            var salaryDifferenceDuringPeriod =
                GetSalaryDifferenceDuringPeriod(averageHourlySalary, firstDay, firstDay.AddDays(vacationDurationDays - 1));
            vacationPeriods.Add(firstDay, salaryDifferenceDuringPeriod);
            Console.WriteLine(
                $"Vacation start on {firstDay}, salary difference: {salaryDifferenceDuringPeriod * 100}%");
            firstDay = firstDay.AddDays(7);
        }

        return vacationPeriods;
    }

    private decimal GetAverageHourlySalaryFor6PreviousCalendarMonths(DateTime firstDay)
    {
        var firstDayOfMonth = new DateTime(firstDay.Year, firstDay.Month, 1);
        var sevenMonthsAgo = firstDayOfMonth.AddMonths(-7);
        var totalHours = 0;
        for (var i = 0; i < 6; i++)
        {
            var currentMonth = sevenMonthsAgo.AddMonths(i);
            totalHours += Data.HoursPerMonth[currentMonth.Year * 100 + currentMonth.Month];
        }

        return 6m / totalHours;
    }
    
    private decimal GetSalaryDifferenceDuringPeriod(decimal averageSalaryPerHour, DateTime periodStart,
        DateTime periodEnd)
    {
        var totalSalary = 0m;
        var currentDay = periodStart;
        var workingDays = 0;
        while (currentDay <= periodEnd)
        {
            if (IsWorkingDay(currentDay))
            {
                workingDays++;
                var hoursToday = IsShortDay(currentDay) ? HoursPerShortDay : HoursPerDay;
                
                    var currentHoursPerMonth = Data.HoursPerMonth[currentDay.Year * 100 + currentDay.Month];
                    var normalHours = currentHoursPerMonth - hoursToday;
                    totalSalary += normalHours / (decimal)currentHoursPerMonth +
                                   hoursToday * averageSalaryPerHour;
            }

            currentDay = currentDay.AddDays(1);
        } 

        return totalSalary - workingDays + 1;
    }

    private bool IsWorkingDay(DateTime day)
    {
        return day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday &&
               !IsHoliday(day);
    }

    private bool IsHoliday(DateTime day)
    {
        return Data.Holidays.Contains(day);
    }

    private bool IsShortDay(DateTime day)
    {
        return Data.Holidays.Contains(day.AddDays(1));
    }
}
