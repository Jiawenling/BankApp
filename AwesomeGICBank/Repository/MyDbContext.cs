using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AwesomeGICBank.Models;

namespace AwesomeGICBank.Repository
{
public class MyDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=:memory:"); 
    }

    public DbSet<Transactions> Balance { get; set; }
    public DbSet<InterestRateRules> InterestRates { get; set; }

    public async Task<bool> AccountExists(string accountID)
    {
        return await Balance.AsNoTracking().AnyAsync(x => x.AccountId == accountID);
    }
    public async Task<Transactions> GetLastTransaction(string accountID)
    {
        return await Balance.AsNoTracking().Where(x => x.AccountId == accountID).OrderBy(x => x.Date).LastOrDefaultAsync();
    }

    public async Task<List<Transactions>> GetAllTransactions(string accountID)
    {
        return await Balance.AsNoTracking().Where(x => x.AccountId == accountID).OrderBy(x => x.Date).ToListAsync();
    }

    public async Task<List<Transactions>> GetTransactionsByMonth(string accountId, DateTime dt)
    {
        var endMonth = dt.AddMonths(1);
        return await Balance.AsNoTracking().Where(x => x.AccountId == accountId && x.Date >= dt && x.Date < endMonth).OrderBy(x => x.Date).ToListAsync();
    }

    public async Task<Transactions> GetFirstBalanceOfCurrentMonth(string accountId, DateTime dt)
    {
        return await Balance.AsNoTracking().Where(x => x.AccountId == accountId && x.Date <= dt).OrderBy(x => x.Date).LastOrDefaultAsync();
    }

    public async Task<InterestRateRules> GetInterestOnSameDay(DateTime dateTime)
    {
        return await InterestRates.Where(x => x.Date == dateTime).FirstOrDefaultAsync();
    }

    public async Task<List<InterestRateRules>> GetAllInterestRateRules()
    {
        return await InterestRates.AsNoTracking().OrderBy(x => x.Date).ToListAsync();
    }

    public async Task<List<InterestRateRules>> GetInterestRateRulesByMonth(DateTime dt)
    {
        var endMonth = dt.AddMonths(1);
        return await InterestRates.AsNoTracking().Where(x => x.Date >= dt && x.Date < endMonth).OrderBy(x => x.Date).ToListAsync();
    }

    public async Task<InterestRateRules> GetFirstInterestOfCurrentMonth(DateTime dt)
    {
        return await InterestRates.AsNoTracking().Where(x => x.Date <= dt).OrderBy(x => x.Date).LastOrDefaultAsync();
    }




}
}