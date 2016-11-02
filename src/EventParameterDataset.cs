//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.SpruceBudworm
{
	/// <summary>
	/// Editable parameters (size and frequency) for wind events for a
	/// collection of ecoregions.
	/// </summary>
	public class EventParameterDataset
	{
		private IEventParameters[] parameters;

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of ecoregions in the dataset.
		/// </summary>
		public int Count
		{
			get {
				return parameters.Length;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The event parameters for an ecoregion.
		/// </summary>
		public IEventParameters this[int ecoregionIndex]
		{
			get {
				return parameters[ecoregionIndex];
			}

			set {
				parameters[ecoregionIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public EventParameterDataset(int ecoregionCount)
		{
			parameters = new IEventParameters[ecoregionCount];
		}

	}
}
