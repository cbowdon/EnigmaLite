using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EnigmaLiteWPF.Views
{
    public static class SelectableTextBox
    {
        #region Attached property
        public static readonly DependencyProperty AutoSelectAllProperty =
            DependencyProperty.RegisterAttached("AutoSelectAll", 
            typeof(bool), 
            typeof(TextBox), 
            new FrameworkPropertyMetadata((bool)false, 
                FrameworkPropertyMetadataOptions.None, 
                new PropertyChangedCallback(OnAutoSelectAllChanged)));

        public static bool GetAutoSelectAll(DependencyObject d)
        {
            return (bool)d.GetValue(AutoSelectAllProperty);
        }

        public static void SetAutoSelectAll(DependencyObject d, bool val)
        {
            d.SetValue(AutoSelectAllProperty, val);
        }
        #endregion

        #region If the dependency property is changed, do this (handler)
        
        public static void OnAutoSelectAllChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var t = sender as TextBox;
            var on = (bool)e.NewValue;

            if (t != null && on)
            {
                t.AddHandler(TextBox.MouseUpEvent, new RoutedEventHandler(SelectAllText), true);
                t.AddHandler(TextBox.MouseDownEvent, new RoutedEventHandler(SelectAllText));
                t.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(SelectAllText));
            }
            else if (t != null && !on)
            {
                t.RemoveHandler(TextBox.MouseUpEvent, new RoutedEventHandler(SelectAllText));
                t.RemoveHandler(TextBox.MouseDownEvent, new RoutedEventHandler(SelectAllText));
                t.RemoveHandler(TextBox.GotFocusEvent, new RoutedEventHandler(SelectAllText));
            }                           
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var t = sender as TextBox;
            if (t != null)
            {
                t.SelectAll();
            }
        }
        #endregion
    }    
}
