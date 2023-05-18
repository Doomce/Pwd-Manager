using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Windows.Graphics.Printing;

namespace PasswordManagerWINUI.BackEndLogic.Objects;

internal class User
{
    
    public ulong UserId { get; set; }
    public string MsUUID { get; set; }
    public DateTime RegDate { get; set; } = DateTime.Now;
    //TOOD: Galbut panaudosim druskai ir pipirams...

    public byte[] PublicKey { get; set; } = Security.GeneratePublicKey();
    
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();


}