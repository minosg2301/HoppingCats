
using System;

public static class MathExt
{
    public static long Clamp(long value, long min, long max) => Math.Min(Math.Max(min, value), max);
}
