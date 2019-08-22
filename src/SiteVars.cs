//  Authors:  Brian Miranda, USFS


using Landis.SpatialModeling;
using Landis.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Landis.Extension.SpruceBudworm
{
    public static class SiteVars
    {
        private static ISiteVar<bool> disturbed;
        private static ISiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts> ageCohorts;
        private static ISiteVar<Landis.Library.BiomassCohorts.ISiteCohorts> biomassCohorts;
        private static ISiteVar<double> budwormDensityL2;
        private static ISiteVar<double> budwormCountSpring;
        private static ISiteVar<double> filteredBudwormSpring;
        private static ISiteVar<double> filteredDensitySpring;
        private static ISiteVar<double> budwormCount;
        private static ISiteVar<double> enemyDensity;
        private static ISiteVar<double> enemyCount;
        private static ISiteVar<double> filteredEnemyCount;
        private static ISiteVar<double> currentHostFoliage;
        private static ISiteVar<double> totalHostFoliage;
        private static ISiteVar<double> pctDefoliation;
        private static ISiteVar<int> maxHostAge;
        private static ISiteVar<double> totalDefoliation;
        private static ISiteVar<double> eggCountFall;
        private static ISiteVar<double> dispersed;
        private static ISiteVar<double> sddOut;
        private static ISiteVar<double> lddOut;
        private static ISiteVar<int> biomassRemoved;
        private static ISiteVar<int> hostBiomass;
        private static ISiteVar<int> decidBiomass;
        private static ISiteVar<double> decidProp;
        private static ISiteVar<double> disperse_n;
        private static ISiteVar<double> disperse_v;
        private static ISiteVar<Event> lastEvent;
        private static ISiteVar<int> timeOfLastEvent;
        private static ISiteVar<int> severity;




        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            disturbed = PlugIn.ModelCore.Landscape.NewSiteVar<bool>();
            budwormDensityL2 = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            ReadFloatMap(parameters.InitSBWDensMap, budwormDensityL2);
            budwormCountSpring = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            filteredBudwormSpring = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            filteredDensitySpring = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            budwormCount = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            enemyDensity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            ReadFloatMap(parameters.InitEnemyDensMap, enemyDensity);
            enemyCount = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            filteredEnemyCount = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            currentHostFoliage = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            totalHostFoliage = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            pctDefoliation = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            maxHostAge = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            totalDefoliation = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            eggCountFall = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            dispersed = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            sddOut = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            lddOut = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            biomassRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            hostBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            decidBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            decidProp = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            disperse_n = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            disperse_v = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            lastEvent = PlugIn.ModelCore.Landscape.NewSiteVar<Event>();
            timeOfLastEvent = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            severity = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

            SiteVars.TimeOfLastEvent.ActiveSiteValues = -10000;

            ageCohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts>("Succession.AgeCohorts");
            biomassCohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.BiomassCohorts.ISiteCohorts>("Succession.BiomassCohorts");

        }

        //---------------------------------------------------------------------
        public static ISiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts> AgeCohorts
        {
            get
            {
                return ageCohorts;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<Landis.Library.BiomassCohorts.ISiteCohorts> BiomassCohorts
        {
            get
            {
                return biomassCohorts;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<bool> Disturbed
        {
            get {
                return disturbed;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> BudwormDensL2Scaled
        {
            get
            {
                return budwormDensityL2;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> BudwormCountSpring
        {
            get
            {
                return budwormCountSpring;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> FilteredBudwormSpring
        {
            get
            {
                return filteredBudwormSpring;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> FilteredDensitySpring
        {
            get
            {
                return filteredDensitySpring;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> BudwormCount
        {
            get
            {
                return budwormCount;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> EnemyDensity
        {
            get
            {
                return enemyDensity;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> EnemyCount
        {
            get
            {
                return enemyCount;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> FilteredEnemyCount
        {
            get
            {
                return filteredEnemyCount;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> CurrentHostFoliage
        {
            get
            {
                return currentHostFoliage;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> TotalHostFoliage
        {
            get
            {
                return totalHostFoliage;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> PctDefoliation
        {
            get
            {
                return pctDefoliation;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> MaxHostAge
        {
            get
            {
                return maxHostAge;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> TotalDefoliation
        {
            get
            {
                return totalDefoliation;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> EggCountFall
        {
            get
            {
                return eggCountFall;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> Dispersed
        {
            get
            {
                return dispersed;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> LDDout
        {
            get
            {
                return lddOut;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> SDDout
        {
            get
            {
                return sddOut;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> BiomassRemoved
        {
            get
            {
                return biomassRemoved;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> HostBiomass
        {
            get
            {
                return hostBiomass;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> DecidBiomass
        {
            get
            {
                return decidBiomass;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> DecidProp
        {
            get
            {
                return decidProp;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> Disperse_n
        {
            get
            {
                return disperse_n;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> Disperse_v
        {
            get
            {
                return disperse_v;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastEvent
        {
            get
            {
                return timeOfLastEvent;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> Severity
        {
            get
            {
                return severity;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<Event> Event
        {
            get
            {
                return lastEvent;
            }
        }
        //---------------------------------------------------------------------
        public static void ReadDoubleMap(string path, ISiteVar<double> siteVariable)
        {
            IInputRaster<DoublePixel> map;

            try
            {
                map = PlugIn.ModelCore.OpenRaster<DoublePixel>(path);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != PlugIn.ModelCore.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapCode = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        siteVariable[site] = mapCode;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public static void ReadFloatMap(string path, ISiteVar<double> siteVariable)
        {
            IInputRaster<FloatPixel> map;

            try
            {
                map = PlugIn.ModelCore.OpenRaster<FloatPixel>(path);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != PlugIn.ModelCore.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map)
            {
                FloatPixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapCode = pixel.MapCode.Value;
                    mapCode = Math.Round(mapCode, 7);
                    if (site.IsActive)
                    {
                        siteVariable[site] = mapCode;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public static void CalculateHostFoliage(Landis.Library.Parameters.Species.AuxParm<bool> SBWHost, int minSuscAge)
        {
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                int currentHostFoliage = 0;
                int totalHostFoliage = 0;
                int maxHostAge = 0;
                int hostBiomass = 0;
                Landis.Library.BiomassCohorts.ISiteCohorts biomassCohortList = SiteVars.BiomassCohorts[site];
                foreach (Landis.Library.BiomassCohorts.ISpeciesCohorts speciesCohorts in biomassCohortList)
                {
                    foreach (Landis.Library.BiomassCohorts.ICohort cohort in speciesCohorts)
                    {
                        if (SBWHost[cohort.Species])
                        {
                            hostBiomass += cohort.Biomass;
                            if (cohort.Age >= minSuscAge)
                            {
                                currentHostFoliage += cohort.CurrentFoliage;
                                totalHostFoliage += cohort.TotalFoliage;
                            }
                            if (cohort.Age > maxHostAge)
                                maxHostAge = cohort.Age;
                        }
                    }
                }
                SiteVars.CurrentHostFoliage[site] = currentHostFoliage;
                SiteVars.TotalHostFoliage[site] = totalHostFoliage;
                SiteVars.MaxHostAge[site] = maxHostAge;
                SiteVars.HostBiomass[site] = hostBiomass;

            }
        }
        //---------------------------------------------------------------------
        public static void CalculatePopulation(double winterSurvival, double randFecund, double phenolLimit)
        {

            double sumCountSpring = 0;
            double sumCountFiltered = 0;
            //double sumEnemyFiltered = 0;
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                if ((PlugIn.ModelCore.CurrentTime > 10) && (site.Location.Row == 11) && (site.Location.Column == 54))
                {
                    int m = 0;
                    int n = m;
                }
                // apply stochastic winter survival (3)
                double budwormCountSpring = SiteVars.BudwormCount[site] * winterSurvival;
                if (budwormCountSpring < 0)
                    budwormCountSpring = 0;
                sumCountSpring += budwormCountSpring;
                SiteVars.BudwormCountSpring[site] = budwormCountSpring;
            }
            // spatial average filter spring budworm counts (4) - not applied in L2-Site
            // Spatial filter in neighborhood
            PlugIn.ModelCore.UI.WriteLine("Spatial filter of L2 budworm.");
            SiteVars.FilteredBudwormSpring.ActiveSiteValues = 0;
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                if ((PlugIn.ModelCore.CurrentTime > 10) && (site.Location.Row == 11) && (site.Location.Column == 54))
                {
                    int m = 0;
                    int n = m;
                }
                //double filteredBudwormSpring = SpatialFilter(site, PlugIn.Parameters.L2FilterRadius);
                double filteredBudwormSpring = LocalDispersal(site, PlugIn.Parameters.L2FilterRadius, PlugIn.Parameters.L2EdgeEffect, PlugIn.Parameters.EcoParameters);
                sumCountFiltered += filteredBudwormSpring;

            }
            // calculate budworm density (5)
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                double budwormDensitySpring = 0;
                if (SiteVars.CurrentHostFoliage[site] > 0)
                    budwormDensitySpring = SiteVars.FilteredBudwormSpring[site] / SiteVars.CurrentHostFoliage[site];
                else
                    budwormDensitySpring = PlugIn.Parameters.MaxBudDensity;
                SiteVars.FilteredDensitySpring[site] = budwormDensitySpring;
            }
            // spatial average filter spring enemies counts (6) - not applied in L2-Site
            // Spatial filter in neighborhood
            PlugIn.ModelCore.UI.WriteLine("Spatial filter of natural enemies.");
            SiteVars.FilteredEnemyCount.ActiveSiteValues = 0;
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                // spatial average filter spring enemy counts
                // Spatial filter in neighborhood
                double filteredEnemyCount = LocalDispersalEnemies(site);
                //sumEnemyFiltered += filteredEnemySpring;
            }

            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                if ((PlugIn.ModelCore.CurrentTime > 10) && (site.Location.Row == 11) && (site.Location.Column == 54))
                {
                    int m = 0;
                    int n = m;
                }

                double budwormDensityL2 = SiteVars.FilteredDensitySpring[site];

                // Apply scaling parameters (7)
                double budwormDensL2Scaled = Math.Pow((budwormDensityL2 / PlugIn.Parameters.PreyM), (1.0 / PlugIn.Parameters.PreyN));
                SiteVars.BudwormDensL2Scaled[site] = budwormDensL2Scaled;

                // calculate scaled budworm L2 count (8)
                double budwormCountL2 = 0;
                double currentHostFoliage = SiteVars.CurrentHostFoliage[site];
                if (currentHostFoliage > 0)
                    budwormCountL2 = budwormDensL2Scaled * currentHostFoliage;

                // calculate enemy density (9)
                double enemyDensitySpring = 0;
                double budwormCountSpring = SiteVars.FilteredBudwormSpring[site];
                double minDensity = 10e-250;
                if (budwormCountSpring > minDensity) // At very low budworm counts the budworm and enemies go to 0
                    enemyDensitySpring = SiteVars.FilteredEnemyCount[site] / budwormCountSpring;
                else
                    budwormCountSpring = 0;
                SiteVars.EnemyDensity[site] = enemyDensitySpring;

                //Apply scaling parameters (10)
                double enemyDensitySpringScaled = Math.Pow((enemyDensitySpring / PlugIn.Parameters.PredM), (1.0 / PlugIn.Parameters.PredN));


                // calculate mating effect (11a)
                double matingEffect = ProportionMatedFunction(PlugIn.Parameters.MatingEffectA, PlugIn.Parameters.MatingEffectB, PlugIn.Parameters.MatingEffectC, budwormDensityL2);

                // calculate deciduous protection effect (11b)
                double decidProportion = CalculateDecidProportion(site, PlugIn.Parameters.Deciduous);
                double decidProtect1 = decidProportion * PlugIn.Parameters.DecidProtectMax1;  // dispersal loss effect
                double decidProtect2 = decidProportion * PlugIn.Parameters.DecidProtectMax2;  // parasite community composition effect

                // recruitment functions (12)
                double ryx = 1 - Math.Exp((-1 * PlugIn.Parameters.EnemyParamb * budwormDensL2Scaled) - decidProtect2);
                double rxy = Math.Exp(-1 * PlugIn.Parameters.EnemyParamc * enemyDensitySpringScaled);
                double rt = PlugIn.Parameters.MaxReproEnemy * ryx * rxy;

                // calculate r't (rprimet) without foliage dependence
                double rprimeyx = Math.Exp(-1 * (PlugIn.Parameters.SBWParamb + decidProtect1) * Math.Pow(budwormDensL2Scaled, PlugIn.Parameters.SBWParama));
                double rprimexy = Math.Exp(-1 * (PlugIn.Parameters.SBWParamc * enemyDensitySpringScaled));
                double fecundity = 216.8;
                double rprimet = fecundity * PlugIn.Parameters.MaxReproSBW * rprimeyx * rprimexy * matingEffect;

                // Host Tree Damage
                // Calculate defoliation (15)
                // Use Regniere and You 1991; Eq. 13
                //double defolPopulationComp = 0.385; // Product of indivudual instar [lambda + (1-lambda)*S]
                double allLarvalSurvival = rprimeyx * rprimexy;
                double defolPopulationComp = (PlugIn.Parameters.DefolLambda + (1.0 - PlugIn.Parameters.DefolLambda) * allLarvalSurvival); // Product of indivudual instar [lambda + (1-lambda)*S]
                double etaDefol = 870; // mg foliage removed per budworm
                double pctDefol = 100 * etaDefol * 0.001 * budwormDensL2Scaled * defolPopulationComp; //convert mg to g (0.001)
                // cap defoliation at 100%
                pctDefol = Math.Min(pctDefol, 100);
                // do not allow damage if no budworm
                if (!(budwormCountL2 > 0))
                    pctDefol = 0;
                SiteVars.PctDefoliation[site] = pctDefol;

                // calculate r''t (rprime2t) with defoliation effect on fecundity
                // calculate defoliation effect on fecundity
                double rprimeZ = CalculateRprimeZ(pctDefol); //Nealis & Regniere 2004, Fig 2
                double rprime2t = fecundity * PlugIn.Parameters.MaxReproSBW * rprimeZ * rprimeyx * rprimexy * matingEffect;

                // calculate enemy density following recruitment (12)
                double enemyDensitySummer = enemyDensitySpringScaled * rt;

                // calculate budworm density following recruitment (12)
                double budwormDensitySummer = 0;
                if (SiteVars.FilteredDensitySpring[site] > 0)
                {
                    if (PlugIn.Parameters.DefolFecundReduction)  // Defoliation-adjusted fecundity
                    {
                        budwormDensitySummer = Math.Max(SiteVars.BudwormDensL2Scaled[site] * rprime2t, 0);
                    }
                    else  // No Defoliation-adjusted fecundity
                    {
                        budwormDensitySummer = Math.Max(SiteVars.BudwormDensL2Scaled[site] * rprimet, 0);
                    }
                }

                // calculate number of budworm and enemies (13)
                double budwormCountSummer = budwormDensitySummer * currentHostFoliage;
                double enemyCountWinter = enemyDensitySummer * budwormCountSummer;
                SiteVars.EnemyCount[site] = enemyCountWinter;

                // apply stochastic predation (14a)
                double budwormCountFall = budwormCountSummer * randFecund;
                if (budwormCountFall < 0)
                    budwormCountFall = 0;

                // apply stochastic phenological limitation (14b)
                double eggCountFall = budwormCountFall * phenolLimit;
                if (eggCountFall < 0)
                    eggCountFall = 0;
                SiteVars.EggCountFall[site] = eggCountFall;
            }
        }
        //---------------------------------------------------------------------
        // 2-parameter mating effect - no longer used
        public static double ProportionMatedFunction(double a, double b, double x)
        {
            double y = 1.0 - Math.Exp(-1.0 * a * x - b);
            return y;
        }
        //---------------------------------------------------------------------
        // 3-parameter mating effect
        public static double ProportionMatedFunction(double a, double b, double c, double x)
        {
            double y = 1.0 - Math.Exp(-1.0 * Math.Pow(a, c) * x - b);
            return y;
        }
        //---------------------------------------------------------------------
        public static double CalculateDecidProportion(Site site, Landis.Library.Parameters.Species.AuxParm<bool> Deciduous)
        {
            double decidProportion = 0;
            int siteBiomass = 0;
            int decidBiomass = 0;
            foreach (Landis.Library.BiomassCohorts.ISpeciesCohorts speciesCohorts in SiteVars.BiomassCohorts[site])
            {
                foreach (Landis.Library.BiomassCohorts.ICohort cohort in speciesCohorts)
                {
                    int cohortBiomass = cohort.Biomass;
                    siteBiomass += cohortBiomass;
                    if (Deciduous[cohort.Species])
                    {
                        decidBiomass += cohortBiomass;
                    }
                }
            }
            if (siteBiomass > 0)
                decidProportion = (double)decidBiomass / (double)siteBiomass;
            SiteVars.DecidBiomass[site] = decidBiomass;
            SiteVars.DecidProp[site] = decidProportion;

            return decidProportion;
        }
        //---------------------------------------------------------------------
        public static void CalculateDefoliation()
        {
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                PartialDisturbance.ReduceCohortBiomass(site);
            }
        }
        //---------------------------------------------------------------------
        // This calculates average count among neighbors - NOT USED
        public static double SpatialFilter(Site site, double l2FilterRadius)
        {
            double CellLength = PlugIn.ModelCore.CellLength;

            List<RelativeLocation> neighborhood = new List<RelativeLocation>();

            int numCellRadius = (int)(l2FilterRadius / CellLength);
            //PlugIn.ModelCore.UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",l2FilterRadius, CellLength, numCellRadius);
            double centroidDistance = 0;
            double cellLength = CellLength;

            double totalCount = 0;
            int cellCount = 0;
            for (int row = (numCellRadius * -1); row <= numCellRadius; row++)
            {
                for (int col = (numCellRadius * -1); col <= numCellRadius; col++)
                {
                    centroidDistance = DistanceFromCenter(row, col);

                    //PlugIn.ModelCore.Log.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if (centroidDistance <= l2FilterRadius)
                    {
                        RelativeLocation relLocation = new RelativeLocation(row, col);
                        neighborhood.Add(relLocation);
                        Site neighbor = site.GetNeighbor(relLocation);
                        if (neighbor.IsActive)
                        {
                            totalCount += SiteVars.BudwormCountSpring[neighbor];
                            cellCount++;
                        }
                    }
                }
            }
            double filteredDensity = totalCount / (double)cellCount;
            return filteredDensity;
        }
        //---------------------------------------------------------------------
        // This disperses local count among self and neighbors
        public static double LocalDispersal(Site site, double l2FilterRadius, string edgeEffect, IEcoParameters[] ecoParameters)
        {
            List<Site> siteList = new List<Site>();
            List<bool> leftMapList = new List<bool>();
            int maxCells = 0;
            double sumDensity = 0;
            FindNeighborSites(out siteList, out leftMapList, out maxCells, out sumDensity, site, l2FilterRadius, "l2", ecoParameters);
            int siteCount = siteList.Count;
            double dispersedCount = 0;
            // Calculate number to disperse to each site in neighborhood
            if (edgeEffect.Equals("Unbiased", StringComparison.OrdinalIgnoreCase))
            {
                dispersedCount = SiteVars.BudwormCountSpring[site] / maxCells;
            }
            else
            {
                //throw error
                string mesg = string.Format("L2EdgeEffect must be Unbiased");
                throw new System.ApplicationException(mesg);
            }

            // Disperse to all neighbor sites
            double sumDispersed = 0;
            foreach (Site disperseSite in siteList)
            {
                SiteVars.FilteredBudwormSpring[disperseSite] += dispersedCount;
                sumDispersed += dispersedCount;
            }

            // With wrapping we do not need to assum inputs from non-active cells
            //// Add assumed input from non-active neigbors is EdgeEffect == Same
            //if (edgeEffect.Equals("Same", StringComparison.OrdinalIgnoreCase))
            //{
            //    double siteInput = ((maxCells - siteCount) * dispersedCount);
            //    SiteVars.FilteredBudwormSpring[site] += siteInput ;
            //    sumDispersed += siteInput;
            //}

            return sumDispersed;
        }
        //---------------------------------------------------------------------
        // This disperses local count among self and neighbors
        public static double LocalDispersalEnemies(Site site)
        {
            List<Site> siteList = new List<Site>();
            int maxCells = 0;
            double sumDensity = 0;
            List<string> leftMapList = new List<string>();

            FindNeighborSites(out siteList, out leftMapList, out maxCells, out sumDensity, site, PlugIn.Parameters.EnemyFilterRadius, "enemies", PlugIn.Parameters.EcoParameters);
            int siteCount = siteList.Count;
            double avgDensity = sumDensity / siteCount;
            double adjSumDensity = sumDensity + (avgDensity * (maxCells - siteCount));
            double enemiesToDisperse = SiteVars.EnemyCount[site] * PlugIn.Parameters.EnemyDispersalProp;
            double enemiesToStayLocal = SiteVars.EnemyCount[site] - enemiesToDisperse;
            double enemiesBiasedDisperse = enemiesToDisperse * PlugIn.Parameters.EnemyBiasedProp;
            double enemiesUnbiasedDisperse = enemiesToDisperse - enemiesBiasedDisperse;
            double dispersedCount = 0;
            // Calculate number to disperse to each site in neighborhood           
            double unbiasedDisperseCount = enemiesUnbiasedDisperse / maxCells; // For Unbiased

            // Disperse to all neighbor sites
            double sumDispersed = 0;
            int siteIndex = 0;
            foreach (Site disperseSite in siteList)
            {
                dispersedCount = unbiasedDisperseCount;
                string leftMap = leftMapList[siteIndex];
                if (PlugIn.Parameters.EnemyEdgeEffect.Equals("Biased", StringComparison.OrdinalIgnoreCase))
                {
                    double biasIndex = SiteVars.FilteredBudwormSpring[disperseSite] / sumDensity;
                    dispersedCount += enemiesBiasedDisperse * biasIndex;
                }
                double edgeWrapReduction = 1.0;
                if (leftMap.Contains("N"))
                {
                    edgeWrapReduction = edgeWrapReduction * PlugIn.Parameters.EnemyEdgeWrapReduction_N;
                }
                if (leftMap.Contains("E"))
                {
                    edgeWrapReduction = edgeWrapReduction * PlugIn.Parameters.EnemyEdgeWrapReduction_E;
                }
                if (leftMap.Contains("S"))
                {
                    edgeWrapReduction = edgeWrapReduction * PlugIn.Parameters.EnemyEdgeWrapReduction_S;
                }
                if (leftMap.Contains("W"))
                {
                    edgeWrapReduction = edgeWrapReduction * PlugIn.Parameters.EnemyEdgeWrapReduction_W;
                }
                SiteVars.FilteredEnemyCount[disperseSite] += dispersedCount * edgeWrapReduction;
                sumDispersed += dispersedCount * edgeWrapReduction;


                siteIndex++;
            }
            SiteVars.FilteredEnemyCount[site] += enemiesToStayLocal;
            sumDispersed += enemiesToStayLocal;
            // With wrapping we do not need to assum inputs from non-active cells
            //// Add assumed input from non-active neigbors if EdgeEffect == Same or AvgBiased
            //if (edgeEffect.Equals("Same", StringComparison.OrdinalIgnoreCase))
            //{
            //    double siteInput = ((maxCells - siteCount) * dispersedCount);
            //    SiteVars.FilteredEnemyCount[site] += siteInput;
            //    sumDispersed += siteInput;
            //}
            //else if (edgeEffect.Equals("AvgBiased", StringComparison.OrdinalIgnoreCase))
            //{
            //    double siteBias = SiteVars.FilteredBudwormSpring[site] / adjSumDensity;
            //    double siteInput = (maxCells - siteCount) * SiteVars.EnemyCount[site] * siteBias;
            //    SiteVars.FilteredEnemyCount[site] += siteInput;
            //    sumDispersed += siteInput;
            //}
            return sumDispersed;
        }

        // This disperses local count among self and neighbors
        public static double DisperseSDD(Site site, double sddRadius, string edgeEffect, IEcoParameters[] ecoParameters)
        {
            List<Site> siteList = new List<Site>();  //List of cells to disperse to (takes into account edge effects)
            List<bool> leftMapList = new List<bool>();
            int maxCells = 0;  //Total cells in the neighborhood regardless of edge effects
            double sumDensity = 0;  //Sum of densities across sites (takes into account edge effects)
            FindNeighborSites(out siteList, out leftMapList, out maxCells, out sumDensity, site, sddRadius, "sdd", ecoParameters);
            int siteCount = siteList.Count;  //Number if cells to disperse to
            double avgDensity = sumDensity / siteCount;  //Average density per cell
            double adjSumDensity = sumDensity + (avgDensity * (maxCells - siteCount));

            double dispersedCount = 0;
            // Calculate number to disperse to each site in neighborhood
            dispersedCount = SiteVars.SDDout[site] / (double)maxCells; //For Unbiased

            // Disperse to all neighbor sites
            double sumDispersed = 0;
            foreach (Site disperseSite in siteList)
            {
                if ((sumDensity > 0) && (edgeEffect.Equals("Biased", StringComparison.OrdinalIgnoreCase)))
                {
                    double biasIndex = SiteVars.TotalHostFoliage[disperseSite] / sumDensity;
                    dispersedCount = SiteVars.SDDout[site] * biasIndex;
                }
                SiteVars.Dispersed[disperseSite] += dispersedCount;
                sumDispersed += dispersedCount;
            }
            // With wrapping we do not need to assum inputs from non-active cells
            //// Add assumed input from non-active neigbors if EdgeEffect == Same or AvgBiased
            //if (edgeEffect.Equals("Same", StringComparison.OrdinalIgnoreCase))
            //{
            //    double siteInput = ((maxCells - siteCount) * dispersedCount);
            //    SiteVars.Dispersed[site] += siteInput;
            //    sumDispersed += siteInput;
            //}
            //else if (edgeEffect.Equals("AvgBiased", StringComparison.OrdinalIgnoreCase))
            //{
            //    double siteBias = SiteVars.TotalHostFoliage[site] / adjSumDensity;
            //    double siteInput = (maxCells - siteCount) * SiteVars.SDDout[site] * siteBias;
            //    SiteVars.Dispersed[site] += siteInput;
            //    sumDispersed += siteInput;
            //}
            return sumDispersed;
        }
        //-------------------------------------------------------
        // This disperses local count among self and neighbors
        // MOdified method - Replaced
        //public static double DisperseSDD(Site site, double sddRadius, string edgeEffect, IEcoParameters[] ecoParameters)
        //{
        //    List<Site> siteList = new List<Site>();  //List of cells to disperse to (takes into account edge effects)
        //    int maxCells = 0;  //Total cells in the neighborhood 
        //    double sumDensity = 0;  //Sum of densities across sites (takes into account edge effects)
        //    FindNeighborSites(out siteList, out maxCells, out sumDensity, site, sddRadius, "sdd", ecoParameters, edgeEffect);
        //    int siteCount = siteList.Count;  //Number if cells to disperse to
        //    double avgDensity = sumDensity / siteCount;  //Average density per cell
        //    double adjSumDensity = sumDensity + (avgDensity * (maxCells - siteCount));

        //    double dispersedCount = 0;
        //    // Calculate number to disperse to each site in neighborhood
        //    if (edgeEffect.Equals("Reflect", StringComparison.OrdinalIgnoreCase))
        //    {
        //        dispersedCount = SiteVars.SDDout[site] / (double)siteCount;
        //    }
        //    else if (edgeEffect.Equals("Absorb", StringComparison.OrdinalIgnoreCase))
        //    {
        //        dispersedCount = SiteVars.SDDout[site] / (double)maxCells;
        //    }
        //    // Disperse to all neighbor sites
        //    double sumDispersed = 0;
        //    foreach (Site disperseSite in siteList)
        //    {
        //        if (edgeEffect.Equals("Biased", StringComparison.OrdinalIgnoreCase))
        //        {
        //            double biasIndex = SiteVars.TotalHostFoliage[disperseSite] / sumDensity;
        //            dispersedCount = SiteVars.SDDout[site] * biasIndex;
        //        }
        //        else if (edgeEffect.Equals("AvgBiased", StringComparison.OrdinalIgnoreCase))
        //        {
        //            double biasIndex = SiteVars.TotalHostFoliage[disperseSite] / adjSumDensity;
        //            dispersedCount = SiteVars.SDDout[site] * biasIndex;
        //        }
        //        SiteVars.Dispersed[disperseSite] += dispersedCount;
        //        sumDispersed += dispersedCount;
        //    }
        //    // Add assumed input from non-active neigbors if EdgeEffect == Same or AvgBiased
        //    if (edgeEffect.Equals("Same", StringComparison.OrdinalIgnoreCase))
        //    {
        //        double siteInput = ((maxCells - siteCount) * dispersedCount);
        //        SiteVars.Dispersed[site] += siteInput;
        //        sumDispersed += siteInput;
        //    }
        //    else if (edgeEffect.Equals("AvgBiased", StringComparison.OrdinalIgnoreCase))
        //    {
        //        double siteBias = SiteVars.TotalHostFoliage[site] / adjSumDensity;
        //        double siteInput = (maxCells - siteCount) * SiteVars.SDDout[site] * siteBias;
        //        SiteVars.Dispersed[site] += siteInput;
        //        sumDispersed += siteInput;
        //    }
        //    return sumDispersed;
        //}
        //-------------------------------------------------------
        //// This disperses local count among self and neighbors biased by budworm count
        //public static double BiasedDispersalEnemies(Site site, double enemyFilterRadius, string edgeEffect)
        //{
        //    List<Site> siteList = new List<Site>();
        //    int maxCells = 0;
        //    bool enemies = true;
        //    double sumDensity = 0;
        //    FindNeighborSites(out siteList, out maxCells, out sumDensity, site, enemyFilterRadius, enemies);
        //    // Calculate number to disperse to each site in neighborhood
        //    int siteCount = siteList.Count;


        //    // Disperse to all neighbor sites
        //    double sumDispersed = 0;
        //    foreach (Site disperseSite in siteList)
        //    {
        //        double biasIndex = SiteVars.FilteredBudwormSpring[disperseSite] / sumDensity;
        //        double dispersedCount = SiteVars.EnemyCount[disperseSite] * biasIndex;
        //        SiteVars.FilteredEnemyCount[disperseSite] += dispersedCount;
        //        sumDispersed += dispersedCount;
        //    }

        //    return sumDispersed;
        //}
        //-------------------------------------------------------
        // Modified version - not working yet
        private static void FindNeighborSites(out List<Site> siteList, out int maxCells, out double sumDensity, Site site, double radius, string option, IEcoParameters[] ecoParameters, string mapEdgeEffect)
        {
            double CellLength = PlugIn.ModelCore.CellLength;

            //List<RelativeLocation> neighborhood = new List<RelativeLocation>();

            int numCellRadius = (int)(radius / CellLength);
            //PlugIn.ModelCore.UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",radius, CellLength, numCellRadius);
            double centroidDistance = 0;
            double cellLength = CellLength;

            // Count the neighbor cells to disperse to (including source site)
            List<Site> tempSiteList = new List<Site>();
            int maxCellCount = 0;
            int cellCount = 0;
            double tempSumDensity = 0;
            for (int row = (numCellRadius * -1); row <= numCellRadius; row++)
            {
                for (int col = (numCellRadius * -1); col <= numCellRadius; col++)
                {
                    centroidDistance = DistanceFromCenter(row, col);
                    //PlugIn.ModelCore.Log.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if (centroidDistance <= radius)
                    {
                        RelativeLocation relLocation = new RelativeLocation(row, col);
                        //neighborhood.Add(relLocation);

                        Site neighbor = site.GetNeighbor(relLocation);
                        IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[neighbor];
                        string edgeEffect = ecoParameters[ecoregion.Index].L2EdgeEffect;
                        double neighborDensity = 1.0;
                        double siteDensity = 1.0;
                        if (option == "enemies")
                        {
                            edgeEffect = ecoParameters[ecoregion.Index].EnemyEdgeEffect;
                            neighborDensity = SiteVars.FilteredBudwormSpring[neighbor];
                            siteDensity = SiteVars.FilteredBudwormSpring[site];
                        }
                        else if (option == "sdd")
                        {
                            edgeEffect = ecoParameters[ecoregion.Index].SDDEdgeEffect;
                            neighborDensity = SiteVars.TotalHostFoliage[neighbor];
                            siteDensity = SiteVars.TotalHostFoliage[site];
                        }
                        if (edgeEffect == null)
                        {
                            if (neighbor.IsActive)
                            {
                                // in map, but no edge effect defined
                                // treat as "Absorb"
                                edgeEffect = "Absorb";
                            }
                            else
                            {
                                // edge effect not defined, inactive
                                // treat as "MapEdge"
                                if (mapEdgeEffect == null)
                                {
                                    // MapEdgeEffect not defined
                                    // Treat as "Absorb"
                                    edgeEffect = "Absorb";
                                }
                                else
                                {
                                    edgeEffect = mapEdgeEffect;
                                }
                            }
                        }

                        if (edgeEffect.Equals("Reflect", StringComparison.OrdinalIgnoreCase))
                        {
                            if (neighbor.IsActive)
                            {
                                cellCount++;
                                maxCellCount++;
                                tempSiteList.Add(neighbor);
                            }
                            tempSumDensity += neighborDensity;
                        }
                        else if (edgeEffect.Equals("Absorb", StringComparison.OrdinalIgnoreCase))
                        {
                            tempSiteList.Add(neighbor);
                            cellCount++;
                            maxCellCount++;
                            tempSumDensity += neighborDensity;
                        }
                        else if (edgeEffect.Equals("Same", StringComparison.OrdinalIgnoreCase))
                        {
                            tempSiteList.Add(neighbor);
                            cellCount++;
                            maxCellCount++;
                            tempSumDensity += neighborDensity;
                        }
                        else if (edgeEffect.Equals("Biased", StringComparison.OrdinalIgnoreCase))
                        {
                            tempSiteList.Add(neighbor);
                            cellCount++;
                            maxCellCount++;
                            tempSumDensity += neighborDensity;
                        }
                        else if (edgeEffect.Equals("AvgBiased", StringComparison.OrdinalIgnoreCase))
                        {
                            if (neighbor.IsActive)
                            {
                                tempSiteList.Add(neighbor);
                                tempSumDensity += neighborDensity;
                            }
                            cellCount++;
                            maxCellCount++;
                        }
                        else
                        {
                            //throw error
                            string mesg = string.Format("Edge effect for {0} is not Reflect, Absorb, Same, Biased or AvgBiased", option);
                            throw new System.ApplicationException(mesg);
                        }
                    }
                }
            }
            siteList = tempSiteList;
            maxCells = maxCellCount;
            sumDensity = tempSumDensity;
        }
        //Modified code - wrapping, with ecoregion edge effects
        private static void FindNeighborSites(out List<Site> siteList, out List<bool> leftMapList, out int maxCells, out double sumDensity, Site site, double radius, string option, IEcoParameters[] ecoParameters)
        {
            double CellLength = PlugIn.ModelCore.CellLength;

            //List<RelativeLocation> neighborhood = new List<RelativeLocation>();

            int numCellRadius = (int)(radius / CellLength);
            //PlugIn.ModelCore.UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",radius, CellLength, numCellRadius);
            double centroidDistance = 0;
            double cellLength = CellLength;

            // Count the neighbor cells to disperse to (including source site)
            List<Site> tempSiteList = new List<Site>();
            List<bool> tempLeftList = new List<bool>();
            int maxCellCount = 0;
            double tempSumDensity = 0;
            for (int row = (numCellRadius * -1); row <= numCellRadius; row++)
            {
                for (int col = (numCellRadius * -1); col <= numCellRadius; col++)
                {
                    centroidDistance = DistanceFromCenter(row, col);
                    //PlugIn.ModelCore.Log.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if (centroidDistance <= radius)
                    {
                        int j = col;
                        int k = row;

                        int target_x = site.Location.Column + j;
                        int target_y = site.Location.Row + k;

                        bool leftMap = false;  //  Does dispersal leave the map (wrap)?

                        int landscapeRows = PlugIn.ModelCore.Landscape.Rows;
                        int landscapeCols = PlugIn.ModelCore.Landscape.Columns;

                        if (target_x < 0 || target_y < 0 || target_x > landscapeCols || target_y > landscapeRows)
                        {
                            leftMap = true;  // Dispersal goes off the map and wraps
                        }

                        //remainRow=SIGN(C4)*MOD(ABS(C4),$B$1)
                        int remainRow = Math.Sign(k) * (Math.Abs(k) % landscapeRows);
                        int remainCol = Math.Sign(j) * (Math.Abs(j) % landscapeCols);
                        //tempY=A4+H4
                        int tempY = site.Location.Row + remainRow;
                        int tempX = site.Location.Column + remainCol;
                        //source_y=IF(J4<1,$B$1+J4,IF(J4>$B$1,MOD(J4,$B$1),J4))
                        if (tempY < 1)
                        {
                            target_y = landscapeRows + tempY;
                        }
                        else
                        {
                            if (tempY > landscapeRows)
                            {
                                target_y = tempY % landscapeRows;
                            }
                            else
                            {
                                target_y = tempY;
                            }
                        }
                        if (tempX < 1)
                        {
                            target_x = landscapeCols + tempX;
                        }
                        else
                        {
                            if (tempX > landscapeCols)
                            {
                                target_x = tempX % landscapeCols;
                            }
                            else
                            {
                                target_x = tempX;
                            }
                        }
                        RelativeLocation targetLocation = new RelativeLocation(target_y - site.Location.Row, target_x - site.Location.Column);
                        Site neighbor = site.GetNeighbor(targetLocation);
                        IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[neighbor];
                        if (ecoregion == null)
                        {
                            ecoregion = PlugIn.ModelCore.Ecoregions[0];
                        }
                        string edgeEffect = ecoParameters[ecoregion.Index].L2EdgeEffect;
                        double neighborDensity = 1.0;

                        if (option == "enemies")
                        {
                            edgeEffect = ecoParameters[ecoregion.Index].EnemyEdgeEffect;
                            neighborDensity = SiteVars.FilteredBudwormSpring[neighbor];

                        }
                        else if (option == "sdd")
                        {
                            edgeEffect = ecoParameters[ecoregion.Index].SDDEdgeEffect;
                            neighborDensity = SiteVars.TotalHostFoliage[neighbor];

                        }

                        if (edgeEffect == null)
                        {
                            // in map, but no edge effect defined
                            // treat as "Absorb"
                            edgeEffect = "Absorb";
                        }

                        if (edgeEffect.Equals("Reflect", StringComparison.OrdinalIgnoreCase))
                        {
                            if (neighbor.IsActive)
                            {
                                maxCellCount++;
                                tempSiteList.Add(neighbor);
                                tempLeftList.Add(leftMap);
                            }
                            tempSumDensity += neighborDensity;
                        }
                        else if (edgeEffect.Equals("Absorb", StringComparison.OrdinalIgnoreCase))
                        {
                            if (neighbor.IsActive)
                            {
                                tempSiteList.Add(neighbor);
                                tempLeftList.Add(leftMap);
                            }
                            maxCellCount++;
                            tempSumDensity += neighborDensity;
                        }
                        else
                        {
                            //throw error
                            string mesg = string.Format("Edge effect for {0} is not Reflect, Absorb", option);
                            throw new System.ApplicationException(mesg);
                        }
                    }
                }
            }
            siteList = tempSiteList;
            maxCells = maxCellCount;
            sumDensity = tempSumDensity;
            leftMapList = tempLeftList;
            //return siteList;
        }

        //Modified code - wrapping, with edge wrap reductions
        private static void FindNeighborSites(out List<Site> siteList, out List<string> leftMapList, out int maxCells, out double sumDensity, Site site, double radius, string option, IEcoParameters[] ecoParameters)
        {
            double CellLength = PlugIn.ModelCore.CellLength;

            //List<RelativeLocation> neighborhood = new List<RelativeLocation>();

            int numCellRadius = (int)(radius / CellLength);
            //PlugIn.ModelCore.UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",radius, CellLength, numCellRadius);
            double centroidDistance = 0;
            double cellLength = CellLength;

            // Count the neighbor cells to disperse to (including source site)
            List<Site> tempSiteList = new List<Site>();
            List<string> tempLeftList = new List<string>();
            int maxCellCount = 0;
            double tempSumDensity = 0;
            for (int row = (numCellRadius * -1); row <= numCellRadius; row++)
            {
                for (int col = (numCellRadius * -1); col <= numCellRadius; col++)
                {
                    centroidDistance = DistanceFromCenter(row, col);
                    //PlugIn.ModelCore.Log.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if (centroidDistance <= radius)
                    {
                        int j = col;
                        int k = row;

                        int target_x = site.Location.Column + j;
                        int target_y = site.Location.Row + k;

                        string leftMap = "";


                        int landscapeRows = PlugIn.ModelCore.Landscape.Rows;
                        int landscapeCols = PlugIn.ModelCore.Landscape.Columns;

                        if (target_x < 0) // Dispersal goes off the map to the West
                        {
                            leftMap += "W";
                        }
                        if (target_y < 0) // Dispersal goes off the map to the North
                        {
                            leftMap += "N";
                        }
                        if (target_x > landscapeCols) // Dispersal goes off the map to the East
                        {
                            leftMap += "E";
                        }
                        if (target_y > landscapeRows) // Dispersal goes off the map to the South
                        {
                            leftMap += "S";
                        }

                        //remainRow=SIGN(C4)*MOD(ABS(C4),$B$1)
                        int remainRow = Math.Sign(k) * (Math.Abs(k) % landscapeRows);
                        int remainCol = Math.Sign(j) * (Math.Abs(j) % landscapeCols);
                        //tempY=A4+H4
                        int tempY = site.Location.Row + remainRow;
                        int tempX = site.Location.Column + remainCol;
                        //source_y=IF(J4<1,$B$1+J4,IF(J4>$B$1,MOD(J4,$B$1),J4))
                        if (tempY < 1)
                        {
                            target_y = landscapeRows + tempY;
                        }
                        else
                        {
                            if (tempY > landscapeRows)
                            {
                                target_y = tempY % landscapeRows;
                            }
                            else
                            {
                                target_y = tempY;
                            }
                        }
                        if (tempX < 1)
                        {
                            target_x = landscapeCols + tempX;
                        }
                        else
                        {
                            if (tempX > landscapeCols)
                            {
                                target_x = tempX % landscapeCols;
                            }
                            else
                            {
                                target_x = tempX;
                            }
                        }
                        RelativeLocation targetLocation = new RelativeLocation(target_y - site.Location.Row, target_x - site.Location.Column);
                        Site neighbor = site.GetNeighbor(targetLocation);
                        IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[neighbor];
                        if (ecoregion == null)
                        {
                            ecoregion = PlugIn.ModelCore.Ecoregions[0];
                        }
                        string edgeEffect = ecoParameters[ecoregion.Index].L2EdgeEffect;
                        double neighborDensity = 1.0;

                        if (option == "enemies")
                        {
                            edgeEffect = ecoParameters[ecoregion.Index].EnemyEdgeEffect;
                            neighborDensity = SiteVars.FilteredBudwormSpring[neighbor];

                        }
                        else if (option == "sdd")
                        {
                            edgeEffect = ecoParameters[ecoregion.Index].SDDEdgeEffect;
                            neighborDensity = SiteVars.TotalHostFoliage[neighbor];

                        }

                        if (edgeEffect == null)
                        {
                            // in map, but no edge effect defined
                            // treat as "Absorb"
                            edgeEffect = "Absorb";
                        }

                        if (edgeEffect.Equals("Reflect", StringComparison.OrdinalIgnoreCase))
                        {
                            if (neighbor.IsActive)
                            {
                                maxCellCount++;
                                tempSiteList.Add(neighbor);
                                tempLeftList.Add(leftMap);
                            }
                            tempSumDensity += neighborDensity;
                        }
                        else if (edgeEffect.Equals("Absorb", StringComparison.OrdinalIgnoreCase))
                        {
                            if (neighbor.IsActive)
                            {
                                tempSiteList.Add(neighbor);
                                tempLeftList.Add(leftMap);
                            }
                            maxCellCount++;
                            tempSumDensity += neighborDensity;
                        }
                        else
                        {
                            //throw error
                            string mesg = string.Format("Edge effect for {0} is not Reflect, Absorb", option);
                            throw new System.ApplicationException(mesg);
                        }
                    }
                }
            }
            siteList = tempSiteList;
            maxCells = maxCellCount;
            sumDensity = tempSumDensity;
            leftMapList = tempLeftList;
            //return siteList;
        }
        //-------------------------------------------------------
        //Original code - non-wrapping, no ecoregion edge effects
        private static void FindNeighborSites(out List<Site> siteList, out int maxCells, out double sumDensity, Site site, double radius, string option)
        {
            double CellLength = PlugIn.ModelCore.CellLength;

            //List<RelativeLocation> neighborhood = new List<RelativeLocation>();

            int numCellRadius = (int)(radius / CellLength);
            //PlugIn.ModelCore.UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",radius, CellLength, numCellRadius);
            double centroidDistance = 0;
            double cellLength = CellLength;

            // Count the neighbor cells to disperse to (including source site)
            List<Site> tempSiteList = new List<Site>();
            int maxCellCount = 0;
            int cellCount = 0;
            double tempSumDensity = 0;
            for (int row = (numCellRadius * -1); row <= numCellRadius; row++)
            {
                for (int col = (numCellRadius * -1); col <= numCellRadius; col++)
                {
                    centroidDistance = DistanceFromCenter(row, col);
                    //PlugIn.ModelCore.Log.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if (centroidDistance <= radius)
                    {
                        RelativeLocation relLocation = new RelativeLocation(row, col);
                        //neighborhood.Add(relLocation);
                        maxCellCount++;
                        Site neighbor = site.GetNeighbor(relLocation);
                        if (neighbor.IsActive)
                        {
                            if (option == "enemies")
                            {
                                if (SiteVars.FilteredBudwormSpring[neighbor] > 0)
                                {
                                    cellCount++;
                                    tempSiteList.Add(neighbor);
                                    tempSumDensity += SiteVars.FilteredBudwormSpring[neighbor];
                                }
                            }
                            else if (option == "sdd")
                            {
                                if (SiteVars.TotalHostFoliage[neighbor] > 0)
                                {
                                    cellCount++;
                                    tempSiteList.Add(neighbor);
                                    tempSumDensity += SiteVars.TotalHostFoliage[neighbor];
                                }
                            }
                            else
                            {
                                cellCount++;
                                tempSiteList.Add(neighbor);
                            }
                        }
                    }
                }
            }
            siteList = tempSiteList;
            maxCells = maxCellCount;
            sumDensity = tempSumDensity;
            //return siteList;
        }

        //Calculate the distance from a location to a center
        //point (row and column = 0).
        private static double DistanceFromCenter(double row, double column)
        {
            double CellLength = PlugIn.ModelCore.CellLength;
            row = System.Math.Abs(row) * CellLength;
            column = System.Math.Abs(column) * CellLength;
            double aSq = System.Math.Pow(column, 2);
            double bSq = System.Math.Pow(row, 2);
            return System.Math.Sqrt(aSq + bSq);
        }

        //Calculate rprimez
        //Nealis & Regniere 2004, Fig 2
        public static double CalculateRprimeZ(double pctDefoliation)
        {
            return (-0.0054 * pctDefoliation + 1);
        }


    }
}
