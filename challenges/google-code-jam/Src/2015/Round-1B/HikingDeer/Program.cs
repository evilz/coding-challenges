using System;
using System.IO;

namespace ConsoleApplication2
{
    static class Program
    {
        private const string IN_EXTENSION = ".in";
        private const string OUT_EXTENSION = ".out";

        private const string FILENAME = "sample";
        //private const string FILENAME = "small";
        //private const string FILENAME = "large";

        static void Main()
        {
            var inputStream = File.OpenText(FILENAME + IN_EXTENSION);
            var outputStream = File.CreateText(FILENAME + OUT_EXTENSION);

            var caseCount = inputStream.NextNumber();
            for (int i = 1; i <= caseCount; i++)
            {
                var input = Input.Parse(inputStream);
                var solution = Solve(input);
                PrintCaseSolution(outputStream, i, solution);
            }

            inputStream.Close();
            inputStream.Dispose();

            outputStream.Close();
            outputStream.Dispose();
        }

        private static void PrintCaseSolution(TextWriter writer, int i, int solution)
        {
            var format = "Case #{0}: {1}";
            Console.WriteLine(format, i + 1, solution);
            writer.WriteLine(format, i + 1, solution);
        }


        static int Solve(Input testCase)
        {
            return 0;
        }
    }

}