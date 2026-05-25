using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteHouseOfPancakes
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			int T = 0;
			var cases = new List<Tuple<int, List<int>>>();

			// parse input
			using (var stream = File.OpenText("large.in"))
			{
				T = stream.ExtractValues<int>().First();

				for (int i = 0; i < T; i++)
				{
					var diner = stream.ExtractValues<int>().First();
					var plates = stream.ExtractValues<int>().ToList();

					var testCase = new Tuple<int, List<int>>(diner, plates);
					cases.Add(testCase);

				}
			}

			// Find solution
			using (var sw = File.CreateText("large.out"))
			{
				for (int i = 0; i < T; i++)
				{
					var testCase = cases[i];
					var plates = testCase.Item2;
					var max = plates.Max();
					var solution = max;

					for (var target = 1; target <= max; target++)
					{
						var total = target;
						for (var j = 0; j < testCase.Item1; j++)
						{
							var currentPlate = plates[j];

							if (currentPlate <= target) continue;

							total += currentPlate/target;

							if (currentPlate%target == 0)
								total--;
						}
						solution = Math.Min(solution, total);
					}

					Console.WriteLine("Case #{0}: {1}", i + 1, solution);
					sw.WriteLine("Case #{0}: {1}", i + 1, solution);
				}
			}
		}
	}
}
