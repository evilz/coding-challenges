using System;
using System.IO;
using ConsoleApplication;

namespace CounterCulture
{
    static class Program
    {
        private const string IN_EXTENSION = ".in";
        private const string OUT_EXTENSION = ".out";

        //private const string FILENAME = "sample";
        //private const string FILENAME = "small";
        private const string FILENAME = "large";

        static void Main()
        {
            var inputStream = File.OpenText(FILENAME + IN_EXTENSION);
            var outputStream = File.CreateText(FILENAME + OUT_EXTENSION);

            var caseCount = inputStream.NextNumber();
            for (int i = 1; i <= caseCount; i++)
            {
                var input = Input.Parse(inputStream);
                var solution = Solve3(input);
                PrintCaseSolution(outputStream, i, solution);
            }

            inputStream.Close();
            inputStream.Dispose();

            outputStream.Close();
            outputStream.Dispose();
        }

        private static void PrintCaseSolution(TextWriter writer, int i, long solution)
        {
            var format = "Case #{0}: {1}";
            Console.WriteLine(format, i, solution);
            writer.WriteLine(format, i, solution);
        }

        public static long Reverse(long x)
        {
            long reverse = 0;
            while (x > 0)
            {
                var rem = x % 10;
                reverse = (reverse * 10) + rem;
                x = x / 10;

            }
            return reverse;
        }
        static long Solve3(Input testCase)
        {
            var target = testCase.NumberToSay;
            
            long pow = 1;
            while (pow * pow <= target) pow *= 10;

            long solution = 0;
            while (target >= 10)
            {
                while ((pow / 10) * (pow / 10) > target) pow /= 10;

                var rem = target % pow;
                if (rem == 0)
                {
                    solution++;
                    target--;
                }
                else
                {
                    solution += rem;
                    target -= rem - 1;
                    long reversed = Reverse(target);
                    if (target != reversed)
                    {
                        target = reversed;
                    }
                    else
                    {
                        target--;
                    }
                }
            }
            return solution + target;
        }
    }

}