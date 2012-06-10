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
						Solution.SplitByWords(),
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
			Problem = problem;
			var chars = Problem.SplitByChars ();
			var freqs = chars.RankFrequency ();			
			Cipher = (CipherDictionary)TextAnalysis.SubsDict (
				freqs,
				realCharFreqs
			);
			SubAndScore ();
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

