using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace PasswordManagerWINUI.BackEndLogic.Objects;

internal class User
{
    
    public ulong UserId { get; set; }
    public string UserName { get; set; }
    
    public DateTime RegDate { get; set; } = DateTime.Now;
    //TOOD: Galbut panaudosim druskai ir pipirams...




    public byte[] PublicKey { get; set; } = Security.GeneratePublicKey();
    
    public ICollection<Account> Accounts { get; } = new List<Account>();
    
   
}