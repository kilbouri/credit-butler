using System.Collections.Generic;
using System.Linq;
using CreditButler.Data;
using CreditButler.TransactionGenerators;
using Spectre.Console;

TransactionGenerator generator = new RBCOnlineHTMLTransactionGenerator("./test.html")
    // .WithCredits()
    .WithDebits()
    .WithTransactionTBodyXPath("/html/body/app-root/rbc-online-banking-layout/div/table/tbody/tr/td/main/rbc-details-layout-widget/rbc-frame/article/section/div[3]/rbc-transaction-table-container/rbc-cc-transactions-container/div[2]/rbc-cc-transaction-table/div/div/div[1]/table/tbody");

// butler to figure out how much my parents owe me for groceries and stuff
Dictionary<string, bool> knownVendors = new();
Dictionary<string, Money> vendorCosts = new();
Transaction[] transactions = generator.Generate().ToArray();

Money totalOwing = new Money(false, 0, 0);
foreach (Transaction transaction in transactions)
{
    if (knownVendors.TryGetValue(transaction.Vendor, out bool isIncluded))
    {
        if (isIncluded)
        {
            totalOwing += transaction.Value;
            vendorCosts[transaction.Vendor] += transaction.Value;
        }

        continue;
    }

    // we haven't seen this vendor before, we need to ask whether or not to include it
    bool shouldInclude = AnsiConsole.Confirm($"Should [red]{transaction.Vendor}[/] be included?");
    knownVendors.Add(transaction.Vendor, shouldInclude);

    if (shouldInclude)
    {
        totalOwing += transaction.Value;
        vendorCosts.Add(transaction.Vendor, transaction.Value);
    }
}

AnsiConsole.Write(new Rule("Summary")
{
    Justification = Justify.Left,
});

foreach (var (vendor, total) in vendorCosts.OrderBy(x => x.Key))
{
    AnsiConsole.MarkupLine($"[white]{vendor}:[/] [grey]{total}[/]");
}

AnsiConsole.MarkupLine($"[green]Total: {totalOwing}[/]");
