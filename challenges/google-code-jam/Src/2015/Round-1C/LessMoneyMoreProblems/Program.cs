using System;
using System.IO;
using System.Text;

namespace C
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
            return 0;
        }
    }

    #region Models
    public struct Input
    {
        public static Input Parse(TextReader reader)
        {
            return new Input
            {
                C = reader.NextNumber<int>(),
                D = reader.NextNumber<int>(),
                V = reader.NextNumber<long>()
            };
        }

        public int C { get; set; }
        public int D { get; set; }
        public long V { get; set; }
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