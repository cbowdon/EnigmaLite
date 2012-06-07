using System;
using System.Collections.Generic;
using System.Linq;

namespace EnigmaLite
{
	public class CollectionStatistics<T>
	{
		public CollectionStatistics (IEnumerable<T> data)
		{			
		}
		
		public IDictionary<T, double> SingleFrequencies { get; protected set; }
		public IDictionary<T, double> DoubleFrequencies { get; protected set; }
	}
}

