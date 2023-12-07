using System;

namespace CreditButler.Data;

public readonly struct Transaction
{
    public enum TransactionType
    {
        Credit,
        Debit
    }

    public readonly string Name;
    public readonly DateTime Date;
    public readonly Money Value;
    public readonly TransactionType Type;

    public Transaction(string name, DateTime date, Money value, TransactionType type)
    {
        (Name, Date, Value, Type) = (name, date, value, type);
    }

    public override string ToString()
    {
        string debitCreditString = this.Type == TransactionType.Credit ? "Credit" : "Debit";
        return $"{debitCreditString}ed {Value} for {Name} on {Date}";
    }
}
