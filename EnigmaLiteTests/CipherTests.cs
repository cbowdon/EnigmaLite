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
		
		public CipherTests ()
		{
			// load some text
			cleanText = File.ReadAllText (shortStory);			
			
			// create basic substition cipher			
			cipher = new Dictionary<char, char> ();
			for (int i = 0; i < 255; i++) {
				cipher.Add ((char)i, (char)(i + 1));
			}
			cipher.Add ((char)255, (char)0); 
			
			crypted = cleanText.SubChars (cipher);			
		}
		
		[Test]
		public void Solver ()
		{
			// instantiating class should give first solution
			CipherSolver solver = new CipherSolver (crypted);			
			Assert.AreEqual (cleanText, solver.Solution, "exact same text");
			Assert.AreEqual (1.0, solver.SolutionScore, 1e-5, "perfect score");
			// should be able to mess with it
			solver.Cipher ['e'] = '!';
			Assert.AreNotEqual (cleanText, solver.Solution, "different text");
			Assert.Less (solver.SolutionScore, 1.0, "bad score");
			// and fix it
			solver.Cipher ['e'] = 'd';
			Assert.AreEqual (cleanText, solver.Solution, "exact same text again");
			Assert.AreEqual (1.0, solver.SolutionScore, 1e-5, "perfect score again");
		}
		
		[Test]
		public void ReSolve ()
		{
			int eventFired = 0;

			CipherSolver solver = new CipherSolver (crypted);			
			solver.SolutionUpdated += delegate(object sender, EventArgs e) {
				eventFired++;	
			};
			
			Assert.AreEqual (cleanText, solver.Solution, "exact same text");
			Assert.AreEqual (1.0, solver.SolutionScore, 1e-5, "perfect score");
			
			var before = solver.Cipher;
			
			Assert.AreEqual (0, eventFired);
			
			// sovle a new problem
			solver.Solve (cleanText);			
			Assert.AreEqual (1, eventFired);
			Assert.AreNotEqual (before, solver.Cipher);			
			var sol = solver.Solution;
			
			// should be able to mess with it
			solver.Cipher ['e'] = '!';
			Assert.AreNotEqual (sol, solver.Solution, "different solution");			
			Assert.AreEqual (2, eventFired);
		}
	}
}

