using System.Globalization;
using System.Text.RegularExpressions;

namespace Hurudza.Common.Utils.Extensions;

public static class StringExtentions
{
    public static string MaskAccountNumber(this string number)
    {
        var firstTwo = number.Substring(0, 2);
        var lastThree = number.Substring(number.Length - 3);
        var final = $"{firstTwo}***{lastThree}";

        return final;
    }

    public static decimal GetAmountDecimal(this string number)
    {
        var stripped = number.Replace("$", string.Empty);

        var amount = Convert.ToDecimal(stripped);

        return amount;
    }

    /// <summary>
    /// Removes "_" from a string and starts every word with a capital letter
    /// </summary>
    /// <param name="value"></param>
    /// <returns>string converted to Title Case</returns>
    public static string ToTitleCase(this string value)
    {
        var cleaned = value.Trim().Replace("_", " ").ToLower();
        TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
        string output = cultInfo.ToTitleCase(cleaned);
        return output;
    }

    public static string GetTextBeforeNumber(this string text)
    {
        return Regex.Match(text, @"^[^0-9]*").Value.Trim();
    }

    public static string GetTextBeforeString(this string text, string compare)
    {
        return text[..text.ToUpper().IndexOf(compare, StringComparison.Ordinal)].Trim();
    }

    public static bool IsNumeric(this string val)
    {
        return int.TryParse(val, out _);
    }

    public static bool ContainsNumber(this string val)
    {
        return val.Any(char.IsDigit);
    }
}