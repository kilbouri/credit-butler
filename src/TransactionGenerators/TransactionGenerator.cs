using System.Collections.Generic;
using CreditButler.Data;

namespace CreditButler.TransactionGenerators;

public interface TransactionGenerator
{
    /// <summary>
    /// Generates a sequence of transactions.
    /// </summary>
    /// <returns>A sequence of transactions.</returns>
    IEnumerable<Transaction> Generate();
}
