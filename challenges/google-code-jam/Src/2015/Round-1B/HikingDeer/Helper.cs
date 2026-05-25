using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ConsoleApplication2
{

	public static class Helpers
	{
		public static IEnumerable<T> ExtractValues<T>(this StreamReader reader, char separator = ' ')
		{
			var line = reader.ReadLine();
			return line.ExtractValues<T>(separator);
		}


		public static T Convert<T>(this string input)
		{
			// NOT in PCL :(
			var converter = TypeDescriptor.GetConverter(typeof (T));
			//Cast ConvertFromString(string text) : object to (T)
			return (T) converter.ConvertFromString(input);
		}

		public static IEnumerable<T> ExtractValues<T>(this string input, char separator = ' ')
		{
			var splited = input.Split(separator);
			var result = splited.Select(Convert<T>);
			return result;
		}
	}
}