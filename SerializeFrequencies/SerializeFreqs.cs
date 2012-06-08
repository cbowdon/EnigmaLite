using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using EnigmaLite;

namespace SerializeFrequencies
{
    class SerializeFreqs
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Serializing...");
            var inputFile = args.Length > 0 ? args[0] : "DNA - Private Life Of Genghis Khan.txt";
			SerializeWordsAndChars (inputFile);
            Console.WriteLine("Serialized!");			
        }        
		
		static void SerializeWordsAndChars (string inputFile)
		{
            string cleanText;
            IList<KeyValuePair<char, double>> charFreqs;
            IList<KeyValuePair<string, double>> wordFreqs;
          
            cleanText = File.ReadAllText(inputFile);

            wordFreqs = cleanText.SplitByWords().RankFrequency().OrderedSingles;
            charFreqs = cleanText.SplitByChars().RankFrequency().OrderedSingles;            

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

