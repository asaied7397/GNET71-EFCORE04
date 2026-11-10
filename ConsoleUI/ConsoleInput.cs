using System.Globalization;
using System.Net.Mail;

namespace NationalBank.ConsoleUI;

public static class ConsoleInput
{
    public static string ReadRequired(
        string prompt,
        int maxLength,
        Func<string, bool>? validator = null,
        string? validationMessage = null)
    {
        while (true)
        {
            Console.Write(prompt);

            string value = ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(value) ||
                value.Length > maxLength)
            {
                Console.WriteLine(
                    $"Required: enter 1-{maxLength} characters."
                );

                continue;
            }

            if (validator is not null && !validator(value))
            {
                Console.WriteLine(
                    validationMessage ??
                    "Invalid value. Please try again."
                );

                continue;
            }

            return value;
        }
    }

    public static int ReadInteger(
        string prompt,
        int minimum,
        int maximum = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);

            bool valid = int.TryParse(
                ReadLine(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int value
            );

            if (valid &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Enter a whole number between {minimum} and {maximum}."
            );
        }
    }

    public static DateTime ReadDateOfBirth()
    {
        while (true)
        {
            Console.Write(
                "Date of birth (yyyy-MM-dd): "
            );

            bool valid = DateTime.TryParseExact(
                ReadLine().Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date
            );

            if (valid && date.Date <= DateTime.Today)
            {
                return date.Date;
            }

            Console.WriteLine(
                "Enter a valid date in yyyy-MM-dd format, " +
                "not in the future."
            );
        }
    }

    public static T ReadEnum<T>(
        string title
    ) where T : struct, Enum
    {
        Console.WriteLine(title);

        foreach (T value in Enum.GetValues<T>())
        {
            Console.WriteLine(
                $"  {Convert.ToInt32(value)}) {value}"
            );
        }

        while (true)
        {
            int choice = ReadInteger(
                "Choice: ",
                1
            );

            if (Enum.IsDefined(typeof(T), choice))
            {
                return (T)Enum.ToObject(
                    typeof(T),
                    choice
                );
            }

            Console.WriteLine(
                "Choose one of the listed options."
            );
        }
    }

    public static string ReadEmail()
    {
        return ReadRequired(
            "Email: ",
            200,
            value =>
                !value.Any(char.IsWhiteSpace) &&
                MailAddress.TryCreate(value, out var address) &&
                address.Address == value,
            "Enter a valid email address, " +
            "for example name@example.com."
        );
    }

    public static string ReadPhone()
    {
        return ReadRequired(
            "Phone: ",
            20,
            value =>
            {
                string digits = value.StartsWith('+')
                    ? value[1..]
                    : value;

                return digits.Length >= 7 &&
                       digits.All(char.IsAsciiDigit);
            },
            "Use at least 7 digits, with an optional leading + " +
            "(maximum 20 characters)."
        );
    }

    public static string ReadNationalId()
    {
        return ReadRequired(
            "National ID: ",
            20,
            value => value.All(char.IsAsciiDigit),
            "National ID must contain digits only (maximum 20)."
        );
    }

    public static string ReadCode(
        string prompt,
        int maximum)
    {
        return ReadRequired(
            prompt,
            maximum,
            value => value.All(
                c => char.IsAsciiLetterOrDigit(c) ||
                     c == '-'
            ),
            "Use letters, digits and hyphens only."
        ).ToUpperInvariant();
    }

    public static void Pause()
    {
        Console.WriteLine(
            "\nPress any key to return to the menu..."
        );

        if (Console.IsInputRedirected)
        {
            ReadLine();
        }
        else
        {
            Console.ReadKey(intercept: true);
        }
    }

    private static string ReadLine()
    {
        return Console.ReadLine()
            ?? throw new OperationCanceledException(
                "Input stream was closed."
            );
    }
}