using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using EnigmaLite;

namespace SerializeFrequencies
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Serializing...");
            SerializeWordsAndChars ();
            Console.WriteLine("Serialized!");
        }        
		
		static void SerializeWordsAndChars ()
		{
            string shortStory, cleanText;
            List<KeyValuePair<char, double>> charFreqs;
            List<KeyValuePair<string, double>> wordFreqs;

            shortStory = "DNA - Private Life Of Genghis Khan.txt";
            cleanText = File.ReadAllText(shortStory);

            wordFreqs = cleanText.SplitByWords().RankFrequency();
            charFreqs = cleanText.SplitByChars().RankFrequency();            

			using (Stream stream = File.Open("chars.bin", FileMode.Create)) {
				BinaryFormatter bin = new BinaryFormatter ();
				bin.Serialize (stream, charFreqs);
			}
			
			using (Stream stream = File.Open("words.bin", FileMode.Create)) {
				BinaryFormatter bin = new BinaryFormatter ();
				bin.Serialize (stream, wordFreqs);
			}
		}
    }
}

