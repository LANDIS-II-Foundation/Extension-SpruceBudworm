using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

  

namespace junk1
{
  

    class Program
    {
        static void Main(string[] args)
        {
            var s1 = Stopwatch.StartNew();

            var probability = new List<float>();
            var index = new List<int>();
            Random r = new Random();

            int nbPixels = 100000000;
            for (int i = 0; i < nbPixels; i++)
            {
                // Populate numbers with just even numbers.
                probability.Add((float)r.NextDouble());
                index.Add(i);

                if (i%(nbPixels/101) ==0)
                    Console.Write(".");
            }

            Console.WriteLine();
            Console.WriteLine("-----");
            
            var p1 = probability.ToArray();
            var i1 = index.ToArray();
            Array.Sort(p1,i1);


            s1.Stop();

            Console.WriteLine("Time to create Array of " + nbPixels + " pixels:" +((double)(s1.Elapsed.TotalSeconds)).ToString("0.00 s"));
            Console.WriteLine();


            var s2 = Stopwatch.StartNew();

            int nbEval = 10000000;
            float[] s = new float[nbEval];
            int[] ii = new int[nbEval];

            //Parallel.For(0, nbEval, i =>
            for(int i=0;i<nbEval;i++)
            {
                s[i] = (float)r.NextDouble();
                int pos = ~(Array.BinarySearch(p1, s[i]));

                //take lower bound
                if (ii[i] == 0)
                    ii[i] = i1[0];
                else
                    ii[i] = i1[pos - 1];

                //take upper bound
                //if (ii[i] == i1.Length)
                //    ii[i] = i1[i1.Length-1];
                //else
                //    ii[i] = i1[pos];

            }

            

            s2.Stop();

            Console.WriteLine("Time to search " + nbEval + " probability:" + ((double)(s2.Elapsed.TotalSeconds)).ToString("0.00 s"));
            Console.WriteLine();
           
        }
    }
}
