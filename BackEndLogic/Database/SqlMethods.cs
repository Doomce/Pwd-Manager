using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using PasswordManagerWINUI.BackEndLogic.Objects;

namespace PasswordManagerWINUI.BackEndLogic.Database;

internal class SqlMethods
{
    #nullable enable
    internal static User? PwMngAccount { get; set; }

    public static async void AddOrGetUser(string msAccId)
    {
        await using (var db = new MysqlDbContext())
        {
            try
            {
                PwMngAccount = await db.Users.FirstAsync(user => user.MsUUID == msAccId);
            }
            catch (InvalidOperationException)
            {
                PwMngAccount = new User() { MsUUID = msAccId };
                db.Users.Add(PwMngAccount);
                db.SaveChanges();
            }
        }
    }

    public static void LogOutUser()
    {
        if (PwMngAccount == null) return;
        PwMngAccount = null;
    }
    
    public static async Task<bool> WritePassToDB(PasswordItem pwItem)
    {
        if (PwMngAccount == null) return false;
        var db = new MysqlDbContext();
        var account = new Account()
        {
            Platform = pwItem.Title,
            UserName = pwItem.UserName,
        };
        account.EncryptPassword(pwItem.Password, PwMngAccount.PublicKey);
        Prieskonis.Papipirinti(ref account);
        PwMngAccount.Accounts.Add(account);
        db.Users.Update(PwMngAccount);
        db.SaveChanges();
        return true;
    }
    
    public static async Task<List<Account>> GetPassFromDb()
    {
        if (PwMngAccount == null) return Enumerable.Empty<Account>().ToList();
        var db = new MysqlDbContext();
        return db.Accounts.Where(acc => acc.UserId == PwMngAccount.UserId).ToList();
    }

    public static async void RemovePass(string platform, string username)
    {
        var db = new MysqlDbContext();
        var acc = db.Accounts.Find((PwMngAccount.UserId, platform, username));
        PwMngAccount.Accounts.Remove(acc);
        db.Users.Update(PwMngAccount);
        db.SaveChanges();
    }
    
    public static async void EditAccount(Account account)
    {
        var db = new MysqlDbContext();
        var acc = db.Accounts.Entry(account);
        db.Users.Update(PwMngAccount);
        db.SaveChanges();
    }
    

    public static void getUsers()
    {
        using var db = new MysqlDbContext();
        User user = db.Find<User>((ulong)1);

        if (user == null)
        {
            //user = new User() { DisplayName = "Petras" };
            db.Users.Add(user);
        }

        var account = new Account()
        {
            Platform = "Google.com",
            UserName = "Test",
        };
        user.Accounts.Add(account);
        db.SaveChanges();
        account.EncryptPassword("123", user.PublicKey);
        Prieskonis.Papipirinti(ref account);
        db.SaveChanges();
    }

    public static void getUserPassword()
    {
        using var db = new MysqlDbContext();
        User petras = db.Find<User>((ulong)1);

        if (petras == null)
        {
            Console.WriteLine("Petro nera");
            return;
        }

        var account = db.Find<Account>(petras.UserId, "Google.com", "Test");
        if (account == null)
        {
            Console.WriteLine("Nepavyko surasti acc");
            return;
        }

        Prieskonis.Padruskinti(ref account);
        Console.WriteLine(Security.DecryptPassword(account.PassKey));
    }
}