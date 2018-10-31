//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

using System.Collections.Generic;

namespace Landis.Extension.SpruceBudworm
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public interface IInputParameters
	{
		/// <summary>
		/// Timestep (years)
		/// </summary>
		int Timestep
		{
			get;set;
		}
        int BudwormStartYear
        { get; set; }

        string InitEnemyDensMap
		{
			get;set;
		}
        double MaxReproEnemy
		{
			get;set;
		}
        double EnemyParamb
		{
			get;set;
		}
        double EnemyParamc
		{
			get;set;
		}
        string InitSBWDensMap
		{
			get;set;
		}
        double MaxReproSBW
		{
			get;set;
		}
        double SBWParama
		{
			get;set;
		}
        double SBWParamb
		{
			get;set;
		}
        double SBWParamc
		{
			get;set;
		}
        double OverwinterMean
        {
            get;set;
        }
        double OverwinterStdev
        {
            get;
            set;
        }
        bool OverwinterConstant
        {
            get;
            set;
        }
        double FecundityMean
        {
            get;
            set;
        }
        double FecundityStdev
        {
            get;
            set;
        }
        bool FecundityConstant
        {
            get;
            set;
        }
        double MatingEffectA
        {
            get;
            set;
        }
        double MatingEffectB
        {
            get;
            set;
        }
        double MatingEffectC
        {
            get;
            set;
        }
        double DecidProtectMax1
        { get; set; }
        double DecidProtectMax2
        { get; set; }
        double PhenolMean
        {
            get;
            set;
        }
        double PhenolStdev
        {
            get;
            set;
        }
        bool PhenolConstant
        {
            get;
            set;
        }
        bool DefolFecundReduction
        { get; set; }
        bool GrowthReduction
        { get; set; }
        bool Mortality
        { get; set; }
        double DefolLambda
        { get; set; }
        double SDDRadius
        { get; set; }
        string SDDEdgeEffect
        { get; set; }
        double EmigrationMinLDD
        { get; set; }
        double EmigrationHalfLDD
        { get; set; }
        double EmigrationMaxLDD
        { get; set; }
        double EmigrationMaxLDDProp
        { get; set; }
        double DispersalMean1
        { get; set; }
        double DispersalMean2
        { get; set; }
        double DispersalWeight1
        { get; set; }
        bool WrapLDD
        { get; set; }
        bool LDDSpeedUp
        { get; set; }
        double LDDEdgeWrapReduction_N
        { get; set; }
        double LDDEdgeWrapReduction_E
        { get; set; }
        double LDDEdgeWrapReduction_S
        { get; set; }
        double LDDEdgeWrapReduction_W
        { get; set; }
        bool PositiveFecundDispersal
        { get; set; }
        int MinSusceptibleAge
        { get; set; }
        double L2FilterRadius
        { get; set; }
        string L2EdgeEffect
        { get; set; }
        double EnemyFilterRadius
        { get; set; }
        double EnemyDispersalProp
        { get; set; }
        string EnemyEdgeEffect
        { get; set; }
        double EnemyBiasedProp
        { get; set; }
        double EnemyEdgeWrapReduction
        { get; set; }
        Landis.Library.Parameters.Species.AuxParm<bool> SBWHost
        { get; set; }
        Landis.Library.Parameters.Species.AuxParm<bool> Deciduous
        { get; set; }
        double PredM
        { get; set; }
        double PredN
        { get; set; }
        double PreyM
        { get; set; }
        double PreyN
        { get; set; }
        double MaxBudDensity
        { get; set; }
        /// <summary>
        /// Name of PctDefol map.
        /// </summary>
        string PctDefolMapName
        {
            get;set;
        }
        /// <summary>
        /// Name of Moratlity map.
        /// </summary>
        string MortalityMapName
        {
            get;
            set;
        }
        /// <summary>
        /// Name of HostFoliage map.
        /// </summary>
        string HostFolMapName
        {
            get;
            set;
        }
        /// <summary>
        /// Name of budworm density map.
        /// </summary>
        string BudwormDensMapName
        {
            get;
            set;
        }
        /// <summary>
        /// Name of enemy density map.
        /// </summary>
        string EnemyDensMapName
        {
            get;
            set;
        }
		/// <summary>
		/// Name of log file.
		/// </summary>
		string LogFileName
		{
			get;set;
		}
        /// <summary>
        /// <summary>
        /// Ecoregion edge effects
        /// </summary>
        IEcoParameters[] EcoParameters
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
	}
}
