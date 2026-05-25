using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MushroomMonster
{
	public struct Input
	{
		public int NumberOfPlates { get; private set; }
		public List<int> Plates { get; private set; }
		
		private Input(int numberOfPlates, List<int> plates)
		{
			this.NumberOfPlates = numberOfPlates;
			this.Plates = plates;
		}

		public static List<Input> Parse(string path)
		{
			List<Input> list = new List<Input>();
			using (var stream = File.OpenText(path))
			{
				var caseCount = stream.ExtractValues<int>().First();

				for (int i = 0; i < caseCount; i++)
				{
					var numberOfPlates = stream.ExtractValues<int>().First();
					var plates = stream.ExtractValues<int>().ToList();

					var testCase = new Input(numberOfPlates, plates);
					list.Add(testCase);
				}
			}
			return list;
		}
	}
}