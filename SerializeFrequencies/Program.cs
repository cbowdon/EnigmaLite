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
			string inputFile;
			if (args.Length > 0) {
				inputFile = args[0];
			} else {
				inputFile = "DNA - Private Life Of Genghis Khan.txt";
			}
            SerializeWordsAndChars (inputFile);
            Console.WriteLine("Serialized!");			
        }        
		
		static void SerializeWordsAndChars (string inputFile)
		{
            string cleanText;
            List<KeyValuePair<char, double>> charFreqs;
            List<KeyValuePair<string, double>> wordFreqs;
          
            cleanText = File.ReadAllText(inputFile);

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

