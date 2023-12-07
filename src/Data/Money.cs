using System;
using System.Text.RegularExpressions;

namespace CreditButler.Data;

public struct Money
{
    private bool negative;
    private uint cents;

    public Money(bool negative, uint dollars, uint cents)
    {
        this.negative = negative;
        this.cents = (dollars * 100) + cents;
    }

    public readonly bool IsNegative()
    {
        return negative;
    }

    public readonly uint GetDollars()
    {
        return cents / 100;
    }

    public readonly uint GetCents()
    {
        return cents % 100;
    }

    public static Money Parse(string str)
    {
        const string NEGATION_KEY = "negation";
        const string DOLLARS_KEY = "dollars";
        const string CENTS_KEY = "cents";

        // https://regex101.com/r/baaqe9/1
        var regexResult = Regex.Match(str, @$"^(?'{NEGATION_KEY}'-?)\$?(?'{DOLLARS_KEY}'\d+)(?:\.(?'{CENTS_KEY}'\d+))?$");
        if (!regexResult.Success)
        {
            throw new FormatException($"String '{str}' was not recognized as a valid {nameof(Money)}");
        }

        var groups = regexResult.Groups;

        bool negative = false;
        uint dollars = 0;
        uint cents = 0;

        if (groups.TryGetValue(NEGATION_KEY, out Group? negationValue))
        {
            negative = !string.IsNullOrEmpty(negationValue?.Value);
        }

        if (groups.TryGetValue(DOLLARS_KEY, out Group? dollarsValue))
        {
            _ = uint.TryParse(dollarsValue.Value, out dollars);
        }

        if (groups.TryGetValue(CENTS_KEY, out Group? centsValue))
        {
            _ = uint.TryParse(centsValue.Value, out cents);
        }

        return new Money(negative, dollars, cents);
    }

    public static bool TryParse(string str, out Money? result)
    {
        try
        {
            result = Parse(str);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public override string ToString()
    {
        string negativeStr = IsNegative() ? "-" : "";
        return $"{negativeStr}${GetDollars()}.{GetCents()}";
    }
}
