using System;
using System.Collections.Generic;

namespace EnigmaLite
{
	public class CipherSolver
	{
		public string Problem { get; private set; }
		public string Solution { get; private set; }
		public Dictionary<char,char> Cipher { get; set; }
		public double SolutionScore { get; private set; }
		
		public CipherSolver (string encryptedText)
		{
			Problem = encryptedText;
			
		}
	}
}

