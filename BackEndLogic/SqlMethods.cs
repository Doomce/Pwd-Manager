using System;
using PasswordManagerWINUI.BackEndLogic.Objects;

namespace PasswordManagerWINUI.BackEndLogic;

internal class SqlMethods
{


    public static void getUsers()
    {
        using var db = new PassMngDbContext();
        User? user = db.Find<User>((ulong)1);
        
        if (user == null)
        {
            user = new User() { UserName = "Petras" };
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
        using var db = new PassMngDbContext();
        User? petras = db.Find<User>((ulong)1);
        
        if (petras == null)
        {
            Console.WriteLine("Petro nera");
            return;
        }

        var account = db.Find<Account>((ulong)petras.UserId, "Google.com", "Test");
        if (account == null)
        {
            Console.WriteLine("Nepavyko surasti acc");
            return;
        }
        
        Prieskonis.Padruskinti(ref account);
        Console.WriteLine(Security.DecryptPassword(account.PassKey));
    }
}