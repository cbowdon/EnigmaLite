using System;
using System.Collections.Generic;

namespace EnigmaLite
{	
	public class CipherDictionary : Dictionary<char,char>
	{
		public event EventHandler ItemChanged;
		
		public new char this [char k] {
			get {
				return (char)base [k];
			}
			set {
				if (base [k] != value) {
					base [k] = value;
					OnItemChanged ();
				}
			}
		}
		
		private void OnItemChanged ()
		{
			if (ItemChanged != null) {
				ItemChanged.Invoke (this, new EventArgs ());				
			}
		}
	}
}

