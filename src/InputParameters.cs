//  Authors:    Brian Miranda

using Landis.Utilities;
using System.Collections.Generic;

namespace Landis.Extension.SpruceBudworm
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public class InputParameters
		: IInputParameters
	{
		private int timestep;
        private int budwormStartYear;
        private string initEnemyDensMap;
        private double maxReproEnemy;
        private double enemyParamb;
        private double enemyParamc;
        private string initSBWDensMap;
        private double maxReproSBW;
        private double sbwParama;
        private double sbwParamb;
        private double sbwParamc;
		private string logFileName;
        private double overwinterMean;
        private double overwinterStdev;
        private bool overwinterConstant;
        private double fecundityMean;
        private double fecundityStdev;
        private bool fecundConstant;
        private double matingEffectA;
        private double matingEffectB;
        private double matingEffectC;
        private double decidProtectMax1;
        private double decidProtectMax2;
        private double phenolMean;
        private double phenolStdev;
        private bool phenolConstant;
        private bool defolFecundReduction;
        private bool growthReduction;
        private bool mortality;
        private double defolLambda;
        private double sddRadius;
        private string sddEdgeEffect;
        private double emigrationMinLDD;
        private double emigrationHalfLDD;
        private double emigrationMaxLDD;
        private double emigrationMaxLDDProp;
        private double dispersalMean1;
        private double dispersalMean2;
        private double dispersalWeight1;
        private string dispersalFile;
        private bool wrapLDD;
        private bool lddSpeedUp;
        private double lddEdgeWrapReduction_N;
        private double lddEdgeWrapReduction_E;
        private double lddEdgeWrapReduction_S;
        private double lddEdgeWrapReduction_W;
        private bool positiveFecundDispersal;
        private int minSusceptibleAge;
        private double l2FilterRadius;
        private string l2EdgeEffect;
        private double enemyFilterRadius;
        private double enemyDispersalProp;
        private string enemyEdgeEffect;
        private double enemyBiasedProp;
        private double enemyEdgeWrapReduction_N;
        private double enemyEdgeWrapReduction_E;
        private double enemyEdgeWrapReduction_S;
        private double enemyEdgeWrapReduction_W;
        private Landis.Library.Parameters.Species.AuxParm<bool> sbwHost;
        private Landis.Library.Parameters.Species.AuxParm<bool> deciduous;
        private string pctDefolMapName;
        private string mortalityMapName;
        private string hostFolMapName;
        private string budwormDensMapName;
        private string enemyDensMapName;
        private double predM = 1.0;
        private double predN = 1.0;
        private double preyM = 1.0;
        private double preyN = 1.0;
        private double maxBudDensity;
        private IEcoParameters[] ecoParameters;


		/// <summary>
		/// Timestep (years)
		/// </summary>
		public int Timestep
		{
			get {
				return timestep;
			}
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                      "Value must be = or > 0.");
                timestep = value;
            }
		}
        //---------------------------------------------------------------------
        /// <summary>
        /// Start year for budworm
        /// </summary>
        public int BudwormStartYear
        {
            get
            {
                return budwormStartYear;
            }
            set
            {
                budwormStartYear = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// <summary>
        /// Initial enemy density (larvae per budworm).
        /// </summary>
        public string InitEnemyDensMap
        {
            get
            {
                return initEnemyDensMap;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                initEnemyDensMap = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Max reproductive rate for natural enemies.
        /// </summary>
        public double MaxReproEnemy
        {
            get
            {
                return maxReproEnemy;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                maxReproEnemy = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Natural enemy population parameters b.
        /// </summary>
        public double EnemyParamb
        {
            get
            {
                return enemyParamb;
            }
            set
            {
                enemyParamb = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Natural enemy population parameters c.
        /// </summary>
        public double EnemyParamc
        {
            get
            {
                return enemyParamc;
            }
            set
            {
                enemyParamc = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Initial budworm density (budworm per branch).
        /// </summary>
        public string InitSBWDensMap
        {
            get
            {
                return initSBWDensMap;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                initSBWDensMap = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Max reproductive rate for natural enemies.
        /// </summary>
        public double MaxReproSBW
        {
            get
            {
                return maxReproSBW;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                maxReproSBW = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// SBW population parameters a'.
        /// </summary>
        public double SBWParama
        {
            get
            {
                return sbwParama;
            }
            set
            {
                sbwParama = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// SBW population parameters b'.
        /// </summary>
        public double SBWParamb
        {
            get
            {
                return sbwParamb;
            }
            set
            {
                sbwParamb = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// SBW population parameters c'.
        /// </summary>
        public double SBWParamc
        {
            get
            {
                return sbwParamc;
            }
            set
            {
                sbwParamc = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Overwinter random function (normal dist) mean.
        /// </summary>
        public double OverwinterMean
        {
            get
            {
                return overwinterMean;
            }
            set
            {
                overwinterMean = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Overwinter random function (normal dist) standard deviation.
        /// </summary>
        public double OverwinterStdev
        {
            
            get
            {
                return overwinterStdev;
            }
            set
            {
                overwinterStdev = value;
            }
        
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Overwinter function is constant (boolean).
        /// </summary>
        public bool OverwinterConstant
        {
            get
            {
                return overwinterConstant;
            }
            set
            {
                overwinterConstant = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fecundity random function (normal dist) mean.
        /// </summary>
        public double FecundityMean
        {
            get
            {
                return fecundityMean;
            }
            set
            {
                fecundityMean = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fecundity random function (normal dist) stdev.
        /// </summary>
        public double FecundityStdev
        {
            get
            {
                return fecundityStdev;
            }
            set
            {
                fecundityStdev = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fecundity function is constant (boolean).
        /// </summary>
        public bool FecundityConstant
        {
            get
            {
                return fecundConstant;
            }
            set
            {
                fecundConstant = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Mating effect parameter a.
        /// </summary>
        public double MatingEffectA
        {
            get
            {
                return matingEffectA;
            }
            set
            {
                matingEffectA = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Mating effect parameter b.
        /// </summary>
        public double MatingEffectB
        {
            get
            {
                return matingEffectB;
            }
            set
            {
                matingEffectB = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Mating effect parameter c.
        /// </summary>
        public double MatingEffectC
        {
            get
            {
                return matingEffectC;
            }
            set
            {
                matingEffectC = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Maximum Deciduous effect on dispersal loss.
        /// </summary>
        public double DecidProtectMax1
        {
            get
            {
                return decidProtectMax1;
            }
            set
            {
                decidProtectMax1 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Maximum Deciduous effect on parasite community composition.
        /// </summary>
        public double DecidProtectMax2
        {
            get
            {
                return decidProtectMax2;
            }
            set
            {
                decidProtectMax2 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Phenological limitation random function (normal dist) mean.
        /// </summary>
        public double PhenolMean
        {
            get
            {
                return phenolMean;
            }
            set
            {
                phenolMean = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Phenological limitation random function (normal dist) stdev.
        /// </summary>
        public double PhenolStdev
        {
            get
            {
                return phenolStdev;
            }
            set
            {
                phenolStdev = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Phenological limitation function is constant (boolean).
        /// </summary>
        public bool PhenolConstant
        {
            get
            {
                return phenolConstant;
            }
            set
            {
                phenolConstant = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Defoliation Fecundity Reduction.
        /// </summary>
        public bool DefolFecundReduction
        {
            get
            {
                return defolFecundReduction;
            }
            set
            {
                    defolFecundReduction = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Growth Reduction.
        /// </summary>
        public bool GrowthReduction
        {
            get
            {
                return growthReduction;
            }
            set
            {
                    growthReduction = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Mortality.
        /// </summary>
        public bool Mortality
        {
            get
            {
                return mortality;
            }
            set
            {
                    mortality = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Defoliation Lambda.
        /// </summary>
        public double DefolLambda
        {
            get
            {
                return defolLambda;
            }
            set
            {
                defolLambda = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Radius of adult SDD dispersal (m)
        /// </summary>
        public double SDDRadius
        {
            get
            {
                return sddRadius;
            }
            set
            {
                sddRadius = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Edge effect parameter
        /// Determine how edge is treated for short distance dispersal of adult budworm
        /// Same:  Nonactive cells have same number as focal cell
        /// Absorbed:  Nonactive cells are sinks and provide nothing to other cells
        /// Reflected: Nonactive cells do not receive anything, all is dispersed among active cells
        /// Biased:  Budworm counts dispersed in proportion to total host foilage within neighborhood
        /// AvgBiased:  Budworm counts dispersed in proportion to total host foilage within neighborhood, but nonactives assumed to have same eggs to disperse and same avg host foliage as rest of neighborhood
        /// </summary>
        public string SDDEdgeEffect
        {
            get
            {
                return sddEdgeEffect;
            }
            set
            {
                sddEdgeEffect = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Emigration Mininum Long Distance Dispersal (LDD) proportion
        /// </summary>
        public double EmigrationMinLDD
        {
            get
            {
                return emigrationMinLDD;
            }
            set
            {
                emigrationMinLDD = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Emigration Half Long Distance Dispersal (LDD) proportion
        /// </summary>
        public double EmigrationHalfLDD
        {
            get
            {
                return emigrationHalfLDD;
            }
            set
            {
                emigrationHalfLDD = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Emigration Maximum Long Distance Dispersal (LDD) proportion
        /// </summary>
        public double EmigrationMaxLDD
        {
            get
            {
                return emigrationMaxLDD;
            }
            set
            {
                emigrationMaxLDD = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Emigration Maximum Long Distance Dispersal (LDD) proportion
        /// </summary>
        public double EmigrationMaxLDDProp
        {
            get
            {
                return emigrationMaxLDDProp;
            }
            set
            {
                emigrationMaxLDDProp = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Long Distance Dispersal (LDD) mean dispersal distance 1 for mixed exponential
        /// </summary>
        public double DispersalMean1
        {
            get
            {
                return dispersalMean1;
            }
            set
            {
                dispersalMean1 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Long Distance Dispersal (LDD) mean dispersal distance 2 for mixed exponential
        /// </summary>
        public double DispersalMean2
        {
            get
            {
                return dispersalMean2;
            }
            set
            {
                dispersalMean2 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Long Distance Dispersal (LDD) weight for dispersal component 1 for mixed exponential
        /// </summary>
        public double DispersalWeight1
        {
            get
            {
                return dispersalWeight1;
            }
            set
            {
                dispersalWeight1 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Long Distance Dispersal (LDD) 2-dimensional probablity file
        /// </summary>
        public string DispersalFile
        {
            get
            {
                return dispersalFile;
            }
            set
            {
                dispersalFile = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Turn on/off wrapping of long distance dispersers to keep them on the map (mass balance)
        /// </summary>
        public bool WrapLDD
        {
            get
            {
                return wrapLDD;
            }
            set
            {
                wrapLDD = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Turn on/off faster dispersal calcs
        /// </summary>
        public bool LDDSpeedUp
        {
            get
            {
                return lddSpeedUp;
            }
            set
            {
                lddSpeedUp = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Reduction in survival due to dispersal off N map edge
        /// </summary>
        public double LDDEdgeWrapReduction_N
        {
            get
            {
                return lddEdgeWrapReduction_N;
            }
            set
            {
                lddEdgeWrapReduction_N = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Reduction in survival due to dispersal off E map edge
        /// </summary>
        public double LDDEdgeWrapReduction_E
        {
            get
            {
                return lddEdgeWrapReduction_E;
            }
            set
            {
                lddEdgeWrapReduction_E = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Reduction in survival due to dispersal off S map edge
        /// </summary>
        public double LDDEdgeWrapReduction_S
        {
            get
            {
                return lddEdgeWrapReduction_S;
            }
            set
            {
                lddEdgeWrapReduction_S = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Reduction in survival due to dispersal off W map edge
        /// </summary>
        public double LDDEdgeWrapReduction_W
        {
            get
            {
                return lddEdgeWrapReduction_W;
            }
            set
            {
                lddEdgeWrapReduction_W = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Positive Fecundity effect on LD Dispersal.
        /// </summary>
        public bool PositiveFecundDispersal
        {
            get
            {
                return positiveFecundDispersal;
            }
            set
            {
                    positiveFecundDispersal = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Minimum susceptible age for host
        /// </summary>
        public int MinSusceptibleAge
        {
            get
            {
                return minSusceptibleAge;
            }
            set
            {
                minSusceptibleAge = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Radius of L2 dispersal (m)
        /// </summary>
        public double L2FilterRadius
        {
            get
            {
                return l2FilterRadius;
            }
            set
            {
                l2FilterRadius = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Edge effect parameter
        /// Determine how edge is treated for local dispersal of budworm 
        /// Same:  Nonactive cells have same number as focal cell
        /// Absorbed:  Nonactive cells are sinks and provide nothing to other cells
        /// Reflected: Nonactive cells do not receive anything, all is dispersed among active cells
        /// </summary>
        public string L2EdgeEffect
        {
            get
            {
                return l2EdgeEffect;
            }
            set
            {
                l2EdgeEffect = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Radius of enemy dispersal (m)
        /// </summary>
        public double EnemyFilterRadius
        {
            get
            {
                return enemyFilterRadius;
            }
            set
            {
                enemyFilterRadius = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Proportion of natural enemies that disperse
        /// </summary>
        public double EnemyDispersalProp
        {
            get
            {
                return enemyDispersalProp;
            }
            set
            {
                enemyDispersalProp = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Edge effect parameter
        /// Determine how edge is treated for local dispersal of enemies
        /// Same:  Nonactive cells have same number as focal cell
        /// Absorbed:  Nonactive cells are sinks and provide nothing to other cells
        /// Reflected: Nonactive cells do not receive anything, all is dispersed among active cells
        /// Biased:  Enemies dispersed in proportion to total spruce budworm density within neighborhood
        /// AvgBiased:  Enemies dispersed in proportion to total spruce budworm density within neighborhood, but nonactives assumed to have same enemies to disperse and same avg spruce budworm density as rest of neighborhood
        /// </summary>
        public string EnemyEdgeEffect
        {
            get
            {
                return enemyEdgeEffect;
            }
            set
            {
                enemyEdgeEffect = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Enemy proportion biased dispersal
        /// </summary>
        public double EnemyBiasedProp
        {
            get
            {
                return enemyBiasedProp;
            }
            set
            {
                enemyBiasedProp = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Enemy reduction in dispersal survival due to dispersal off the N edge of the map (wrapping)
        /// </summary>
        public double EnemyEdgeWrapReduction_N
        {
            get
            {
                return enemyEdgeWrapReduction_N;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "EnemyEdgeWrapReduction_N must be >= 0.");
                enemyEdgeWrapReduction_N = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Enemy reduction in dispersal survival due to dispersal off the E edge of the map (wrapping)
        /// </summary>
        public double EnemyEdgeWrapReduction_E
        {
            get
            {
                return enemyEdgeWrapReduction_E;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "EnemyEdgeWrapReduction_E must be >= 0.");
                enemyEdgeWrapReduction_E = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Enemy reduction in dispersal survival due to dispersal off the S edge of the map (wrapping)
        /// </summary>
        public double EnemyEdgeWrapReduction_S
        {
            get
            {
                return enemyEdgeWrapReduction_S;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "EnemyEdgeWrapReduction_S must be >= 0.");
                enemyEdgeWrapReduction_S = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Enemy reduction in dispersal survival due to dispersal off the W edge of the map (wrapping)
        /// </summary>
        public double EnemyEdgeWrapReduction_W
        {
            get
            {
                return enemyEdgeWrapReduction_W;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "EnemyEdgeWrapReduction_W must be >= 0.");
                enemyEdgeWrapReduction_W = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Species is host for SBW (boolean).
        /// </summary>
        public Landis.Library.Parameters.Species.AuxParm<bool> SBWHost
        {
            get
            {
                return sbwHost;
            }
            set
            {
                sbwHost = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Species is deciduous (boolean).
        /// </summary>
        public Landis.Library.Parameters.Species.AuxParm<bool> Deciduous
        {
            get
            {
                return deciduous;
            }
            set
            {
                deciduous = value;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Predator scaling parameter m.
        /// </summary>
        public double PredM
        {
            get
            {
                return predM;
            }
            set
            {
                predM = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Predator scaling parameter n.
        /// </summary>
        public double PredN
        {
            get
            {
                return predN;
            }
            set
            {
                predN = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Prey scaling parameter m.
        /// </summary>
        public double PreyM
        {
            get
            {
                return preyM;
            }
            set
            {
                preyM = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Prey scaling parameter n.
        /// </summary>
        public double PreyN
        {
            get
            {
                return preyN;
            }
            set
            {
                preyN = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Assumed budworm density when host foliage = 0.
        /// </summary>
        public double MaxBudDensity
        {
            get
            {
                return maxBudDensity;
            }
            set
            {
                maxBudDensity = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of PctDefol map.
        /// </summary>
        public string PctDefolMapName
        {
            get
            {
                return pctDefolMapName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                MapNames.CheckTemplateVars(value);
                pctDefolMapName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of mortality map.
        /// </summary>
        public string MortalityMapName
        {
            get
            {
                return mortalityMapName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                MapNames.CheckTemplateVars(value);
                mortalityMapName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of host foliage map.
        /// </summary>
        public string HostFolMapName
        {
            get
            {
                return hostFolMapName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                MapNames.CheckTemplateVars(value);
                hostFolMapName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of budworm density map.
        /// </summary>
        public string BudwormDensMapName
        {
            get
            {
                return budwormDensMapName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                MapNames.CheckTemplateVars(value);
                budwormDensMapName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of enemy density map.
        /// </summary>
        public string EnemyDensMapName
        {
            get
            {
                return enemyDensMapName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                MapNames.CheckTemplateVars(value);
                enemyDensMapName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
		/// Name of log file.
		/// </summary>
		public string LogFileName
		{
			get {
				return logFileName;
			}
            set {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                logFileName = value;
            }
		}

        //---------------------------------------------------------------------
        /// <summary>
        /// Ecoregion edge effects
        /// </summary>
        /// 		/// <remarks>
        /// Use Ecoregion.Index property to index this array.
        /// </remarks>
        public IEcoParameters[] EcoParameters
        {
            get
            {
                return ecoParameters;
            }
            set
            {
                ecoParameters = value;
            }
        }
        //---------------------------------------------------------------------
        public InputParameters(int ecoregionCount)
        {
            sbwHost = new Library.Parameters.Species.AuxParm<bool>(PlugIn.ModelCore.Species);
            deciduous = new Library.Parameters.Species.AuxParm<bool>(PlugIn.ModelCore.Species);
            EcoParameters = new IEcoParameters[ecoregionCount];
            for (int i = 0; i < ecoregionCount; i++)
                EcoParameters[i] = new EcoParameters();
        }

	}
}
