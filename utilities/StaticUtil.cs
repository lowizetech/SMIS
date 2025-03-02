using System;
using System.Collections.Generic;

/// <summary>
/// Overall, This Code Base Will Handle Static Functionalities
/// Such As Capitalize First Letters In A Word/Sentence (str)
/// Or Has A Console Feature The Will Console.WriteLine() With Design
/// </summary>

namespace SMIS.utilities
{
    public static class StaticUtil
    {
        public static string Capitalize(ReadOnlySpan<char> input)
        {
            if (input.IsEmpty)
                return string.Empty;

            bool capitalizeNextChar = true;
            bool modified = false;
            char[]? buffer = null;

            for (int i = default; i < input.Length; i++)
            {
                char currentChar = input[i];
                
                if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                    capitalizeNextChar = true;

                else if (capitalizeNextChar && char.IsLower(currentChar))
                {
                    if (!modified)
                    {
                        buffer = input.ToArray();
                        modified = true;
                    }
                    buffer![i] = char.ToUpperInvariant(currentChar);
                    capitalizeNextChar = false;
                }
                else capitalizeNextChar = false;
            }
            return modified ? new string(buffer!) : input.ToString();
        }

        public static void Message(
            string? str,
            bool allCaps = false,
            ConsoleColor color = ConsoleColor.Red
        ) {
            if (string.IsNullOrWhiteSpace(str)) return;

            string returnStr = allCaps ? str.ToUpperInvariant() : Capitalize(str);
            
            int strLength = (str ?? "").Length;
            int winWidth = Console.WindowWidth;
            
            if (strLength >= winWidth)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"\n{returnStr}\n");
                Console.ResetColor();
                return;
            }

            int totalPad = winWidth - strLength;
            string centeredStr = returnStr.PadLeft(strLength + totalPad / 2).PadRight(winWidth);

            Console.ForegroundColor = color;
            Console.WriteLine($"\n{centeredStr}\n");
            Console.ResetColor();
        }
    }
}