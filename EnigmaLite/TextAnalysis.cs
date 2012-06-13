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
		
		/// <summary>
		/// Get the number of character substitions required to change original string to target.
		/// </summary>
		/// <returns>
		/// Number of substition steps required (-1 if not possible)
		/// </returns>
		/// <param name='targetStr'>
		/// Target string.
		/// </param>
		/// <param name='origStr'>
		/// Original string.
		/// </param>
		/// <param name='miniCipher'>
		/// Character substitions key.
		/// </param>
		public static int SubsRequired (string targetStr, string origStr, out Dictionary<char,char> miniCipher)
		{
			/// get letter histogram for each
			/// if histogram does not have the same shape, fail
			/// else count and return the number of non-identicals			
			
			// assign these first
			var tLen = targetStr.Length;
			miniCipher = new Dictionary<char, char> (tLen);
			
			// bomb out early			
			if (tLen != origStr.Length) {
				return -1;
			}
			
			var targ = targetStr.ToCharArray ();
			var orig = origStr.ToCharArray ();			
			var cipher = new Dictionary<char,char> (tLen);
			
			// loop through and build dictionary
			for (int i = 0; i < tLen; i++) {
																							
				// got it in the minicipher?
				try {
					cipher.Add (targ [i], orig [i]);
				} catch {
					// if it's already in the minicipher, is it the same?					
					if (cipher [targ [i]] != orig [i]) {
						return -1;
					}
				}				
			}						
						
			foreach (var kv in cipher) {
				if (kv.Key != kv.Value) {
					miniCipher.Add (kv.Key, kv.Value);
				}
			}
			
			// return number of changes
			return miniCipher.Count;
		}
		
		/// <summary>
		/// Get the number of character substitions required to change original string to target.
		/// </summary>
		/// <returns>
		/// Number of substition steps required (-1 if not possible)
		/// </returns>
		/// <param name='targetStr'>
		/// Target string.
		/// </param>
		/// <param name='origStr'>
		/// Original string.
		/// </param>
		public static int SubsRequired (string targetStr, string origStr)
		{
			Dictionary<char,char> waste;
			return SubsRequired (targetStr, origStr, out waste);
		}				
	
//		/// <summary>
//		/// Finds the best match (not inc. already-perfect matches) and subs letters as necessary
//		/// </summary>
//		/// <returns>
//		/// The original text with the new substitions
//		/// </returns>
//		/// <param name='origStr'>
//		/// Original string.
//		/// </param>
//		/// <param name='realWords'>
//		/// Real words.
//		/// </param> 

//		public static string SolveByMatching (string oneStep, Frequencies<string> freqs, out Dictionary<char,char> miniCipher)
//		{
//			/// Finds the closest match to the most frequent word (not including perfect matches). 
//			/// If multiple matches with equal closeness, the first.
//			/// If no match >50%, closest match to second most frequent word (and so on).	
//		
//			// from SubsRequired to a 'closeness' score
//			Func<int,string,double> matchScore = (x, y) => {
//				if (x < 0) {
//					return 0.0;
//				} else if (x == 0) {
//					return 1.0;
//				} else {
//					return 1.0 - x / (double)y.Length;
//				}				
//			};
//			
//			var origWords = oneStep.SplitByWords ();
//			miniCipher = new Dictionary<char, char> ();
//			
//			Dictionary<char,char> tempDict;
//			var highestMatchScore = 0.0;				
//			
//			foreach (var f in freqs.OrderedSingles) {
//				
//				foreach (var o in origWords) {
//					
//					var x = matchScore (SubsRequired (f.Key, o, out tempDict), o);
//					
//					if (x > highestMatchScore && x != 1.0) {
//						highestMatchScore = x;
//						miniCipher = tempDict;
//					}
//				}
//				// if highest matchScore is 0.5 <= mS < 1.0				
//				// break and use that miniCipher
//				if (highestMatchScore > 0.5 && highestMatchScore < 1.0) {
//					break;
//				}
//			}								
//			
//			foreach (var i in miniCipher) {
//				Console.Write ("--> ");
//				Console.WriteLine (i);
//			}
//			// returns the string after subbing those words
//			return "";
//		}
		
		public static Dictionary<char,char> ClosestMatch (IEnumerable<string> words, string word)
		{
			double hiScore = 0;
			Dictionary<char,char> miniCipher = new Dictionary<char, char> ();
			Dictionary<char,char> tempCipher;
			foreach (var w in words) {				
				var score = SubsRequired (w, word, out tempCipher);				
				if (score > 0) {
					var frac = 1.0 - score / (double)word.Length;
					if (frac > hiScore) {
						hiScore = frac;
						miniCipher = tempCipher;
					}
				}
			}
			return miniCipher;
		}
		
		public static IDictionary<char,char> MergeDicts (IDictionary<char,char> d1, IDictionary<char,char> d2)
		{
			var d3 = d1;
			
			var reverseD1 = new Dictionary<char,char> ();
			foreach (var kv in d1) {
				reverseD1.Add (kv.Value, kv.Key);
			}
			
			foreach (var kv in d2) {
								
				// does d1 contain the key?
				char v;
				var hasKey = d1.TryGetValue (kv.Key, out v);
				
				// does d1 contain the value?
				char k;
				var hasVal = reverseD1.TryGetValue (kv.Value, out k);
				
				if (hasKey && hasVal) {
					
					// set the key = value
					d3[kv.Key] = kv.Value;
					// give the key that formerly had that value, 
					// the former value of the key that has been updated
					d3[k] = v;
					// i.e. swap
					
				} else if (hasKey) {
					
					d3[kv.Key] = kv.Value;
					
				} else if (hasVal) {
					
					d3[k] = '*';
					d3.Add (kv.Key, kv.Value);
					
				} else {
					
					d3.Add (kv.Key, kv.Value);
					
				}						
			}
			
			return d3;
		}
	}
}

