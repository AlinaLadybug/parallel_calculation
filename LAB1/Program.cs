using System.Linq;
using System.Threading;
using System;
using System.Diagnostics;

namespace LAB1
{
    class Program
    {
        private readonly static int SIZE = 1000;
        private readonly static int RANDOM_MAX_VALUE = 100;
        private readonly static int NUMBER_JOBS = 7;

        static void Main(string[] args)
        {
            int[,] arr = new int[SIZE, SIZE];
            arr = GenerateMatrix();

            int[] vector = new int[SIZE];
            vector = GenerateVector();

            var serialResult = GenerateSerialResult(arr, vector);
            var parallelResult = GenerateParallelResult(arr, vector);
            CompareResults(serialResult, parallelResult);
        }



        private static int[,] GenerateMatrix()
        {
            int[,] arr = new int[SIZE, SIZE];
            // Console.Write("\nThe matrix is : \n");
            for (int i = 0; i < SIZE; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < SIZE; j++)
                {
                    arr[i, j] = new Random().Next(RANDOM_MAX_VALUE);
                    // Console.Write("{0}\t", arr[i, j]);
                }
            }
            Console.Write("\n\n");
            return arr;
        }

        private static int[] GenerateVector()
        {
            int[] vector = new int[SIZE];
            Console.Write("\nThe vector is : \n");
            for (int i = 0; i < SIZE; i++)
            {
                vector[i] = new Random().Next(RANDOM_MAX_VALUE);
                // Console.Write("{0}\t", vector[i]);
            }
            Console.Write("\n\n");
            return vector;
        }
        private static int[] GenerateSerialResult(int[,] arr, int[] vector)
        {
            Console.WriteLine("Serial Result :");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int[] serialResult = new int[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                serialResult[i] = 0;
                for (int j = 0; j < SIZE; j++)
                {
                    serialResult[i] += arr[i, j] * vector[j];
                }
                // Console.Write("{0}\t", serialResult[i]);
            }
            stopWatch.Stop();
            var serialTime = stopWatch.ElapsedMilliseconds;
            Console.WriteLine($"\nElapsed Time of Serial Result : {serialTime}.");
            return serialResult;
        }


        private static int[] GenerateParallelResult(int[,] arr, int[] vector)
        {
            Console.WriteLine("Parallel Result :");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Thread[] threads = new Thread[NUMBER_JOBS];
            int[][] threadsResult = new int[NUMBER_JOBS][];
            for (int i = 0; i < NUMBER_JOBS; i++)
            {
                int currentJob = i;
                threads[i] = new Thread(() => threadsResult[currentJob] = CalculateByCurrentJob(arr, vector, currentJob));
                threads[i].Start();
            }
            for (int i = 0; i < NUMBER_JOBS; i++)
            {
                threads[i].Join();
            }

            int[] parallelResult = new int[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < NUMBER_JOBS; j++)
                {
                    parallelResult[i] += threadsResult[j][i];
                }
                // Console.Write("{0}\t", parallelResult[i]);
            }

            stopWatch.Stop();
            var parallelTime = stopWatch.ElapsedMilliseconds;
            Console.WriteLine($"\nElapsed Time of Parallel Result : {parallelTime}.");
            return parallelResult;
        }

        private static int[] CalculateByCurrentJob(int[,] arr, int[] vector, int currentJob)
        {
            int startIndex = SIZE / NUMBER_JOBS * currentJob;
            int endIndex = currentJob == NUMBER_JOBS - 1 ? SIZE : SIZE / NUMBER_JOBS * (currentJob + 1);
            int[] parallelResult = new int[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                parallelResult[i] = 0;
                for (int j = startIndex; j < endIndex; j++)
                {
                    parallelResult[i] += arr[i, j] * vector[j];
                }
            }
            return parallelResult;
        }

        private static void CompareResults(int[] serialResult, int[] parallelResult)
        {
            var isEqual = serialResult.SequenceEqual(parallelResult);
            Console.WriteLine($"Are results equal: {isEqual}.");
        }
    }
}