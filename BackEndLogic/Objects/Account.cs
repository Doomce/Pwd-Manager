using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManagerWINUI.BackEndLogic.Objects;

internal class Account
{
    public string Platform { get; set; }
    public string UserName { get; set; }
    
    public byte[] PassKey { get; set; }
    public DateTime Generated { get; set; } = DateTime.Now;

    public ulong UserId { get; set; } 
    public virtual User User { get; set; }
    
    [NotMapped]
    private string Salt
    {
        set { value = "SSS"; }
    }
    
    
    public void EncryptPassword(string password, byte[] pubKey)
    {
        using RSA rsa = RSA.Create(2048);
        rsa.ImportRSAPublicKey(pubKey, out _);
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(password);
        
        PassKey = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.OaepSHA256);
    }

    public PasswordItem ToPasswordItem()
    {
        var acc = this;
        Prieskonis.Padruskinti(ref acc);
        return new PasswordItem()
        {
            Title = acc.Platform,
            UserName = acc.UserName,
            Password = Security.DecryptPassword(acc.PassKey)
        };
    }
}