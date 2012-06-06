using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaLite
{
    public interface IDecipherer
    {
        string Problem { get; }
        string Solution { get; }
        double SolutionScore { get; }
        CipherDictionary Cipher { get; set; }
        void Solve(string problem);
        event EventHandler SolutionUpdated;
    }
}
