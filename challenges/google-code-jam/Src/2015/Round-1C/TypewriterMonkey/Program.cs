// Copyright (c) evilz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Github : https://github.com/evilz
// ### CODE JAM 2015 ###
// Round 1C - Problem B. Typewriter Monkey : https://code.google.com/codejam/contest/4244486/dashboard#s=p1

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace B
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

        private static void PrintCaseSolution(TextWriter writer, int i, double solution)
        {
            var format = "Case #{0}: {1}";
            Console.WriteLine(format, i, solution);
            writer.WriteLine(format, i, solution);
        }
        
        static double Solve(Input testCase)
        {
            if (testCase.TargetWord.Any(l => !testCase.Keyboard.Contains(l)))
            {
                return 0;
            }
            if (testCase.S < testCase.TargetWord.Length)
                return 0;

            var keyCount = testCase.Keyboard
                .Distinct()
                .ToDictionary(  c => c,
                                c => testCase.Keyboard.Count(c1 => c1 == c)
                  );
            
            double probability = 1;
            for (int i = 0; i < testCase.L; i++)
            {
                probability *= keyCount[testCase.TargetWord[i]] / (double)testCase.K;
            }
            probability *= (testCase.S - testCase.L + 1);
            long max = 0;
            if (probability != 0)
            {
                max = calcmax(testCase.Keyboard, testCase.S, testCase.TargetWord);
            }
           
            var a = 1;
            var b = 1;

            var probByLetter = CreateAllProba(testCase);
            var result = 0.0;
            for (int i = 0; i < testCase.S; i++)
            {
                var begingProb = 1.0;
                for (int j = 0; j < testCase.L; j++)
                {
                    var c = testCase.TargetWord[j];
                    begingProb *=probByLetter[c];
                }

                result *= begingProb;
            }


            var minBananas = testCase.S/testCase.L;

            var r = minBananas - result  ;
          
            return r;
        }

        private static long calcmax(string keys, int S, string word)
        {
            string maxS = word;
            long count = 1;
            int starti = 1;
            while (maxS.Length < S)
            {
                int i = starti;
                int j = 0;
                while (i < maxS.Length && maxS[i] == word[j])
                {
                    i++;
                    j++;
                }
                if (i == maxS.Length)
                {
                    if (maxS.Length + word.Length - j <= S)
                    {
                        maxS += word.Substring(j);
                        count++;
                    }
                    else
                    {
                        return count;
                    }
                }
                starti++;
            }
            return count;
        }

        private static Dictionary<char,double> CreateAllProba(Input testCase)
        {
            var result = new Dictionary<char, double>();
            foreach (var l in testCase.TargetWord)
            {
                if(result.ContainsKey(l)) continue;
   
                var countOfL = testCase.Keyboard.Count(c => c == l);             
                result.Add(l,(double)countOfL/testCase.K);

            }
            return result;
        }
    }

    #region Models
    public struct Input
    {
        public static Input Parse(TextReader reader)
        {
            return new Input
            {
                K = reader.NextNumber<int>(),
                L = reader.NextNumber<int>(),
                S = reader.NextNumber<int>(),
                Keyboard = reader.NextString(),
                TargetWord = reader.NextString(),
            };
        }

        /// <summary>
        /// Number of key on keyboard
        /// </summary>
        public int K { get; set; }

        /// <summary>
        /// Target word length
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// Keypress count
        /// </summary>
        public int S { get; set; }

        /// <summary>
        /// Keyboard keys of uppercase English letters 
        /// </summary>
        public string Keyboard { get; set; }

        /// <summary>
        /// Uppercase English letters representing the target word.
        /// </summary>
        public string TargetWord { get; set; }
    }

    #endregion

    #region SANNER HELPER

    public static class ScannerExtensions
    {

        public static T Parse<T>(this string sValue) where T : IConvertible
        {
            var type = typeof(T);
            var m = type.GetMethod("Parse", new[] { typeof(string) });
            return (T)m.Invoke(null, new object[] { sValue });
        }

        /// <summary>
        /// Get next string
        /// 
        ///	 ## Example
        /// 
        ///		string input = "    the string  032423";
        ///		var reader = new StringReader(input);
        ///		var s1 = reader.NextString();
        ///
        /// </summary>
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

        public static T[] NextNumberArray<T>(this TextReader reader, int size) where T : IConvertible
        {
            T[] array = new T[size];
            for (int i = 0; i < size; i++)
            {
                T token = reader.NextNumber<T>();
                array[i] = token;
            }
            return array;
        }

        public static string[] NextStringArray<T>(this TextReader reader, int size)
        {
            string[] array = new string[size];
            for (int i = 0; i < size; i++)
            {
                string token = reader.NextString();
                array[i] = token;
            }
            return array;
        }



        public static bool IsWhiteSpace(this int charcode)
        {
            return Char.IsWhiteSpace((char)charcode);
        }

        public static bool IsWhiteSpace(this char charcode)
        {
            return Char.IsWhiteSpace(charcode);
        }
    }

    #endregion

}