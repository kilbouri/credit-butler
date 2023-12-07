using System;
using CreditButler.TransactionGenerators;

TransactionGenerator generator = new RBCOnlineHTMLTransactionGenerator("./test.html")
    .WithCredits()
    .WithDebits()
    .WithTransactionTBodyXPath("/html/body/app-root/rbc-online-banking-layout/div/table/tbody/tr/td/main/rbc-details-layout-widget/rbc-frame/article/section/div[3]/rbc-transaction-table-container/rbc-cc-transactions-container/div[2]/rbc-cc-transaction-table/div/div/div[1]/table/tbody");

foreach (var transaction in generator.Generate())
{
    Console.WriteLine(transaction);
}
