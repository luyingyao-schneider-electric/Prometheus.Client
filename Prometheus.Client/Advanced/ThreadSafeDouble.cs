﻿using System;
using System.Globalization;
using System.Threading;

namespace Prometheus.Client.Advanced
{
    public struct ThreadSafeDouble
    {
        private long _value;

        public ThreadSafeDouble(double value)
        {
            _value = BitConverter.DoubleToInt64Bits(value);
        }

        public double Value
        {
            get
            {
                return BitConverter.Int64BitsToDouble(Interlocked.Read(ref _value));
            }
            set
            {
                Interlocked.Exchange(ref _value, BitConverter.DoubleToInt64Bits(value));
            }
        }

        public void Add(double increment)
        {
            while (true)
            {
                long initialValue = _value;
                double computedValue = BitConverter.Int64BitsToDouble(initialValue) + increment;

                if (initialValue == Interlocked.CompareExchange(ref _value, BitConverter.DoubleToInt64Bits(computedValue), initialValue))
                    return;
            }
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (obj is ThreadSafeDouble)
                return Value.Equals(((ThreadSafeDouble)obj).Value);

            return Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
