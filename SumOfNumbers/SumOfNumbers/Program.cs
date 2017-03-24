using System;
using System.Collections.Generic;

namespace SumOfNumbers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var numbers = new List<LargeDecimal>();
            Console.WriteLine("Input exponential numbers, each on new line, for finishing input \"=\". \nRight input example: 1.01e15, 2.05e+25, 0.95e-10.");
            Console.WriteLine("Wrong input example: -1.0e-5, -1, 1e5, -1e5, etc.");
            var str = "";
            do
            {
                str = Console.ReadLine().Trim();

                if (str != "=")
                {
                    try
                    {
                        numbers.Add(LargeDecimal.FromString(str));
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Wrong number format, please input number in format: {Me(+-)N}. \nWhere M is mantiss, N - exponent.");
                        Console.WriteLine("Error details: " + e.Message);
                    }
                }
                else
                {
                    break;
                }
            } while (true);

            var result = LargeDecimal.Zero;

            foreach (var number in numbers)
            {
                result += number;
            }
            Console.WriteLine("Sum of digits is " + result);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}