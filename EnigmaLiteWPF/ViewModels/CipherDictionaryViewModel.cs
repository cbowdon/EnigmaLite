using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using EnigmaLite;

namespace EnigmaLiteWPF.ViewModels
{
    class CipherDictionaryViewModel : ObservableCollectionPlus<CipherPair>, INotifyPropertyChanged
    {
        protected CipherDictionary cipherDictionary;

        public CipherDictionaryViewModel(CipherDictionary cd)
        {
            cipherDictionary = cd;
            LoadFromDict(cipherDictionary);            
        }

        /// <summary>
        /// Clear internal list and fill from a dictionary
        /// </summary>
        /// <param name="dict"></param>
        protected void LoadFromDict(Dictionary<char, char> dict)
        {
            base.ClearItems();
            // sort the original by keys
            var sd = new SortedDictionary<char, char>(dict);
            // could maintain a copy of original?
            foreach (var kv in sd)
            {
                Add(new CipherPair(kv.Key, kv.Value));
            }
        }        

        protected override void ContainedElementChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);           
            var s = sender as CipherPair;
            var existing = from i in this.Items where i.CipherValue == s.CipherValue && i.CipherKey != s.CipherKey select i;
            if (existing.ToList().Count > 0)
            {
                foreach (var i in existing) {
                    i.CipherValue = '*';
                    cipherDictionary[i.CipherKey] = '*';
                }
            }
            cipherDictionary[s.CipherKey] = s.CipherValue;            
        }      
    }
}
