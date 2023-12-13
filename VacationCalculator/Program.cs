using System;
using System.Collections.Generic;
using System.IO;

namespace VacationCalculator;

public class Program
{
    public static void Main(string[] args)
    {
        var year = 2024;
        var calculator = new Calculator();
        var vacationPeriods = calculator.Calculate(year);
        GenerateCsv(year, vacationPeriods);
    }

    private static void GenerateCsv(int year, IDictionary<DateTime, decimal> vacationPeriods)
    {
        using var file = File.Open($"vacation_{year}.csv", FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(file);

        writer.Write($"{year};");
        for (var month = 1; month <= 12; month++)
        {
            writer.Write($"{month};");
        }

        writer.WriteLine();

        for (var day = 1; day <= 31; day++)
        {
            writer.Write($"{day};");
            for (var month = 1; month <= 12; month++)
            {
                if (day > DateTime.DaysInMonth(year, month) ||
                    !vacationPeriods.TryGetValue(new DateTime(year, month, day), out var impact))
                {
                    writer.Write(";");
                }
                else
                {
                    writer.Write($"{Math.Round(impact, 4)};");
                }
            }

            writer.WriteLine();
        }
    }
}
