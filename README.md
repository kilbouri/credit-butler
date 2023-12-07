# Credit Butler

My parents are wonderful and cover my grocery expenses while I'm at University. But I have a bad habit of not
tracking my grocery bills. I keep receipts, but that's a lot of work to sift through at once.

So, I decided to make a little app that lets me scrape my online banking page's transaction history for the
credit card I purchase groceries on, then filter the purchases down to which ones are groceries and which aren't.
The program then tells me exactly how much I spent on groceries over a given time period.

This probably isn't too useful to most people, but it is for me. For that reason I'm not going to provide much in the way of instructions.

To run it, you need .NET 6. `dotnet run` to build & run. But before that you need to customize the software to your needs. The XPath you provide the
scraper will probably be different. Also, I recommend `Save Page WE` for saving the currently-rendered view of your online banking portal.
