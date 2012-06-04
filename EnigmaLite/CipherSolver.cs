using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace EnigmaLite
{
	public class CipherSolver
	{
		#region Protected properties		
		protected Dictionary<string,double> realWordFreqs { get; set; }

		protected List<KeyValuePair<char,double>> realCharFreqs { get; set; }
		#endregion
		
		#region Public properties	
		public string Problem { get; protected set; }

		public string Solution { get; protected set; }

        public double SolutionScore { get; protected set; }

        // the only publicly set property
		public CipherDictionary Cipher { get; set; }

        protected string _realWordsFile = "words.bin";
        public string RealWordsFile
        {
            get
            {
                return _realWordsFile;
            }
            set
            {
                if (_realWordsFile != value)
                {
                    _realWordsFile = value;
                    DeserializeRealWords ();
                    SolutionScore = TextAnalysis.ScoreSubd(Solution.SplitByWords(), realWordFreqs);
                }
            }
        }

        protected string _realCharsFile = "chars.bin";
        public string RealCharsFile
        {
            get
            {
                return _realCharsFile;
            }
            set
            {
                if (_realCharsFile != value)
                {
                    _realCharsFile = value;
                    DeserializeRealChars();
                    Solve ();
                }
            }
        }
        #endregion
		
		#region Constructor
		public CipherSolver (string encryptedText)
		{
			Problem = encryptedText;			
			DeserializeRealWords ();
            DeserializeRealChars ();
			Solve ();
			Cipher.ItemChanged += delegate(object sender, EventArgs e) { SubAndScore(); };			
		}
		#endregion
		
		#region Protected methods
		protected void DeserializeRealWords ()
		{
			using (Stream stream = File.Open(RealWordsFile, FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();
				var deserializedWords = (List<KeyValuePair<string,double>>)bin.Deserialize (stream);
				realWordFreqs = deserializedWords.ToDict ();
			}
        }
        protected void DeserializeRealChars ()
        {
			using (Stream stream = File.Open(RealCharsFile, FileMode.Open)) {
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
            if (SolutionUpdated != null)
            {
                SolutionUpdated.Invoke(this, new EventArgs());
            }
		}
		#endregion

        #region Events
        public event EventHandler SolutionUpdated;
        #endregion
    }
}

