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
        #region Properties     
        protected string _inputText = "Input the ciphered text here.";
        public string InputText
        {
            get
            {
                return _inputText;
            }
            set
            {
                if (_inputText != value)
                {
                    _inputText = value;
                    StatusMessage = "TEXT CHANGED!";
                    cipherSolver = new CipherSolver(_inputText);
                    RaisePropertyChanged("InputText");
                }
            }
        }

        protected string _outputText = "";
        public string OutputText
        {
            get 
            { 
                return _outputText; 
            }
            set
            {
                if (_outputText != value)
                {
                    _outputText = value;
                    RaisePropertyChanged("OutputText");
                }
            }
        }

        protected CipherSolver cipherSolver { get; set; }

        protected Dictionary<char, char> XCipher;
        public CipherSolverViewModel Cipher { get; set; }

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
            cipherSolver = new CipherSolver(InputText);
            Cipher = new CipherSolverViewModel(cipherSolver.Cipher, InputText);            
        }
        #endregion

    } 
}
