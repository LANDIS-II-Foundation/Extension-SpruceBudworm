using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;

namespace Landis.Extension.SpruceBudworm
{
      public class RelativeLocationWeighted
        {
            private RelativeLocation location;
            private double weight;

            //---------------------------------------------------------------------
            public RelativeLocation Location
            {
                get
                {
                    return location;
                }
                set
                {
                    location = value;
                }
            }

            public double Weight
            {
                get
                {
                    return weight;
                }
                set
                {
                    weight = value;
                }
            }

            public RelativeLocationWeighted(RelativeLocation location, double weight)
            {
                this.location = location;
                this.weight = weight;
            }

        }
    
}
