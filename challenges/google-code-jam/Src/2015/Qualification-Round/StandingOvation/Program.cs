// Copyright (c) evilz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Github : https://github.com/evilz
//
// ### CODE JAM 2015 ###
// Qualification Round 2015 - Problem A. Standing Ovation : https://code.google.com/codejam/contest/6224486/dashboard#s=p0&a=2
// Analyse : https://code.google.com/codejam/contest/6224486/dashboard#s=a&a=0

using System;
using System.IO;
using System.Text;

namespace StandingOvation
{
    static class Program
    {
        private const string IN_EXTENSION = ".in";
        private const string OUT_EXTENSION = ".out";
        private const string OUT_PATH = "../../";

        private const string FILENAME = "sample";
        //private const string FILENAME = "small";
        //private const string FILENAME = "large";

        static void Main()
        {
            var inputStream = File.OpenText(FILENAME + IN_EXTENSION);
            var outputStream = File.CreateText(OUT_PATH + FILENAME + OUT_EXTENSION);

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
            Console.WriteLine(format, i, solution);
            writer.WriteLine(format, i, solution);
        }
        
        static int Solve(Input testCase)
        {
            var minInvite = 0;
            var t = 0;
            for (int k = 0; k <= testCase.Smax; k++)
            {
                minInvite = Math.Max(minInvite, k - t);
                t += (testCase.Audience[k] - '0');
            }
            return minInvite;
        }
    }

    #region MODELS
    public struct Input
    {
        public static Input Parse(TextReader reader)
        {
            return new Input
            {
                Smax = reader.NextNumber(),
                Audience = reader.NextString()
            };
        }

        /// <summary>
        /// Maximum shyness level of the shyest person in the audience
        /// </summary>
        public int Smax { get; private set; }

        /// <summary>
        /// Audience members 
        /// </summary>
        public string Audience { get; private set; }
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