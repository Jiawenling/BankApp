using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwesomeGICBank.Models;
using AwesomeGICBank.Repository;
using AwesomeGICBank.Utils;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AwesomeGICBank.Service
{
    internal class BankService
    {
        private bool _quit = false;
        private MyDbContext _db;

        public BankService(MyDbContext context)
        {
            _db = context;
        }

        public async Task Start()
        {
            while (!_quit)
            {
                Console.WriteLine(Constants.MESSAGE_WELCOME);

                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                await ParseMenuOptions(input);
            }
        }

        private async Task ParseMenuOptions(string input)
        {
            switch (input.ToLower())
            {
                case "q":
                    _quit = true;
                    Console.WriteLine();
                    Console.WriteLine(Constants.MESSAGE_QUIT);
                    break;
                case "t":
                    await PerformAction(Constants.MESSAGE_TRANSACTIONS, InputTransaction);
                    break;
                case "i":
                    await PerformAction(Constants.MESSAGE_INTERESTRATES, SetInterestRateRules);
                    break;
                case "p":
                    await PerformAction(Constants.MESSAGE_STATEMENT, PrintBalance);
                    break;
                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid Input!");
                    Console.WriteLine();
                    break;

            }
        }

        private string GetNextTxnId(string currentDate, string lastTxnId)
        {
            var arr = lastTxnId.Split('-');
            var lastDate = arr[0];
            
            if (currentDate == lastDate)
            {
                int.TryParse(arr[1].TrimStart('0'), out int currNum);
                var nextNumber = currNum + 1;
                var nextNumString = currNum < 10? "0" + nextNumber: nextNumber.ToString();
                return lastDate + '-' + nextNumString;
            }
            return currentDate + "-01"; 
        }

        private async Task InputTransaction(string input)
        {
            string[] inputs = input.Split(' ');
            if (inputs.Length < 4) throw new Exception("Invalid number of inputs");
            var currentDate = inputs[0];
            var dt = InputParser.ParseDate(currentDate);
            var account = inputs[1];
            var type = InputParser.ParseTypeTransaction(inputs[2]);
            var amount = InputParser.ParseAmount(inputs[3]);

            bool exists = await _db.AccountExists(account);
            if (!exists)
            {
                if (type == Constants.WITHDRAW) throw new Exception("First transaction cannot be a withdrawal. Transaction not successful");
                _db.Balance.Add(new Models.Transactions()
                {
                    AccountId = account,
                    TransactionType = type,
                    Amount = amount,
                    Date = dt,
                    TxnId = currentDate + "-01",
                    Balance = amount
                });
                _db.SaveChanges();
            }
            else
            {
                var lastTransaction = await _db.GetLastTransaction(account);
                if (dt < lastTransaction.Date) throw new Exception("Date must be later than last transaction date. Transaction not successful");
                var balance = 0.00;
                if(type == Constants.WITHDRAW)
                {
                    balance = lastTransaction.Balance - amount;
                    if (balance < 0) throw new Exception("You cannot withdraw more than your current balance. Transaction not successful");
                }
                else
                {
                    balance = lastTransaction.Balance + amount;
                }
                _db.Balance.Add(new Models.Transactions()
                {
                    AccountId = account,
                    TransactionType = type,
                    Amount = amount,
                    Date = dt,
                    TxnId = GetNextTxnId(currentDate, lastTransaction.TxnId),
                    Balance = balance
                });
                _db.SaveChanges();

            }

            await ShowTransactions(account);

        }

        private async Task ShowTransactions(string accountId)
        {
            var transactions = await _db.GetAllTransactions(accountId);
            var transformed = transactions.Select(x => new List<string>() { x.Date.ToString("yyyyMMdd"), x.TxnId, x.TransactionType.ToString(), x.Amount.ToString("F") }).ToList();
            TableGenerator.PrintTable($"Account: {accountId}",new List<string>() { "Date", "Txn Id", "Type", "Amount" }, transformed);
        }

        private async Task SetInterestRateRules(string input)
        {
            string[] inputs = input.Split(' ');
            if (inputs.Length < 3) throw new Exception("Invalid number of inputs");
            var dt = InputParser.ParseDate(inputs[0]);
            var ruleId = inputs[1];
            var interest = InputParser.ParseInterest(inputs[2]);

            var lastInterestRate = await _db.GetInterestOnSameDay(dt);

            if (lastInterestRate!=null)
            {
                lastInterestRate.Rate = interest;
                lastInterestRate.RuleId = ruleId;
                _db.Update(lastInterestRate);
                _db.SaveChanges();
            }
            else
            {
                _db.Add(new InterestRateRules()
                {
                    Date = dt,
                    RuleId = ruleId,
                    Rate = interest,
                });
                _db.SaveChanges();
            }
            
            await PrintInterestRateRules();
        }

        private async Task PrintInterestRateRules()
        {
            var data = await _db.GetAllInterestRateRules();
            var transformed = data.Select(x => new List<string> { x.Date.ToString(Constants.DATEFORMAT), x.RuleId, x.Rate.ToString("F") })
                .ToList();
            TableGenerator.PrintTable("Interest rules:", new List<string>() { "Date", "RuleId", "Rate (%)" }, transformed);
        }

        private async Task PrintBalance(string input)
        {
            string[] inputs = input.Split(' ');
            if (inputs.Length < 2) throw new Exception("Invalid number of inputs");
            var dt = InputParser.ParseDate(inputs[1], "yyyyMM");
            var firstDayOfMonth = new DateTime(dt.Year, dt.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var account = inputs[0];
            if(!await _db.AccountExists(account))
                throw new Exception("Account does not exist");

            var transactions = await _db.GetTransactionsByMonth(account, firstDayOfMonth);
            if (transactions == null || transactions.Count <= 0)
                throw new Exception("There are no transactions for this month");


            var interestRateRule = await _db.GetInterestRateRulesByMonth(firstDayOfMonth);
            var q = transactions.Select(x => new { Type = "B", Amt = x.Balance, Date = x.Date })
                .Concat(interestRateRule.Select(x => new { Type = "R", Amt = x.Rate, Date = x.Date }))
                .OrderBy(x => x.Date)
                .ToList();
            var firstInterest = await _db.GetFirstInterestOfCurrentMonth(firstDayOfMonth);
            var firstBal = await _db.GetFirstBalanceOfCurrentMonth(account, firstDayOfMonth);

            var interest = 0.00;
            var date = firstDayOfMonth;
            var rate = firstInterest?.Rate ?? 0.00;
            var bal = firstBal?.Balance ?? 0.00;
            foreach (var x in q) {
                if (x.Date > date)
                {
                    interest += ((x.Date - date).Days * bal * rate / 100);
                    date = x.Date;
                }
                if (x.Type == "B")
                {
                    bal = x.Amt;
                }
                else 
                {
                    rate = x.Amt;
                }
            }

            if (date < lastDayOfMonth)
            {
                interest += ((lastDayOfMonth.AddDays(1) - date).Days * bal * rate / 100);
            }

            interest /= 365; 

            transactions.Add(new Transactions()
            {
                AccountId = account,
                Amount = interest,
                TxnId = " ",
                TransactionType = 'I',
                Date = lastDayOfMonth,
                Balance = bal + interest,
            });

            var transformed = transactions.Select(x => new List<string>() { x.Date.ToString("yyyyMMdd"), x.TxnId, x.TransactionType.ToString(), x.Amount.ToString("F") , x.Balance.ToString("F")}).ToList();
            TableGenerator.PrintTable($"Account: {account}", new List<string>() { "Date", "Txn Id", "Type", "Amount", "Balance"}, transformed);
        }

        private async Task PerformAction(string message, Func<string, Task> bankAction)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine(message);
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) return;
                await bankAction.Invoke(input);

            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("-----------------------");
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("-----------------------");
            }
            Console.WriteLine();
            while (!_quit)
            {
            Console.WriteLine(Constants.MESSAGE_NEXTACTION);
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) break;
                await ParseMenuOptions(input);
            }
        }

    }

        
    
}
