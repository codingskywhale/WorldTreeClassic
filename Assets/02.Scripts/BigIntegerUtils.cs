using System.Collections.Generic;
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
        string[] standardSuffixes = {
        "K", "M", "B", "T", "P", "Q", "s", "S", "O", "N",
        "D", "U", "d", "Tt", "Qd", "Qt", "Sx", "Sp", "Oc", "No", "V"
    };

        int exponent = 3;

        // 표준 접미사를 먼저 적용
        for (int i = 0; i < standardSuffixes.Length; i++, exponent += 3)
        {
            if (number < BigInteger.Pow(10, exponent))
            {
                return ((double)number / (double)BigInteger.Pow(10, exponent - 3)).ToString("0.##") + standardSuffixes[i];
            }
        }

        // 표준 접미사 이후에는 알파벳 조합을 사용
        string[] extendedSuffixes = GenerateExtendedSuffixes();

        for (int i = 0; i < extendedSuffixes.Length; i++, exponent += 3)
        {
            if (number < BigInteger.Pow(10, exponent))
            {
                return ((double)number / (double)BigInteger.Pow(10, exponent - 3)).ToString("0.##") + extendedSuffixes[i];
            }
        }

        // 만약 number가 아주 큰 값일 경우, 최대 접미사를 붙여서 반환
        return ((double)number / (double)BigInteger.Pow(10, exponent - 3)).ToString("0.##") + extendedSuffixes[extendedSuffixes.Length - 1];
    }

    private static string[] GenerateExtendedSuffixes()
    {
        List<string> suffixes = new List<string>();

        // 이중 알파벳 접미사 (aa ~ zz)
        for (char first = 'a'; first <= 'z'; first++)
        {
            for (char second = 'a'; second <= 'z'; second++)
            {
                suffixes.Add(first.ToString() + second.ToString());
            }
        }

        return suffixes.ToArray();
    }
}
