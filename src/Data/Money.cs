using System;
using System.Text.RegularExpressions;

namespace CreditButler.Data;

public struct Money
{
    private bool negative;
    private uint cents;

    /// <summary>
    /// Creates a Money value of $0.00
    /// </summary>
    public Money() : this(false, 0, 00) { }

    /// <summary>
    /// Creates an optionally-negative Money value with the provided dollars and cents.
    /// </summary>
    public Money(bool negative, uint dollars, uint cents) : this(negative, 100 * dollars + cents) { }

    /// <summary>
    /// Creates an optionally-negative Money value from cents alone. Cents exceeding 100 will
    /// be rolled into dollars.
    /// </summary>
    public Money(bool negative, uint cents)
    {
        this.negative = negative;
        this.cents = cents;
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

    public override readonly string ToString()
    {
        string negativeStr = IsNegative() ? "-" : "";
        string dollarStr = GetDollars().ToString();
        string centStr = GetCents().ToString().PadLeft(2, '0');
        return $"{negativeStr}${dollarStr}.{centStr}";
    }

    public static Money operator +(Money mine, Money other)
    {
        long myCents = (mine.negative ? -1 : 1) * mine.cents;
        long theirCents = (other.negative ? -1 : 1) * other.cents;

        long totalCents = myCents + theirCents;
        bool negative = totalCents < 0;

        uint unsignedCents;
        checked
        {
            unsignedCents = (uint)Math.Abs(totalCents);
        }

        return new Money(negative, unsignedCents);
    }

    public static Money operator -(Money mine, Money other)
    {
        long myCents = (mine.negative ? -1 : 1) * mine.cents;
        long theirCents = (other.negative ? -1 : 1) * other.cents;

        long totalCents = myCents - theirCents;
        bool negative = totalCents < 0;

        uint unsignedCents;
        checked
        {
            unsignedCents = (uint)Math.Abs(totalCents);
        }

        return new Money(negative, unsignedCents);
    }

    public static Money Parse(string str)
    {
        const string NEGATION_KEY = "negation";
        const string DOLLARS_KEY = "dollars";
        const string CENTS_KEY = "cents";

        var regexResult = Regex.Match(str, @$"^(?'{NEGATION_KEY}'-?)\$?(?'{DOLLARS_KEY}'\d+)(?:\.(?'{CENTS_KEY}'\d\d?))?$");
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
            _ = uint.TryParse(centsValue.Value.PadRight(2, '0'), out cents);
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
}
