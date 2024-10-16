#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2024                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

using System;
using System.Collections.Generic;

namespace PiWeb.Api.Training.Events;

public static class ConsoleUI
{
    public static void WriteInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static string InputText(string header, string defaultValue = "")
    {
        Console.Write(header + (defaultValue.Length > 0 ? $" (optional, default: {defaultValue})" : "") + ": ");
        var input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
            return input;

        Console.WriteLine(defaultValue);
        return defaultValue;
    }

    public static T SelectOption<T>(string header, IList<T> options, Func<T, string>? toString = null)
    {
        if (options.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(options));

        Console.WriteLine($"{header}:");

        for (var i = 0; i < options.Count; i++)
            Console.WriteLine($"  ({i + 1})\t{toString?.Invoke(options[i]) ?? options[i]?.ToString()}");

        do
        {
            Console.Write("Option: ");

            var option = Console.ReadLine();

            if (int.TryParse(option, out var optionIndex) && optionIndex >= 1 && optionIndex <= options.Count)
                return options[optionIndex - 1];
            else
                WriteError("Invalid value");
        } while (true);
    }
}
