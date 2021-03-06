﻿using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using Accord.Math;

namespace mobilityOptimizer
{
    public static class Program
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
            double Epsilon = 1*Math.Pow(10,-10);

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

            // Initialize algorithm variables
            double global_min = double.PositiveInfinity;
            double[] total_dist = new double[pop_size];
            // double[] dist_history = new double[num_iter];
            double[][] tmp_pop_rte = new double[16][];
            double[][] tmp_pop_brk = new double[16][];
            double[][] new_pop_rte = new double[pop_size][];
            double[][] new_pop_brk = new double[pop_size][];
            double[][] rng = new double[max_salesmen][];
            for(int i = 0; i < 16; i++){
                tmp_pop_rte[i] = new double[numOfCities];
                tmp_pop_brk[i] = new double[1];
            }
            for(int i = 0; i < pop_size; i++){
                new_pop_rte[i] = new double[numOfCities];
                new_pop_brk[i] = new double[1];
            }
            int iter = 0;
            int iter2go = 0;
            double[] opt_rte = new double[numOfCities];
            double[] opt_brk = new double[max_salesmen];
            double[] p_brk = new double[max_salesmen];
            double[] p_rte;
        

            ///////////////////////////////////////////
            //
            // Run the GA
            //
            ///////////////////////////////////////////
            while(iter2go < num_iter){
                Console.WriteLine("iter2go: " + iter2go);
                Console.WriteLine("num_iter: " + num_iter);
                iter2go = iter2go + 1;
                iter = iter + 1;                

                // Evaluate each Population Member (Calculate Total Distance)
                for(int i = 0; i < pop_size; i++){
                    p_rte = pop_rte[i];
                    p_brk = pop_brk[i];
                    int salesmen = p_brk.Length + 1;
                    rng = CalcRange(p_brk, numOfCities, true);

                    // // DEBUG Testing for CalcRange
                    // Console.WriteLine("\n >>>>> rng[][]: ");
                    // for (int j = 0; j < rng.Length; j++)
                    // {
                    //     for (int k = 0; k < rng[0].Length; k++)
                    //     {
                    //         Console.Write(" {0} ", rng[j][k]);    
                    //     }
                    //     Console.WriteLine();
                    // }                    
                    // // END DEBUG

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

                            int indeces = Tour.Length - 1;
                            d[j] = CalcTourLength(Tour, dmat, D0, indeces);

                            // DEBUG Test for CalcTourLength
                            //Console.WriteLine("\n >>>>> d[{0}]: {1}", j, d[j]);
                            // END DEBUG

                        } else {
                            Tour = new double[] {Convert.ToDouble(j), Convert.ToDouble(j)};
                            d[j] = 0;
                        }
                    }
                    total_dist[i] = d.Max() + Epsilon * d.Sum(); // lo maximo que haya en d + Epsillon * sum(d)
                }

                // Find the Best Route in the Population
                // Here's where you find the most optimum route in the population
                int index;
                double min_dist = total_dist.Min(out index);                
                // dist_history[iter] = min_dist;
                if(min_dist < global_min){
                    iter2go = 0;
                    int generation = iter;
                    global_min = min_dist;
                    opt_rte = pop_rte[index];
                    opt_brk = pop_brk[index];
                    int salesmen = p_brk.Length + 1;
                    rng = CalcRange(p_brk, numOfCities, true);
                    if(show_prog){
                        // TODO OPT Plot the best route
                    }
                }

                // Genetic Algorithm Operators
                double[] rand_grouping = randperm(pop_size);
                int ops = 16;
                for(int p = ops; p <= pop_size; p = p + ops){
                    // Populate rtes
                    double[] diff_p_numbers = getSequentialNumbers(p-ops+1, p-1);
                    double[] sub_rand_grouping = getSubArrayWithSequence(rand_grouping, diff_p_numbers);
                    double[][] rtes = getSubMatrixWithSequence(pop_rte, sub_rand_grouping);

                    // Populate brks
                    double[][] brks = getSubMatrixWithSequence(pop_brk, sub_rand_grouping);

                    // Populate dists
                    double[] dists = getSubArrayWithSequence(total_dist, sub_rand_grouping);

                    // double[] sub_rand_grouping = SubArrayDeepClone(rand_grouping, p-ops+1, p);
                    // double[][] rtes = new double[sub_rand_grouping.Length][];
                    // double[][] brks = new double[sub_rand_grouping.Length][];
                    // double[] dists = new double[sub_rand_grouping.Length];
                    // for(int i = 0; i < sub_rand_grouping.Length; i++){
                    //     rtes[i] = pop_rte[(int)sub_rand_grouping[i]];
                    //     brks[i] = pop_brk[(int)sub_rand_grouping[i]];
                    //     dists[i] = total_dist[(int)sub_rand_grouping[i]];
                    // }

                    int idx;
                    double ignore = dists.Min(out idx);
                    double[] best_of_8_rte = rtes[idx];
                    double[] best_of_8_brk = brks[idx];
                    double[] rte_ins_pts = new double[2];
                    Random random = new Random();  
                    rte_ins_pts[0] = Math.Ceiling((numOfCities - 1) * random.NextDouble());
                    rte_ins_pts[1] = Math.Ceiling((numOfCities - 1) * random.NextDouble());
                    Array.Sort(rte_ins_pts);
                    double I = rte_ins_pts[0];
                    double J = rte_ins_pts[1];

                    // Generate new solutions
                    for(int k = 0; k < ops; k++){
                        tmp_pop_rte[k] = (double[])best_of_8_rte.Clone();
                        tmp_pop_brk[k] = (double[])best_of_8_brk.Clone();
                        switch(k){                            
                            case 1: // Flip
                                fliplr(tmp_pop_rte, k, I, J);
                                break;
                            case 2: // Swap
                                swap(tmp_pop_rte, k, I, J);
                                break;
                            case 3: // Slide
                                slide(tmp_pop_rte, k, I, J);
                                break;
                            case 4: // Change Breaks
                                tmp_pop_brk[k] = randbreak(max_salesmen, numOfCities, min_tour);
                                break;
                            case 5: // Flip, Change Breaks
                                fliplr(tmp_pop_rte, k, I, J);
                                tmp_pop_brk[k] = randbreak(max_salesmen, numOfCities, min_tour);
                                break;
                            case 6: // Swap, Change Breaks
                                swap(tmp_pop_rte, k, I, J);
                                tmp_pop_brk[k] = randbreak(max_salesmen, numOfCities, min_tour);
                                break;
                            case 7: // Slide, Change Breaks
                                slide(tmp_pop_rte, k, I, J);
                                tmp_pop_brk[k] = randbreak(max_salesmen, numOfCities, min_tour);
                                break;
                            case 8:
                                int l = random.Next((int)Math.Min(numOfCities-J-1, Math.Floor(Math.Sqrt(numOfCities))));
                                double diffIl = ((I+l) - I) + 1;
                                double diffJl = ((J+l) - J) + 1;
                                double[] diffIlNums = new double[(int)diffIl];
                                double[] diffJlNums = new double[(int)diffJl];
                                for(int i = (int)I, j = 0; i <= I+l; i++, j++){
                                    diffIlNums[j] = i;
                                }
                                for(int i = (int)J, j = 0; i <= J+l; i++, j++){
                                    diffJlNums[j] = i;
                                }
                                double[] sub_tmp_pop_rteIl = new double[(int)diffIlNums.Length];
                                double[] sub_tmp_pop_rteJl = new double[(int)diffJlNums.Length];
                                for(int i = 0; i < diffIlNums.Length; i++){
                                    sub_tmp_pop_rteIl[i] = tmp_pop_rte[k][(int)diffIlNums[i]];
                                }
                                for(int i = 0; i < diffJlNums.Length; i++){
                                    sub_tmp_pop_rteJl[i] = tmp_pop_rte[k][(int)diffJlNums[i]];
                                }
                                double[] temp1 = sub_tmp_pop_rteIl;
                                double[] temp2 = sub_tmp_pop_rteJl;

                                for(int i = (int)I, j = 0; i <= I+l; i++, j++){
                                    tmp_pop_rte[k][i] = temp2[j];
                                    tmp_pop_rte[k][i] = temp1[j];
                                }
                                break;
                            case 11: // Remove tasks from agent and add at the end
                                //  TODO checar falla
                                l = random.Next(0, max_salesmen - 1);
                                double[] temp = new double[tmp_pop_brk[k].Length];
                                for(int i = 0; i < l; i++){
                                    temp[i] = tmp_pop_brk[k][i];
                                }
                                for(int i = l+1; i < tmp_pop_brk[k].Length; i++){
                                    temp[i] = tmp_pop_brk[k][i];
                                }
                                temp[tmp_pop_brk[k].Length-1] = numOfCities;
                                temp.CopyTo(tmp_pop_brk[k]);

                                break;
                            case 12: // Remove tasks from agent and add at the beginning
                                l = random.Next(0, max_salesmen - 1);
                                temp = new double[tmp_pop_brk[k].Length];
                                temp[0] = 0;
                                for(int i = 0, j = 1; i < l; i++, j++){
                                    temp[j] = tmp_pop_brk[k][i];
                                }
                                for(int i = l+1; i < tmp_pop_brk[k].Length; i++){
                                    temp[i] = tmp_pop_brk[k][i];
                                }
                                temp.CopyTo(tmp_pop_brk[k]);
                                
                                break;
                            default: // swap close points
                                if(I < numOfCities){
                                    swapClosePoints(tmp_pop_rte, k, I);
                                }
                                break;
                        }
                    }
                    for(int i = p-ops, j = 0; i < p; i++, j++){
                        new_pop_rte[i] = tmp_pop_rte[j];
                        new_pop_brk[i] = tmp_pop_brk[j];
                    }
                }
                pop_rte = new_pop_rte;
                pop_brk = new_pop_brk;
            }

            ///////////////////////////////////////////
            //
            // Return Outpus
            //
            ///////////////////////////////////////////
            double[][] best_tour = new double[max_salesmen][];
            for(int i = 0; i < max_salesmen; i++){
                if(rng[i][0] <= rng[i][1]){
                    double rngStart, rngEnd, diffRng;
                    rngStart = rng[i][0];
                    rngEnd = rng[i][1];
                    diffRng = (int)rngEnd - (int)rngStart;
                    double[] rngNumbers = new double[(int)diffRng + 1];
                    for(int j = (int)rngStart, y = 0; j <= rngEnd; j++, y++){
                        rngNumbers[y] = j;
                    }
                    double[] sub_opt_rte = new double[rngNumbers.Length];
                    for(int j = 0; j < rngNumbers.Length; j++){
                        sub_opt_rte[j] = opt_rte[(int)rngNumbers[j]];
                    }
                    double[] temp = new double[sub_opt_rte.Length + 2];
                    temp[0] = i;
                    sub_opt_rte.CopyTo(temp, 1);
                    temp[temp.Length - 1] = i;

                    best_tour[i] = temp;
                }else{          
                    best_tour[i] = new double[2]{i, i};
                }                
            }    
            for(int y = 0; y < best_tour.Length; y++){
                for(int x = 0; x < best_tour[y].Length; x++){
                    Console.Write(best_tour[y][x] + " ");
                }
                Console.WriteLine("");
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
            if(p_brk.Length < 3){
                throw new Exception("p_brk");
            }
            int i;

            double[][] rng = new double[p_brk.Length + 1][];
            
            for (i = 0; i < rng.Length; i++)
            {
                rng[i] = new double[2];
            }

            for (i = 0; i < rng.Length - 1; i++)
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

            i = rng.Length - 1;

            if (p_brk[p_brk.Length - 1] < n && p_brk[p_brk.Length - 1] != 1) {
                rng[i][0] = p_brk[p_brk.Length - 1] + 1;
                rng[i][1] = n - 1;
            } else if (p_brk[p_brk.Length - 1] < n && p_brk[p_brk.Length - 1] == 1) {
                rng[i][0] = p_brk[p_brk.Length - 1];
                rng[i][1] = n - 1;
            } else {
                rng[i][0] = p_brk[p_brk.Length - 1];
                rng[i][1] = n - 2;
            }

            return rng;
        }

        static double[] randbreak(int max_salesmen, int n, int min_tour) {
            int     num_brks = max_salesmen - 1;
            double[] breaks = new double[num_brks];

            for (int i = 0; i < num_brks; i++)
            {
                double newRand = new Random().Next(0, n);

                breaks[i] = newRand;
            }

            Array.Sort(breaks);
            if(breaks.Length < 3){
                throw new Exception("breaks");
            }
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
            for(int j = 0; j < num; j++){
                array[j] = j;
            }
            int i;
            int n = array.Length;
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

        // Gets a subpart of an array
        static T[] SubArrayDeepClone<T>(this T[] data, int index, int length)
        {
            T[] arrCopy = new T[length];
            Array.Copy(data, index, arrCopy, 0, length);
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, arrCopy);
                ms.Position = 0;
                return (T[])bf.Deserialize(ms);
            }
        }

        // Flips matrix elements
        static void fliplr(double[][] data, int k, double I, double J){
            int i = (int)I, j = (int)J;
            while(i < j){
                double temp = data[k][j];
                data[k][j] = data[k][i];
                data[k][i] = temp;
                i++;
                j--;
            }
        }

        // Swaps matrix elements
        static void swap(double[][] data, int k, double I, double J){
            int i = (int)I, j = (int)J;
            double temp = data[k][j];
            data[k][j] = data[k][i];
            data[k][i] = temp;
        }

        // Slides matrix elements
        static void slide(double[][] data, int k, double I, double J){
            int i = (int)I, j = (int)J;
            double tempFinal = data[k][i];
            for(i = i; i < j; i++){
                data[k][i] = data[k][i+1];
            }
             data[k][i] = tempFinal;
        }

        // Swap close points
        static void swapClosePoints(double[][] data, int k, double I){
            // If it's last element then swap with third
            if(I == data[k].Length - 1){
                I--;
            }
            double tempSwap = data[k][(int)I];
            data[k][(int)I] = data[k][(int)I + 1];
            data[k][(int)I + 1] = tempSwap;
        }

        static double[] getSequentialNumbers(int start, int end){
            double[] numbers;
            int numDiff;

            // +1 because it's inclusive
            numDiff = end - start + 1;            
            numbers = new double[numDiff];
            for(int i = start, j = 0; i <= end; i++, j++){
                numbers[j] = i;
            }

            return numbers;
        }

        static double[] getSubArrayWithSequence(double[] data, double[] sequence){
            double[] subArray = new double[sequence.Length];

            for(int i = 0; i < sequence.Length; i++){
                subArray[i] = data[(int)sequence[i]];
            }

            return subArray;
        }

        static double[][] getSubMatrixWithSequence(double[][] data, double[] sequence){
            double[][] subMatrix = new double[sequence.Length][];

            for(int i = 0; i < sequence.Length; i++){
                subMatrix[i] = data[(int)sequence[i]];
            }

            return subMatrix;
        }
    }
}
