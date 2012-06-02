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
    class CipherCollection : ObservableCollectionPlus<CipherPair>
    {
        public string InputText { get; set; }
        public string OutputText { get; set; }

        public CipherCollection(Dictionary<char, char> originalKey, string cipheredText)
        {
            InputText = cipheredText;
            LoadFromDict(originalKey);
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
            OutputText = TextAnalysis.SubChars(InputText, dict);
        }        

        protected override void ContainedElementChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);           
            var s = sender as CipherPair;
            var existing = from i in this.Items where i.CipherValue == s.CipherValue && i.CipherKey != s.CipherKey select i;
            if (existing.ToList().Count > 0)
            {
                foreach (var i in existing) {
                    i.CipherValue = (char)0;
                }
            }            
        }

    }
}
