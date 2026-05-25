// Copyright (c) evilz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Github : https://github.com/evilz
//
// ### CODE JAM 2015 ###
// Qualification Round 2015 - Problem C. Dijkstra : https://code.google.com/codejam/contest/6224486/dashboard#s=p2
// Analyse : https://code.google.com/codejam/contest/6224486/dashboard#s=a&a=2

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dijkstra
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

        private static void PrintCaseSolution(TextWriter writer, int i, string solution)
        {
            var format = "Case #{0}: {1}";
            Console.WriteLine(format, i, solution);
            writer.WriteLine(format, i, solution);
        }
        
        static string Solve(Input testCase)
        {
            // The multiplication matrix M is defined as a 5 x 5 matrix
            // where the first column and the first row are not used (for value 0).
            // The second row and the second column is for an identity value 1. 
            // The third, fourth and fifth (rows and columns) represent i, j, and k,
            // respectively, identical to the quaternion multiplication matrix.
            var M = new List<int[]>
            {
                new[] {0,  0,  0,  0,  0},
                new[] {0,  1,  2,  3,  4},
                new[] {0,  2, -1,  4, -3},
                new[] {0,  3, -4, -1,  2},
                new[] {0,  4,  3, -2, -1}
            }; 

            Func<int, int, int> mul = (a, b) =>
            {
                var sign = a*b > 0 ? 1 : -1;
                return sign*M[Math.Abs(a)][Math.Abs(b)];
            };

            Func<int[], int, int, int> multiplyAll = (s, l, x) =>
            {
                var value = 1;

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < l; j++)
                    {
                        value = mul(value, s[j]);
                    }
                }
                return value;
            };


            // We can improve it further to O(L).
            // Observe that the multiplication values (to itself)
            // will always repeat to the original value after 4 
            // consecutive multiplications, thus we only need to do
            // at most X mod 4 multiplications to compute value^X:
            Func<int, long, int> power = (a, n) =>
            {
                var value = 1;
                for (int i = 0; i < n % 4; i++)
                {
                    value = mul(value, a);
                }
                return value;
            };

            Func<int[], long, long, int> multiplyAllOpt = (s, l, x) =>
            {
                var value = 1;
                for (int i = 0; i < l; i++)
                {
                    value = mul(value, s[i]);
                }
                return power(value, x); //computes value^X
            };
            
            Func<int[], long, long, bool> constructFirstTwo = (s, l, x) =>
            {
                var iValue = 1;
                var jValue = 1;
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < l; j++)
                    {
                        if (iValue != 2) iValue = mul(iValue, s[j]);
                        else if (jValue != 3) jValue = mul(jValue, s[j]);
                    }
                }
                return iValue == 2 && jValue == 3;
            };

            // maps 'i' => 2, 'j' => 3, 'k' => 4
            var mappedS = testCase.S.Select(c => c - 'i' + 2).ToArray();


            // No solution if the whole string cannot be reduced to - 1
            // Suppose the string S is the whole concatenated string 
            // formed by repeating the given input string X times.
            // If we can break the string S into three non - empty substrings A, B, C
            // where A +B + C = S such that A reduces to i, B reduces to j, 
            // and C reduces to k, then the string S reduces to string "ijk",
            // which then reduces to -1.
            // Therefore, if the string S cannot be reduced to -1,
            // then there is no solution.
            // Reducing the string S can be done by simply multiplying 
            // all the characters in S into one value.
           
            // var canBeMinusOne = multiplyAll(mappedS, testCase.L, testCase.X) == -1;
            var canBeMinusOne = multiplyAllOpt(mappedS, testCase.L, testCase.X) == -1;
            

            // If the string S can be reduced to -1,
            // it doesn't mean that there is a solution for S.
            // There are many strings (e.g., "ii", "jj", etc.) 
            // that reduces to -1 but do not form a concatenation of 
            // three substrings that reduce to i, j, and k, respectively.
            // The first two substrings must reduce to i and j, respectively
            //
            //From now on, we only consider whole strings that reduce to -1.
            // To determine whether a string S can be broken into three non-empty
            // substrings A, B, C where each reduces to i, j, k respectively, 
            // we only need to find the first two substrings. The last substring 
            // is guaranteed to reduce to k since the whole string reduces to -1.
            //var canBeIJ = construct_first_two(mappedS, testCase.L, testCase.X);

            var canBeIJ = constructFirstTwo(mappedS, testCase.L, Math.Min(8, testCase.X));

            return canBeMinusOne && canBeIJ ? "YES" : "NO";
            
        }
    }

    #region MODELS
    public struct Input
    {
        public static Input Parse(TextReader reader)
        {
            return new Input
            {
                L = reader.NextNumber<long>(),
                X = reader.NextNumber<long>(),
                S = reader.NextString()
            };
        }

        /// <summary>
        /// Input length
        /// </summary>
        public long L { get; private set; }

        /// <summary>
        /// Input repeat times
        /// </summary>
        public long X { get; private set; }

        /// <summary>
        /// Input string of i j k
        /// </summary>
        public string S { get; private set; }
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