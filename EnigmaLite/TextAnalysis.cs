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
		
//		public static IDictionary<char,char> MergeDict (IDictionary<char,char> d1, IDictionary<char,char> d2)
//		{
//			var d3 = d1;
//			
//			var inverseD1 = new Dictionary<char,char> ();
//			foreach (var kv in d1) {
//				Console.WriteLine ("> {0}", kv);
//				inverseD1.Add (kv.Value, kv.Key);
//			}	
//			
//			var inverseD2 = new Dictionary<char,char> ();
//			foreach (var kv in d2) {
//				inverseD2.Add (kv.Value, kv.Key);
//			}
//			
//			// example pt1
//			// d1 contains [U,u] and [i,t]
//			// d2 contains [u,t] - we want to make d3 contain [U,t] and [i,u]
//			// rd1 contains [u,U] and [t,i]
//			//
//			// example pt2
//			// d1 contains [i,t] and [e,h]
//			// d2 contains [t,h] - we want to make d3 contain [i,h] and [?,?]
//			// rd1 contains [t,i] and [h,e]
//			//
//			foreach (var kv in d2) {
//				Console.WriteLine ("--> {0}", kv);
//				
//				// kv = [u,t]
//				
//				var k = inverseD1 [kv.Key]; // k = U
//				
//				d3 [k] = kv.Value;	// d3[U] = t														
//				
//				Console.WriteLine ("{0}\tto become\t[{1}, {2}]", kv, k, d3 [k]);
//				
//				// rd1[t] = v = i
//				// d3[i] = u			
//				char v1, v2;
//				var hasD1Duplicate = inverseD1.TryGetValue (kv.Value, out v1);				
//				var hasD2KeyDuplicate = d2.TryGetValue (kv.Value, out v2);
//
//				Console.WriteLine ("{0}\thasD1Duplicate\t{1}", kv.Value, hasD1Duplicate);
//				
//				
//				if (hasD2KeyDuplicate) {
//					d2.Remove(kv.Value);
//					//d2.Add (?,v2);
//					// exception: modified the collection we iterate through
//				}
//				
//				if (hasD1Duplicate) {
//					Console.WriteLine (
//						"In d3:\t[{0},{1}]\tto become\t[{2},{3}]",
//						v1,
//						d3 [v1],
//						v1,
//						kv.Key
//					);
//					d3 [v1] = kv.Key;					
//					try {
//						var temp = d2 [v1];
//						d2.Remove (v1);
//						d2.Add (v1, temp);
//					} catch {
//						
//					}					
//				}				
//
//				
//				foreach (var z in d3) {
//					Console.WriteLine ("|{0}|", z);
//				}
//				// the bug is: the swapping action above can block subsequent d2 additions
//
//			}
//			foreach (var kv in d3) {
//				Console.WriteLine (kv);
//			}
//			return d3;
//		}				
	
		public static IDictionary<char,char> MergeDict (IDictionary<char,char> dOriginal, IDictionary<char,char> dChanges)
		{
			var dNew = dOriginal;
			
			var invOrig = InverseDictionary (dOriginal);
			var invChng = InverseDictionary (dChanges);
			
			foreach (var kv in dChanges) {
				// get and set Key in dOriginal that has Value = kv.Key
				var origKey = invOrig [kv.Key];
				dNew [origKey] = kv.Value;
				// if dNew already has a Key with Value = kv.Value
				// i.e. we now have a duplicate Value in dNew
				// find that Key:
				char dupKey;
				if (invOrig.TryGetValue (kv.Value, out dupKey)) {
					// and set its Value to...
					// a Value that is not in dOriginal or dChanges
					dNew [dupKey] = GetFreshKey (invOrig, invChng);
				}
			}				
			
			return dNew;
		}
		
		public static Dictionary<TValue,TKey> InverseDictionary<TValue,TKey> (IDictionary<TKey, TValue> original)
		{			
			var inv = new Dictionary<TValue,TKey> ();
			foreach (var kv in original) {
				inv.Add (kv.Value, kv.Key);
			}		
			return inv;
		}
		
		/// <summary>
		/// Gets a key not contained in either d1 or d2
		/// </summary>
		/// <returns>
		/// The fresh key.
		/// </returns>
		/// <param name='d1'>
		/// D1.
		/// </param>
		/// <param name='d2'>
		/// D2.
		/// </param>
		private static char GetFreshKey (IDictionary<char,char> d1, IDictionary<char,char> d2)
		{
			char newKey;
			var rand = new Random ();
			while (true) {
				newKey = (char)rand.Next (32, 128);
				if (!(d1.ContainsKey (newKey) && d2.ContainsKey (newKey))) {				
					break;									
				}				
			}
			return newKey;
		}
	}
}

