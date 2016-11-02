//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

namespace Landis.Extension.SpruceBudworm
{
    /// <summary>
    /// Size and frequency parameters for wind events in an ecoregion.
    /// </summary>
    public interface IEventParameters
    {
        /// <summary>
        /// Maximum event size (hectares).
        /// </summary>
        double MaxSize
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Mean event size (hectares).
        /// </summary>
        double MeanSize
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum event size (hectares).
        /// </summary>
        double MinSize
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Wind rotation period (years).
        /// </summary>
        int RotationPeriod
        {
            get;set;
        }
    }
}
