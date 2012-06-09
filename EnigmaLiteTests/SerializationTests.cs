using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using EnigmaLite;
using NUnit.Framework;

namespace EnigmaLiteTests
{
	[TestFixture()]
	public class SerializationTests
	{						
		string shortStory, cleanText;
		Frequencies<char> charFreqs;
		Frequencies<string> wordFreqs;
		
		public SerializationTests ()
		{	
			shortStory = "DNA - Private Life Of Genghis Khan.txt";		
			cleanText = File.ReadAllText (shortStory);			
						
			wordFreqs = cleanText.SplitByWords ().RankFrequency ();
			charFreqs = cleanText.SplitByChars ().RankFrequency ();			
			
			SerializeWordsAndChars ();
		}
		
		void SerializeWordsAndChars ()
		{			
			using (Stream stream = File.Open("chars.bin", FileMode.Create)) {
				BinaryFormatter bin = new BinaryFormatter ();
				bin.Serialize (stream, charFreqs);
			}
			
			using (Stream stream = File.Open("words.bin", FileMode.Create)) {
				BinaryFormatter bin = new BinaryFormatter ();
				bin.Serialize (stream, wordFreqs);
			}
		}
		
		[Test()]
		public void UnserializeWords ()
		{
			using (Stream stream = File.Open("words.bin", FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();

				var deserializedWords = (Frequencies<string>)bin.Deserialize (stream);
				
				CompareFrequencies (deserializedWords, wordFreqs);
			}
		}
		
		[Test]
		public void UnserializeChars ()
		{
			using (Stream stream = File.Open("chars.bin", FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();

				var deserializedChars = (Frequencies<char>)bin.Deserialize (stream);
				
				CompareFrequencies (deserializedChars, charFreqs);
			}
		}
		
		[Test()]
		public void SerializeFrequencies ()
		{
			using (Stream stream = File.Open("wordFreqs.bin", FileMode.Create)) {
				BinaryFormatter bin = new BinaryFormatter ();
				bin.Serialize (stream, wordFreqs);
			}
		}
		
		[Test()]
		/// "Un" instead of "De" because NUnit runs these in alphabetical order
        /// </laziness>
		public void UnserializeFrequencies ()
		{
			using (Stream stream = File.Open("wordFreqs.bin", FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();
				var deserializedwordFreqs = (Frequencies<string>)bin.Deserialize (stream);
				
				CompareFrequencies (deserializedwordFreqs, wordFreqs);
			}
		}
				
		public void CompareFrequencies<T> (Frequencies<T> f1, Frequencies<T> f2)
		{
			Assert.AreEqual (
				f1.Singles.Count,
				f2.Singles.Count
			);
			Assert.AreEqual (
				f1.Doubles.Count,
				f2.Doubles.Count
			);
			foreach (var kv in f1.Singles) {
				Assert.AreEqual (kv.Value, f2.Singles [kv.Key]);					
			}
			foreach (var kv in f1.Doubles) {
				Assert.AreEqual (kv.Value, f2.Doubles [kv.Key]);					
			}
			for (int i = 0; i < f1.Singles.Count; i++) {
				Assert.AreEqual (
						f1.OrderedSingles [i],
						f2.OrderedSingles [i]
				);
			}
			for (int i = 0; i < f1.Doubles.Count; i++) {
				Assert.AreEqual (
						f1.OrderedDoubles [i],
						f2.OrderedDoubles [i]
				);
			}
		}
	}
}

