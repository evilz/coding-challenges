using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Haircut
{
	static class Program
	{
		private const string IN_EXTENSION = ".in";
		private const string OUT_EXTENSION = ".out";

		//private const string FILENAME = "sample";
		private const string FILENAME = "small";
		//private const string FILENAME = "large";

		static void Main()
		{
			List<Input> cases = Input.Parse(FILENAME + IN_EXTENSION);

			using (var sw = File.CreateText(FILENAME + OUT_EXTENSION))
			{
				for (int i = 0; i < cases.Count; i++)
				{
					var solution = Solve(cases[i]);
					Console.WriteLine("Case #{0}: {1}", i + 1, solution);
					sw.WriteLine("Case #{0}: {1}", i + 1, solution);
				}
			}
		}



		static int Solve(Input testCase)
		{
			// index,  time, currentTime

			var barbers =
				testCase.BarbersTime.Select((time, index)=> new Barber {Index = index, Time = time, RestTime = time}).ToList();

			var min = barbers.Min(b => b.Time);


		//	int[] line = Enumerable.Range(0, testCase.Position).Select(i => -1).ToArray();

			var currentPosition = 0;
			var f = true;
			while (true)
			{
				var list = barbers.OrderBy(b => b.Time).ThenBy(b => b.Index).Where(b => b.Time == b.RestTime && b.Client == -1);
				Barber barber;
				//if (f)
				//{
				//	barber = barbers.OrderBy(b => b.Time).ThenBy(b => b.Index).FirstOrDefault(b => b.Time == b.RestTime && b.Client != -1);
				//	f = false;
				//}

				var result = 0;
				Parallel.ForEach(list, (barber1, state) =>
					//foreach (var barber1 in list)
				{
					barber1.Client = currentPosition;
					currentPosition++;
					//Console.WriteLine("barber {0} take client {1}", barber1.Index, barber1.Client);

					if (barber1.Client == testCase.Position - 1)
					{
						result = barber1.Index + 1;
						state.Stop();
						//return barber1.Index + 1;
					}
				});
				if (result != 0)
				{
					return result;
				}

						
				// RUN
				foreach (var b in barbers)
				{
					b.RestTime = b.RestTime - min;
					if (b.RestTime == 0)
					{
						b.RestTime = b.Time;
						b.Client = -1;
					}
				}

				
			}
			

		}
	}

	class Barber
	{
		public Barber()
		{
			Client = -1;
		}
		public int Index { get; set; }
		public int Time { get; set; }
		public int RestTime { get; set; }

		public int Client { get; set; }
	}
}