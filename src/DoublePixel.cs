//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Extension.SpruceBudworm
{
    public class DoublePixel : Pixel
    {
        public Band<double> MapCode  = "The numeric code for each raster cell";

        public DoublePixel()
        {
            SetBands(MapCode);
        }
    }
}
