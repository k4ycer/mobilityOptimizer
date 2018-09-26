using System;

namespace mobilityOptimizer
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        static void Mdmtspv_ga(double[,] xy, int max_salesmen, double depots, int CostType, int min_tour, int pop_size, int num_iter, bool show_prog, bool show_res, double[,] dmat){
            
        }

        static double[] randbreak(int max_salesmen, int n, int min_tour) {
            double[] breaks;

            int num_brks = max_salesmen - 1;
            breaks = new double[num_brks];

            for (int i = 0; i < num_brks; i++)
            {
                double newRand = new Random().Next(1, n);

                foreach (double item in breaks)
                {
                    if (item == newRand) 
                    {
                        i--;
                        continue;
                    }
                }

                breaks[i] = newRand;
            }

            Array.Sort(breaks);

            return breaks;
        }
    }
}
