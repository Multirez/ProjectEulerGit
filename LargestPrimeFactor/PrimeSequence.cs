using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElerTest
{
    internal class PrimeSequence : IEnumerable<long>
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
            while (true)
            {
                yield return value;
                value += step;
            } 
        }

        public IEnumerator<long> GetPrimeEnumerator()
        {
            long n = 1;
            var enumList = new LinkedList<IEnumerator<long>>();

            while (true)
            {
                n++;
                var enumNode = enumList.First;
                bool isPrime = true;
                while (enumNode != null)
                {
                    var sequence = enumNode.Value;
                    while (n > sequence.Current)
                        sequence.MoveNext();

                    isPrime &= n != sequence.Current;
                    enumNode = enumNode.Next;
                }

                if(isPrime)
                {
                    yield return n;
                    var stepSequence = GetStepSequence(n);
                    stepSequence.MoveNext();
                    enumList.AddLast(stepSequence);
                }
            }                
        }
    }
}
