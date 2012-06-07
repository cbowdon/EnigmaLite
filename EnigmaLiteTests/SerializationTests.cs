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
		IList<KeyValuePair<char,double>> charFreqs;
		IList<KeyValuePair<string,double>> wordFreqs;
		Frequencies<string> freqs;
		
		public SerializationTests ()
		{	
			shortStory = "DNA - Private Life Of Genghis Khan.txt";		
			cleanText = File.ReadAllText (shortStory);			
			
			freqs = cleanText.SplitByWords ().RankFrequency ();
			wordFreqs = cleanText.SplitByWords ().RankFrequency ().OrderedSingles;
			charFreqs = cleanText.SplitByChars ().RankFrequency ().OrderedSingles;			
			
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
		public void DeserializeWords ()
		{
			using (Stream stream = File.Open("words.bin", FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();

				var deserializedWords = (List<KeyValuePair<string,double>>)bin.Deserialize (stream);
				
				Assert.AreEqual (deserializedWords, wordFreqs);
			}
		}
		
		[Test]
		public void DeserializeChars ()
		{
			using (Stream stream = File.Open("chars.bin", FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();

				var deserializedChars = (List<KeyValuePair<char,double>>)bin.Deserialize (stream);
				
				Assert.AreEqual (deserializedChars, charFreqs);
			}
		}
		
		[Test()]
		public void SerializeFrequencies ()
		{
			using (Stream stream = File.Open("freqs.bin", FileMode.Create)) {
				BinaryFormatter bin = new BinaryFormatter ();
				bin.Serialize (stream, freqs);
			}
		}
		
		[Test()]
		public void DeSerializeFrequencies ()
		{
			using (Stream stream = File.Open("freqs.bin", FileMode.Open)) {
				BinaryFormatter bin = new BinaryFormatter ();
				var deserializedFreqs = (Frequencies<string>)bin.Deserialize (stream);

				Assert.AreEqual (deserializedFreqs.Singles.Count, freqs.Singles.Count);
				Assert.AreEqual (deserializedFreqs.Doubles.Count, freqs.Doubles.Count);
				foreach (var kv in deserializedFreqs.Singles) {
					Assert.AreEqual (kv.Value, freqs.Singles [kv.Key]);					
				}
				foreach (var kv in deserializedFreqs.Doubles) {
					Assert.AreEqual (kv.Value, freqs.Doubles [kv.Key]);					
				}
				for (int i = 0; i < deserializedFreqs.Singles.Count; i++) {
					Assert.AreEqual (
						deserializedFreqs.OrderedSingles[i],
						freqs.OrderedSingles[i]
					);
				}
				for (int i = 0; i < deserializedFreqs.Doubles.Count; i++) {
					Assert.AreEqual (
						deserializedFreqs.OrderedDoubles[i],
						freqs.OrderedDoubles[i]
					);
				}
			}
		}
	}
}

