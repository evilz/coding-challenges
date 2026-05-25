using System.IO;

namespace ConsoleApplication2
{
	public struct Input
	{
	
		public static Input Parse(TextReader reader)
		{
		   var i = reader.NextNumber<int>();

            return new Input();
		}
	}
}