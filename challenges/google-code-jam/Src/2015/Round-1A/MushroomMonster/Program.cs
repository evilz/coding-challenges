using System;
using System.Collections.Generic;
using System.IO;
using MushroomMonster;

namespace SubRoundA
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
			List<Input> cases = Input.Parse(FILENAME + IN_EXTENSION);

			using (var sw = File.CreateText(FILENAME + OUT_EXTENSION))
			{
				for (int i = 0; i < cases.Count; i++)
				{
					var solution = Solve(cases[i]);
					Console.WriteLine("Case #{0}: {1} {2}", i + 1, solution.Item1,solution.Item2);
					sw.WriteLine("Case #{0}: {1} {2}", i + 1, solution.Item1, solution.Item2);
				}
			}
		}

		static Tuple<int,int> Solve(Input testCase)
		{
			// method 1
			var result1 = SolveMethod1(testCase);
			var result2 = SolveMethod2(testCase);
			return  new Tuple<int,int>(result1, result2);
		}

		private static int SolveMethod1(Input testCase)
		{
			int acc = 0;
			for (int i = 0; i < testCase.NumberOfPlates-1; i++)
			{
				var current = testCase.Plates[i];
				var next = testCase.Plates[i + 1];

				var calc = current - next;
				if (calc > 0)
				{
					acc += calc;
				}
				
			}
			return acc;
		}

		private static int SolveMethod2(Input testCase)
		{
			int acc = 0;
			int rest = 0;

			var speed = 0;

			for (int i = 0; i < testCase.NumberOfPlates - 1; i++)
			{
				var current = testCase.Plates[i];
				var next = testCase.Plates[i + 1];
				speed = Math.Max(speed, current - next);
			}

			for (int i = 0; i < testCase.NumberOfPlates - 1; i++)
			{
				var current = testCase.Plates[i];
				acc += Math.Min(current, speed);
				
			}
			return acc;
		}
	}
}