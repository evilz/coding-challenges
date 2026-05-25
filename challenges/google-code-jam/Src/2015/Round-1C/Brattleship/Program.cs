// Copyright (c) evilz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Github : https://github.com/evilz
// ### CODE JAM 2015 ###
// Round 1C - Problem B. Typewriter Monkey : https://code.google.com/codejam/contest/4244486/dashboard#s=p0

using System;
using System.IO;
using System.Text;

namespace Brattleship
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
                var solution = Solve(input);
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
        
        static long Solve(Input testCase)
        {
            // for each row without the ship try ship size
            var missedRows = (testCase.C / testCase.W) * (testCase.R - 1); 
            // for row with ship, same minus the first hit and then size of ship
            var finalRow = (testCase.C - 1) / testCase.W + testCase.W;
            var result = missedRows + finalRow;
            return result;
        }
    }

    #region MODELS
    public struct Input
    {
        public static Input Parse(TextReader reader)
        {
            return new Input
            {
                R = reader.NextNumber<int>(),
                C = reader.NextNumber<int>(),
                W = reader.NextNumber<int>()
            };
        }

        public int R { get; private set; }
        public int C { get; private set; }
        public int W { get; private set; }
    }

    #endregion

    #region SANNER HELPER

    public static class ScannerExtensions
    {
        private static T Parse<T>(this string sValue) where T : IConvertible
        {
            var type = typeof(T);
            var m = type.GetMethod("Parse", new[] { typeof(string) });
            return (T)m.Invoke(null, new object[] { sValue });
        }

        public static string NextString(this TextReader reader)
        {
            var sb = new StringBuilder();
            var lastChar = reader.Read();
            while (lastChar > -1)
            {
                if (lastChar.IsWhiteSpace())
                {
                    if (sb.Length > 0)
                    {
                        break;
                    }
                }
                else
                {
                    sb.Append((char)lastChar);
                }
                lastChar = reader.Read();
            }

            return sb.ToString();
        }

        public static int NextNumber(this TextReader reader)
        {
            var token = reader.NextString();
            return int.Parse(token);
        }

        public static T NextNumber<T>(this TextReader reader) where T : IConvertible
        {
            var token = reader.NextString();
            return (token.Parse<T>());
        }


        private static bool IsWhiteSpace(this int charcode)
        {
            return char.IsWhiteSpace((char)charcode);
        }
    }

    #endregion

}