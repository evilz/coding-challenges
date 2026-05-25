using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace InfiniteHouseOfPancakes
{

		public static class Helpers
		{
			public static Stream AsStream(this string input)
			{
				// convert string to stream
				byte[] byteArray = Encoding.UTF8.GetBytes(input);
				//byte[] byteArray = Encoding.ASCII.GetBytes(contents);
				MemoryStream stream = new MemoryStream(byteArray);
				return stream;
			}
			public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
			{
				//TODO : GUARD HERE argument null checking omitted
				foreach (T item in sequence) action(item);
			}

			public static IEnumerable<T> ExtractValues<T>(this StreamReader reader, char separator = ' ')
			{
				var line = reader.ReadLine();
				return line.ExtractValues<T>(separator);
			}

			public static T Convert<T>(this string input)
			{
				// NOT in PCL :(
				var converter = TypeDescriptor.GetConverter(typeof(T));
				//Cast ConvertFromString(string text) : object to (T)
				return (T)converter.ConvertFromString(input);
			}

			public static IEnumerable<T> ExtractValues<T>(this string input, char separator = ' ')
			{
				var splited = input.Split(separator);
				var result = splited.Select(Convert<T>);
				return result;
			}

		
			private static Random random = new Random();

			public static T SelectRandom<T>(this IEnumerable<T> sequence)
			{
				if (sequence == null) { throw new ArgumentNullException(); }
				if (!sequence.Any()) { throw new ArgumentException("The sequence is empty."); }

				//optimization for ICollection<T>
				if (sequence is ICollection<T>)
				{
					ICollection<T> col = (ICollection<T>)sequence;
					return col.ElementAt(random.Next(col.Count));
				}

				int count = 1;
				T selected = default(T);

				foreach (T element in sequence)
				{
					if (random.Next(count++) == 0)
					{
						//Select the current element with 1/count probability
						selected = element;
					}
				}

				return selected;
			}
		

			public static bool Between(this int x, int min, int max, bool exclusive = false)
			{
				if (exclusive)
					return x > min && x < max;
				return x >= min && x <= max;
			}
		}
	

}
