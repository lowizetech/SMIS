using System;
using System.Collections.Generic;

/// <summary>
/// Overall, This Code Base Will Handle All Enum Field Input
/// Such As Student's Sex And Strand
/// It Is Also Has 2 Different Use Cases Depending On The Usage (add/update)
/// </summary>

namespace SMIS.utilities
{
    public static class TInput
    {
        public static T GetEnumInput<T>(
            string? prompt,
            Dictionary<string, string>? aliases = null,
            bool editMode = false,
            T? currentValue = default) where T : struct, Enum
        {
            aliases ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            while (true)
            {
                Console.Write(editMode && currentValue != null
                    ? $"\n{prompt} ({currentValue}): "
                    : $"\n{prompt}: ");

                string? input = Console.ReadLine()?.Trim().ToUpperInvariant();

                if (editMode && string.IsNullOrWhiteSpace(input) && currentValue != null)
                    return currentValue.Value;
                    
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"\nInvalid Input. {typeof(T).Name} cannot be empty.");
                    continue;
                }

                if (int.TryParse(input, out _))
                {
                    Console.WriteLine($"\nInvalid Input. Numeric Values Are Not Allowed For {typeof(T).Name}\n");
                    continue;
                }

                if (aliases.TryGetValue(input, out string? mappedValue))
                    input = mappedValue;

                if (Enum.TryParse(input, true, out T result))
                    return result;

                Console.WriteLine($"\nInvalid Input. Please Enter A Valid {typeof(T).Name}");
            }
        }
    }
}