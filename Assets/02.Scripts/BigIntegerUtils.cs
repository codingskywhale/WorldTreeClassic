using System.Numerics;

public static class BigIntegerUtils
{
    /// <summary>
    /// BigInteger 값을 적절한 단위로 포맷하여 문자열로 반환합니다.
    /// </summary>
    /// <param name="number">포맷할 BigInteger 값</param>
    /// <returns>포맷된 문자열</returns>
    public static string FormatBigInteger(BigInteger number)
    {
        if (number >= BigInteger.Pow(10, 75))
            return ((double)number / (double)BigInteger.Pow(10, 75)).ToString("0.##") + "Z";
        if (number >= BigInteger.Pow(10, 72))
            return ((double)number / (double)BigInteger.Pow(10, 72)).ToString("0.##") + "Y";
        if (number >= BigInteger.Pow(10, 69))
            return ((double)number / (double)BigInteger.Pow(10, 69)).ToString("0.##") + "X";
        if (number >= BigInteger.Pow(10, 66))
            return ((double)number / (double)BigInteger.Pow(10, 66)).ToString("0.##") + "W";
        if (number >= BigInteger.Pow(10, 63))
            return ((double)number / (double)BigInteger.Pow(10, 63)).ToString("0.##") + "V";
        if (number >= BigInteger.Pow(10, 60))
            return ((double)number / (double)BigInteger.Pow(10, 60)).ToString("0.##") + "No";
        if (number >= BigInteger.Pow(10, 57))
            return ((double)number / (double)BigInteger.Pow(10, 57)).ToString("0.##") + "Oc";
        if (number >= BigInteger.Pow(10, 54))
            return ((double)number / (double)BigInteger.Pow(10, 54)).ToString("0.##") + "Sp";
        if (number >= BigInteger.Pow(10, 51))
            return ((double)number / (double)BigInteger.Pow(10, 51)).ToString("0.##") + "Sx";
        if (number >= BigInteger.Pow(10, 48))
            return ((double)number / (double)BigInteger.Pow(10, 48)).ToString("0.##") + "Qt";
        if (number >= BigInteger.Pow(10, 45))
            return ((double)number / (double)BigInteger.Pow(10, 45)).ToString("0.##") + "Qd";
        if (number >= BigInteger.Pow(10, 42))
            return ((double)number / (double)BigInteger.Pow(10, 42)).ToString("0.##") + "Tt";
        if (number >= BigInteger.Pow(10, 39))
            return ((double)number / (double)BigInteger.Pow(10, 39)).ToString("0.##") + "d";
        if (number >= BigInteger.Pow(10, 36))
            return ((double)number / (double)BigInteger.Pow(10, 36)).ToString("0.##") + "U";
        if (number >= BigInteger.Pow(10, 33))
            return ((double)number / (double)BigInteger.Pow(10, 33)).ToString("0.##") + "D";
        if (number >= BigInteger.Pow(10, 30))
            return ((double)number / (double)BigInteger.Pow(10, 30)).ToString("0.##") + "N";
        if (number >= BigInteger.Pow(10, 27))
            return ((double)number / (double)BigInteger.Pow(10, 27)).ToString("0.##") + "O";
        if (number >= BigInteger.Pow(10, 24))
            return ((double)number / (double)BigInteger.Pow(10, 24)).ToString("0.##") + "S";
        if (number >= BigInteger.Pow(10, 21))
            return ((double)number / (double)BigInteger.Pow(10, 21)).ToString("0.##") + "s";
        if (number >= BigInteger.Pow(10, 18))
            return ((double)number / (double)BigInteger.Pow(10, 18)).ToString("0.##") + "Q";
        if (number >= BigInteger.Pow(10, 15))
            return ((double)number / (double)BigInteger.Pow(10, 15)).ToString("0.##") + "P";
        if (number >= BigInteger.Pow(10, 12))
            return ((double)number / (double)BigInteger.Pow(10, 12)).ToString("0.##") + "T";
        if (number >= BigInteger.Pow(10, 9))
            return ((double)number / (double)BigInteger.Pow(10, 9)).ToString("0.##") + "B";
        if (number >= BigInteger.Pow(10, 6))
            return ((double)number / (double)BigInteger.Pow(10, 6)).ToString("0.##") + "M";
        if (number >= BigInteger.Pow(10, 3))
            return ((double)number / (double)BigInteger.Pow(10, 3)).ToString("0.##") + "K";

        // 1000 미만의 숫자는 그대로 출력
        return number.ToString();
    }



}
