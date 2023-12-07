using System;
using System.Collections.Generic;
using System.Linq;
using CreditButler.Data;
using HtmlAgilityPack;
using Microsoft.Toolkit.Diagnostics;

namespace CreditButler.TransactionGenerators;

public class RBCOnlineHTMLTransactionGenerator : HTMLTransactionGenerator
{
    private DateTime? startDate = null;
    private DateTime? endDate = null;

    private bool ignoreCredits = true;
    private bool ignoreDebits = true;

    private string transactionTableXPath = "";

    public RBCOnlineHTMLTransactionGenerator(string filePath) : base(filePath) { }

    public RBCOnlineHTMLTransactionGenerator OnDateInterval(DateTime start, DateTime end)
    {
        (startDate, endDate) = (start, end);
        return this;
    }

    public RBCOnlineHTMLTransactionGenerator WithCredits()
    {
        this.ignoreCredits = false;
        return this;
    }

    public RBCOnlineHTMLTransactionGenerator WithDebits()
    {
        this.ignoreDebits = false;
        return this;
    }

    /// <summary>
    /// Set the XPath used to look up the table of transactions you want to use.
    /// Note that the XPath should target the <c>tbody</c> element, not the 
    /// <c>table</c> element.
    /// </summary>
    public RBCOnlineHTMLTransactionGenerator WithTransactionTBodyXPath(string xpath)
    {
        transactionTableXPath = xpath;
        return this;
    }

    protected override IEnumerable<Transaction> GenerateFromHTMLDocument(HtmlDocument document)
    {
        Guard.IsNotEmpty(transactionTableXPath, nameof(transactionTableXPath));

        HtmlNode rootNode = document.DocumentNode;
        HtmlNode transactionTable = rootNode.SelectSingleNode(transactionTableXPath);

        // The table is expected to look something like
        // <tbody>
        //   <tr>
        //     <td>Date String</td>
        //     <td>
        //       <div>
        //         <div>Transaction Name</div>
        //       </div>
        //     </td>
        //     <td>
        //       <span>debit value</span>
        //     </td>
        //     <td>
        //       <span>credit value</span>
        //     </td>
        //   </tr>
        // </tbody>

        foreach (var row in transactionTable.Elements("tr"))
        {
            HtmlNode[] dataEntries = row.Elements("td").ToArray();
            string dateStr = dataEntries[0].InnerText;
            string nameStr = dataEntries[1].InnerText.Trim();
            string debitStr = dataEntries[2].InnerText;
            string creditStr = dataEntries[3].InnerText;

            DateTime date = DateTime.Parse(dateStr);

            bool isBeforeStartDate = startDate.HasValue && startDate > date;
            bool isAfterEndDate = endDate.HasValue && endDate < date;
            if (isBeforeStartDate || isAfterEndDate)
            {
                continue;
            }

            Money? debitValue, creditValue;
            _ = Money.TryParse(debitStr, out debitValue);
            _ = Money.TryParse(creditStr, out creditValue);

            if (!ignoreDebits && debitValue.HasValue)
            {
                yield return new Transaction(nameStr, date, debitValue.Value, Transaction.TransactionType.Debit);
            }

            if (!ignoreCredits && creditValue.HasValue)
            {
                yield return new Transaction(nameStr, date, creditValue.Value, Transaction.TransactionType.Credit);
            }
        }
    }
}
