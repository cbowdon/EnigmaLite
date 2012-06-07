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
		
		public SerializationTests ()
		{	
			shortStory = "DNA - Private Life Of Genghis Khan.txt";		
			cleanText = File.ReadAllText (shortStory);			
			
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
	}
}

