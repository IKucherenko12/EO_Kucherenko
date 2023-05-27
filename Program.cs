using System;
using System.Collections.Generic;
using System.Linq;

namespace EO
{
    class Program
    {

        static Random random = new Random();

        static int populationSize = 4;
        static int chromosomeLength = 6;

        static int numberOfCrossovers = 2;
        static int numberOfMutations = 1;

        static int maxGenerations = 100;

        static int minValue = -1;
        static int maxValue = 62;

        static void Main(string[] args)
        {
            var maxResult = new Dictionary<string, double>();

            Dictionary<string, double> population = InitializePopulation();

            for (int generation = 0; generation < maxGenerations; generation++)
            {
                Console.WriteLine("Generation " + (generation + 1));

                population = NormalizedPopulation(population);

                Console.WriteLine("Population:");
                OutputDictionary(population);

                Dictionary<string, double> newPopulation = new Dictionary<string, double>();

                for (int i = 0; i < numberOfCrossovers; i++)
                {

                    var countCross = 0;
                    var toAddCross = false;
                    while (countCross < 10)
                    {
                        string parent1 = SelectRandomKey(population);
                        string parent2 = SelectRandomKey(population);

                        while (parent1 == parent2)
                            parent2 = SelectRandomKey(population);

                        string child1, child2;

                        Crossover(parent1, parent2, out child1, out child2);

                        try
                        {
                            newPopulation.Add(child1, 0.0);
                            newPopulation.Add(child2, 0.0);
                            toAddCross = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            countCross++;
                        }

                    }

                    if (!toAddCross)
                    {
                        Console.WriteLine("Impossible to crossover");
                        OutputMaxResult(maxResult);
                        return;
                    }


                }

                for (int i = 0; i < numberOfMutations; i++)
                {
                    var countMut = 0;
                    var toAddMut = false;
                    while (countMut < 10)
                    {
                        string randomKey = SelectRandomKey(newPopulation);

                        string mutatedChild = Mutate(randomKey);

                        newPopulation.Remove(randomKey.ToString());

                        try
                        {
                            newPopulation.Add(mutatedChild, 0.0);
                            toAddMut = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            countMut++;
                        }

                    }

                    if (!toAddMut)
                    {
                        Console.WriteLine("Impossible to mutate");
                        OutputMaxResult(maxResult);
                        return;
                    }

                }


                newPopulation = NormalizedPopulation(newPopulation);

                Console.WriteLine("New Population of children:");
                OutputDictionary(newPopulation);

                var allPopulations = newPopulation.Union(population).ToDictionary(x => x.Key, x => x.Value);
                var resultPopulation = new Dictionary<string, double>();


                if (allPopulations.Count > 4)
                {
                    while (resultPopulation.Count < 4)
                    {
                        var maxValue = allPopulations.Values.Max(); 

                        var keyOfMaxValue =
                            allPopulations.Aggregate((x, y) => x.Value > y.Value ? x : y).Key; // "a"

                        resultPopulation.Add(keyOfMaxValue, maxValue);

                        allPopulations.Remove(keyOfMaxValue);

                    }

                    population = resultPopulation;

                }
                else
                    population = allPopulations;

                Console.WriteLine("Best result population:");
                OutputDictionary(population);

                var bfv = population.Values.Max();
                var bn = population.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                Console.WriteLine("BEST F(x): " + bfv);
                Console.WriteLine("BEST X: " + bn);
                Console.WriteLine("BEST X: " + Convert.ToInt32(bn, 2));


                if (maxResult.Count == 0)
                    maxResult.Add(bn, bfv);

                if (population.Values.Max() > maxResult.Values.Max())
                {
                    maxResult.Clear();
                    maxResult.Add(bn, bfv);
                }

            }


            OutputMaxResult(maxResult);

        }

        private static void OutputDictionary(Dictionary<string, double> dic)
        {
            foreach (var mr in dic)
            {
                Console.WriteLine("F(x): " + mr.Value + ", " + "X: " + mr.Key + ", " + "X: " + Convert.ToInt32(mr.Key, 2));
            }
        }


        private static void OutputMaxResult(Dictionary<string, double> maxResult)
        {
            Console.WriteLine("It is impossible to continue the execution of the algorithm.Best result: ");
            foreach (var mr in maxResult)
            {
                Console.WriteLine("BEST F(x): " + mr.Value);
                Console.WriteLine("BEST X: " + mr.Key);
                Console.WriteLine("BEST X: " + Convert.ToInt32(mr.Key, 2));
            }
        }

        static Dictionary<string, double> InitializePopulation()
        {
            Dictionary<string, double> population = new Dictionary<string, double>();

            for (int i = 0; i < populationSize; i++)
            {
                string chromosome = GenerateRandomChromosome();

                bool keyExists = population.ContainsKey(chromosome);
                if (keyExists)
                {
                    populationSize = populationSize - 1;
                    break;

                }
                else
                {
                    population.Add(chromosome, 0.0);
                }

            }


            return population;
        }

        static string GenerateRandomChromosome()
        {
            string chromosome = "";

            var num = random.Next(minValue, maxValue);

            if (num >= 0)
                chromosome = Convert.ToString(num, 2).PadLeft(6, '0');
            else
            {
                var t = Convert.ToString(num, 2);
                chromosome = t.Substring((t.Length - 6), 6);

            }

            return chromosome;
        }


        static string SelectRandomKey(Dictionary<string, double> population)
        {
            List<string> keyList = new List<string>(population.Keys);

            var randomKey = keyList[random.Next(keyList.Count)];

            return randomKey;
        }

        static void Crossover(string parent1, string parent2, out string child1, out string child2)
        {
            child1 = parent1;
            child2 = parent2;

            int crossoverPoint = random.Next(0, chromosomeLength);
            child1 = parent1.Substring(0, crossoverPoint) + parent2.Substring(crossoverPoint);
            child2 = parent2.Substring(0, crossoverPoint) + parent1.Substring(crossoverPoint);

        }

        static string Mutate(string chromosome)
        {
            char[] chromosomeArray = chromosome.ToCharArray();

            var randomi = random.Next(0, chromosomeLength);
            chromosomeArray[randomi] = chromosomeArray[randomi] == '0' ? '1' : '0';

            return new string(chromosomeArray);
        }

        static Dictionary<string, double> NormalizedPopulation(Dictionary<string, double> population)
        {

            var newPop = new Dictionary<string, double>();

            foreach (var p in population)
            {
                int decimalValue = Convert.ToInt32(p.Key, 2);

                double decodedValue;

                decodedValue = (double)decimalValue;

                double fitnessValue = -43 + 9 * (decodedValue - 10) - 3 * Math.Pow((decodedValue - 10), 2) + 4 * Math.Pow((decodedValue - 10), 3);

                newPop.Add(p.Key, fitnessValue);
            }

            return newPop;
        }

    }

}
