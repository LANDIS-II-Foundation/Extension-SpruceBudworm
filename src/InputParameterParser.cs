//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Landis.Extension.SpruceBudworm
{
    /// <summary>
    /// A parser that reads the plug-in's parameters from text input.
    /// </summary>
    public class InputParameterParser
        : TextParser<IInputParameters>
    {
        public static IEcoregionDataset EcoregionsDataset = PlugIn.ModelCore.Ecoregions;

        //---------------------------------------------------------------------

        public InputParameterParser()
        {
            // FIXME: Hack to ensure that Percentage is registered with InputValues
            Edu.Wisc.Forest.Flel.Util.Percentage p = new Edu.Wisc.Forest.Flel.Util.Percentage();
        }
        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }
        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != PlugIn.ExtensionName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);

            InputParameters parameters = new InputParameters(PlugIn.ModelCore.Ecoregions.Count);

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<int> budwormStartYear = new InputVar<int>("BudwormStartYear");
            ReadVar(budwormStartYear);
            parameters.BudwormStartYear = budwormStartYear.Value;

            //  Read table of population parameters
            ReadName("PopulationParameters");
            StringReader currentLine = new StringReader(CurrentLine);
            
            InputVar<double> maxReproEnemy = new InputVar<double>("rm");
            ReadValue(maxReproEnemy, currentLine);
            parameters.MaxReproEnemy = maxReproEnemy.Value;

            InputVar<double> enemyParamb = new InputVar<double>("b");
            ReadValue(enemyParamb, currentLine);
            parameters.EnemyParamb = enemyParamb.Value;

            InputVar<double> enemyParamc = new InputVar<double>("c");
            ReadValue(enemyParamc, currentLine);
            parameters.EnemyParamc = enemyParamc.Value;

            InputVar<double> maxReproSBW = new InputVar<double>("r'm");
            ReadValue(maxReproSBW, currentLine);
            parameters.MaxReproSBW = maxReproSBW.Value;

            InputVar<double> sbwParama = new InputVar<double>("a'");
            ReadValue(sbwParama, currentLine);
            parameters.SBWParama = sbwParama.Value;

            InputVar<double> sbwParamb = new InputVar<double>("b'");
            ReadValue(sbwParamb, currentLine);
            parameters.SBWParamb = sbwParamb.Value;

            InputVar<double> sbwParamc = new InputVar<double>("c'");
            ReadValue(sbwParamc, currentLine);
            parameters.SBWParamc = sbwParamc.Value;
            GetNextLine();

            ReadName("ScalingParameters");
            currentLine = new StringReader(CurrentLine);
            InputVar<double> predM = new InputVar<double>("PredM");
            ReadValue(predM, currentLine);
            parameters.PredM = predM.Value;
            InputVar<double> predN = new InputVar<double>("PredN");
            ReadValue(predN, currentLine);
            parameters.PredN = predN.Value;
            GetNextLine();

            currentLine = new StringReader(CurrentLine);
            InputVar<double> preyM = new InputVar<double>("PreyM");
            ReadValue(preyM, currentLine);
            parameters.PreyM = preyM.Value;
            InputVar<double> preyN = new InputVar<double>("PreyN");
            ReadValue(preyN, currentLine);
            parameters.PreyN = preyN.Value;
            GetNextLine();

               // Read initial density map paths
            InputVar<string> initSBWDensMap = new InputVar<string>("InitialBudwormDensityMap");
            ReadVar(initSBWDensMap);
            parameters.InitSBWDensMap = initSBWDensMap.Value;
            InputVar<string> initEnemyDensMap = new InputVar<string>("InitialEnemyDensityMap");
            ReadVar(initEnemyDensMap);
            parameters.InitEnemyDensMap = initEnemyDensMap.Value;


            //  Read Overwinter Random parameters
            ReadName("OverwinterRandom");
            currentLine = new StringReader(CurrentLine);

            InputVar<double> overwinterMean = new InputVar<double>("OverwinterMean");
            ReadValue(overwinterMean, currentLine);
            parameters.OverwinterMean = overwinterMean.Value;

            InputVar<double> overwinterStdev = new InputVar<double>("OverwinterStdev");
            ReadValue(overwinterStdev, currentLine);
            parameters.OverwinterStdev = overwinterStdev.Value;
            if (overwinterStdev.Value == 0)
                parameters.OverwinterConstant = true;
            GetNextLine();

            //  Read Fecundity Random parameters
            ReadName("FecundityRandom");
            currentLine = new StringReader(CurrentLine);

            InputVar<double> fecundityMean = new InputVar<double>("FecundityMean");
            ReadValue(fecundityMean, currentLine);
            parameters.FecundityMean = fecundityMean.Value;

            InputVar<double> fecundityStdev = new InputVar<double>("FecundityStdev");
            ReadValue(fecundityStdev, currentLine);
            parameters.FecundityStdev = fecundityStdev.Value;
            if (fecundityStdev.Value == 0)
                parameters.FecundityConstant = true;
            GetNextLine();

            // Read Mating Effect parameters
            ReadName("MatingEffect");
            currentLine = new StringReader(CurrentLine);

            InputVar<double> matingEffectA = new InputVar<double>("MatingEffectA");
            ReadValue(matingEffectA, currentLine);
            parameters.MatingEffectA = matingEffectA.Value;

            InputVar<double> matingEffectB = new InputVar<double>("MatingEffectB");
            ReadValue(matingEffectB, currentLine);
            parameters.MatingEffectB = matingEffectB.Value;

            InputVar<double> matingEffectC = new InputVar<double>("MatingEffectC");
            ReadValue(matingEffectC, currentLine);
            parameters.MatingEffectC = matingEffectC.Value;
            GetNextLine();

            // Read Deciduous Protection parameters
            InputVar<double> decidProtectMax1 = new InputVar<double>("DeciduousProtection1");
            ReadVar(decidProtectMax1);
            parameters.DecidProtectMax1 = decidProtectMax1.Value;

            InputVar<double> decidProtectMax2 = new InputVar<double>("DeciduousProtection2");
            ReadVar(decidProtectMax2);
            parameters.DecidProtectMax2 = decidProtectMax2.Value;

            //  Read Phenological Limitation Random parameters
            ReadName("PhenologicalLimitation");
            currentLine = new StringReader(CurrentLine);

            InputVar<double> phenolMean = new InputVar<double>("PhenolMean");
            ReadValue(phenolMean, currentLine);
            parameters.PhenolMean = phenolMean.Value;

            InputVar<double> phenolStdev = new InputVar<double>("PhenolStdev");
            ReadValue(phenolStdev, currentLine);
            parameters.PhenolStdev = phenolStdev.Value;
            if (phenolStdev.Value == 0)
                parameters.PhenolConstant = true;
            GetNextLine();
            
            // Read Defoliation Fecundity Reduction boolean
            InputVar<bool> defolFecundReduction = new InputVar<bool>("DefoliationFecundityReduction");
            ReadVar(defolFecundReduction);
            parameters.DefolFecundReduction = defolFecundReduction.Value;

            // Read Growth Reduction boolean
            InputVar<bool> growthReduction = new InputVar<bool>("GrowthReduction");
            ReadVar(growthReduction);
            parameters.GrowthReduction = growthReduction.Value;

            // Read Mortality boolean
            InputVar<bool> mortality = new InputVar<bool>("Mortality");
            ReadVar(mortality);
            parameters.Mortality = mortality.Value;
            
            // Read Defoliation Lambda parameter
            InputVar<double> defolLambda = new InputVar<double>("DefoliationLambda");
            ReadVar(defolLambda);
            parameters.DefolLambda = defolLambda.Value;

            // Read SDD Radius
            InputVar<double> sddRadius = new InputVar<double>("SDDRadius");
            ReadVar(sddRadius);
            parameters.SDDRadius = sddRadius.Value;

            // Read SDDEdge Effect parameter
            InputVar<string> sddEdgeEffect = new InputVar<string>("SDDEdgeEffect");
            ReadVar(sddEdgeEffect);
            parameters.SDDEdgeEffect = sddEdgeEffect.Value;

            Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
            InputVar<string> ecoName = new InputVar<string>("SDDEcoregion");

            while (!AtEndOfInput && CurrentName != "Emigration")
            {
                currentLine = new StringReader(CurrentLine);
                ReadValue(ecoName, currentLine);

                IEcoregion ecoregion = EcoregionsDataset[ecoName.Value.Actual];
                if (ecoregion == null)
                    throw new InputValueException(ecoName.Value.String,
                                                  "{0} is not an ecoregion name.",
                                                  ecoName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(ecoregion.Name, out lineNumber))
                    throw new InputValueException(ecoName.Value.String,
                                                  "The ecoregion {0} was previously used on line {1}",
                                                  ecoName.Value.String, lineNumber);
                else
                    lineNumbers[ecoregion.Name] = LineNumber;
                IEcoParameters ecoParms = new EcoParameters();
                if (parameters.EcoParameters[ecoregion.Index] != null)
                {
                    ecoParms = parameters.EcoParameters[ecoregion.Index];
                }
                ReadValue(sddEdgeEffect, currentLine);
                ecoParms.SDDEdgeEffect = sddEdgeEffect.Value;
                parameters.EcoParameters[ecoregion.Index] = ecoParms;

                CheckNoDataAfter("the " + sddEdgeEffect.Name + " column",
                                 currentLine);
                GetNextLine();
            }
            
            // Read Emigration parameters
            ReadName("Emigration");
            currentLine = new StringReader(CurrentLine);

            InputVar<double> emigrationMinLDD = new InputVar<double>("EmigrationMinLDD");
            ReadValue(emigrationMinLDD, currentLine);
            parameters.EmigrationMinLDD = emigrationMinLDD.Value;

            InputVar<double> emigrationHalfLDD = new InputVar<double>("EmigrationHalfLDD");
            ReadValue(emigrationHalfLDD, currentLine);
            parameters.EmigrationHalfLDD = emigrationHalfLDD.Value;

            InputVar<double> emigrationMaxLDD = new InputVar<double>("EmigrationMaxLDD");
            ReadValue(emigrationMaxLDD, currentLine);
            parameters.EmigrationMaxLDD = emigrationMaxLDD.Value;

            InputVar<double> emigrationMaxLDDProp = new InputVar<double>("EmigrationMaxLDDProp");
            ReadValue(emigrationMaxLDDProp, currentLine);
            parameters.EmigrationMaxLDDProp = emigrationMaxLDDProp.Value;
            GetNextLine();

            // Read Dispersal parameters
            ReadName("LDDDispersalKernel");
            currentLine = new StringReader(CurrentLine);

            InputVar<double> dispersalMean1 = new InputVar<double>("DispersalMean1");
            ReadValue(dispersalMean1, currentLine);
            parameters.DispersalMean1 = dispersalMean1.Value;

            InputVar<double> dispersalMean2 = new InputVar<double>("DispersalMean2");
            ReadValue(dispersalMean2, currentLine);
            parameters.DispersalMean2 = dispersalMean2.Value;

            InputVar<double> dispersalWeight1 = new InputVar<double>("DispersalWeight1");
            ReadValue(dispersalWeight1, currentLine);
            parameters.DispersalWeight1 = dispersalWeight1.Value;
            GetNextLine();

            InputVar<bool> wrapLDD = new InputVar<bool>("WrapLDD");
            ReadVar(wrapLDD);
            parameters.WrapLDD = wrapLDD.Value;

            InputVar<bool> lddSpeedUp = new InputVar<bool>("LDDSpeedUp");
            if (ReadOptionalVar(lddSpeedUp))
            {
                parameters.LDDSpeedUp = lddSpeedUp.Value;
            }
            else
            {
                parameters.LDDSpeedUp = false;
            }

            // Read LDD Edge Wrap Reduction
            InputVar<double> lddEdgeWrapReduction = new InputVar<double>("LDDEdgeWrapReduction");
            ReadVar(lddEdgeWrapReduction);
            parameters.LDDEdgeWrapReduction = lddEdgeWrapReduction.Value;

            // Read Positive Fecundity Dispersal boolean
            InputVar<bool> positiveFecundDispersal = new InputVar<bool>("PositiveFecundityDispersal");
            ReadVar(positiveFecundDispersal);
            parameters.PositiveFecundDispersal = positiveFecundDispersal.Value;
            
            // Read Min Susceptible Age
            InputVar<int> minSusceptibleAge = new InputVar<int>("MinSusceptibleAge");
            ReadVar(minSusceptibleAge);
            parameters.MinSusceptibleAge = minSusceptibleAge.Value;

            // Read L2 Filter Radius
            InputVar<double> l2FilterRadius = new InputVar<double>("L2FilterRadius");
            ReadVar(l2FilterRadius);
            parameters.L2FilterRadius = l2FilterRadius.Value;

            // Read L2Edge Effect parameter
            InputVar<string> l2EdgeEffect = new InputVar<string>("L2EdgeEffect");
            ReadVar(l2EdgeEffect);
            parameters.L2EdgeEffect = l2EdgeEffect.Value;

            lineNumbers = new Dictionary<string, int>();
            ecoName = new InputVar<string>("L2Ecoregion");

            while (!AtEndOfInput && CurrentName != "EnemyFilterRadius")
            {
                currentLine = new StringReader(CurrentLine);
                ReadValue(ecoName, currentLine);

                IEcoregion ecoregion = EcoregionsDataset[ecoName.Value.Actual];
                if (ecoregion == null)
                    throw new InputValueException(ecoName.Value.String,
                                                  "{0} is not an ecoregion name.",
                                                  ecoName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(ecoregion.Name, out lineNumber))
                    throw new InputValueException(ecoName.Value.String,
                                                  "The ecoregion {0} was previously used on line {1}",
                                                  ecoName.Value.String, lineNumber);
                else
                    lineNumbers[ecoregion.Name] = LineNumber;
                IEcoParameters ecoParms = new EcoParameters();
                if (parameters.EcoParameters[ecoregion.Index] != null)
                {
                    ecoParms = parameters.EcoParameters[ecoregion.Index];
                }
                ReadValue(l2EdgeEffect, currentLine);
                ecoParms.L2EdgeEffect = l2EdgeEffect.Value;
                parameters.EcoParameters[ecoregion.Index] = ecoParms;

                CheckNoDataAfter("the " + l2EdgeEffect.Name + " column",
                                 currentLine);
                GetNextLine();
            }

            // Read Enemy Filter Radius
            InputVar<double> enemyFilterRadius = new InputVar<double>("EnemyFilterRadius");
            ReadVar(enemyFilterRadius);
            parameters.EnemyFilterRadius = enemyFilterRadius.Value;

            // Read Enemy DispersalProp
            InputVar<double> enemyDispersalProp = new InputVar<double>("EnemyDispersalProp");
            if(ReadOptionalVar(enemyDispersalProp))
            {
                parameters.EnemyDispersalProp = enemyDispersalProp.Value;
            }
            else
            {
                parameters.EnemyDispersalProp = 1.0;
            }

            // Read Enemy Edge Wrap Reduction
            InputVar<double> enemyEdgeWrapReduction = new InputVar<double>("EnemyEdgeWrapReduction");
            ReadVar(enemyEdgeWrapReduction);
            parameters.EnemyEdgeWrapReduction = enemyEdgeWrapReduction.Value;
            
            // Read EnemyEdge Effect parameter
            InputVar<double> enemyBiasedProp = new InputVar<double>("EnemyBiasedProp");
            ReadVar(enemyBiasedProp);
            parameters.EnemyBiasedProp = enemyBiasedProp.Value;
            if (parameters.EnemyBiasedProp > 0)
                parameters.EnemyEdgeEffect = "Biased";
            else
                parameters.EnemyEdgeEffect = "Unbiased";
            InputVar<string> enemyEdgeEffect = new InputVar<string>("EnemyEdgeEffect");
         

            lineNumbers = new Dictionary<string, int>();
            ecoName = new InputVar<string>("L2Ecoregion");
            while (!AtEndOfInput && CurrentName != "MaxBudwormDensity")
            {
                currentLine = new StringReader(CurrentLine);
                ReadValue(ecoName, currentLine);

                IEcoregion ecoregion = EcoregionsDataset[ecoName.Value.Actual];
                if (ecoregion == null)
                    throw new InputValueException(ecoName.Value.String,
                                                  "{0} is not an ecoregion name.",
                                                  ecoName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(ecoregion.Name, out lineNumber))
                    throw new InputValueException(ecoName.Value.String,
                                                  "The ecoregion {0} was previously used on line {1}",
                                                  ecoName.Value.String, lineNumber);
                else
                    lineNumbers[ecoregion.Name] = LineNumber;
                IEcoParameters ecoParms = new EcoParameters();
                if (parameters.EcoParameters[ecoregion.Index] != null)
                {
                    ecoParms = parameters.EcoParameters[ecoregion.Index];
                }
                ReadValue(enemyEdgeEffect, currentLine);
                ecoParms.EnemyEdgeEffect = enemyEdgeEffect.Value;
                parameters.EcoParameters[ecoregion.Index] = ecoParms;

                CheckNoDataAfter("the " + enemyEdgeEffect.Name + " column",
                                 currentLine);
                GetNextLine();
            }

            // Read Max Bud Density parameter
            InputVar<double> maxBudDensity = new InputVar<double>("MaxBudwormDensity");
            ReadVar(maxBudDensity);
            parameters.MaxBudDensity = maxBudDensity.Value;

            // Read host species list
            ReadName("HostSpeciesList");
            InputVar<string> sppName = new InputVar<string>("Species");
            while (!AtEndOfInput && CurrentName != "DeciduousSpeciesList")
            {
                currentLine = new StringReader(CurrentLine);
                ReadValue(sppName, currentLine);
                ISpecies species = PlugIn.ModelCore.Species[sppName.Value.Actual];

                if (species == null)
                    throw new InputValueException(sppName.Value.String,
                                                  "{0} is not a species name.",
                                                  sppName.Value.String);
                parameters.SBWHost[species] = true;
                GetNextLine();
            }

            // Read host species list
            ReadName("DeciduousSpeciesList");
            InputVar<string> sppName2 = new InputVar<string>("Species");
            while (!AtEndOfInput && CurrentName != "LogFile" && CurrentName != "PctDefolMapName" && CurrentName != "MortalityMapName" && CurrentName != "HostFoliageMapName" && CurrentName != "BudwormDensityMapName")
            {
                currentLine = new StringReader(CurrentLine);
                ReadValue(sppName2, currentLine);
                ISpecies species = PlugIn.ModelCore.Species[sppName2.Value.Actual];

                if (species == null)
                    throw new InputValueException(sppName2.Value.String,
                                                  "{0} is not a species name.",
                                                  sppName2.Value.String);
                parameters.Deciduous[species] = true;
                GetNextLine();
            }

            // Read optional output map file names
            InputVar<string> pctDefolMapName = new InputVar<string>("PctDefolMapName");
            if (ReadOptionalVar(pctDefolMapName))
            {
                parameters.PctDefolMapName = pctDefolMapName.Value;
            }

            InputVar<string> mortalityMapName = new InputVar<string>("MortalityMapName");
            if (ReadOptionalVar(mortalityMapName))
            {
                parameters.MortalityMapName = mortalityMapName.Value;
            }

            InputVar<string> hostFoliageMapName = new InputVar<string>("HostFoliageMapName");
            if (ReadOptionalVar(hostFoliageMapName))
            {
                parameters.HostFolMapName = hostFoliageMapName.Value;
            }

            InputVar<string> budwormDensMapName = new InputVar<string>("BudwormDensityMapName");
            if (ReadOptionalVar(budwormDensMapName))
            {
                parameters.BudwormDensMapName = budwormDensMapName.Value;
            }

            InputVar<string> enemyDensMapName = new InputVar<string>("EnemyDensityMapName");
            if (ReadOptionalVar(enemyDensMapName))
            {
                parameters.EnemyDensMapName = enemyDensMapName.Value;
            }

            // Read log file path
            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            CheckNoDataAfter(string.Format("the {0} parameter", logFile.Name));

            return parameters; //.GetComplete();
        }
    }
}
