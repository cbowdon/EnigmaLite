using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EnigmaLiteWPF.ViewModels
{
    class CipherPair : ObservableObject
    {
        #region Properties 
        private readonly char cipherKey;
        public char CipherKey { get { return cipherKey; } }

        protected char val;
        public char CipherValue
        {
            get
            {
                return val;
            }
            set
            {
                if (val != value)
                {
                    val = value;
                    OnCipherEdited(this);
                    RaisePropertyChanged("CipherValue");                    
                }
            }
        }
        #endregion

        #region Constructors
        public CipherPair(char k, char v)
        {
            cipherKey = k;
            CipherValue = v;
        }        
        #endregion

        #region events
        public event EventHandler CipherEditedEvt;
        protected void OnCipherEdited(object sender)
        {
            if (CipherEditedEvt != null)
            {
                // fire this
                CipherEditedEvt(this, new EventArgs());
                // then, fire RaisePropertyChanged?                
            }
        }
        #endregion

    }
}
