using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Dijkstra
{
	internal class ProgramSave
	{

		static readonly List<Tuple<string, string, bool>> _patterns = new List<Tuple<string, string, bool>>
						{

			// aa => -1
			new Tuple<string, string, bool>("^ii",string.Empty,true),
				new Tuple<string, string, bool>("^jj",string.Empty,true),
				new Tuple<string, string, bool>("^kk",string.Empty,true),

				new Tuple<string, string, bool>("^ij","k",false),
				new Tuple<string, string, bool>("^ik","j",true),
				new Tuple<string, string, bool>("^ji","k",true),
				new Tuple<string, string, bool>("^jk","i",false),
				new Tuple<string, string, bool>("^ki","j",false),
				new Tuple<string, string, bool>("^kj","i",true),

						};

		private static void Main2(string[] args)
		{
			
			


			int T = 0;
			var cases = new List<Tuple<long, long, string>>();

			// parse
			using (var stream = File.OpenText("large.in"))
			{

				T = stream.ExtractValues<int>().First();

				for (int i = 0; i < T; i++)
				{
					var l1 = stream.ExtractValues<long>().ToArray();
					var s = stream.ExtractValues<string>().First();
					var testCase = new Tuple<long, long, string>(l1[0], l1[1], s);
					cases.Add(testCase);
				}

			}

			using (var sw = File.CreateText("large.out"))
			{
				for (int i = 0; i < T; i++)
				{
					var testCase = cases[i];

					var repeat = testCase.Item2;
					var substring = testCase.Item3;

					//StringBuilder finalstring = new StringBuilder();
					//for (int j = 0; j < testCase.Item2; j++)
					//{
					//	finalstring.Append(testCase.Item3);
					//}

					//var s = finalstring.ToString();


					var startI = false;
					var thenJ = false;
					long neg = 0;
					var s = substring;
					repeat--;

					long reducedneg = 0;
					var reduced = ReduceToChar(s, out reducedneg);


					while (repeat > 0)
					{
						reduced = ReduceToChar(reduced + reduced, out reducedneg);
						repeat = repeat/2;

						if (reduced == "i" && !startI)
						{
							startI = true;
							repeat--;
						}
						if (startI && reduced == "j" && !thenJ)
						{
							thenJ = true;
							repeat--;
						}
						
					}

					string result = ((neg % 2 == 0) && startI && thenJ && reduced == "k") ? "YES" : "NO";

					
						Console.WriteLine("Case #{0}: {1} => {2}", i + 1, result, s);
						sw.WriteLine("Case #{0}: {1}", i + 1, result);
					
					
					
					//while (s.Length > 1)
					//{
					//	foreach (var p in _patterns)
					//	{
					//		if (s.StartsWith("i") && !startI)
					//		{
					//			startI = true;
					//			s = s.Substring(1);
					//			break;
					//		}
					//		if (startI && s.StartsWith("j") && !thenJ)
					//		{
					//			thenJ = true;
					//			s = s.Substring(1);
					//			break;
					//		}
					//		s = Matcher(p, s,ref neg);

					//		if (s.Length == 1 && repeat > 0)
					//		{
					//			s += reduced;
					//			repeat--;
					//		}
					//	}
					//}

					////string result = ((neg % 2 == 0) && s == "ijk") ? "YES" : "NO";
					//string result = ((neg % 2 == 0) && startI && thenJ && s == "k") ? "YES" : "NO";

					//Console.WriteLine("Case #{0}: {1} => {2}", i + 1, result, s);
					//sw.WriteLine("Case #{0}: {1}", i + 1, result);

				}
			}
			
		}

		private static string ReduceToChar(string s, out long neg)
		{
			neg = 0;
			while (s.Length > 1)
			{
				foreach (var p in _patterns)
				{
					s = Matcher(p, s, ref neg);
				}
			}
			return s;
		}

		private static string Matcher(Tuple<string, string, bool> p, string s,ref long neg)
		{
			var regex = new Regex(p.Item1);
			if (Regex.Matches(s, p.Item1).Count > 0)
			{
				s = regex.Replace(s, p.Item2, 1);
				if (p.Item3)
					neg++;
			}
			return s;
		}
	}
}
