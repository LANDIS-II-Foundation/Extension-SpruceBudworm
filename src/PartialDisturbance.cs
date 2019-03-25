//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Landis.Core;
using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;

using System.Collections.Generic;
using System;

namespace Landis.Extension.SpruceBudworm
{
    /// <summary>
    /// A biomass disturbance that handles partial thinning of cohorts.
    /// </summary>
    public class PartialDisturbance
        : IDisturbance
    {
        private static PartialDisturbance singleton;

        private static ActiveSite currentSite;

        //---------------------------------------------------------------------

        ActiveSite Landis.Library.BiomassCohorts.IDisturbance.CurrentSite
        {
            get
            {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------

        ExtensionType IDisturbance.Type
        {
            get
            {
                return PlugIn.ExtType;
            }
        }

        //---------------------------------------------------------------------

        static PartialDisturbance()
        {
            singleton = new PartialDisturbance();
        }

        //---------------------------------------------------------------------

        public PartialDisturbance()
        {
        }

        //---------------------------------------------------------------------

        int IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            int currentDefol = 0;    
            if (PlugIn.Parameters.SBWHost[cohort.Species] && (cohort.Age >= PlugIn.Parameters.MinSusceptibleAge))
                {

                    // assign defoliation to cohorts skewed by species and age (13a)
                    // These species names are hard-wired
                    // FIXME
                    double sppConvert = 0.0;
                    if (cohort.Species.Name == "abiebals")
                        sppConvert = 1.0;
                    else if (cohort.Species.Name == "piceglau")
                        sppConvert = 0.75;
                    else if (cohort.Species.Name == "picemari")
                        sppConvert = 0.375;
                    double cohortDefol = SiteVars.PctDefoliation[currentSite] * sppConvert * cohort.Age / SiteVars.MaxHostAge[currentSite];
                    if (cohortDefol > 100)
                        cohortDefol = 100;
                    cohort.UpdateDefoliationHistory(cohortDefol / 100.0);
                    currentDefol = (int)Math.Round((cohort.CurrentFoliage * (cohortDefol / 100)));  // Assumes all defoliation comes out of current year foliage, convert percentage to proportion

                    SiteVars.TotalDefoliation[currentSite] += currentDefol;
                }


            if (currentDefol > cohort.Biomass || currentDefol < 0)
            {
                PlugIn.ModelCore.UI.WriteLine("Cohort Total Mortality={0}. Cohort Biomass={1}. Site R/C={2}/{3}.", currentDefol, cohort.Biomass, currentSite.Location.Row, currentSite.Location.Column);
                throw new System.ApplicationException("Error: Total Mortality is not between 0 and cohort biomass");
            }

            // Calculate cumulative defoliation (16)
            double cohortPctMortality = Impacts.CohortMortality(cohort);
            // Calculate host tree impacts (17)
            int cohortMortality = (int) (cohortPctMortality * (cohort.Biomass - currentDefol));
            int totalBiomassReduction = currentDefol + cohortMortality;
            SiteVars.BiomassRemoved[currentSite] += totalBiomassReduction;

            return totalBiomassReduction;

        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reduces the biomass of cohorts that have been marked for partial
        /// reduction.
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site)
        {
            currentSite = site;
            SiteVars.BiomassCohorts[site].ReduceOrKillBiomassCohorts(singleton);
        }
    }
}
