using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElerTest
{
    internal class PrimeSequenceOptimized : IEnumerable<long>
    {
        #region IEnumerable<long>
        IEnumerator<long> IEnumerable<long>.GetEnumerator()
        {
            return GetPrimeEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetPrimeEnumerator();
        }
        #endregion

        public IEnumerator<long> GetStepSequence(long step)
        {
            long value = step * step;
            step *= 2;
            while (true)
            {
                yield return value;
                value += step;
            } 
        }

        private struct Counter
        {
            private long _step;
            private long _current;

            public long Current => _current;

            public Counter(long prime)
            {
                _current = prime * prime;
                _step = prime * 2;
            }

            public void Shift(long target)
            {
                while (target > _current)
                    _current += _step;
            }
        }


        public IEnumerator<long> GetPrimeEnumerator()
        {
            yield return 2;
            long n = 1;
            var enumList = new LinkedList<Counter>();

            while (true)
            {
                n += 2;
                var enumNode = enumList.First;
                bool isPrime = true;
                while (isPrime && enumNode != null)
                {
                    var sequence = enumNode.Value;
                    sequence.Shift(n);

                    isPrime &= n != sequence.Current;
                    enumNode = enumNode.Next;
                }

                if(isPrime)
                {
                    yield return n;
                    enumList.AddLast(new Counter(n));
                }
            }                
        }
    }
}
