// Copyright 2014 University of Notre Dame
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Landis.Utilities;
using Landis.Core;
using System.Collections.Generic;
using Landis.Library.Parameters;
using System;

namespace Landis.Extension.SpruceBudworm
{
    /// <summary>
    /// A parser that reads the extension's input and output parameters from
    /// a text file.
    /// </summary>
    public class DispersalKernelParser
        //: TextParser<InputParameters>
    {
        private ISpeciesDataset speciesDataset;
        private Dictionary<string, int> speciesLineNumbers;

        public static class Names
        {
            public const string LogMessage = "LogMessage";
            public const string ProbabilityTable = "ProbabilityTable";
        }

        //---------------------------------------------------------------------

        static DispersalKernelParser()
        {
            //ParsingUtils.RegisterForInputValues();
        }

        //---------------------------------------------------------------------

        public string LandisDataValue
        {
            get {
                return "DispersalKernel2D";
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DispersalKernelParser(ISpeciesDataset speciesDataset)
        {
            this.speciesDataset = speciesDataset;
            this.speciesLineNumbers = new Dictionary<string, int>();
        }

        //---------------------------------------------------------------------

        /*protected override bool Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != LandisDataValue)
            {
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", LandisDataValue);
                return false;
            }
            else
            {
                return true;
            }

        }
        */
        //---------------------------------------------------------------------
        // populating the dispersal probability lookup table
        public static Dictionary<double, Dictionary<double, double>> InitializeFromFile(string fileName)
        {
            Dictionary<double, Dictionary<double, double>> dispersalKernel = new Dictionary<double, Dictionary<double, double>>(); // dir, dist
            List<string> message = new List<string>();
            int sppIndex = 0;

            //TODO
            // Read LogMessage and write to Landis.log
         
                if (fileName == null)
                {
                    message.Add("No dispersal kernel file found: " + fileName + ".");
                }
                else
                {
                    // Read the file and display it line by line.  
                    string line;
                    bool startTable = false;
                    bool exceedMaxDist = false;
                    System.IO.StreamReader textFile = new System.IO.StreamReader(fileName);
                while ((line = textFile.ReadLine()) != null)
                {
                    if (startTable)
                    {
                        string[] words = line.Split(',');
                        double dist = Double.Parse(words[0]);
                        double dir = Double.Parse(words[1]);
                        double prob = Double.Parse(words[2]);
                        double round_dist = Math.Round(dist, 8);
                        double round_dir = Math.Round(dir, 8);
                        if (dispersalKernel.ContainsKey(round_dir))
                        {
                            Dictionary<double, double> dirDictionary = dispersalKernel[round_dir];
                            dirDictionary.Add(round_dist, prob);
                            dispersalKernel[round_dir] = dirDictionary;
                        }
                        else
                        {
                            Dictionary<double, double> dirDictionary = new Dictionary<double, double>();
                            dirDictionary.Add(round_dist, prob);
                            dispersalKernel.Add(round_dir, dirDictionary);
                        }
                       
                    }
                    if (line == "ProbabilityTable")
                    {
                        startTable = true;
                    }
                }

                    textFile.Close();
                }

            
            return dispersalKernel;
        }
        //---------------------------------------------------------------------
    }
}