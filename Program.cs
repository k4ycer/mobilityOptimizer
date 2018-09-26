using System;

namespace mobilityOptimizer
{
    class Program
    {
        static void Main(string[] args)
        {
            double[][] xy = new double[][]{
                new double[] {6.4521, 0.9905},
                new double[] {4.7095, 5.3684},
                new double[] {2.6193, 9.9160},
                new double[] {1.9785, 8.6393},
                new double[] {6.5496, 3.4523}
            };
            int max_salesmen = 4;
            double[][] depots = new double[][]{
                new double[]{9.9159, 3.0997},
                new double[]{6.2633, 9.5936},
                new double[]{9.2927, 1.7393},
                new double[]{1.1341, 3.6955},
            };
            int CostType = 2;
            int min_tour = 1;
            int pop_size = 80;
            int num_iter = 1000;
            bool show_prog = true;
            bool show_res = false;
            double[][] dmat = new double[][]{
                new double[]{0, 4.7120, 9.7136, 8.8610, 2.4637},
                new double[]{4.7120, 0, 5.0050, 4.2611, 2.6566},
                new double[]{9.7136, 5.0050, 0, 1.4285, 7.5648},
                new double[]{8.8610, 4.2611, 1.4285, 0, 6.9137},
                new double[]{2.4637, 2.6566, 7.5648, 6.9137, 0}
            };

            // Mdmtspv_ga(xy, max_salesmen, depots, CostType, min_tour, pop_size, num_iter, show_prog, show_res, dmat);

            double[] Tour;

            CalcTourLength(Tour, dmat, d0, Tour.Length - 1);
        }

        static void Mdmtspv_ga(double[][] xy, int max_salesmen, double[][] depots, int CostType, int min_tour, int pop_size, int num_iter, bool show_prog, bool show_res, double[][] dmat)
        {
            // TODO: checar que Epsilon este bien
            double Epsilon = 1*Math.E-10;

            /* Distances to Depots
            Assumes that each salesman is located at a different depot and there are
            enough depots */
            int numOfCities = xy.Length;
            int dimensions = 2;
            double[][] D0 = new double[max_salesmen][];
            for(int i = 0; i < max_salesmen; i++){
                D0[i] = new double[numOfCities];
                for(int j = 0; j < numOfCities; j++){
                    double[] restaFila = substractVectors(depots[i], xy[j]);
                    D0[i][j] = Accord.Math.Norm.Euclidean(restaFila); 
                    Console.Write(D0[i][j] + " ");
                }
            }
            
            // Initialize the Populations
            double[][] pop_rte = new double[pop_size][];
            double[][] pop_brk = new double[pop_size][];
            for(int i = 0; i < pop_size; i++){
                pop_rte[i] = randperm(numOfCities);
                pop_brk[i] = randbreak(max_salesmen, numOfCities, min_tour);
            }
            
            Console.Write("");
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

        ///////////////////////////////////////////
        //
        // Helper functions
        //
        ///////////////////////////////////////////

        // Substract two vectors
        static double[] substractVectors(double[] vector1, double[] vector2){
            double[] res = new double[vector1.Length];
            for(int i = 0; i < vector1.Length; i++){
                res[i] = vector1[i] - vector2[i];
            }

            return res;
        }

        // Returns a vector permutation
        static double[] randperm(int num){
            double[] array = new double[num];
            Random random = new Random();
            int n = array.Length;
            for(int j = 0; j < num; j++){
                array[j] = j;
            }
            int i;
            double temp;
            while(n > 1){
                n--;
                i = random.Next(n + 1);
                temp = array[i];
                array[i] = array[n];
                array[n] = temp;
            }
            return array;
        }
}
