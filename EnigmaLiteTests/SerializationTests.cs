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
		Frequencies<char> charwordFreqs;
		Frequencies<string> wordFreqs;
		
		public SerializationTests ()
		{	
			shortStory = "DNA - Private Life Of Genghis Khan.txt";		
			cleanText = File.ReadAllText (shortStory);			
						
			wordFreqs = cleanText.SplitByWords ().RankFrequency ();
			charwordFreqs = cleanText.SplitByChars ().RankFrequency ();			
			
			SerializeWordsAndChars ();
		}
		
		void SerializeWordsAndChars ()
		{			
			using (Stream stream = File.Open("chars.bin", FileMode.Create)) {
				BinaryFormatter bin = new BinaryFormatter ();
				bin.Serialize (stream, charwordFreqs);
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
				
				Assert.AreEqual (deserializedWords, wordFreqs);
			}
		}
		
		[Test]
		public void UnserializeChars ()
		{
			using (Stream stream = File.Open("chars.bin", FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();

				var deserializedChars = (Frequencies<char>)bin.Deserialize (stream);
				
				Assert.AreEqual (deserializedChars, charwordFreqs);
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

				Assert.AreEqual (deserializedwordFreqs.Singles.Count, wordFreqs.Singles.Count);
				Assert.AreEqual (deserializedwordFreqs.Doubles.Count, wordFreqs.Doubles.Count);
				foreach (var kv in deserializedwordFreqs.Singles) {
					Assert.AreEqual (kv.Value, wordFreqs.Singles [kv.Key]);					
				}
				foreach (var kv in deserializedwordFreqs.Doubles) {
					Assert.AreEqual (kv.Value, wordFreqs.Doubles [kv.Key]);					
				}
				for (int i = 0; i < deserializedwordFreqs.Singles.Count; i++) {
					Assert.AreEqual (
						deserializedwordFreqs.OrderedSingles[i],
						wordFreqs.OrderedSingles[i]
					);
				}
				for (int i = 0; i < deserializedwordFreqs.Doubles.Count; i++) {
					Assert.AreEqual (
						deserializedwordFreqs.OrderedDoubles[i],
						wordFreqs.OrderedDoubles[i]
					);
				}
			}
		}
	}
}

