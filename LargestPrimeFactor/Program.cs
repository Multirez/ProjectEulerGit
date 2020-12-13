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
            long n = 15;// 77521;
            Console.WriteLine(message);
            while (long.TryParse(Console.ReadLine(), out n))
            {
                var sw = Stopwatch.StartNew();
                long answer = GetMaxPrimeDividerRequrse(n);
                sw.Stop();
                Console.WriteLine($"Answer:{answer} Elapsed:{sw.Elapsed}\n");
                Console.WriteLine(message);
            }           
        }

        private static long GetMaxPrimeDivider(long input)
        {
            long maxValue = (long)Math.Sqrt(input);
            long maxDivider = input;
            var priveEnum = new PrimeSequenceAlt().GetPrimeEnumerator();
            priveEnum.MoveNext();

            long counter = 0;
            while (priveEnum.Current <= maxValue)
            {
                if (TestDivide2(input, priveEnum.Current))
                {
                    maxDivider = priveEnum.Current;
                    //Console.WriteLine(" " + priveEnum.Current);
                }
                //Console.WriteLine("MaxValue: " + maxValue);
                priveEnum.MoveNext();
                counter++;
            }
            Console.WriteLine("Prime count: " + counter + " last " + priveEnum.Current);
            return maxDivider;
        }

        private static long GetMaxPrimeDividerRequrse(long input)
        {
            long maxValue = (long)Math.Sqrt(input);
            long maxDivider = input;
            var priveEnum = new PrimeSequenceOptimized().GetPrimeEnumerator();

            long counter = 0;
            long divided;
            while (priveEnum.MoveNext() && priveEnum.Current <= maxValue)
            {
                if (TestDivide(input, priveEnum.Current, out divided))
                {
                    maxDivider = priveEnum.Current;
                    if (divided > maxDivider)
                        divided = GetMaxPrimeDividerRequrse(divided);
                    return Math.Max(maxDivider, divided);
                }
                counter++;
            }
            //Console.WriteLine("Prime count: " + counter + " last " + priveEnum.Current);
            return maxDivider;
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
