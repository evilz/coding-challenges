using System.IO;
using CounterCulture;

namespace ConsoleApplication
{
	public struct Input
	{
		public static Input Parse(TextReader reader)
		{
		    return new Input
		    {
		        NumberToSay = reader.NextNumber<long>()
		    };
		}

	    public long NumberToSay { get; set; }
	}
}