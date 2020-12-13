using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElerTest
{
    internal class PrimeSequenceAlt : IEnumerable<long>
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

        private const int _BufferSize = 1024;
        private const int _BufferPosMask = _BufferSize - 1;
        private const int _ValueBits = 32;
        private const int _IndexShift = 5;
        private const int _IndexMask = _ValueBits - 1;
        private const int _BufferCount = _BufferSize * _ValueBits;

        private class WriteBuffer
        {  
            private int _bitIndex = 0;
            private int _value;
            private int[] _buffer;
            private int _bufferPos;
            public long Position { get; private set; }

            public WriteBuffer(long shift, int[] buffer)
            {
                Position = shift;
                _buffer = buffer;
                _bufferPos = 0;
                _value = buffer[_bufferPos];
            }

            public void WriteOne()
            {
                _value |= 1 << _bitIndex;
                //Position++;

                //if (++_byteIndex == _ValueBites)
                //{
                //    _byteIndex = 0; 
                //    _buffer[_bufferPos++] = _value;
                //    _value = _buffer[_bufferPos & _BufferPosMask];
                //}
            }

            public void Shift(long count)
            {
                Apply();
                ShiftUnsafe(count);
            }

            private void ShiftUnsafe(long count)
            {
                Position += count;
                int shift = (int)count + _bitIndex;
                _bufferPos += shift >> _IndexShift;
                _value = _buffer[_bufferPos];
                _bitIndex = shift & _IndexMask;
            }

            public void Apply()
            {
                _buffer[_bufferPos] = _value; // save current
            }
        }

        private class BufferReader
        {
            private int[] _buffer;
            private int _bitIndex = 0;
            private int _value;
            private int _bufferPos;
            
            public long Position { get; private set; }

            public BufferReader(int[] buffer, long bufferShift)
            {
                _buffer = buffer;
                _bufferPos = 0;
                _value = _buffer[_bufferPos];
                _bitIndex = 0;
                Position = bufferShift;
            }

            private int current => _value & (1 << _bitIndex);
            private bool notPrime => current > 0;

            public bool TryRead(out long prime)
            {
                Position++;

                bool canRead = true;
                while (canRead && notPrime)
                {
                    if (++_bitIndex == _ValueBits)
                    {
                        _bufferPos++;
                        _value = _buffer[_bufferPos & _BufferPosMask];
                        _bitIndex = 0;
                        canRead = _bufferPos < _buffer.Length;
                    }
                }

                prime = Shift2Prime(Position);
                return canRead;
            }
        }

        private class Sequence
        {
            private long _bufferShift = 0;
            private long _lastValue;
            private long _step;

            public Sequence(long prime, long bufferShift)
            {
                _lastValue = Prime2Shift(prime);
                _step = prime;
                _bufferShift = bufferShift;
            }
            
            public void WriteSequence(int[] targetBuffer)
            {
                long bufferEnd = _bufferShift + _BufferCount;

                if (_lastValue < bufferEnd)
                {
                    var wb = new WriteBuffer(_bufferShift, targetBuffer);
                    long shift = _lastValue - _bufferShift;
                    do
                    {
                        wb.Shift(shift);
                        wb.WriteOne();
                        shift = _step;
                        _lastValue += _step;
                    }
                    while (_lastValue < bufferEnd);
                }

                _bufferShift += _BufferCount;
            }
        }

        private static long Shift2Prime(long shift) => (shift << 1) - 1;
        private static long Prime2Shift(long prime) => (prime + 1) >> 1;

        public IEnumerator<long> GetPrimeEnumerator()
        {
            long prime = 2;
            yield return prime;
            long bufferShift = Prime2Shift(prime);
            var sequenceList = new LinkedList<Sequence>();
            int[] buffer = new int[_BufferSize];

            while (true)
            {
                var br = new BufferReader(buffer, bufferShift);
                while(br.TryRead(out prime))
                {
                    yield return prime;
                    var primeSeq = new Sequence(prime, bufferShift);
                    primeSeq.WriteSequence(buffer);
                    sequenceList.AddLast(primeSeq);
                }
                bufferShift += _BufferCount;
                Array.Clear(buffer, 0, _BufferSize);
                ApplyAll(buffer, sequenceList);
            }
        }

        private void ApplyAll(int[] buffer, LinkedList<Sequence> sequences)
        {
            // TODO: Will be apply in async mode
            foreach(var sequence in sequences)
            {
                sequence.WriteSequence(buffer);
            }
        }
    }
}
