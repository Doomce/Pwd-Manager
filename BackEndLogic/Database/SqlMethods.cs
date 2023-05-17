using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using PasswordManagerWINUI.BackEndLogic.Objects;

namespace PasswordManagerWINUI.BackEndLogic.Database;

internal class SqlMethods
{
    #nullable enable
    internal static User? PwMngAccount { get; set; }

    public async static void AddOrGetUser(string msAccId)
    {
        using var db = new MysqlDbContext();
        PwMngAccount = await db.Users.FirstAsync(user => user.MsUUID == msAccId);
        Console.WriteLine(PwMngAccount);
    }

    public static void LogOutUser()
    {
        if (PwMngAccount == null) return;
        PwMngAccount = null;
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
        account.EncryptPassword("123");
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