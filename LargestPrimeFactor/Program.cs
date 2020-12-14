using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = "Input number and press [Enter]:";
            long n = 15;// 77521, 120170295029;
            Console.WriteLine(message);
            while (long.TryParse(Console.ReadLine(), out n))
            {
                var sw = Stopwatch.StartNew();
                long answer = GetMaxPrimeDividerRequrse(n);
                //long answer = GetPrimeById(n);
                sw.Stop();
                Console.WriteLine($"Answer:{answer} Elapsed:{sw.Elapsed}\n");
                Console.WriteLine(message);
            }           
        }

        private static long GetMaxPrimeDividerRequrse(long input)
        {
            long maxValue = (long)Math.Sqrt(input);
            long maxDivider = input;
            var primeEnum = new PrimeSequenceAlt().GetPrimeEnumerator();

            long counter = 0;
            long divided;
            while (primeEnum.MoveNext() && primeEnum.Current <= maxValue)
            {
                if (TestDivide(input, primeEnum.Current, out divided))
                {
                    maxDivider = primeEnum.Current;
                    if (divided > maxDivider)
                        divided = GetMaxPrimeDividerRequrse(divided);
                    return Math.Max(maxDivider, divided);
                }
                counter++;
            }
            //Console.WriteLine("Prime count: " + counter + " last " + primeEnum.Current);
            return maxDivider;
        }

        private static long GetPrimeById(long count)
        {
            var primeEnum = new PrimeSequenceAlt().GetPrimeEnumerator();
            for (long i = 0; i < count; i++)
                primeEnum.MoveNext();

            return primeEnum.Current;
        }

        private static bool TestDivide(long value, long div, out long result)
        {
            result = value / div;
            return value == (div * result);
        }

        private static bool TestDivide2(long value, long div)
        {
            return value == (div * (value / div));
        }

        private static bool TestDivideMod(long value, long div)
        {
            return (value % div) == 0;
        }
    }
}
