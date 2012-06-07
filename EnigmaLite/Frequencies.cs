using System;
using System.Collections.Generic;
using System.Linq;

namespace EnigmaLite
{
	public class Frequencies<T>
	{
		public Frequencies (IDictionary<T, double> singles, IDictionary<T, double> doubles)
		{			
			Singles = singles;
			Doubles = doubles;
		}
		
		public IDictionary<T, double> Singles { get; protected set; }

		public IDictionary<T, double> Doubles { get; protected set; }
		
		protected IList<KeyValuePair<T,double>> _orderedSingles;
		public IList<KeyValuePair<T,double>> OrderedSingles {
			get {
				if (_orderedSingles == null) {
					_orderedSingles = (from entry in Singles orderby entry.Value descending select entry).ToList();
				}
				return _orderedSingles;
			}	
		}
		
		protected IList<KeyValuePair<T,double>> _orderedDoubles;
		public IList<KeyValuePair<T,double>> OrderedDoubles {
			get {
				if (_orderedDoubles == null) {
					_orderedDoubles = (from entry in Singles orderby entry.Value descending select entry).ToList();
				}
				return _orderedDoubles;
			}	
		}		
	}
}

