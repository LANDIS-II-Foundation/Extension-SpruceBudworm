using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;

namespace Landis.Extension.SpruceBudworm
{
    class Dispersal
    {
        private static Dictionary<double, double> dispersal_probability;
        private static Dictionary<double, double> cumulative_dispersal_probability;
        private static int max_dispersal_distance_pixels;

        public static void CalculateDispersal()
        {
            SiteVars.Dispersed.ActiveSiteValues = 0;
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                // Calculate LDD/SDD ratio (16)
                double LDDHabitat = 0;
                double LDDFlight = 0;
                double LDDRatio = 0;
                double siteDefol = SiteVars.TotalDefoliation[site]/SiteVars.CurrentHostFoliage[site];
                double minLDD = PlugIn.Parameters.EmigrationMinLDD;
                double halfLDD = PlugIn.Parameters.EmigrationHalfLDD;
                double maxLDD = PlugIn.Parameters.EmigrationMaxLDD;
                double maxLDDProp = PlugIn.Parameters.EmigrationMaxLDDProp;

                if ((siteDefol > minLDD) && (siteDefol <= halfLDD))
                {
                    double m1 = 0;
                    if ((halfLDD - minLDD) > 0)
                    {
                        m1 = 0.5 / (halfLDD - minLDD);
                    }
                    double b1 = 0.5 - (m1 * halfLDD);
                    LDDHabitat = m1 * siteDefol + b1;
                }
                else if ((siteDefol > halfLDD) && (siteDefol <= maxLDD))
                {
                    double m2 = 0;
                    if ((maxLDD - halfLDD) > 0)
                    {
                        m2 = 0.5 / (maxLDD - halfLDD);
                    }
                    double b2 = 1.0 - (m2 * maxLDD);
                    LDDHabitat = m2 * siteDefol + b2;
                }
                else if (siteDefol > maxLDD)
                    LDDHabitat = 1.0;

                double slope = (maxLDDProp - (1 - maxLDDProp)) / (1.0 - 0.46);
                double intercept = maxLDDProp - slope;
                double rprimeZ = (-0.0054 * SiteVars.PctDefoliation[site] + 1);
                if (PlugIn.Parameters.PositiveFecundDispersal)
                {
                    if (rprimeZ < 0.46)
                        LDDFlight = 0.0;
                    else
                        LDDFlight = slope * rprimeZ + intercept;
                }
                else
                {
                    double slope2 = (-1.0) * slope;
                    double intercept2 = (-1.0) * intercept + 1.0;
                    if (rprimeZ < 0.46)
                        LDDFlight = 1.0;
                    else
                        LDDFlight = slope2 * rprimeZ + intercept2;
                }
                LDDRatio = LDDHabitat * LDDFlight;

                // Calculate LDD dispersers (16b)
                double LDDout = (int) (SiteVars.EggCountFall[site] * LDDRatio);
                SiteVars.LDDout[site] = LDDout;
                SiteVars.SDDout[site] = SiteVars.EggCountFall[site] - LDDout;

            }
            PlugIn.ModelCore.UI.WriteLine("LD Dispersal of moths...");
            SiteVars.Disperse_n.ActiveSiteValues = 0;
            SiteVars.Disperse_v.ActiveSiteValues = 0;
            SiteVars.Dispersed.ActiveSiteValues = 0;
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                if (SiteVars.LDDout[site] > 0)
                {
                    // Distribute LDD dispersers (17)          
                    if (PlugIn.Parameters.LDDSpeedUp)
                    {
                        
                    }
                    else
                    {
                        DisperseLDD(site, PlugIn.Parameters.WrapLDD);
                    }
                }

                // Calculate SDD dispersers (16b)
                SiteVars.DisperseSDD(site, PlugIn.Parameters.SDDRadius, PlugIn.Parameters.SDDEdgeEffect);

            }
            foreach (Site site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                if (!PlugIn.Parameters.LDDSpeedUp)
                {
                    if (SiteVars.Disperse_n[site] > 0)
                    {
                        DisperseLDD_Part2(site);
                    }
                }

                SiteVars.BudwormCount[site] = SiteVars.EggCountFall[site] - SiteVars.LDDout[site] - SiteVars.SDDout[site] + SiteVars.Dispersed[site];
            }
        }

        public static void DisperseLDDSpeedUp(Site site, bool wrapLDD)
        {
            int disperseCount = (int)Math.Round(SiteVars.LDDout[site]);
            List<double> disperseList = new List<double>();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 1;
            double randNum = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            for (int i = 1; i <= disperseCount; i++)
            {                
                randNum = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

                foreach(KeyValuePair<double,double> entry in cumulative_dispersal_probability)
                {
                    double cumProb = entry.Value;
                    if (cumProb > randNum)
                    {
                        double dist = entry.Key;
                        disperseList.Add(dist);
                        break;
                    }
                  
                }

            }
            foreach(double distance in disperseList)
            {
                double randDir = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble() * Math.PI * 2;
                double dj = Math.Cos(randDir) * distance;
                double dk = Math.Sin(randDir) * distance;
                int j = (int)Math.Round(dj / PlugIn.ModelCore.CellLength);
                int k = (int)Math.Round(dk / PlugIn.ModelCore.CellLength);

                int target_x = site.Location.Column + j;
                int target_y = site.Location.Row + k;

                // wrapLDD causes dispersers to stay within the landscape by wrapping the dispersal vector around the landscape (i.e., torus)
                if (wrapLDD)
                {
                    int landscapeRows = PlugIn.ModelCore.Landscape.Rows;
                    int landscapeCols = PlugIn.ModelCore.Landscape.Columns;

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

                }
                RelativeLocation targetLocation = new RelativeLocation(target_y - site.Location.Row, target_x - site.Location.Column);
                Site targetSite = site.GetNeighbor(targetLocation);
                SiteVars.Dispersed[targetSite] ++;
            }
        }

        public static void DisperseLDD(Site site, bool wrapLDD)
        {
          
            double max_distance_dispersed = 0;
            //if (dispersal_type == Dispersal_Type.STATIC)
            //{
                int limit = max_dispersal_distance_pixels;
                for (int j = -limit; j <= limit; j++)
                {
                    for (int k = -limit; k <= limit; k++)
                    {
                        double r, p, dj, dk;
                        int target_x, target_y;
                        dj = j * PlugIn.ModelCore.CellLength;
                        dk = k * PlugIn.ModelCore.CellLength;

                        r = Math.Sqrt(dj * dj + dk * dk);
                        p = dispersal_probability[r];
                                                
                        target_x = site.Location.Column + j;
                        target_y = site.Location.Row + k;
                        // wrapLDD causes dispersers to stay within the landscape by wrapping the dispersal vector around the landscape (i.e., torus)
                        if(wrapLDD)
                        {
                            int landscapeRows = PlugIn.ModelCore.Landscape.Rows;
                            int landscapeCols = PlugIn.ModelCore.Landscape.Columns;

                            //remainRow=SIGN(C4)*MOD(ABS(C4),$B$1)
                            int remainRow = Math.Sign(k) * (Math.Abs(k) % landscapeRows);
                            int remainCol = Math.Sign(j) * (Math.Abs(j) % landscapeCols);
                            //tempY=A4+H4
                            int tempY = site.Location.Row + remainRow;
                            int tempX = site.Location.Column + remainCol;
                            //source_y=IF(J4<1,$B$1+J4,IF(J4>$B$1,MOD(J4,$B$1),J4))
                            if(tempY < 1)
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
                             
                        }
                        RelativeLocation targetLocation = new RelativeLocation(k, j);
                        Site targetSite = site.GetNeighbor(targetLocation);

                        if (targetSite.IsActive)
                        {
                            if (p * SiteVars.LDDout[site] >= 1 && r > max_distance_dispersed)
                                max_distance_dispersed = r;
                            SiteVars.Disperse_n[targetSite] = SiteVars.Disperse_n[targetSite] + p * SiteVars.LDDout[site];
                            SiteVars.Disperse_v[targetSite] = SiteVars.Disperse_v[targetSite] + p * (1 - p) * SiteVars.LDDout[site];
                        }
                    }
                }

            //}
        }


        public static void DisperseLDD_Part2(Site site)
        {
            PlugIn.ModelCore.NormalDistribution.Mu = 0;
            PlugIn.ModelCore.NormalDistribution.Sigma = 1;
            double randNorm = PlugIn.ModelCore.NormalDistribution.NextDouble();
            randNorm = PlugIn.ModelCore.NormalDistribution.NextDouble();
            double dispersed = Math.Max(0,SiteVars.Disperse_n[site] + randNorm * Math.Sqrt(SiteVars.Disperse_v[site]));
            /*// If stochastic dispersal of integers use below, otherwise decimal dispersal
            int disperseInt = (int)dispersed;
            if (dispersed < 1)
            {
                PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0.0;
                PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1.0;
                double randUnif = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
                randUnif = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
                if (randUnif < dispersed)
                    disperseInt = 1;
            }
             * */
            SiteVars.Dispersed[site] += dispersed;
        }

        //---------------------------------------------------------------------
       
        

        public static void Initialize()
        {
            dispersal_probability = new Dictionary<double, double>();
            cumulative_dispersal_probability = new Dictionary<double, double>();
            double max_dispersal_distance = max_dispersal_window();
            max_dispersal_distance_pixels = (int)(max_dispersal_distance / PlugIn.ModelCore.CellLength);
            dispersal_probability.Clear();
            cumulative_dispersal_probability.Clear();
            double total_p = 0;
            Dictionary<double, int> dispersal_prob_count = new Dictionary<double, int>(); ;
            for (int x = 0; x <= max_dispersal_distance_pixels; x++) // (int x = -all_species[s].max_dispersal_distance_pixels; x <= all_species[s].max_dispersal_distance_pixels; x++)
            {
                for (int y = x; y <= max_dispersal_distance_pixels; y++) // (int y = -all_species[s].max_dispersal_distance_pixels; y <= all_species[s].max_dispersal_distance_pixels; y++)
                {
                    double dx, dy, r, p;
                    dx = PlugIn.ModelCore.CellLength * x;
                    dy = PlugIn.ModelCore.CellLength * y;
                    r = Math.Sqrt(dx * dx + dy * dy);
                    p = dispersal_prob(x, y);
                    if (dispersal_probability.ContainsKey(r))
                    {
                        dispersal_probability[r] += p;
                        dispersal_prob_count[r]++;
                    }
                    else
                    {
                        dispersal_probability.Add(r, p);
                        dispersal_prob_count.Add(r, 1);
                    }

                    if (x == 0 && y == 0)
                    {
                        total_p += p;
                    }
                    else if (x == y || x == 0 || y == 0)
                    {
                        total_p += 4 * p;
                    }
                    else
                    {
                        total_p += 8 * p;
                    }
                }
            }

            double cumulative_prob = 0;
            foreach (double r in dispersal_prob_count.Keys)
            {
                dispersal_probability[r] = dispersal_probability[r] / dispersal_prob_count[r];
                cumulative_prob += dispersal_probability[r];
                cumulative_dispersal_probability[r] = cumulative_prob;
            }
        }
        private static double max_dispersal_window()
        {
            double step,
           max_dist, total_max_seeds, dist = 0, r = 0, n = 0, p = 0;

            // distance increment fro numeric integration
            // 100 evaluation should sufficient without over kill
            step = PlugIn.ModelCore.CellLength / 100;

            // upper limit on distance set to map diagonal
            max_dist = PlugIn.ModelCore.CellLength * Math.Sqrt(PlugIn.ModelCore.Landscape.Columns * PlugIn.ModelCore.Landscape.Columns + PlugIn.ModelCore.Landscape.Rows * PlugIn.ModelCore.Landscape.Rows);

            // maximum possible number of eggs to be dispersed by moths
            // Should this be succesion timestep?  If so how?
            total_max_seeds = 10000 * PlugIn.Parameters.Timestep * PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength;

            // stop when all seeds have been accounted for
            while (total_max_seeds - n > 1)
            {
                p = 0;
                for (int i = 1; i <= 100; i++)
                {
                    r = dist + step * i - 0.5 * step;
                    p += marginal_prob(r) * step;
                }

                // accumulate new seeds into n
                n += total_max_seeds * p;

                // increment distance
                dist = dist + PlugIn.ModelCore.CellLength ;

                // set bound to dist
                if (dist > max_dist)
                    dist = max_dist;
                if (dist < 0)
                    dist = 0;
            }
            return dist;
        }
        private static double marginal_prob(double r)
        {

            double mean1 = 5;
            double mean2 = 25;
            double weight1 = 0.5;

            double weight2 = 1 - weight1;
            //if (dispersal_model == Dispersal_Model.DOUBLE_EXPONENTIAL)
            // set bounds to means
            if (mean1 < 0)
                mean1 = 0;
            if (mean2 < 0)
                mean2 = 0;

            double c = r / ((weight1 * mean1 + weight2 * mean2));
            double p1 = (weight1 / mean1) * Math.Exp(-r / mean1);
            double p2 = (weight2 / mean2) * Math.Exp(-r / mean2);
            double prob = c * (p1 + p2);
            return prob;
        }

        private static double dispersal_prob( int x, int y)
        {
            int mc_draws = 10;
            double prob;
            double[] p = new double[mc_draws];
            double half_pixel_size = PlugIn.ModelCore.CellLength / 2;
            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1.0;
            double randUnif = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            for (int m = 0; m < mc_draws; m++)
            {
                double x0 = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble() * PlugIn.ModelCore.CellLength - half_pixel_size;
                double y0 = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble() * PlugIn.ModelCore.CellLength - half_pixel_size;
                double xl = x * PlugIn.ModelCore.CellLength + PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble() * PlugIn.ModelCore.CellLength - half_pixel_size;
                double yl = y * PlugIn.ModelCore.CellLength + PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble() * PlugIn.ModelCore.CellLength - half_pixel_size;
                p[m] = displacement_prob(x0, y0, xl, yl);
            }
            double p_mean = p.Average();
            prob = PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength * p_mean;

            // set bounds to prob
            if (prob > 1)
                prob = 1;
            if (prob < 0)
                prob = 0;

            return prob;
        }

        private static double displacement_prob(double x0, double y0, double xl, double yl)
        {
            double dx, dy, r, prob = 0;
            dx = xl - x0;   // displacement in x direction
            dy = yl - y0;   // displacement in y direction
            r = Math.Sqrt(dx * dx + dy * dy); // distance

            //if (dispersal_model == Dispersal_Model.DOUBLE_EXPONENTIAL)
            //{
                double w1, w2, mean1, mean2, c, part1, part2;
                w1 = PlugIn.Parameters.DispersalWeight1;
                w2 = 1 - w1;
                mean1 =  PlugIn.Parameters.DispersalMean1;
                mean2 = PlugIn.Parameters.DispersalMean2;

                // set bounds to means
                if (mean1 < 0)
                    mean1 = 0;
                if (mean2 < 0)
                    mean2 = 0;

                c = 1 / (2 * Math.PI * (w1 * mean1 + w2 * mean2));
                part1 = (w1 / mean1) * Math.Exp(-r / mean1);
                part2 = (w2 / mean2) * Math.Exp(-r / mean2);
                prob = c * (part1 + part2);
           // }
            /*else if (dispersal_model == Dispersal_Model.TWODT)
            {
                double a, b, part1, part2;
                a = s.dispersal_parameters[0];  // shape parameter
                b = s.dispersal_parameters[1];  // scale parameter
                part1 = a / (Math.PI * b);
                part2 = 1 + (r * r) / b;
                prob = part1 * 1 / (Math.Pow(part2, a + 1));
            }
            */
            // ... additional kernels can be added here ...

            // set bounds to prob
            if (prob < 0)
                prob = 0;

            return prob;
        }

        private static bool isInside(int x, int y) // check if the coordinates are inside the map
        {
            return (x > 0 && y > 0 && x < PlugIn.ModelCore.Landscape.Columns && y < PlugIn.ModelCore.Landscape.Rows);
        }
    }
}
