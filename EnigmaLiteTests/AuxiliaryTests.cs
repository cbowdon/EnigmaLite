using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using EnigmaLite;
using NUnit.Framework;

namespace EnigmaLiteTests
{
	[TestFixture()]
	public class AuxiliaryTests
	{
		protected string shortStory = "DNA - Private Life Of Genghis Khan.txt";
		protected string wordText = "Hello my name is John, I'm the batman in my spare time and I like to eat cabbage. The best thing about cabbage is the crunchiness.".ToLower ();
		protected string charText = "aaaaabbbbbbccccddddef... F;";
		protected string cyphered = "zzzzzeeeeeebbbbrrrrAsuuuCk!";
		protected Dictionary<string, double> realWordFreqs;
		
		[SetUp]
		public void SetUp ()
		{
			realWordFreqs = new Dictionary<string, double> ();
			realWordFreqs.Add ("the", 3.0 / 26);
			realWordFreqs.Add ("is", 2.0 / 26);
			realWordFreqs.Add ("my", 2.0 / 26);
			realWordFreqs.Add ("cabbage", 2.0 / 26);
			realWordFreqs.Add ("thing", 1.0 / 26);
			realWordFreqs.Add ("about", 1.0 / 26);
			realWordFreqs.Add ("name", 1.0 / 26);
			realWordFreqs.Add ("john", 1.0 / 26);
			realWordFreqs.Add ("best", 1.0 / 26);
			realWordFreqs.Add ("i'm", 1.0 / 26);
			realWordFreqs.Add ("and", 1.0 / 26);
			realWordFreqs.Add ("in", 1.0 / 26);
			realWordFreqs.Add ("time", 1.0 / 26);
			realWordFreqs.Add ("i", 1.0 / 26);
			realWordFreqs.Add ("eat", 1.0 / 26);
			realWordFreqs.Add ("batman", 1.0 / 26);
			realWordFreqs.Add ("hello", 1.0 / 26);
			realWordFreqs.Add ("crunchiness", 1.0 / 26);
			realWordFreqs.Add ("spare", 1.0 / 26);
			realWordFreqs.Add ("to", 1.0 / 26);
			realWordFreqs.Add ("like", 1.0 / 26);
		}
		
		[Test()]
		public void WordFreqs ()
		{
			List<string> words = wordText.SplitByWords ();
			Assert.AreEqual (26, words.Count);
			
			Assert.AreEqual (3196, File.ReadAllText (shortStory).SplitByWords ().Count);
			
			var freqs = words.RankFrequency ().OrderedSingles;
			Assert.AreEqual (realWordFreqs.Count, freqs.Count);			
			
			var sorted = (from r in realWordFreqs orderby r.Value descending select r).ToList ();			
			
			Assert.AreEqual (sorted [0].Key, freqs [0].Key);			
			
			for (int i = 0; i < sorted.Count; i++) {
				Assert.AreEqual (
					sorted [i].Value,
					freqs [i].Value,
					1e-5,
					string.Format ("{0}. differing frequency values", i)
				);
			}									
		}
		
		[Test()]
		public void CharFreqs ()
		{
			var chars = charText.SplitByChars ();
			Assert.AreEqual (charText.Length, chars.Count);
			var freqs = chars.RankFrequency ().OrderedSingles;
			
			Assert.AreEqual ('b', freqs [0].Key);
			Assert.AreEqual (6.0 / 27, freqs [0].Value, 1e-5);
			
			Assert.AreEqual ('a', freqs [1].Key);
			Assert.AreEqual (5.0 / 27, freqs [1].Value, 1e-5);
			
			Assert.AreEqual ('.', freqs [4].Key);
			Assert.AreEqual (3.0 / 27, freqs [4].Value);									
		}
		
		[Test()]
		public void SubsDict ()
		{
			var engFreqs = TextAnalysis.RankFrequency (charText).OrderedSingles;
			var cypFreqs = TextAnalysis.RankFrequency (cyphered).OrderedSingles;
			
			var ans = new Dictionary<char,char> ();
			ans.Add ('z', 'a');
			ans.Add ('e', 'b');
			ans.Add ('b', 'c');
			ans.Add ('r', 'd');
			ans.Add ('A', 'e');
			ans.Add ('s', 'f');
			ans.Add ('u', '.');
			ans.Add ('C', ' ');
			ans.Add ('k', 'F');
			ans.Add ('!', ';');
			
			var dict = TextAnalysis.SubsDict (cypFreqs, engFreqs);
			
			Assert.AreEqual (ans.Count, dict.Count);
			
			Assert.AreEqual (ans ['z'], dict ['z']);
			Assert.AreEqual (ans ['e'], dict ['e']);
			Assert.AreEqual (ans ['b'], dict ['b']);
			Assert.AreEqual (ans ['r'], dict ['r']);
			Assert.AreEqual (ans ['A'], dict ['A']);
			Assert.AreEqual (ans ['s'], dict ['s']);
			Assert.AreEqual (ans ['u'], dict ['u']);
			Assert.AreEqual (ans ['C'], dict ['C']);
			Assert.AreEqual (ans ['k'], dict ['k']);
			Assert.AreEqual (ans ['!'], dict ['!']);
		}
		
		[Test]
		public void ScoreSubd ()
		{
			var act = new List<string> ();
			act.Add ("the");			
			act.Add ("i");
			act.Add ("hello");
			act.Add ("is");
			act.Add ("my");
			act.Add ("i'm");
			act.Add ("batman");			
			act.Add ("name");
			act.Add ("john");
			act.Add ("spare");
			act.Add ("time");
			act.Add ("like");
			act.Add ("cabbage");
			act.Add ("eat");
			act.Add ("to");
			act.Add ("best");
			act.Add ("crunchiness");
			act.Add ("thing");
			act.Add ("about");
			
			foreach (var a in act) {
				Assert.IsTrue (realWordFreqs.ContainsKey (a));
			}			
			
			Assert.AreEqual (1.0, TextAnalysis.ScoreSubd (act, realWordFreqs), 1e-5);							
			
			var notAct = from a in act select string.Concat (a, "monkeyjockey");
			Assert.AreEqual (
				0.0,
				TextAnalysis.ScoreSubd (notAct.ToList (), realWordFreqs),
				1e-5
			);
		}
		
		[Test]
		public void GenghisKhan ()
		{
			var text = File.ReadAllText (shortStory);
			
			var chars = text.SplitByChars ();
			var words = text.SplitByWords ();
			
			var freqs = chars.RankFrequency ().OrderedSingles;
			
			// create basic substition cipher
			var cipher = new Dictionary<char, char> ();
			for (int i = 0; i < 255; i++) {
				cipher.Add ((char)i, (char)(i + 1));
			}
			cipher.Add ((char)255, (char)0); 
			
			var crypted = text.SubChars (cipher);
			var encryptedStory = "Encrypted DNA.txt";
			using (TextWriter tw = new StreamWriter(encryptedStory, false)) {
				tw.Write (crypted);
			}

			var cText = File.ReadAllText (encryptedStory);
			// ciphered text frequencies
			var ctf = cText.SplitByChars ().RankFrequency ().OrderedSingles; 
			
			var subsDict = TextAnalysis.SubsDict (ctf, freqs);
		
			// subsDict should be inverse of cipher
			foreach (var kv in subsDict) {
				Assert.AreEqual (kv.Key, cipher [kv.Value]);
			} 			
			
			var decrypted = crypted.SubChars (subsDict);
			var decryptedStory = "Decrypted DNA.txt";
			using (TextWriter tw = new StreamWriter(decryptedStory, false)) {
				tw.Write (decrypted);
			}
			
			// diff "Decrypted DNA" against the original story, expect no differences
			
			// score should be 1.0
			var deci = File.ReadAllText (decryptedStory).SplitByWords ();
			var real = words.RankFrequency ().Singles;
			var score = TextAnalysis.ScoreSubd (deci, real); 
			Assert.AreEqual (1.0, score, 1e-5);
		}
		
		[Test()]
		public void RankDoublesFrequency ()
		{			
			var textToRank = "Hello, my name is Frederick von Wolfenhausen, and I am a vacuum cleaner in the space-time continuum.";
			
			var singles = new Dictionary<char,double> ();
			#region singles actual answer
			singles.Add (' ', 0.16);
			singles.Add ('e', 0.11);
			singles.Add ('n', 0.09);
			singles.Add ('a', 0.08);
			singles.Add ('m', 0.06);
			singles.Add ('i', 0.05);
			singles.Add ('c', 0.05);
			singles.Add ('u', 0.05);
			singles.Add ('l', 0.04);
			singles.Add ('o', 0.04);
			singles.Add ('s', 0.03);
			singles.Add ('r', 0.03);
			singles.Add ('t', 0.03);
			singles.Add (',', 0.02);
			singles.Add ('d', 0.02);
			singles.Add ('v', 0.02);
			singles.Add ('h', 0.02);
			singles.Add ('H', 0.01);
			singles.Add ('y', 0.01);
			singles.Add ('F', 0.01);
			singles.Add ('k', 0.01);
			singles.Add ('W', 0.01);
			singles.Add ('f', 0.01);
			singles.Add ('I', 0.01);
			singles.Add ('p', 0.01);
			singles.Add ('-', 0.01);
			singles.Add ('.', 0.01);
			#endregion
			
			var doubles = new Dictionary<char,double> ();
			doubles.Add ('u', 2.0 / textToRank.Length);
			doubles.Add ('l', 1.0 / textToRank.Length);
			
			var ans = textToRank.RankFrequency ();
			
			Assert.AreEqual (singles.Count, ans.Singles.Count, "Single char frequency counts match");
			Assert.AreEqual (doubles.Count, ans.Doubles.Count, "Double char frequency counts match");
			
			Assert.AreEqual(ans.Doubles['u'], doubles['u']);
			Assert.AreEqual(ans.Doubles['l'], doubles['l']);
		}
	}
}

