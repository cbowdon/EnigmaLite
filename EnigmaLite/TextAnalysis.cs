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
		/// Ranks the frequency of item occurence in an IEnumerable.
		/// </summary>
		/// <returns>
		/// Frequency object
		/// </returns>
		/// <param name='input'>
		/// Input.
		/// </param>
		/// <typeparam name='T'>
		/// The item type.
		/// </typeparam>
		public static Frequencies<T> RankFrequency<T> (this IEnumerable<T> input)
		{
			var frac = 1.0 / input.Count ();			
			var singles = new Dictionary<T, double> ();			
			var doubles = new Dictionary<T, double> ();
			
			T prev = default(T);
			var first = true;
			foreach (T i in input) {
				/// I think try catch is more efficient than
				/// if dict[i], dict[i] += 1
				/// because it searches only once
				try { 
					singles [i] += 1.0 * frac;
				} catch {
					singles.Add (i, 1.0 * frac);
				}
				
				// include doubles
				if (i.Equals (prev) && !first) {
					try {
						doubles [i] += 1.0 * frac;
					} catch {
						doubles.Add (i, 1.0 * frac);
					}
				}
				// carry it forward
				prev = i;
				
				// avoid unnecessarily counting the default(T) twice
				if (first) {
					first = false;
				}
			}
									
			return new Frequencies<T> (singles, doubles);
		}
		
		/// <summary>
		/// IEnumerable<KeyValuePair> to Dict. 
		/// </summary>
		/// <returns>
		/// The dict.
		/// </returns>
		/// <param name='input'>
		/// Input.
		/// </param>
		public static Dictionary<string,double> ToDict (this IEnumerable<KeyValuePair<string,double>> input)
		{
			var dict = new Dictionary<string,double> (input.Count ());
			foreach (var i in input) {
				try {
					dict.Add (i.Key, i.Value);
				} catch {
					// do nothing	
				}				 
			}
			return dict;
		}
		
        /// <summary>
        /// Generate a substitution dictionary (char to char)
        /// </summary>
        /// <param name="cyph"></param>
        /// <param name="real"></param>
        /// <returns></returns>
		public static CipherDictionary SubsDict (Frequencies<char> cyph, Frequencies<char> real)
		{
			var l1 = real.Singles.Count;
			var l2 = cyph.Singles.Count;
			
			var dict = new CipherDictionary ();
			
			for (int i = 0; i < Math.Min(l1,l2); i++) {
				dict.Add (cyph.OrderedSingles [i].Key, real.OrderedSingles [i].Key);
			}
			
			return dict;
		}	
		
		/// <summary>
		/// Fraction of words in the decipher-attempt that are real words
		/// </summary>
		/// <param name="deciphered">List of words in deciphered text</param>
		/// <param name="real">Dictionary of real words and their frequencies</param>
		/// <returns>Score between 0.0 and 1.0</returns>
		public static double ScoreSubd (IList<string> deciphered, IDictionary<string,double> real)
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
		
		/// <summary>
		/// Apply substition dictionary to string.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="dict"></param>
		/// <returns></returns>
		public static string SubChars (this string str, IDictionary<char,char> dict)
		{
			var allChars = str.ToCharArray ();
			var len = allChars.Length;
			var newChars = new char[len];
			for (int i = 0; i < len; i++) {
				try {
					newChars [i] = dict [allChars [i]];
				} catch {
					newChars [i] = '*';
				}
			}			
			return new String (newChars);
		}
	}
}

