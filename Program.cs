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

        static double CalcTourLength(double[] Tour, double[][] dmat, double[][] d0, int indeces) {
            double VehicleTourLength = d0[Tour[1], Tour[2]];

            for (int c = 2; c <= indeces-1; c++)
            {
                VehicleTourLength = VehicleTourLength + dmat[Tour[c+1]][Tour[c]];
            }

            VehicleTourLength = VehicleTourLength + d0[Tour[indices+1]][Tour[indeces]];

            return VehicleTourLength;
        }

        static double[][] CalcRange(double[] p_brk, int n, bool flag) {
            double[][] rng = new double[p_brk.Length, 2];
            
            int i;

            for (i = 0; i < p_brk.Length; i++)
            {
                if (flag && p_brk[i] > 1) {
                    rng[i][0] = 1;
                    rng[i][1] = p_brk[i];

                    flag = false;
                } else if (flag) {
                    rng[i][0] = 1;
                    rng[i][1] = 0;
                } else if (p_brk[i] <= p_brk[i - 1]) {
                    rng[i][0] = p_brk[i-1];
                    rng[i][1] = p_brk[i];
                } else if (i < p_brk.Length) {
                    rng[i][0] = p_brk[i-1] + 1;
                    rng[i][1] = p_brk[i];
                } else {
                    rng[i][0] = p_brk[i-1] + 1;
                    rng[i][1] = p_brk[i];
                }
            }

            if (p_brk[p_brk.Length - 1] < n && p_brk[p_brk.Length - 1] != 1) {
                rng[i + 1][0] = p_brk[p_brk.Length - 1] + 1;
                rng[i + 1][1] = n;
            } else if (p_brk[p_brk.Length - 1] < n && p_brk[p_brk.Length - 1] == 1) {
                rng[i + 1][0] = p_brk[p_brk.Length - 1];
                rng[i + 1][1] = n;
            } else {
                rng[i + 1][0] = p_brk[p_brk.Length-1];
                rng[i + 1][1] = n - 1;
            }

            return rng;
        }

        static double[] randbreak(int max_salesmen, int n, int min_tour) {
            double[] breaks;

            int     num_brks = max_salesmen - 1;
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
