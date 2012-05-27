using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace EnigmaLite
{
	public static class TextAnalysis
	{		
		public static List<string> SplitByWords (this string text)
		{
			var noNewLines = Regex.Replace (text, "\n|\r", " ");
			var noPunc = Regex.Replace (noNewLines, "[^A-Za-z\' ]", "");			
			var split = Regex.Split (noPunc, " +");
			return split.ToList ();			
		}
		
		public static List<char> SplitByChars (this string text)
		{			
			return text.ToCharArray ().ToList ();
		}
		
		/// <summary>
		/// Ranks the frequency of occurrence.
		/// N.B. This is case-sensitive.
		/// </summary>
		/// <returns>
		/// A list of KeyValue pairs of item and its normalised frequency 
		/// </returns>
		/// <param name='input'>
		/// Items to be frequency-ranked
		/// </param>
		/// <typeparam name='T'>
		/// Item type
		/// </typeparam>
		public static List<KeyValuePair<T,double>> RankFrequency<T> (this IEnumerable<T> input)
		{
			var frac = 1.0 / input.Count ();			
			var tally = new Dictionary<T, double> ();
			
			foreach (T i in input) {
				/// I think try catch is more efficient than
				/// if dict[i], dict[i] += 1
				/// because it searches only once
				try { 
					tally [i] += 1.0 * frac;
				} catch {
					tally.Add (i, 1.0 * frac);
				}
			}
			
			// the power of linq
			var sorted = from entry in tally orderby entry.Value descending select entry;
			
			return sorted.ToList ();
		}
		
		public static Dictionary<char, char> SubsDict (List<KeyValuePair<char,double>> cyph, List<KeyValuePair<char,double>> real)
		{
			var l1 = real.Count;
			var l2 = cyph.Count;
			
			var dict = new Dictionary<char,char> ();
			
			for (int i = 0; i < Math.Min(l1,l2); i++) {
				dict.Add (cyph [i].Key, real [i].Key);
			}
			
			return dict;
		}
		
		public static double ScoreSubd (List<string> deciphered, Dictionary<string,double> real)
		{
			var len = deciphered.Count;
			var sum = 0.0;
			for (int i = 0; i < len; i++) {
				if (real.ContainsKey (deciphered [i])) {
					sum += 1.0 / len;
				}
			}
			return sum;
		}
		
		public static string SubChars (this string str, Dictionary<char,char> dict)
		{
			throw new NotImplementedException ();
		}
	}
}
