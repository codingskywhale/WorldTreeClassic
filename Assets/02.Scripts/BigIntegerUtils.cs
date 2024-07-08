using System.Numerics;

public static class BigIntegerUtils
{
    public static string FormatBigInteger(BigInteger number)
    {
        if (number >= BigInteger.Pow(10, 33))
            return ((double)number / (double)BigInteger.Pow(10, 33)).ToString("0.##") + "d";
        if (number >= BigInteger.Pow(10, 30))
            return ((double)number / (double)BigInteger.Pow(10, 30)).ToString("0.##") + "c";
        if (number >= BigInteger.Pow(10, 27))
            return ((double)number / (double)BigInteger.Pow(10, 27)).ToString("0.##") + "b";
        if (number >= BigInteger.Pow(10, 24))
            return ((double)number / (double)BigInteger.Pow(10, 24)).ToString("0.##") + "a";
        if (number >= BigInteger.Pow(10, 15))
            return ((double)number / (double)BigInteger.Pow(10, 15)).ToString("0.##") + "Q";
        if (number >= BigInteger.Pow(10, 12))
            return ((double)number / (double)BigInteger.Pow(10, 12)).ToString("0.##") + "T";
        if (number >= BigInteger.Pow(10, 9))
            return ((double)number / (double)BigInteger.Pow(10, 9)).ToString("0.##") + "B";
        if (number >= BigInteger.Pow(10, 6))
            return ((double)number / (double)BigInteger.Pow(10, 6)).ToString("0.##") + "M";
        if (number >= BigInteger.Pow(10, 3))
            return ((double)number / (double)BigInteger.Pow(10, 3)).ToString("0.##") + "K";
        return number.ToString();
    }
}
