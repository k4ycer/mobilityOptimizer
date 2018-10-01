# mobilityOptimizer

A solver for the symmetric, multi-traveller, multi-depot, closed-end TSP.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

You will need .NET Core installed in your system to run mobilityOptimizer. Follow the instructions [here](https://www.microsoft.com/net/learn/dotnet/hello-world-tutorial) to do so.

## Running the tests

To run personalized tests, you can modify the following variables contained in main: 

* **xy**: double[][] matrix, contains the locations of all cities.
* **max_salesmen**: limits the max number of available salesmen to solve the TSP.
* **depots**: double[][] matrix, contains the locations of all depots.
* **pop_size**: establishes the size of a population.
* **num_iter**: number of iterations to run after a solution has been found.
* **dmat**: adyacency matrix.

Open a terminal in mobilityOptmizer's root directory and execute:

```
dotnet run
```

This will build and run the project.

## Authors

* **Eduardo Torres** - [GitHub/k4cer](https://github.com/k4ycer)
* **Leví Carbellido** - [GitHub/epicLevi](https://github.com/epicLevi)

## Acknowledgments

* This project is an adaptation of Elad Kivelevitch's Matlab implementation of a solution to the TSP. Available [here](https://www.mathworks.com/matlabcentral/fileexchange/31814-mdmtspv_ga-multiple-depot-multiple-traveling-salesmen-problem-solved-by-genetic-algorithm).
