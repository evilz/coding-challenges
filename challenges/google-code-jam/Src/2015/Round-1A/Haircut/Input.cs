using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haircut
{
	public struct Input
	{
		public int BarbersCount { get; private set; }
		public int Position { get; private set; }
		public List<int> BarbersTime { get; private set; }

		private Input(int barbersCount, int position, List<int> barbersTime)
		{
			BarbersCount = barbersCount;
			Position = position;
			BarbersTime = barbersTime;
		}


		public static List<Input> Parse(string path)
		{
			List<Input> list = new List<Input>();
			using (var stream = File.OpenText(path))
			{
				var caseCount = stream.ExtractValues<int>().First();

				for (int i = 0; i < caseCount; i++)
				{
					var BN = stream.ExtractValues<int>().ToList();
					var M = stream.ExtractValues<int>().ToList();

					var testCase = new Input(BN[0],BN[1], M);
					list.Add(testCase);
				}
			}
			return list;
		}
	}
}