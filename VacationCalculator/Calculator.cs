using System;
using System.Collections.Generic;

namespace VacationCalculator;
 
public class Calculator
{
    private const int HoursPerDay = 8;
    private const int HoursPerShortDay = 7;

    public IDictionary<DateTime, decimal> Calculate(int year)
    {
        var firstDay = new DateTime(year, 1, 1);
        while (firstDay.DayOfWeek != DayOfWeek.Monday)
        {
            firstDay = firstDay.AddDays(1);
        }

        var vacationPeriods = new Dictionary<DateTime, decimal>();

        while (firstDay <= new DateTime(year, 12, 31))
        {
            var averageHourlySalary = GetAverageHourlySalaryFor6Months(firstDay);
            var salaryDifferenceDuringPeriod =
                GetSalaryDifferenceDuringPeriod(averageHourlySalary, firstDay, firstDay.AddDays(13));
            vacationPeriods.Add(firstDay, salaryDifferenceDuringPeriod);
            Console.WriteLine(
                $"Vacation start on {firstDay}, salary difference: {salaryDifferenceDuringPeriod * 100}%");
            firstDay = firstDay.AddDays(7);
        }

        return vacationPeriods;
    }

    private decimal GetAverageHourlySalaryFor6Months(DateTime firstDay)
    {
        var sixMonthsAgo = firstDay.AddMonths(-6);
        var currentDay = sixMonthsAgo;
        var totalHours = 0;
        var totalSalary = 0m;
        var totalHoursThisMonth = 0;
        while (currentDay < firstDay)
        {
            if (IsWorkingDay(currentDay))
            {
                var hoursToday = IsShortDay(currentDay) ? HoursPerShortDay : HoursPerDay;
                totalHoursThisMonth += hoursToday;
                totalHours += hoursToday;
            }

            var newWorkingDay = currentDay.AddDays(1);
            if (newWorkingDay.Month != currentDay.Month)
            {
                totalSalary += totalHoursThisMonth /
                               (decimal)Data.HoursPerMonth[currentDay.Year * 100 + currentDay.Month];
                totalHoursThisMonth = 0;
            }

            currentDay = newWorkingDay;
        }

        totalSalary += totalHoursThisMonth / (decimal)Data.HoursPerMonth[currentDay.Year * 100 + currentDay.Month];

        return totalSalary / totalHours;
    }

    private decimal GetSalaryDifferenceDuringPeriod(decimal averageSalaryPerHour, DateTime periodStart,
        DateTime periodEnd)
    {
        var currentDay = periodStart;
        var totalExpectedSalary = 1m;
        var totalSalary = 0m;
        var vacationHoursThisMonth = 0;
        int currentHoursPerMonth;
        int normalHours;
        while (currentDay <= periodEnd)
        {
            if (IsWorkingDay(currentDay))
            {
                var hoursToday = IsShortDay(currentDay) ? HoursPerShortDay : HoursPerDay;
                vacationHoursThisMonth += hoursToday;
            }

            var nextDay = currentDay.AddDays(1);
            if (nextDay.Month != currentDay.Month)
            {
                totalExpectedSalary += 1;
                currentHoursPerMonth = Data.HoursPerMonth[currentDay.Year * 100 + currentDay.Month];
                normalHours = currentHoursPerMonth - vacationHoursThisMonth;
                totalSalary += normalHours / (decimal)currentHoursPerMonth +
                               vacationHoursThisMonth * averageSalaryPerHour;
                vacationHoursThisMonth = 0;
            }

            currentDay = nextDay;
        }

        currentHoursPerMonth = Data.HoursPerMonth[currentDay.Year * 100 + currentDay.Month];
        normalHours = currentHoursPerMonth - vacationHoursThisMonth;
        totalSalary += normalHours / (decimal)currentHoursPerMonth + vacationHoursThisMonth * averageSalaryPerHour;

        return totalSalary / totalExpectedSalary;
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
