﻿using System;
using System.Numerics;
using Accord.Math;

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

            Mdmtspv_ga(xy, max_salesmen, depots, CostType, min_tour, pop_size, num_iter, show_prog, show_res, dmat);
        }

        static void Mdmtspv_ga(double[][] xy, int max_salesmen, double[][] depots, int CostType, int min_tour, int pop_size, int num_iter, bool show_prog, bool show_res, double[][] dmat){
            ///////////////////////////////////////////
            //
            // Initialize
            //
            ///////////////////////////////////////////
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
                }
            }
            
            // Initialize the Populations
            double[][] pop_rte = new double[pop_size][];
            double[][] pop_brk = new double[pop_size][];
            for(int i = 0; i < pop_size; i++){
                pop_rte[i] = randperm(numOfCities);
                pop_brk[i] = randbreak(max_salesmen, numOfCities, min_tour);
            }

            // epicLevi: Currently Testing
            // Console.WriteLine("pop_brk[i] (pop_brk{k}): ");
            // for (int i = 0; i < pop_brk.Length; i++)
            // {
            //     for (int j = 0; j < pop_brk[0].Length; j++)
            //     {
            //         Console.Write(" {0} ", pop_brk[i][j]);
            //     }
            //     Console.WriteLine();
            // }

            // Initialize algorithm variables
            double global_min = double.PositiveInfinity;
            double[] total_dist = new double[pop_size];
            double[] dist_history = new double[num_iter];
            double[][] tmp_pop_rte = new double[8][];
            double[][] tmp_pop_brk = new double[8][];
            double[][] new_pop_rte = new double[pop_size][];
            double[][] new_pop_brk = new double[pop_size][];
            for(int i = 0; i < 8; i++){
                tmp_pop_rte[i] = new double[numOfCities];
                tmp_pop_brk[i] = new double[1];
            }
            for(int i = 0; i < pop_size; i++){
                new_pop_rte[i] = new double[numOfCities];
                new_pop_brk[i] = new double[1];
            }
            int iter = 0;
            int iter2go = 0;
        

            ///////////////////////////////////////////
            //
            // Run the GA
            //
            ///////////////////////////////////////////
            while(iter2go < num_iter){
                iter2go = iter2go + 1;
                iter = iter + 1;

                // Evaluate each Population Member (Calculate Total Distance)
                for(int i = 0; i < pop_size; i++){
                    double[] p_rte = pop_rte[i];
                    double[] p_brk = pop_brk[i];
                    int salesmen = p_brk.Length + 1;
                    double[][] rng = CalcRange(p_brk, numOfCities, true);

                    double[] d = new double[salesmen];
                    double[] Tour;

                    for (int j = 0; j < salesmen; j++)
                    {
                        if (j < rng.Length && rng[j][0] <= rng[j][1]) {
                            int rte_start   = (int)rng[j][0] - 1;
                            int rte_end     = (int)rng[j][1] - 1;
                            int rte_length  = rte_end - rte_start + 1;
                            double[] midTour = new double[rte_length];

                            for (int k = 0; k < midTour.Length; k++)
                            {
                                midTour[k] = p_rte[k + rte_start];
                            }

                            Tour    = new double[midTour.Length + 2];
                            Tour[0] = j;
                            Tour[Tour.Length - 1] = j;

                            for (int k = 1; k < Tour.Length - 1; k++)
                            {
                                Tour[k] = midTour[k - 1];
                            }
                        } else {
                            Tour = new double[] {Convert.ToDouble(j), Convert.ToDouble(j)};
                            d[j] = 0;
                        }
                    }
                }
            }
        }

        static double CalcTourLength(double[] Tour, double[][] dmat, double[][] d0, int indeces) {            
            double VehicleTourLength = d0[(int)Tour[0]][(int)Tour[1]];

            for (int c = 2; c <= indeces-1; c++)
            {
                VehicleTourLength = VehicleTourLength + dmat[(int)Tour[c+1]][(int)Tour[c]];
            }

            // Console.WriteLine("d0.Length: {0}, Tour[indeces]: {1}", d0.Length, Tour[indeces]);
            // Console.WriteLine("d0[0].Length: {0}, Tour[indeces-1]: {1}", d0[0].Length, Tour[indeces-1]);

            // Console.WriteLine("d0[(int)Tour[indeces]]", d0[(int)Tour[indeces]]);
            // Console.WriteLine("d0[(int)Tour[indeces]][(int)Tour[indeces-1]]: ", d0[(int)Tour[indeces]][(int)Tour[indeces-1]]);

            VehicleTourLength = VehicleTourLength + d0[(int)Tour[indeces]][(int)Tour[indeces-1]];

            return VehicleTourLength;
        }

        static double[][] CalcRange(double[] p_brk, int n, bool flag) {
            int i;
            double[][] rng = new double[p_brk.Length][];
            
            for (i = 0; i < p_brk.Length; i++)
            {
                rng[i] = new double[2];
            }

            for (i = 0; i < p_brk.Length; i++)
            {
                if (flag && p_brk[i] > 1) {
                    rng[i][0] = 1;
                    rng[i][1] = p_brk[i];

                    flag = false;
                } else if (flag) {
                    rng[i][0] = 1;
                    rng[i][1] = 0;
                } else if (i >= 1 && p_brk[i] <= p_brk[i - 1]) {
                    rng[i][0] = p_brk[i-1];
                    rng[i][1] = p_brk[i];
                } else if (i >= 1 && i < p_brk.Length) {
                    rng[i][0] = p_brk[i-1] + 1;
                    rng[i][1] = p_brk[i];
                } else {
                    rng[i][0] = p_brk[i-1] + 1;
                    rng[i][1] = p_brk[i];
                }
            }

            if (p_brk[p_brk.Length - 1] < n && p_brk[p_brk.Length - 1] != 1) {
                rng[i - 1][0] = p_brk[p_brk.Length - 1] + 1;
                rng[i - 1][1] = n;
            } else if (p_brk[p_brk.Length - 1] < n && p_brk[p_brk.Length - 1] == 1) {
                rng[i - 1][0] = p_brk[p_brk.Length - 1];
                rng[i - 1][1] = n;
            } else {
                rng[i - 1][0] = p_brk[p_brk.Length-1];
                rng[i - 1][1] = n - 1;
            }

            return rng;
        }

        static double[] randbreak(int max_salesmen, int n, int min_tour) {
            int     num_brks = max_salesmen;
            double[] breaks = new double[num_brks];

            for (int i = 0; i < num_brks; i++)
            {
                double newRand = new Random().Next(1, n);

                while (isDuplicate(newRand, breaks)) {
                    newRand = new Random().Next(1, n);
                }

                breaks[i] = newRand;
            }

            Array.Sort(breaks);

            return breaks;
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

        static bool isDuplicate(double n, double[] arr) {
            foreach (double item in arr)
            {
                if (item == n)
                    return true;
            }

            return false;
        }
    }
}
