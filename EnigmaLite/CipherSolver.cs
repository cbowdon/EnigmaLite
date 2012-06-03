using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace EnigmaLite
{
	public class CipherSolver
	{
		#region Protected properties
		protected string realWordsFile { get; set; }

		protected string realCharsFile { get; set; }

		protected Dictionary<string,double> realWordFreqs { get; set; }

		protected List<KeyValuePair<char,double>> realCharFreqs { get; set; }
		#endregion
		
		#region Public properties	
		public string Problem { get; protected set; }

		public string Solution { get; protected set; }

		// the only publicly set property
		public CipherDictionary Cipher { get; set; }
		
		public double SolutionScore { get; protected set; }
		#endregion
		
		#region Constructor
		public CipherSolver (string encryptedText)
		{
			Problem = encryptedText;			
			realWordsFile = "words.bin";
			realCharsFile = "chars.bin";
			DeserializeRealFreqs ();
			Solve ();
			Cipher.ItemChanged += delegate(object sender, EventArgs e) { SubAndScore(); };			
		}
		#endregion
		
		#region Protected methods
		protected void DeserializeRealFreqs ()
		{
			using (Stream stream = File.Open(realWordsFile, FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();
				var deserializedWords = (List<KeyValuePair<string,double>>)bin.Deserialize (stream);
				realWordFreqs = deserializedWords.ToDict ();
			}
			
			using (Stream stream = File.Open(realCharsFile, FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();
				realCharFreqs = (List<KeyValuePair<char,double>>)bin.Deserialize (stream);				
			}
		}
		
		protected void Solve ()
		{
			var chars = Problem.SplitByChars ();
			var freqs = chars.RankFrequency ();
			Cipher = (CipherDictionary)TextAnalysis.SubsDict (freqs, realCharFreqs);
			SubAndScore ();
		}
		
		protected void SubAndScore ()
		{
			Solution = Problem.SubChars (Cipher);
			SolutionScore = TextAnalysis.ScoreSubd (
				Solution.SplitByWords (),
				realWordFreqs
			);
		}
		#endregion
	}
}

