using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace EnigmaLite
{
	public class CipherSolver : IDecipherer
	{
		#region Protected properties		
		protected Frequencies<string> realWordFreqs { get; set; }

		protected Frequencies<char> realCharFreqs { get; set; }
		#endregion
		
		#region Public properties	
		public string Problem { get; protected set; }

		public string Solution { get; protected set; }

		public double SolutionScore { get; protected set; }

		// the only publicly set property
		public CipherDictionary Cipher { get; set; }

		protected string _realWordsFile = "words.bin";

		public string RealWordsFile {
			get {
				return _realWordsFile;
			}
			set {
				if (_realWordsFile != value) {
					_realWordsFile = value;
					DeserializeRealWords ();
					SolutionScore = TextAnalysis.ScoreSubd (
						Solution.SplitByWords (),
						realWordFreqs.Singles
					);
				}
			}
		}

		protected string _realCharsFile = "chars.bin";

		public string RealCharsFile {
			get {
				return _realCharsFile;
			}
			set {
				if (_realCharsFile != value) {
					_realCharsFile = value;
					DeserializeRealChars ();
					Solve (Problem);
				}
			}
		}
        #endregion
		
		#region Constructor
		public CipherSolver (string encryptedText)
		{
			DeserializeRealWords ();
			DeserializeRealChars ();
			Solve (encryptedText);			
		}
		#endregion
		
		#region Protected methods
		protected void DeserializeRealWords ()
		{
			using (Stream stream = File.Open(RealWordsFile, FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();
				realWordFreqs = (Frequencies<string>)bin.Deserialize (stream);				
			}
		}

		protected void DeserializeRealChars ()
		{
			using (Stream stream = File.Open(RealCharsFile, FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();
				realCharFreqs = (Frequencies<char>)bin.Deserialize (stream);				
			}
		}
						
		protected void SubAndScore ()
		{
			Solution = Problem.SubChars (Cipher);
			SolutionScore = TextAnalysis.ScoreSubd (
				Solution.SplitByWords (),
				realWordFreqs.Singles
			);
			if (SolutionUpdated != null) {
				SolutionUpdated.Invoke (this, new EventArgs ());
			}
		}		
		#endregion

        #region Public methods
		public void Solve (string problem)
		{
			// frequency solution
//			Problem = problem;
//			var chars = Problem.SplitByChars ();
//			var freqs = chars.RankFrequency ();			
//			Cipher = (CipherDictionary)TextAnalysis.SubsDict (
//				freqs,
//				realCharFreqs
//			);
//			SubAndScore ();
//			Cipher.ItemChanged += delegate(object sender, EventArgs e) {
//				SubAndScore ();
//			};			         
			
			MatchSolve (problem);
		}
		
		public void MatchSolve (string problem)
		{
			// set problem
			Problem = problem;
			// initial frequency analysis solution
			var chars = Problem.SplitByChars ();
			var freqs = chars.RankFrequency ();			
			Cipher = (CipherDictionary)TextAnalysis.SubsDict (
				freqs,
				realCharFreqs
			);
			SubAndScore ();
			
//			Console.WriteLine (Cipher.Count);
//			Cipher.Keys.ToList ().ForEach (x => Console.WriteLine ("{0}\t{1}", (int)x, (int)Cipher [x]));
			
			Console.WriteLine (Solution);

			Func<int,int> matchSolve = x => {
				// get closest match to a real word in the solution so far
				var modDict = TextAnalysis.ClosestMatch (
				Solution.SplitByWords (),
				realWordFreqs.OrderedSingles [x].Key
				);				
				// update the cipher dictionary such that the closest match is becomes a real word
				Cipher = (CipherDictionary)TextAnalysis.UpdateDict (Cipher, modDict);			
				SubAndScore ();
				Console.WriteLine (x);
				Console.WriteLine (Solution);
				return x;
			};
			
//			Console.WriteLine ("{0}<-----", realWordFreqs.OrderedSingles [0].Key);
//			
//			foreach (var kv in modDict) {
//				Console.WriteLine ("{0}\t{1}", kv, Cipher [kv.Key]);				
//			}			
//			

			for (int i = 0; i < 10; i++) {
				matchSolve (i);
			}
			
			SubAndScore ();
			
			Console.WriteLine (Solution);
			
			Cipher.ItemChanged += delegate(object sender, EventArgs e) {
				SubAndScore ();
			};
		}
        #endregion

        #region Events
		public event EventHandler SolutionUpdated;
        #endregion
	}
}

