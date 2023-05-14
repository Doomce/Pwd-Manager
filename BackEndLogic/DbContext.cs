using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using PasswordManagerWINUI.BackEndLogic.Objects;

namespace PasswordManagerWINUI.BackEndLogic;

internal class PassMngDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    private string _sqlConnStr = new MySqlConnectionStringBuilder()
    {
        Server = "localhost",
        UserID = "root",
        Password = "Prestige14",
        Database = "pass_manager"
    }.ConnectionString;

    public PassMngDbContext() {
        try
        {
            var databaseCreator = (Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator);
            databaseCreator.CreateTables();
        }
        catch (MySqlException)
        {
            //A SqlException will be thrown if tables already exist. So simply ignore it.
        } 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().HasKey(p => new {p.UserId, p.Platform, p.UserName });
        
        modelBuilder.Entity<User>()
            .HasMany(user => user.Accounts)
            .WithOne(acc => acc.User)
            .HasForeignKey("UserId")
            .IsRequired(false);
    }
    
    
    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySql(_sqlConnStr, ServerVersion.AutoDetect(_sqlConnStr));
    }
}