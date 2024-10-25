
using System;
using System.Linq;
using AwesomeGICBank.Repository;
using AwesomeGICBank.Service;
using AwesomeGICBank.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using (var ctx = new MyDbContext())
{
    ctx.Database.OpenConnection();
    ctx.Database.EnsureCreated();

    var bankService = new BankService(ctx);
    await bankService.Start();
    ctx.Database.CloseConnection();
} 




