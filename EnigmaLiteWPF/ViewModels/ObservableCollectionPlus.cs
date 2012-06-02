using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EnigmaLiteWPF.ViewModels
{
    /// <summary>
    /// An ObservableCollection that is subscribed to PropertyChanged events of its items
    /// From soren.enemaerke on StackOverflow
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ObservableCollectionPlus<T> : ObservableCollection<T> where T : INotifyPropertyChanged 
    {        
        // when the collection is changed
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // go through the new (old) items and subscribe (unsubscribe)
            Subscribe(e.NewItems);
            Unsubscribe(e.OldItems);
            // fire event
            base.OnCollectionChanged(e);
        }

        private void Subscribe(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                {
                    element.PropertyChanged += ContainedElementChanged;
                }
            }
        }

        private void Unsubscribe(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                {
                    element.PropertyChanged -= ContainedElementChanged;
                }
            }
        }

        protected override void ClearItems()
        {
            foreach (T element in this)
            {
                element.PropertyChanged -= ContainedElementChanged;
            }
            base.ClearItems();
        }

        protected virtual void ContainedElementChanged(object sender, PropertyChangedEventArgs e)
        {
            // raise ObservableCollection's property-changed event                        
            OnPropertyChanged(e);
            // do something interesting?
        }
    }
}
