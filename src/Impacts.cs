using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.SpruceBudworm
{
    class Impacts
    {
        public static void Initialize()
        {
            Landis.Library.BiomassCohorts.CohortGrowthReduction.Compute = Impacts.CohortGrowthReduction;
        }
        //---------------------------------------------------------------------
        //Periodic (5 year) Mortality Rate = Intercept + AGE + AGE*CD + CD^2 + CD^3
        static double[] params_MR_AGE_BF = new double[] { 0.1182, -0.003634, 0.0001112, -0.0001563, 0.000002064 };
        static double[] params_MR_AGE_BS = new double[] { 0.1509, -0.002707, 0.00007719, -0.0001869, 0.000002194 };
        static double[] params_MR_AGE_RS = new double[] { 0.1509, -0.002707, 0.00007719, -0.0001869, 0.000002194 };
        static double[] params_MR_AGE_WS = new double[] { 0.1005, -0.002187, 0.00006505, -0.0001375, 0.000001891 };
        static double[] params_MR_AGE_NONHOST = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
        //---------------------------------------------------------------------
        /// <summary>
        /// Predicts additive probability of tree death in current simulation year
        /// as a function of <paramref name="host"/>, <paramref name="PCD"/>, and <paramref name="age"/>.
        /// </summary>
        /// <param name="host">SBW host species.</param>
        /// <param name="PCD">
        /// Periodic (5-year) average cumulative defoliation 
        /// as defined by MacLean et al. (2001).
        /// </param>
        /// <param name="age">Tree age.</param>
        /// <returns>Additive tree mortality rate (probability of death in current year).</returns>
        public static double GetMortalityRate_AGE(ISpecies species, double PCD, int age)
        {
            if (PCD < 0.35)
                return 0; //Avoid extrapolation issues.
            else
                PCD *= 100; //Convert units to percent

            double rate = 0;
            double[] b = null;

            if (species.Name == "abiebals")
            {
                b = params_MR_AGE_BF;
                if (age > 90) age = 90; //Cap to avoid extrapolation issues.
            }
            else if (species.Name == "picemari")
            {
                b = params_MR_AGE_BS;
                if (age > 125) age = 125;
            }
            else if (species.Name == "picerube")
            {
                b = params_MR_AGE_RS;
                if (age > 125) age = 125;
            }
            else if (species.Name == "piceglau")
            {
                b = params_MR_AGE_WS;
                if (age > 125) age = 125;
            }
            else
            {
                b = params_MR_AGE_NONHOST;
            }
                
            //Periodic (5 year) Mortality Rate = Intercept + AGE + CD*AGE + CD^2 + CD^3
            rate = b[0] +
                   b[1] * age +
                   b[2] * age * PCD +
                   b[3] * PCD * PCD +
                   b[4] * PCD * PCD * PCD;

            //Trap out of bounds extrapolated predictions. 
            if (rate < 0)
                return 0;
            else if (rate > 1)
                return 1;

            //Convert from periodic (5 year) to annual mortality rate using the reverse compound interest formula
            //(original fit was based on 5 year mortality rates, so we need to first predict periodic, then convert to annual).
            rate = 1 - Math.Pow(1 - rate, 1 / 5.0);

            return rate;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate growth reduction (proportion)
        /// </summary>
        /// <param name="cohort"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public static double CohortGrowthReduction(Landis.Library.BiomassCohorts.ICohort cohort, ActiveSite site)
        {
            // Calculate cumulative defoliation (13)
            // Cumulative Annual Weighted Defoliation (Hennigar)
            double annWtDefol0 = cohort.DefoliationHistory[0] * 0.28;
            double annWtDefol1 = cohort.DefoliationHistory[1] * 0.26;
            double annWtDefol2 = cohort.DefoliationHistory[2] * 0.22;
            double annWtDefol3 = cohort.DefoliationHistory[3] * 0.13;
            double annWtDefol4 = cohort.DefoliationHistory[4] * 0.08;
            double annWtDefol5 = cohort.DefoliationHistory[5] * 0.03;
            double cumAnnWtDefol = annWtDefol0 + annWtDefol1 + annWtDefol2 + annWtDefol3 + annWtDefol4 + annWtDefol5;

            // Calculate host tree impacts (14)
            // Growth reduction
            double growthReduction = 0;
            if (PlugIn.Parameters.GrowthReduction)
            {
                // Hennigar method
                growthReduction = 1 - (-0.0099 * (cumAnnWtDefol * 100) + 1.0182);

                // Dobesberger method
                //growthReduction = 0.339 * cumAnnWtDefol;

                if (growthReduction > 1.0)  // Cannot exceed 100%
                    growthReduction = 1.0;
                if (growthReduction < 0.0)  // Cannot be less than 0%
                    growthReduction = 0.0;
            }
            return growthReduction;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate cohort mortality (proportion)
        /// </summary>
        /// <param name="cohort"></param>
        /// <returns></returns>
        public static double CohortMortality(Landis.Library.BiomassCohorts.ICohort cohort)
        {

            // Calculate cumulative defoliation (13)
            // Cumulative Annual Weighted Defoliation (Hennigar)
            double annWtDefol0 = cohort.DefoliationHistory[0] * 0.28;
            double annWtDefol1 = cohort.DefoliationHistory[1] * 0.26;
            double annWtDefol2 = cohort.DefoliationHistory[2] * 0.22;
            double annWtDefol3 = cohort.DefoliationHistory[3] * 0.13;
            double annWtDefol4 = cohort.DefoliationHistory[4] * 0.08;
            double annWtDefol5 = cohort.DefoliationHistory[5] * 0.03;
            double cumAnnWtDefol = annWtDefol0 + annWtDefol1 + annWtDefol2 + annWtDefol3 + annWtDefol4 + annWtDefol5;

            // Allocate impacts to cohorts (10a)
            // Mortality
            // Hennigar method
            double percentMortality = 0;
            const double CD_ScaleFactor = 0.952;
            double periodicDefoliation = cumAnnWtDefol * CD_ScaleFactor;
            percentMortality = Impacts.GetMortalityRate_AGE(cohort.Species, periodicDefoliation, cohort.Age);

            return percentMortality;
        }
        //---------------------------------------------------------------------
    }
}
