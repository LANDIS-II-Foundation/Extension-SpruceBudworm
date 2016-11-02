using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Landis.SpatialModeling;

namespace Landis.Extension.SpruceBudworm
{
    class Outputs
    {
           private static StreamWriter log;

           public static void Initialize()
           {

               PlugIn.ModelCore.UI.WriteLine("   Opening spruce budworm log file \"{0}\" ...", PlugIn.Parameters.LogFileName);
               try
               {
                   log = Landis.Data.CreateTextFile(PlugIn.Parameters.LogFileName);
               }
               catch (Exception err)
               {
                   string mesg = string.Format("{0}", err.Message);
                   throw new System.ApplicationException(mesg);
               }
               log.AutoFlush = true;
               log.WriteLine("Time, CurrentHostFoliage, BudwormDensity, EnemyDensity, PctDefoliation, TotalBiomassRemoved,HostBiomass,DecidBiomass,PropDecid, MaxHostFoliage, MaxBudDensity, MaxEnemyDens, MaxPctDefol, MaxSiteBiomassRemoved, MaxHostBio, MaxDecidBio, MaxDecidProp, RandWinter, RandFecund, PhenolLimit");
           }
         //---------------------------------------------------------------------
           public static void WriteLogFile(int CurrentTime, double randWinter, double randFecund, double phenolLimit)
           {
               double hostFoliage = 0;
               double maxHostFoliage = 0;
               double budwormDensity = 0;
               double maxBudwormDensity = 0;
               double enemyDensity = 0;
               double maxEnemyDensity = 0;
               double pctDefol = 0;
               double maxPctDefol = 0;
               double totalBiomassRemoved = 0;
               double maxSiteBiomassRemoved = 0;
               double hostBiomass = 0;
               double maxHostBiomass = 0;
               double decidBiomass = 0;
               double maxDecidBiomass = 0;
               double decidProp = 0;
               double maxDecidProp = 0;

               int siteCount = 0;

               foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
               {
                   siteCount++;
                   hostFoliage += SiteVars.CurrentHostFoliage[site];
                   if (hostFoliage > maxHostFoliage)
                       maxHostFoliage = hostFoliage;
                   hostBiomass += SiteVars.HostBiomass[site];
                   if (SiteVars.HostBiomass[site] > maxHostBiomass)
                       maxHostBiomass = SiteVars.HostBiomass[site];
                   decidBiomass += SiteVars.DecidBiomass[site];
                   if (SiteVars.DecidBiomass[site] > maxDecidBiomass)
                       maxDecidBiomass = SiteVars.DecidBiomass[site];
                   decidProp += SiteVars.DecidProp[site];
                   if (SiteVars.DecidProp[site] > maxDecidProp)
                       maxDecidProp = SiteVars.DecidProp[site];

                   if (CurrentTime >= PlugIn.Parameters.BudwormStartYear)
                   {
                       budwormDensity += SiteVars.FilteredDensitySpring[site];
                       if (SiteVars.FilteredDensitySpring[site] > maxBudwormDensity)
                           maxBudwormDensity = SiteVars.FilteredDensitySpring[site];
                       enemyDensity += SiteVars.EnemyDensity[site];
                       if (SiteVars.EnemyDensity[site] > maxEnemyDensity)
                           maxEnemyDensity = SiteVars.EnemyDensity[site];
                       pctDefol += SiteVars.PctDefoliation[site];
                       if (SiteVars.PctDefoliation[site] > maxPctDefol)
                           maxPctDefol = SiteVars.PctDefoliation[site];
                       totalBiomassRemoved += SiteVars.BiomassRemoved[site];
                       if (SiteVars.BiomassRemoved[site] > maxSiteBiomassRemoved)
                           maxSiteBiomassRemoved = SiteVars.BiomassRemoved[site];


                       // Debug
                       if (!(enemyDensity >= 0))
                       {
                           int m = 0;
                           int n = m;
                       }
                   }
               }

               log.Write("{0}, ",
                        CurrentTime);
               log.Write("{0:0.0}, {1:0.0000}, {2:0.0000}, {3:0.0},{4:0},{5:0},{6:0},{7:0.00},{8:0.0},{9:0.0000},{10:0.00},{11:0.0},{12:0},{13:0},{14:0},{15:0.00},{16:0.000},{17:0.0000},{18:0.0000}",
                   (hostFoliage / (double)siteCount),
                   (budwormDensity / (double)siteCount),
                   (enemyDensity / (double)siteCount),
                   (pctDefol / (double)siteCount),
                   totalBiomassRemoved,
                   (hostBiomass / (double)siteCount),
                   (decidBiomass / (double)siteCount),
                   (decidProp / (double)siteCount),
                   maxHostFoliage,
                   maxBudwormDensity,
                   maxEnemyDensity,
                   maxPctDefol,
                   maxSiteBiomassRemoved,
                   maxHostBiomass,
                   maxDecidBiomass,
                   maxDecidProp,
                   randWinter,
                   randFecund,
                   phenolLimit
                   );
               log.WriteLine("");
           }
        //---------------------------------------------------------------------
           public static void WriteMaps(int CurrentTime)
           {
               if (!(PlugIn.Parameters.PctDefolMapName == null))
               {
                   string pctDefolPath = MapNames.ReplaceTemplateVars(PlugIn.Parameters.PctDefolMapName, CurrentTime);

                   using (IOutputRaster<BytePixel> outputRasterDefol = PlugIn.ModelCore.CreateRaster<BytePixel>(pctDefolPath, PlugIn.ModelCore.Landscape.Dimensions))
                   {
                       BytePixel pixel = outputRasterDefol.BufferPixel;
                       foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                       {
                           if (site.IsActive)
                           {
                               pixel.MapCode.Value = (byte)(SiteVars.PctDefoliation[site]);

                           }
                           else
                           {
                               //  Inactive site
                               pixel.MapCode.Value = 0;
                           }
                           outputRasterDefol.WriteBufferPixel();
                       }
                   }
               }
               if (!(PlugIn.Parameters.MortalityMapName == null))
               {
                   string mortalityPath = MapNames.ReplaceTemplateVars(PlugIn.Parameters.MortalityMapName, CurrentTime);

                   using (IOutputRaster<ShortPixel> outputRasterMortality = PlugIn.ModelCore.CreateRaster<ShortPixel>(mortalityPath, PlugIn.ModelCore.Landscape.Dimensions))
                   {
                       ShortPixel pixel = outputRasterMortality.BufferPixel;
                       foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                       {
                           if (site.IsActive)
                           {
                               pixel.MapCode.Value = (short)(SiteVars.BiomassRemoved[site]);

                           }
                           else
                           {
                               //  Inactive site
                               pixel.MapCode.Value = 0;
                           }
                           outputRasterMortality.WriteBufferPixel();
                       }
                   }
               }
               if (!(PlugIn.Parameters.HostFolMapName == null))
               {
                   string hostFolPath = MapNames.ReplaceTemplateVars(PlugIn.Parameters.HostFolMapName, CurrentTime);

                   using (IOutputRaster<ShortPixel> outputRasterHostFol = PlugIn.ModelCore.CreateRaster<ShortPixel>(hostFolPath, PlugIn.ModelCore.Landscape.Dimensions))
                   {
                       ShortPixel pixel = outputRasterHostFol.BufferPixel;
                       foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                       {
                           if (site.IsActive)
                           {
                               pixel.MapCode.Value = (short)(SiteVars.CurrentHostFoliage[site]);

                           }
                           else
                           {
                               //  Inactive site
                               pixel.MapCode.Value = 0;
                           }
                           outputRasterHostFol.WriteBufferPixel();
                       }
                   }
               }
               if (!(PlugIn.Parameters.BudwormDensMapName == null))
               {
                   string budDensPath = MapNames.ReplaceTemplateVars(PlugIn.Parameters.BudwormDensMapName, CurrentTime);

                   using (IOutputRaster<DoublePixel> outputRasterBudDens = PlugIn.ModelCore.CreateRaster<DoublePixel>(budDensPath, PlugIn.ModelCore.Landscape.Dimensions))
                   {
                       DoublePixel pixel = outputRasterBudDens.BufferPixel;
                       foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                       {
                           if (site.IsActive)
                           {
                               pixel.MapCode.Value = (double)(SiteVars.BudwormDensityL2[site]);

                           }
                           else
                           {
                               //  Inactive site
                               pixel.MapCode.Value = 0;
                           }
                           outputRasterBudDens.WriteBufferPixel();
                       }
                   }
               }
               if (!(PlugIn.Parameters.EnemyDensMapName == null))
               {
                   string enemyDensPath = MapNames.ReplaceTemplateVars(PlugIn.Parameters.EnemyDensMapName, CurrentTime);

                   using (IOutputRaster<DoublePixel> outputRasterEnemyDens = PlugIn.ModelCore.CreateRaster<DoublePixel>(enemyDensPath, PlugIn.ModelCore.Landscape.Dimensions))
                   {
                       DoublePixel pixel = outputRasterEnemyDens.BufferPixel;
                       foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                       {
                           if (site.IsActive)
                           {
                               pixel.MapCode.Value = (double)(SiteVars.EnemyDensity[site]);

                           }
                           else
                           {
                               //  Inactive site
                               pixel.MapCode.Value = 0;
                           }
                           outputRasterEnemyDens.WriteBufferPixel();
                       }
                   }
               }
           }
    }
}
