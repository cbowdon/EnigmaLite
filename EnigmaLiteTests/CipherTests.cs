using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using EnigmaLite;

namespace EnigmaLiteTests
{
	public class CipherTests
	{
		protected string shortStory = "DNA - Private Life Of Genghis Khan.txt";		
		protected string cleanText;
		protected Dictionary<char,char> cipher;
		protected string crypted;
		
		public CipherTests()
		{
			// load some text
            cleanText = File.ReadAllText (shortStory);			
			
			// create basic substition cipher			
			cipher = new Dictionary<char, char> ();
			for (int i = 0; i < 255; i++) {
				cipher.Add ((char)i, (char)(i + 1));
			}
			cipher.Add ((char)255, (char)0); 
			
			crypted = cleanText.SubChars(cipher);			
		}
		
		
		[Test]
		public void CipherSolver ()
		{
			// instantiating class should give first solution
			CipherSolver solver = new CipherSolver(crypted);			
			Assert.AreEqual(cleanText, solver.Solution);
			Assert.AreEqual(1.0, solver.SolutionScore, 1e-5);
			// should be able to mess with it
			solver.Cipher['e'] = '!';
			Assert.AreNotEqual(cleanText, solver.Solution);
			Assert.Less(1.0, solver.SolutionScore);
			// and fix it
			solver.Cipher['e'] = 'f';
			Assert.AreEqual(cleanText, solver.Solution);
			Assert.AreEqual(1.0, solver.SolutionScore, 1e-5);
		}
	}
}

