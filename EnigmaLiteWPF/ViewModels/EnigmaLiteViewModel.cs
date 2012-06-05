using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EnigmaLite;

namespace EnigmaLiteWPF.ViewModels
{
    class EnigmaLiteViewModel : ObservableObject
    {
        #region Protected properties
        protected CipherSolver cipherSolver;
        #endregion

        #region Public properties
        protected string _problemText = "Input the ciphered text on the left.";
        public string ProblemText
        {
            get
            {
                return _problemText;
            }
            set
            {
                if (_problemText != value)
                {
                    _problemText = value;                    
                    RaisePropertyChanged("ProblemText");                    
                }
            }
        }

        protected string _solutionText;
        public string SolutionText
        {
            get 
            {
                if (_solutionText == null)
                {
                    _solutionText = cipherSolver.Solution;
                }
                return _solutionText; 
            }
            set
            {
                if (_solutionText != value)
                {
                    _solutionText = value;
                    RaisePropertyChanged("SolutionText");
                }
            }
        }

        public CipherDictionaryViewModel CDVM { get; set; }

        protected string _score;
        public string Score
        {
            get
            {
                return _score;
            }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    RaisePropertyChanged("Score");
                }
            }
        }

        protected string statusMessage = "";
        public string StatusMessage {
            get
            {
                return statusMessage;
            }
            set
            {
                if (statusMessage != value)
                {
                    statusMessage = value;
                    RaisePropertyChanged("StatusMessage");
                }
            }
        }
        #endregion
        
        #region Constructor
        public EnigmaLiteViewModel()
        {
            // N.B. "TargetInvocationException" may refer to a NullReferenceException
            // i.e. don't forget to instantiate stuff
            SetNewProblem(ProblemText);
            
            //CDVM = new CipherDictionaryViewModel(cipherSolver.Cipher);
            // Set commands
            DecipherText = new RelayCommand<object>(x => SetNewProblem(ProblemText));
        }
        #endregion

        #region Commands
        public RelayCommand<object> DecipherText { get; private set; }
        #endregion

        #region Private methods
        void SetNewProblem(string newProblemText)
        {
            StatusMessage = "Deciphering...";
            if (cipherSolver == null)
            {
                cipherSolver = new CipherSolver(newProblemText);
            }
            else
            {
                cipherSolver.Solve(newProblemText);
            }
            OnSolutionUpdated(this, new EventArgs());            
            cipherSolver.SolutionUpdated += new EventHandler(OnSolutionUpdated);
            CDVM = new CipherDictionaryViewModel(cipherSolver.Cipher);
            RaisePropertyChanged("CDVM");
        }

        void OnSolutionUpdated(object sender, EventArgs e)
        {
            SolutionText = cipherSolver.Solution;
            Score = ScoreToString(cipherSolver.SolutionScore);
            StatusMessage = string.Format("Deciphered - solution score: {0}", Score);
        }

        // simpler than a value converter, to be honest
        string ScoreToString(double score)
        {
            return string.Format("{0:0.00}", score);
        }
        #endregion
    }
}
