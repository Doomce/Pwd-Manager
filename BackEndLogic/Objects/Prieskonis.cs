namespace PasswordManagerWINUI.BackEndLogic.Objects;

internal class Prieskonis
{
    public static void Papipirinti(ref Account account)
    {
        int slot = account.Generated.Millisecond % 256;
        int perPos = account.Generated.Minute + account.Generated.Second + account.Generated.Hour + account.Generated.Month;
        perPos -= account.Generated.Day;
        var bitas = (int)account.PassKey[slot];
        int difference = (bitas + perPos + 256) % 256;
        account.PassKey[slot] = (byte)difference;
    }
    
    public static void Padruskinti(ref Account account)
    {
        int slot = account.Generated.Millisecond % 256;
        int perPos = account.Generated.Minute + account.Generated.Second + account.Generated.Hour + account.Generated.Month;
        perPos -= account.Generated.Day;
        var bitas = (int)account.PassKey[slot];
        int difference = (bitas - perPos + 256) % 256;
        account.PassKey[slot] = (byte)difference;
    }

}