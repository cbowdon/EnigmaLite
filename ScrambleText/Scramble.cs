using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaLite;

namespace ScrambleText
{
    class Scramble
    {
        static void Main(string[] args)
        {
            var inFilename = args.Length > 0 ? args[0] : "DNA - Private Life Of Genghis Khan.txt";
            Console.WriteLine("Scrambling text in {0}...", inFilename);
            var inputText = File.ReadAllText(inFilename);            
            var gs = GenerateScramble(128);
            var outputText = inputText.SubChars(gs);
            var outFilename = string.Format("Scrambled {0}", inFilename);
            using (TextWriter t = new StreamWriter(outFilename))
            {
                t.Write(outputText);
            }
            Console.WriteLine("Output file: {0}", outFilename);
        }

        static Dictionary<char,char> GenerateScramble (int num)
        {            
            var gs = new Dictionary<char, char>();
            var r = new Random();
            var offset = r.Next(num+1);
            Func<int, int> getSub = x =>
            {
                var y = x + offset;
                return y > num ? y - num : y;                
            };

            for (int i = 0; i < num+1; i++)
            {
                gs.Add((char)i, (char)getSub(i));                
            }            
            return gs;
        }
    }
}
