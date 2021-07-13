/* 
    Copyright 2019 Dennis Geller.

    This file is part of FancyCandles.

    FancyCandles is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FancyCandles is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FancyCandles.  If not, see<https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;

namespace FancyCandles
{
    class MyWpfMath
    {
        //----------------------------------------------------------------------------------------------------------------------------------
        // Рассчитывает значения для меток на оси так, чтобы они имели достаточно круглые значения и располагались так, чтобы не мешать друг другу.
        // minValue, maxValue - минимальное и максимальное значение на оси (НЕ обязательно метки). 
        // chartAxisLength - длина отображаемой оси в единицах длины WPF. 
        // chartLabelLength - максимальная длина вдоль оси, отведенная для одной метки (метки не должны залазить друг на друга).
        // Предполагается, что продольный центр метки совпадает со значением на оси.
        public static List<double> CalcTicks(double minValue, double maxValue, double chartAxisLength, double chartLabelLength)
        {
            double lineHeight = (maxValue - minValue) / chartAxisLength * chartLabelLength;
            double lineHeightMaxDigit = HighestDecimalPlace(lineHeight, out _);
            double step = Math.Ceiling(lineHeight / lineHeightMaxDigit) * lineHeightMaxDigit;
            double theMostRoundTick = TheMostRoundValueInsideRange(minValue, maxValue);
            List<double> ticks = new List<double>();
            ticks.Add(theMostRoundTick);

            int step_i = 1;
            double next_tick = theMostRoundTick + step_i * step;
            while (next_tick <= maxValue)
            {
                ticks.Add(next_tick);
                step_i++;
                next_tick = theMostRoundTick + step_i * step;
            }

            step_i = 1;
            next_tick = theMostRoundTick - step_i * step;
            while (next_tick >= minValue)
            {
                ticks.Insert(0, next_tick);
                step_i++;
                next_tick = theMostRoundTick - step_i * step;
            }

            return ticks;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static double HighestDecimalPlace(double x, out int highestDecimalPow)
        {
            if (x >= 1.0)
            {
                if (x >= 1_000_000.0)
                {
                    if (x >= 1_000_000_000_000.0)
                    {
                        highestDecimalPow = 12;
                        return 1_000_000_000_000.0;
                    }
                    else if (x >= 100_000_000_000.0)
                    {
                        highestDecimalPow = 11;
                        return 100_000_000_000.0;
                    }
                    else if (x >= 10_000_000_000.0)
                    {
                        highestDecimalPow = 10;
                        return 10_000_000_000.0;
                    }
                    else if (x >= 1_000_000_000.0)
                    {
                        highestDecimalPow = 9;
                        return 1_000_000_000.0;
                    }
                    else if (x >= 100_000_000.0)
                    {
                        highestDecimalPow = 8;
                        return 100_000_000.0;
                    }
                    else if (x >= 10_000_000.0)
                    {
                        highestDecimalPow = 7;
                        return 10_000_000.0;
                    }
                    else // if (x >= 1_000_000.0)
                    {
                        highestDecimalPow = 6;
                        return 1_000_000.0;
                    }
                }
                else //if (x < 1_000_000.0)
                {
                    if (x >= 100_000.0)
                    {
                        highestDecimalPow = 5;
                        return 100_000.0;
                    }
                    else if (x >= 10_000.0)
                    {
                        highestDecimalPow = 4;
                        return 10_000.0;
                    }
                    else if (x >= 1_000.0)
                    {
                        highestDecimalPow = 3;
                        return 1_000.0;
                    }
                    else if (x >= 100.0)
                    {
                        highestDecimalPow = 2;
                        return 100.0;
                    }
                    else if (x >= 10.0)
                    {
                        highestDecimalPow = 1;
                        return 10.0;
                    }
                    else //if (x >= 1.0)
                    {
                        highestDecimalPow = 0;
                        return 1.0;
                    }
                }
            }
            else //if (x < 1.0)
            {
                if (x >= 0.1)
                {
                    highestDecimalPow = -1;
                    return 0.1;
                }
                else if (x >= 0.01)
                {
                    highestDecimalPow = -2;
                    return 0.01;
                }
                else if (x >= 0.001)
                {
                    highestDecimalPow = -3;
                    return 0.001;
                }
                else if (x >= 0.000_1)
                {
                    highestDecimalPow = -4;
                    return 0.000_1;
                }
                else if (x >= 0.000_01)
                {
                    highestDecimalPow = -5;
                    return 0.000_01;
                }
                else if (x >= 0.000_001)
                {
                    highestDecimalPow = -6;
                    return 0.000_001;
                }
                else if (x >= 0.000_000_1)
                {
                    highestDecimalPow = -7;
                    return 0.000_000_1;
                }
                else if (x >= 0.000_000_01)
                {
                    highestDecimalPow = -8;
                    return 0.000_000_01;
                }
                else if (x >= 0.000_000_001)
                {
                    highestDecimalPow = -9;
                    return 0.000_000_001;
                }
                else
                {
                    highestDecimalPow = int.MinValue;
                    return double.NegativeInfinity;
                }
            }
        }

        public static long HighestDecimalPlace(long x)
        {
            if (x >= 1000000000000)
                return 1000000000000;
            if (x >= 100000000000)
                return 100000000000;
            if (x >= 10000000000)
                return 10000000000;
            if (x >= 1000000000)
                return 1000000000;
            if (x >= 100000000)
                return 100000000;
            if (x >= 10000000)
                return 10000000;
            if (x >= 1000000)
                return 1000000;
            else if (x >= 100000)
                return 100000;
            else if (x >= 10000)
                return 10000;
            else if (x >= 1000)
                return 1000;
            else if (x >= 100)
                return 100;
            else if (x >= 10)
                return 10;
            else if (x >= 1)
                return 1;
            else return 0;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public static double TheMostRoundValueInsideRange(double x0, double x1)
        {
            double max10PowValue = HighestDecimalPlace(Math.Abs(x0), out int max10Pow);
            double mostR = Math.Ceiling(x0 / max10PowValue) * max10PowValue;
            while (mostR > x1)
            {
                max10Pow--;
                max10PowValue = Math.Pow(10.0, max10Pow);
                mostR = Math.Ceiling(x0 / max10PowValue) * max10PowValue;
            }
            return mostR;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
    }
}
