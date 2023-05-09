using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace PasswordManagerWINUI.BackEndLogic
{
    internal class Security
    {
        private static DateTime NextCheck = DateTime.MinValue;


        public static async Task<bool> CheckSecurity()
        {
            if (NextCheck != DateTime.MinValue && DateTime.Compare(NextCheck, DateTime.Now) > 0) return true;

            Debug.WriteLine(await KeyCredentialManager.IsSupportedAsync());
            var credOpt = new KeyCredentialCreationOption();

            try
            {
                var res = await KeyCredentialManager.RequestCreateAsync("NordPass-Padielka", credOpt);
                if (res.Status.Equals(KeyCredentialStatus.UserCanceled)) throw new System.NullReferenceException();
                
                var pubKey = res.Credential.RetrievePublicKey();
                var signres = await res.Credential.RequestSignAsync(pubKey);
                if (signres.Status.Equals(KeyCredentialStatus.Success))
                {
                    NavigationControl.ShowMessage("Sėkminga!", "Jūsų tapatybė patvirtinta! Dabar galite peržiūrėti slaptažodžius.", InfoBarSeverity.Success);
                    NextCheck = DateTime.Now.AddSeconds(45);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                NavigationControl.ShowMessage("Nepavyko atlikti tapatybės nustatymo.", "Galimai atšaukėte tapatybės patvirtinimą arba įvyko netikėta klaida.", InfoBarSeverity.Warning);
                return false;
            }
        }


    }
}
