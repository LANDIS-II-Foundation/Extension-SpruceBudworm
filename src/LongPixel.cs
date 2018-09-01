//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Extension.SpruceBudworm
{
    public class LongPixel : Pixel
    {
        public Band<long> MapCode  = "The numeric code for each raster cell";

        public LongPixel()
        {
            SetBands(MapCode);
        }
    }
}
