//  Authors:  Brian Miranda, USFS

using Landis.SpatialModeling;
using Landis.Core;
using System.Collections.Generic;
using System.IO;
using System;
using Troschuetz.Random;

namespace Landis.Extension.SpruceBudworm
{
    ///<summary>
    /// A disturbance plug-in that simulates wind disturbance.
    /// </summary>

    public class PlugIn
        : ExtensionMain
    {
        public static readonly ExtensionType ExtType = new ExtensionType("disturbance:spruce budworm");
        public static readonly string ExtensionName = "Spruce Budworm";
        private static IInputParameters parameters;
        private static ICore modelCore;


        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName, ExtType)
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }
        //---------------------------------------------------------------------
        public static IInputParameters Parameters
        {
            get
            {
                return parameters;
            }
        }

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            InputParameterParser parser = new InputParameterParser();
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the plug-in with a data file.
        /// </summary>
        /// <param name="dataFile">
        /// Path to the file with initialization data.
        /// </param>
        /// <param name="startTime">
        /// Initial timestep (year): the timestep that will be passed to the
        /// first call to the component's Run method.
        /// </param>
        public override void Initialize()
        {
            Timestep = parameters.Timestep;

            SiteVars.Initialize(parameters);
            Impacts.Initialize();
            Outputs.Initialize();
            Dispersal.Initialize();

            /*
            //Debug distribution
            StreamWriter randLog = Landis.Data.CreateTextFile("G:/Budworm_model/TEST/E_TEST/rand.csv");
            randLog.AutoFlush = true;
            randLog.WriteLine("Rand1, Rand2");
            //Troschuetz.Random.NormalDistribution r1 = new Troschuetz.Random.NormalDistribution();
            for (int i = 0; i < 10000; i++)
            {
               
               r1.Mu = 1;
               r1.Sigma = 0.01;
               double rand = r1.NextDouble();
               rand = r1.NextDouble();

               r1.Mu = 2;
               r1.Sigma = 0.01;
               double rand2 = r1.NextDouble();
               rand2 = r1.NextDouble();
                
                PlugIn.ModelCore.NormalDistribution.Mu = 1;
                PlugIn.ModelCore.NormalDistribution.Sigma = 0.01;
                double rand = PlugIn.ModelCore.NormalDistribution.NextDouble();
                //rand = PlugIn.ModelCore.NormalDistribution.NextDouble();

                PlugIn.ModelCore.NormalDistribution.Mu = 2;
                PlugIn.ModelCore.NormalDistribution.Sigma = 0.01;
                double rand2 = PlugIn.ModelCore.NormalDistribution.NextDouble();
                //rand2 = PlugIn.ModelCore.NormalDistribution.NextDouble();
                
                randLog.Write("{0:0.000}, {1:0.000}",
                         rand,rand2);
                randLog.WriteLine("");
            }
          */

        }

        //---------------------------------------------------------------------

        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            ModelCore.UI.WriteLine("Processing landscape for spruce budworm events ...");

            // Calculate host foliage
            SiteVars.CalculateHostFoliage(parameters.SBWHost, parameters.MinSusceptibleAge);
            double randWinter = 1;
            double randFecund = 1;
            double phenolLimit = 1;

            if (ModelCore.CurrentTime >= parameters.BudwormStartYear)
            {
                if (ModelCore.CurrentTime == parameters.BudwormStartYear)
                {
                    foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
                    {
                        SiteVars.BudwormCount[site] = SiteVars.BudwormDensityL2[site] * SiteVars.CurrentHostFoliage[site];
                        SiteVars.EnemyCount[site] = SiteVars.EnemyDensity[site] * SiteVars.BudwormCount[site];
                    }
                }
                SiteVars.BiomassRemoved.ActiveSiteValues = 0;
                SiteVars.TotalDefoliation.ActiveSiteValues = 0;

                // Draw random overwinter survival (to be applied globally)
                // To be replaced with spatially-autocorrelated winter temp function
                PlugIn.ModelCore.NormalDistribution.Mu = parameters.OverwinterMean;
                if (parameters.OverwinterStdev == 0)
                    PlugIn.ModelCore.NormalDistribution.Sigma = 0.00000000000000000001;
                else
                    PlugIn.ModelCore.NormalDistribution.Sigma = parameters.OverwinterStdev;
                randWinter = PlugIn.ModelCore.NormalDistribution.NextDouble();
                randWinter = PlugIn.ModelCore.NormalDistribution.NextDouble();
                if (parameters.OverwinterConstant)
                    randWinter = parameters.OverwinterMean;

                // Draw random fecundity (to be applied globally)
                // To be replaced by spatially autocorrelated function
                PlugIn.ModelCore.NormalDistribution.Mu = parameters.FecundityMean;
                if (parameters.FecundityStdev == 0)
                    PlugIn.ModelCore.NormalDistribution.Sigma = 0.00000000000000000001;
                else
                    PlugIn.ModelCore.NormalDistribution.Sigma = parameters.FecundityStdev;
                randFecund = PlugIn.ModelCore.NormalDistribution.NextDouble();
                randFecund = PlugIn.ModelCore.NormalDistribution.NextDouble();
                if (parameters.FecundityConstant)
                    randFecund = parameters.FecundityMean;

                // Draw random phenological limitation (to be applied globally)
                // To be replaced by spatially autocorrelated function
                PlugIn.ModelCore.NormalDistribution.Mu = parameters.PhenolMean;
                if (parameters.PhenolStdev == 0)
                    PlugIn.ModelCore.NormalDistribution.Sigma = 0.00000000000000000001;
                else
                    PlugIn.ModelCore.NormalDistribution.Sigma = parameters.PhenolStdev;
                phenolLimit = PlugIn.ModelCore.NormalDistribution.NextDouble();
                phenolLimit = PlugIn.ModelCore.NormalDistribution.NextDouble();
                if (parameters.PhenolConstant)
                    phenolLimit = parameters.PhenolMean;

                SiteVars.CalculatePopulation(randWinter, randFecund, phenolLimit);

                // Calculate cohort defoliation and impacts
                SiteVars.CalculateDefoliation();

                // Calculate long-distance dispersal
                Dispersal.CalculateDispersal();

            }
            Outputs.WriteLogFile(ModelCore.CurrentTime,randWinter,randFecund,phenolLimit);
            Outputs.WriteMaps(ModelCore.CurrentTime);

        }


    }
}
