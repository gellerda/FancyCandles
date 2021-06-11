using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    internal class DiscreteRandomVariableSample
    {
        public readonly int NumberOfValues;
        public long NumberOfObservations { get; private set; }

        private readonly long[] freqsOfValues;
        private readonly int[] positionsOfValues;
        private readonly int[] valuesOfPositions;
        //---------------------------------------------------------------------------------------------------------------------------------------
        public DiscreteRandomVariableSample(int numberOfValues)
        {
            NumberOfValues = numberOfValues;
            freqsOfValues = new long[numberOfValues];
            positionsOfValues = new int[numberOfValues];
            valuesOfPositions = new int[numberOfValues];

            for (int i = 0; i < numberOfValues; i++)
            {
                positionsOfValues[i] = i;
                valuesOfPositions[i] = i;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void AddNewObservation(int value, bool clampValue = true)
        {
            if (clampValue)
            {
                if (value < 0) value = 0;
                else if (value >= NumberOfValues) value = NumberOfValues - 1;
            }
            else if (value < 0 || value >= NumberOfValues)
                throw new ArgumentException("value must be between 0 and (NumberOfValues-1) inclusively.");

            freqsOfValues[value]++;

            int pos = positionsOfValues[value];
            //int prevValue = (pos == 0) ? value : valuesOfPositions[positionsOfValues[pos - 1]];
            int prevValue = (pos == 0) ? value : valuesOfPositions[pos - 1];
            while (freqsOfValues[prevValue] < freqsOfValues[value])
            {
                int c = positionsOfValues[value];
                positionsOfValues[value] = positionsOfValues[prevValue];
                positionsOfValues[prevValue] = c;

                c = valuesOfPositions[pos];
                valuesOfPositions[pos] = valuesOfPositions[pos-1];
                valuesOfPositions[pos-1] = c;

                pos--;
                prevValue = (pos == 0) ? value : valuesOfPositions[pos - 1];
            }

            NumberOfObservations++;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public int MaxValueAmongTopFrequent(double probability)
        {
            if (probability > 1.0 || probability < 0)
                throw new ArgumentException("probability must be between 0 and 1.");

            long int_freq = (long)(probability * NumberOfObservations + 0.5);

            int order_i = 0;
            int value = valuesOfPositions[order_i];
            long s = freqsOfValues[valuesOfPositions[value]];
            int maxValue = value;
            while (s < int_freq)
            {
                order_i++;
                value = valuesOfPositions[order_i];
                s += freqsOfValues[valuesOfPositions[value]];

                if (value > maxValue)
                    maxValue = value;
            }

            return maxValue;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
