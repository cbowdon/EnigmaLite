using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EnigmaLite;

namespace EnigmaLiteWPF.ViewModels
{
    class TextAnalysisViewModel : ObservableObject
    {
        #region Properties
        protected string filename = "";
        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                if (filename != value)
                {
                    filename = value;
                    RaisePropertyChanged("Filename");
                }
            }
        }

        protected string inputText = "Input the ciphered text here.";
        public string InputText
        {
            get
            {
                return inputText;
            }
            set
            {
                if (inputText != value)
                {
                    inputText = value;
                    StatusMessage = "TEXT CHANGED!";
                    RaisePropertyChanged("InputText");
                }
            }
        }

        protected Dictionary<char, char> XCipher;
        public CipherCollection Cipher { get; set; }
 
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
        public TextAnalysisViewModel()
        {
            // N.B. "TargetInvocationException" may refer to a NullReferenceException
            // i.e. don't forget to instantiate stuff

            // this is just temporary dummy data
            XCipher = new Dictionary<char, char>();
            XCipher.Add('a', 'i');
            XCipher.Add('c', 'k');
            XCipher.Add('d', 'l');
            XCipher.Add('e', 'm');
            XCipher.Add('b', 'j');            

            Cipher = new CipherCollection(XCipher, "GshdAsasghbcage");            
        }
        #endregion

    } 
}
