using System;
using System.Collections.Generic;
using System.IO;

namespace NoisyNeighbors
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

        public static int AsInt(bool b)
        {
            return b ? 1 : 0;
        }


        static int Solve(Input testCase)
        {
            int ans1 = 0;
            int ans2 = 0;
		    var v = new List<int>();
            var w = new List<int>();
		
            for (int r = 0; r < testCase.Row; r++)
			    for (int c = 0; c < testCase.Columns; c++) {
				    int cur = 0;
				    for (int t = 0; t < 4; t++) {
					    int ni = r +  AsInt(t == 0) - AsInt (t == 1);

                        int nj = c + AsInt(t == 2) - AsInt(t == 3);

					    if (ni < 0 || nj < 0 || ni >= testCase.Row || nj >= testCase.Columns) continue;
					    cur++;
				    }
				    if ((r + c) % 2 == 0) v.Add(cur); else v.Add(0);
				    if ((r + c) % 2 == 1) w.Add(cur); else w.Add(0);
			    }
		    v.Sort();
		    w.Sort();
		    for (int i = 0; i < testCase.Tenants; i++) {
			    ans1 += v[i];
			    ans2 += w[i];
		    }

            return Math.Min(ans1, ans2);
        }
    }

}