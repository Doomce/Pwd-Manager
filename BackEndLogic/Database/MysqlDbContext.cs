using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using PasswordManagerWINUI.BackEndLogic.Objects;

namespace PasswordManagerWINUI.BackEndLogic.Database;

internal class MysqlDbContext : DbContext
{
    public DbSet<User> Users { get; private set; }
    public DbSet<Account> Accounts { get; private set; }

    private string _sqlConnStr = new MySqlConnectionStringBuilder()
    {
        Server = "localhost",
        UserID = "root",
        Password = "Prestige14",
        Database = "pass_manager"
    }.ConnectionString;

    public MysqlDbContext()
    {
        try
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            databaseCreator.CreateTables();
        }
        catch (MySqlException) { }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasKey(p => new { p.UserId, p.Platform, p.UserName });

        modelBuilder.Entity<Account>()
            .HasOne<User>()
            .WithMany(user => user.Accounts)
            .HasForeignKey(acc => acc.UserId);

        modelBuilder.Entity<User>()
            .HasMany(user => user.Accounts)
            .WithOne(acc => acc.User)
            .HasForeignKey("UserId")
            .IsRequired(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySql(_sqlConnStr, ServerVersion.AutoDetect(_sqlConnStr));
    }
}