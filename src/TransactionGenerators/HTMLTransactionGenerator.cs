using System.Collections.Generic;
using CreditButler.Data;
using HtmlAgilityPack;

namespace CreditButler.TransactionGenerators;

public abstract class HTMLTransactionGenerator : TransactionGenerator
{
    private readonly string filePath;

    /// <summary>
    /// Creates a <see cref="TransactionGenerator"/> from an HTML file.
    /// </summary>
    /// <param name="filePath"></param>
    public HTMLTransactionGenerator(string filePath) => this.filePath = filePath;

    public IEnumerable<Transaction> Generate()
    {
        HtmlDocument document = new()
        {
            OptionFixNestedTags = true
        };
        document.Load(filePath);

        return GenerateFromHTMLDocument(document);
    }

    protected abstract IEnumerable<Transaction> GenerateFromHTMLDocument(HtmlDocument document);
}
