using System;

namespace CreditButler.Data;

public readonly struct Transaction
{
    public enum TransactionType
    {
        Credit,
        Debit
    }

    public readonly string Vendor;
    public readonly DateTime Date;
    public readonly Money Value;
    public readonly TransactionType Type;

    public Transaction(string name, DateTime date, Money value, TransactionType type)
    {
        (Vendor, Date, Value, Type) = (name, date, value, type);
    }

    public override string ToString()
    {
        string debitCreditString = this.Type == TransactionType.Credit ? "Credit" : "Debit";
        return $"{debitCreditString}ed {Value} for {Vendor} on {Date}";
    }
}
