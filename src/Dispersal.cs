using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;
using System.Diagnostics;

namespace Landis.Extension.SpruceBudworm
{
    class Pair
    {
        public double First { get; set; }
        public double Second { get; set; }
        public Pair(double first, double second)
        {
            this.First = first;
            this.Second = second;
        }
    }

    class Triplet
    {
        public double Dir { get; set; }
        public double Distance { get; set; }
        public double Prob { get; set; }

        public Triplet(double dir, double distance, double prob)
        {
            this.Dir = dir;
            this.Distance = distance;
            this.Prob = prob;
        }
    }
    class Dispersal
    {
        private static Dictionary<double,Dictionary<double, double>> dispersal_probability;
        private static List<Triplet> cumulative_dispersal_probability; //(dir,distance,prob)
        private static List<int> dispersalIndex;
        private static List<double> dispersalDir;
        private static List<double> dispersalDist;
        private static List<double> dispersalProb;
        private static Array indexArray;
        private static Array probArray;
        private static Dictionary<double,int> cum_prob_benchmarks; //(prob,index)
        private static Dictionary<double, int> cum_prob_benchmarks_tail; //(prob,index)
        private static Dictionary<double, int> cum_prob_benchmarks_tail2; //(prob,index)
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
                double rprimeZ = SiteVars.CalculateRprimeZ(SiteVars.PctDefoliation[site]);
                //double rprimeZ = (-0.0054 * SiteVars.PctDefoliation[site] + 1);
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

                // Calculate LDD dispersers (18b)

                double LDDout = SiteVars.EggCountFall[site] * LDDRatio; //Units: eggs/m2
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
                        //DisperseLDDSpeedUp(site, PlugIn.Parameters.WrapLDD);
                        DisperseLDDBinarySearch(site, PlugIn.Parameters.WrapLDD,PlugIn.Parameters.LDDEdgeWrapReduction_N, PlugIn.Parameters.LDDEdgeWrapReduction_E,PlugIn.Parameters.LDDEdgeWrapReduction_S,PlugIn.Parameters.LDDEdgeWrapReduction_W);
                    }
                    else
                    {
                        DisperseLDD(site, PlugIn.Parameters.WrapLDD);
                    }
                }

                // Calculate SDD dispersers (20)
                SiteVars.DisperseSDD(site, PlugIn.Parameters.SDDRadius, PlugIn.Parameters.SDDEdgeEffect, PlugIn.Parameters.EcoParameters);

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

        public static void DisperseLDDBinarySearch(Site site, bool wrapLDD, double lddEdgeWrapReduction_N, double lddEdgeWrapReduction_E, double lddEdgeWrapReduction_S, double lddEdgeWrapReduction_W)
        {
            //var s1 = Stopwatch.StartNew();
            int disperseCount = (int)Math.Round(SiteVars.LDDout[site]);

            List<Pair> disperseList = new List<Pair>();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1;
            double randNum = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            List<double> randList = new List<double>();

            for (int i = 1; i <= disperseCount; i++)
            {
                randNum = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
                randList.Add(randNum);
            }

            randList.Sort();

            for (int i = 0; i < randList.Count(); i++)
            {
                double randVal = randList[i];
                int pos = Math.Abs(~(Array.BinarySearch(probArray, randVal)));

                //take lower bound
                //if (pos == 0)
                //    pos = dispersalIndex[0];
                //else
                //    pos = dispersalIndex[pos - 1];

                Pair locationPair = new Pair(dispersalDir[pos], dispersalDist[pos]);
                disperseList.Add(locationPair);
            }

            //s1.Stop();
            //Console.WriteLine("Time to search " + randList.Count() + " probability:" + ((double)(s1.Elapsed.TotalSeconds)).ToString("0.0000 s"));
            //Console.WriteLine();


            foreach (Pair locationPair in disperseList)
            {
                double dir = locationPair.First;
                double distance = locationPair.Second;
                double dj = Math.Cos(dir) * distance;  // distance in x-direction (m)
                double dk = Math.Sin(dir) * distance;  // distance in y-direction (m)
                int j = (int)Math.Round(dj / PlugIn.ModelCore.CellLength);  // distance in x-direction (cells)
                int k = (int)Math.Round(dk / PlugIn.ModelCore.CellLength);  // distance in y-direction (cells)

                int target_x = site.Location.Column + j;
                int target_y = site.Location.Row + k;

                bool leftMapN = false;  //  Does dispersal leave the map to the North (wrap)?
                bool leftMapE = false;  //  Does dispersal leave the map to the East (wrap)?
                bool leftMapS = false;  //  Does dispersal leave the map to the South (wrap)?
                bool leftMapW = false;  //  Does dispersal leave the map to the West (wrap)?

                // wrapLDD causes dispersers to stay within the landscape by wrapping the dispersal vector around the landscape (i.e., torus)
                if (wrapLDD)
                {
                    int landscapeRows = PlugIn.ModelCore.Landscape.Rows;
                    int landscapeCols = PlugIn.ModelCore.Landscape.Columns;

                    if (target_x < 0)
                    {
                        leftMapW = true;  // Dispersal goes off the map to the West and wraps
                    }
                    if (target_y < 0)
                    {
                        leftMapN = true;  // Dispersal goes off the map to the North and wraps
                    }
                    if (target_x > landscapeCols)
                    {
                        leftMapE = true;  // Dispersal goes off the map to the East and wraps
                    }
                    if (target_y > landscapeRows)
                    {
                        leftMapS = true;  // Dispersal goes off the map to the South and wraps
                    }

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
                double addPop = 1;
                if (leftMapN)
                {
                    addPop = addPop * lddEdgeWrapReduction_S;  //Apply the reduction factor for the edge coming back onto the map (opposite of where it left)
                }
                if (leftMapE)
                {
                    addPop = addPop * lddEdgeWrapReduction_W;  //Apply the reduction factor for the edge coming back onto the map (opposite of where it left)
                }
                if (leftMapS)
                {
                    addPop = addPop * lddEdgeWrapReduction_N;  //Apply the reduction factor for the edge coming back onto the map (opposite of where it left)
                }
                if (leftMapW)
                {
                    addPop = addPop * lddEdgeWrapReduction_E;  //Apply the reduction factor for the edge coming back onto the map (opposite of where it left)
                }
                SiteVars.Dispersed[targetSite] = SiteVars.Dispersed[targetSite] + addPop;  //Add dispersed individual (or partial individual)
            }
        
        }

        public static void DisperseLDDSpeedUp(Site site, bool wrapLDD, double lddEdgeWrapReduction)
        {
            //var s1 = Stopwatch.StartNew();
            int disperseCount = (int)Math.Round(SiteVars.LDDout[site]);
            if (disperseCount > 0)
            {
                //PlugIn.ModelCore.UI.WriteLine("Site: " + site.ToString() + " LDD Dispersed:  " + disperseCount.ToString());
                List<Pair> disperseList = new List<Pair>();

                PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0;
                PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1;
                double randNum = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

                List<double> randList = new List<double>();

                for (int i = 1; i <= disperseCount; i++)
                {
                    randNum = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
                    randList.Add(randNum);
                }

                randList.Sort();
                int randIndex = 0;

                int probIndex = 0;
                while (randIndex < randList.Count)
                {
                    double randValue = randList[randIndex];
                    int tempProbIndex = probIndex;
                    if (randValue > cumulative_dispersal_probability[cumulative_dispersal_probability.Count() - 1].Prob)
                    {
                        probIndex = cumulative_dispersal_probability.Count() - 1;
                        Triplet myTriplet = cumulative_dispersal_probability[probIndex];
                        double cumProb = myTriplet.Prob;
                        Pair locationPair = new Pair(myTriplet.Dir, myTriplet.Distance);
                        disperseList.Add(locationPair);
                        randIndex++;
                    }
                    else
                    {
                        double randValRound = Math.Floor(randValue * 1000) / 1000;
                        if (randValRound > 0)
                        {
                            if (cum_prob_benchmarks.ContainsKey(randValRound))
                            {
                                tempProbIndex = cum_prob_benchmarks[randValRound];
                            }
                            else
                            {
                                randValRound = randValRound - 0.001;
                                if (cum_prob_benchmarks.ContainsKey(randValRound))
                                {
                                    tempProbIndex = cum_prob_benchmarks[randValRound];
                                }
                            }
                            if (randValue > 0.997)
                            {
                                if (randValue > 0.999997)
                                {
                                    double randValRoundTail2 = Math.Floor(randValue * 100000000) / 100000000;
                                    if (randValRoundTail2 > 0.99999999)
                                    {
                                        tempProbIndex = cum_prob_benchmarks_tail2[cum_prob_benchmarks_tail2.Keys.Max()];
                                    }
                                    else
                                    {
                                        
                                        if (cum_prob_benchmarks_tail2.ContainsKey(randValRoundTail2))
                                        {
                                            tempProbIndex = cum_prob_benchmarks_tail2[randValRoundTail2];
                                        }
                                        else
                                        {
                                            randValRoundTail2 = randValRoundTail2 - 0.00000001;
                                            if (cum_prob_benchmarks_tail2.ContainsKey(randValRoundTail2))
                                            {
                                                tempProbIndex = cum_prob_benchmarks_tail2[randValRoundTail2];
                                            }
                                        }
                                    }
                                
                                }
                                else
                                {
                                    double randValRoundTail = Math.Floor(randValue * 1000000) / 1000000;
                                    if (randValRoundTail > 0.997)
                                    {
                                        tempProbIndex = cum_prob_benchmarks_tail[cum_prob_benchmarks_tail.Keys.Max()];
                                    }
                                    else
                                    {
                                        if (cum_prob_benchmarks_tail.ContainsKey(randValRoundTail))
                                        {
                                            tempProbIndex = cum_prob_benchmarks_tail[randValRoundTail];
                                        }
                                        else
                                        {
                                            randValRoundTail = randValRoundTail - 0.000001;
                                            if (cum_prob_benchmarks_tail.ContainsKey(randValRoundTail))
                                            {
                                                tempProbIndex = cum_prob_benchmarks_tail[randValRoundTail];
                                            }
                                        }

                                    }
                                }
                            }
                            if (tempProbIndex > probIndex)
                                probIndex = tempProbIndex;
                        }

                        while (probIndex < cumulative_dispersal_probability.Count())
                        {
                            Triplet myTriplet = cumulative_dispersal_probability[probIndex];
                            double cumProb = myTriplet.Prob;
                            if (cumProb > randValue)
                            {
                                Pair locationPair = new Pair(myTriplet.Dir, myTriplet.Distance);
                                disperseList.Add(locationPair);
                                randIndex++;
                                break;
                            }
                            probIndex++;
                        }
                    }
                }
                //s1.Stop();
                //Console.WriteLine("Time to search " + randList.Count() + " probability:" + ((double)(s1.Elapsed.TotalSeconds)).ToString("0.0000 s"));
                //Console.WriteLine();

                /*foreach (Triplet myTriplet in cumulative_dispersal_probability)
                {
                    double cumProb = myTriplet.Prob;
                    if (cumProb > randList[randIndex])
                    {
                        Pair locationPair = new Pair(myTriplet.Dir, myTriplet.Distance);
                        disperseList.Add(locationPair);
                        randIndex++;
                        if (randIndex == randList.Count)
                            break;
                    }
                }*/

                foreach (Pair locationPair in disperseList)
                {
                    double dir = locationPair.First;
                    double distance = locationPair.Second;
                    double dj = Math.Cos(dir) * distance;  // distance in x-direction (m)
                    double dk = Math.Sin(dir) * distance;  // distance in y-direction (m)
                    int j = (int)Math.Round(dj / PlugIn.ModelCore.CellLength);  // distance in x-direction (cells)
                    int k = (int)Math.Round(dk / PlugIn.ModelCore.CellLength);  // distance in y-direction (cells)

                    int target_x = site.Location.Column + j;
                    int target_y = site.Location.Row + k;

                    bool leftMap = false;  //  Does dispersal leave the map (wrap)?

                    // wrapLDD causes dispersers to stay within the landscape by wrapping the dispersal vector around the landscape (i.e., torus)
                    if (wrapLDD)
                    {
                        int landscapeRows = PlugIn.ModelCore.Landscape.Rows;
                        int landscapeCols = PlugIn.ModelCore.Landscape.Columns;

                        
                        if(target_x < 0 || target_y < 0 || target_x > landscapeCols || target_y > landscapeRows)
                        {
                            leftMap = true;  // Dispersal goes off the map and wraps
                        }

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
                    if(leftMap)
                    {
                        SiteVars.Dispersed[targetSite] = SiteVars.Dispersed[targetSite] + lddEdgeWrapReduction;
                    }
                    else
                    {
                        SiteVars.Dispersed[targetSite]++;
                    }
                    
                    //SiteVars.Dispersed[targetSite] = SiteVars.Dispersed[targetSite] + 100; // Each moth carries 100 eggs
                }
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
                        double r, dir, p, dj, dk;
                        int target_x, target_y;
                        dj = j * PlugIn.ModelCore.CellLength;
                        dk = k * PlugIn.ModelCore.CellLength;

                        r = Math.Sqrt(dj * dj + dk * dk);
                    dir = Math.Atan2(dk, dj);
                    double round_dist = Math.Round(r, 8);
                    double round_dir = Math.Round(dir, 8);
                    if (dispersal_probability.ContainsKey(round_dir))
                    {
                        if (dispersal_probability[round_dir].ContainsKey(round_dist))
                        {
                            p = dispersal_probability[round_dir][round_dist];
                        }
                        else
                        {
                            string mesg = string.Format("No dispersal probability for direction: {0}, distance: {1}.", round_dir,round_dist);
                            throw new System.ApplicationException(mesg);
                        }
                    }
                    else
                    {
                        string mesg = string.Format("No dispersal probabilities for direction: {0}.", round_dir);
                        throw new System.ApplicationException(mesg);
                    }                    
                
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
                        //RelativeLocation targetLocation = new RelativeLocation(k, j);
                        RelativeLocation targetLocation = new RelativeLocation(target_y - site.Location.Row, target_x - site.Location.Column);
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
            PlugIn.ModelCore.UI.WriteLine("   Initializing dispersal kernel...");
            //dispersal_probability = new Dictionary<double, Dictionary<double, double>>(); //Key 1 = direction, Key2 = distance);
            dispersal_probability = DispersalKernelParser.InitializeFromFile(PlugIn.Parameters.DispersalFile);
            
            
            //cumulative_dispersal_probability = new Dictionary<double, Dictionary<double, double>>(); //Key 1 = direction, Key2 = distance
            cumulative_dispersal_probability = new List<Triplet>();
            /*
            cum_prob_benchmarks = new Dictionary<double, int>();
            cum_prob_benchmarks_tail = new Dictionary<double, int>();
            cum_prob_benchmarks_tail2 = new Dictionary<double, int>();
            */
            dispersalIndex = new List<int>();
            dispersalDir = new List<double>();
            dispersalDist = new List<double>();            
            dispersalProb = new List<double>();
            double cumulative_p = 0;
            int index = 0;
            List<double> dirKeys = new List<double>(dispersal_probability.Keys);

            foreach(double dir in dirKeys)
            {
                Dictionary<double,double> dirDict = dispersal_probability[dir];
                List<double> distKeys = new List<double>(dirDict.Keys);
                foreach(double dist in distKeys)
                {
                    double prob = dirDict[dist];
                    cumulative_p += prob;
                    dispersalIndex.Add(index);
                    index++;
                    dispersalDir.Add(dir);
                    dispersalDist.Add(dist);
                    dispersalProb.Add(cumulative_p);
                }

            }

            /*
            double max_dispersal_distance = max_dispersal_window();
            max_dispersal_distance_pixels = (int)(max_dispersal_distance / PlugIn.ModelCore.CellLength);
            dispersal_probability.Clear();
            cumulative_dispersal_probability.Clear();
            double total_p = 0;
            double cumulative_p = 0;
            int index = 0;
            Dictionary<double, int> dispersal_prob_count = new Dictionary<double, int>(); ;
            for (int x = 0; x <= max_dispersal_distance_pixels; x++) // (int x = -all_species[s].max_dispersal_distance_pixels; x <= all_species[s].max_dispersal_distance_pixels; x++)
            {
                for (int y = x; y <= max_dispersal_distance_pixels; y++) // (int y = -all_species[s].max_dispersal_distance_pixels; y <= all_species[s].max_dispersal_distance_pixels; y++)
                {
                    double dx, dy, r, p, dir;
                    dx = PlugIn.ModelCore.CellLength * x;
                    dy = PlugIn.ModelCore.CellLength * y;
                    r = Math.Sqrt(dx * dx + dy * dy);
                    p = dispersal_prob(x, y);                    
                    dir = Math.Asin(dy / r);
                    if (r == 0)
                        dir = 0;
                    if (x == 0 && y == 0)
                    {
                        cumulative_p += p;
                        total_p += p;
                        Triplet myTriplet = new Triplet(dir, r, cumulative_p);
                        cumulative_dispersal_probability.Add(myTriplet);
                        dispersalIndex.Add(index);
                        index++;
                        dispersalDir.Add(dir);
                        dispersalDist.Add(r);
                        dispersalProb.Add(cumulative_p);
                    }
                    else if (x == y || x == 0 || y == 0)
                    {
                        total_p += 4 * p;
                        for(int i=0;i<=3;i++)
                        {
                            cumulative_p += p;
                            double myDir = dir + i * (Math.PI / 2);
                            Triplet myTriplet = new Triplet(myDir, r, cumulative_p);
                            cumulative_dispersal_probability.Add(myTriplet);
                            dispersalIndex.Add(index);
                        index++;
                        dispersalDir.Add(dir);
                        dispersalDist.Add(r);
                        dispersalProb.Add(cumulative_p);
                        }
                    }
                    else
                    {
                        total_p += 8 * p;
                        for (int i = 0; i <= 7; i++)
                        {
                            cumulative_p += p;
                            double myDir = dir + i * (Math.PI / 4);
                            Triplet myTriplet = new Triplet(myDir, r, cumulative_p);
                            cumulative_dispersal_probability.Add(myTriplet);
                            dispersalIndex.Add(index);
                        index++;
                        dispersalDir.Add(dir);
                        dispersalDist.Add(r);
                        dispersalProb.Add(cumulative_p);
                        }
                    }
                    if (dispersal_probability.ContainsKey(r))
                    {
                        if(dispersal_probability[r].ContainsKey()
                        dispersal_probability[r] += p;
                        dispersal_prob_count[r]++;
                    }
                    else
                    {
                        dispersal_probability.Add(r, p);
                        dispersal_prob_count.Add(r, 1);
                    }
                    if(cumulative_dispersal_probability.ContainsKey(dir))
                    {
                        cumulative_dispersal_probability[dir].Add(r, cumulative_p);                        
                    }
                    else{
                        Dictionary<double,double> cum_disp_prob_dir = new Dictionary<double,double>();
                        cum_disp_prob_dir.Add(r, cumulative_p);
                        cumulative_dispersal_probability.Add(dir,cum_disp_prob_dir);
                    }
                     
                }
            }
            */
            indexArray = dispersalIndex.ToArray();
            
            double dispersalProbMax = dispersalProb.Max();
            List<double> dispersalProbAdj = dispersalProb.Select(z => z / dispersalProbMax).ToList();
            dispersalProbAdj[dispersalProbAdj.Count() - 1] = 1;
            probArray = dispersalProbAdj.ToArray();

            /*//double cumulative_prob = 0;
            foreach (double r in dispersal_prob_count.Keys)
            {
                dispersal_probability[r] = dispersal_probability[r] / dispersal_prob_count[r];
                //cumulative_prob += dispersal_probability[r];
                //cumulative_dispersal_probability[r] = cumulative_prob;
            }

            double mark = 0.001;            
            double mark_tail_start = 0.997001;
            double mark_tail = mark_tail_start;
            double mark_tail2_start = 0.99999701;
            double mark_tail2 = mark_tail2_start;
            int cumProbIndex = 0;
            foreach (Triplet myTriplet in cumulative_dispersal_probability)
            {
                if((myTriplet.Prob >= mark) && (myTriplet.Prob < mark_tail_start))
                {
                    cum_prob_benchmarks.Add(mark, cumProbIndex);
                    mark = Math.Round((mark + 0.001)* 1000) / 1000;
                }
                if ((myTriplet.Prob >= mark_tail) && (myTriplet.Prob < mark_tail2_start))
                {
                    cum_prob_benchmarks_tail.Add(mark_tail, cumProbIndex);
                    mark_tail = Math.Round((mark_tail + 0.000001) * 1000000) / 1000000;
                }
                if (myTriplet.Prob >= mark_tail2)
                {
                    cum_prob_benchmarks_tail2.Add(mark_tail2, cumProbIndex);
                    mark_tail2 = Math.Round((mark_tail2 + 0.00000001) * 100000000) / 100000000;
                    if (mark_tail2 >= 1.0000000000000)
                    {
                        break;
                    }
                }
                cumProbIndex++;
            }

            /*
            // For testing purposes
            string path1 = "C:/BRM/LANDIS_II/GitCode/Extension-SpruceBudworm/test/benchmarks.csv";
            string path2 = "C:/BRM/LANDIS_II/GitCode/Extension-SpruceBudworm/test/benchmarks_tail.csv";
            string path3 = "C:/BRM/LANDIS_II/GitCode/Extension-SpruceBudworm/test/benchmarks_tail2.csv";
            String csvBenchmarks = String.Join(Environment.NewLine, cum_prob_benchmarks.Select(d => d.Key.ToString() + "," + d.Value.ToString()).ToArray());
            String csvBenchmarksTail = String.Join(Environment.NewLine, cum_prob_benchmarks_tail.Select(d => d.Key.ToString() + "," + d.Value.ToString()).ToArray());
            String csvBenchmarksTail2 = String.Join(Environment.NewLine, cum_prob_benchmarks_tail2.Select(d => d.Key.ToString() + "," + d.Value.ToString()).ToArray());
            System.IO.File.WriteAllText(path1, csvBenchmarks);
            System.IO.File.WriteAllText(path2, csvBenchmarksTail);
            System.IO.File.WriteAllText(path3, csvBenchmarksTail2);
             */
            
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

            if (PlugIn.Parameters.WrapLDD)
            {
                max_dist = Double.PositiveInfinity;
            }
            // maximum possible number of moths to be dispersed 
            total_max_seeds = 500 * PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength ;

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

            double mean1 = PlugIn.Parameters.DispersalMean1;
            double mean2 = PlugIn.Parameters.DispersalMean2;
            double weight1 = PlugIn.Parameters.DispersalWeight1;

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
