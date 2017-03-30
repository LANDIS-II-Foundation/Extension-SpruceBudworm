using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Extension.SpruceBudworm
{

    /// <summary>
    /// Extra Ecoregion Paramaters
    /// </summary>
    public interface IEcoParameters
    {
        string L2EdgeEffect { get; set; }
        string SDDEdgeEffect { get; set; }
        string EnemyEdgeEffect { get; set; }
    }

}

namespace Landis.Extension.SpruceBudworm
{
    public class EcoParameters
        : IEcoParameters
    {
        private string l2EdgeEffect;
        private string sddEdgeEffect;
        private string enemyEdgeEffect;
        //---------------------------------------------------------------------
        /// <summary>
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
        public EcoParameters()
        {
        }
        //---------------------------------------------------------------------
    }
}
