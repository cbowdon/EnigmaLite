using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace EnigmaLite
{
	[Serializable()]
	public class Frequencies<T> : ISerializable
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
		
		#region ISerializable members
		public Frequencies (SerializationInfo info, StreamingContext context)
		{
			Singles = (IDictionary<T, double>)info.GetValue("Singles", typeof(IDictionary<T, double>));
			Doubles = (IDictionary<T, double>)info.GetValue("Doubles", typeof(IDictionary<T, double>));
		}
		
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Singles", Singles);
			info.AddValue("Doubles", Doubles);			
		}
		#endregion
	}
}

