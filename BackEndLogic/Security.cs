using Microsoft.UI.Xaml.Controls;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace PasswordManagerWINUI.BackEndLogic
{
    internal class Security
    {
        private static readonly byte[] PrivateKey = Convert.FromBase64String("MIIEowIBAAKCAQEAxl2XKHdp5gkUN/gzmTmLHudjmjHKKBx3MgogQ4QeVEOo3yfxwKUmLoCHsmSkHzIJKgexhsKbf+q4wz4VjAPnd56sDi+ZID8Zn7AQfhtNcVVkLcmzMvRumd89zRnS+ff7FrOzQoD7LjWsDE90r+KABSPgg3fKrgbMfSN7eZVyUWo8nsZ4QBT618D3NTO2mKkWe0QtmW8MAsFiuqKWfITC2lj+ClylrthPybVHsBL61aL6vfCMBYUseXKU78+Gm0Kldvi4eZr5k2bsGVQBEozLL8J+Ri1b/lgGJv5f2V1MBknkL0nfjnbN6564sWNLuIyceL66RXAPnu0Ip1xyp6lp7QIDAQABAoIBADJrg4pjn+U6lYsMaYEe4T+/rM96rFm5sopjjIPcxyC/+eKO8qH22Felj9hGQHk1bKLkRbltKb0/2iq2Ux+9tk3vZOhOLOvHXin6xJZpNiwCem4xAH2JJ7uwxLKnR7sFzyqcDopu4mmCdvLCF3TFa6jiz6RbBIdrbvq31CC5Xc9XcC3IDUdBss8VwPRfLQ987Czrzeg13eT6+C5XKrZHafzxqYZ4/TRh1EQYjgu5cUdXwUHQtpIvKa0E+rRDX81yp4tK1JChS3SzXnQ8/hdkD1ojMzPlYtSP5PqysvU+gQtkPaNZebGyAlBu5+gWa7pKf0hcbWzTX0lPBGCH4JWe1LkCgYEA/UgOml4f1jwiN3lHymYsPI4YK9OsU8+pFeEK24geqiarHPVMlf4F1CTCkps3Ug38f3XbV7GuT0GjDYBLIPKrr2IIAly5q4uvk+OutHc1SqSvixYBqtwH4je91IcumzAKa3bb8OdMloKMMy+h9wFtWUX5NEnqgHRlPLDZLU7YOAcCgYEAyH6kBpZ0ey3acAii9tJYhsm7zs/gURE9x743io3UthCJufvprVCCrrKsqGn5TdnMpX0aVNUWlPxTkzgZV7j3BtVvGZUu0dN7qxRtuTjMx3qXTyoDsXkR0JmoSJA3Qvz0nRZfSS7mSi7ujiDezz+tuvaYUGPzSPRmVH/GmKYwSWsCgYBSNclHobWDvBD/IIaE05UYMqb10sbkjUq8p0b26r/JSrPPum8ZYFJUAXqGS4sxPwjt9jszw6BoaU2bXEEGeL3xN0iSO7aoT7a1sflK6kaJ69pusr3nz3Nfoegjy5z9EiLPrszE7M6XK9iF5LgVjIkJqQSyTCupdh5GDMDxS94ykQKBgQCkeAaUBN4eofm7x8nvUZQfVeODftFTjyIv9aFYPorMo3pnn/gMHaxmJNov+WbybwVKh/qOtpKkuuzQKsfXRzsVVwahZNiYdbTQHZz4wVhzFuSCo0OOVXPAvBvpEqzSffEn389gmGF/X0qAOwSr3F/mrB08bRGIujwrevnuBP49awKBgFf4F6Mq0GjLlSSPUZkV0OhoyCaWBvOj9H1elH9ir2wgixw79SXDuUG9v568e7xiEnMhbciEpzdDiK48Y/VKxWkZpjkmNxul50RGNW4c856nCm2kKqxxqaqJkyS0IaZS01LOxRcyrjRm4NMu7Lllqi/pKoTa4xb5t/E/efq6jEPC");
        //var privKEY = rsa.ExportRSAPrivateKey();
        //Convert.ToBase64String(privKEY);

        private static DateTime _nextCheck = DateTime.MinValue;
        private static bool _isAuthInProgress = false;

        public static byte[] GeneratePublicKey()
        {
            using RSA rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(PrivateKey, out _);
            return rsa.ExportRSAPublicKey();
        }
        
        public static string DecryptPassword(byte[] hash)
        {
            using RSA rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(PrivateKey, out _);
            return Encoding.UTF8.GetString(rsa.Decrypt(hash, RSAEncryptionPadding.OaepSHA256));
        }

        public static async Task<bool> CheckSecurity()
        {
            if (_isAuthInProgress) return false;
            if (_nextCheck != DateTime.MinValue && DateTime.Compare(_nextCheck, DateTime.Now) > 0) return true;
            var credOpt = new KeyCredentialCreationOption();
            NavigationControl.ShowMessage(
                "Luktelkite...", 
                "Vyksta tapatybės tikrinimo procesas.",
                InfoBarSeverity.Informational);
            _isAuthInProgress = true;
            try
            {
                var res = await KeyCredentialManager.RequestCreateAsync("NordPass-Padielka", credOpt);
                if (res.Status == KeyCredentialStatus.UserCanceled) throw new NullReferenceException();
                
                var pubKey = res.Credential.RetrievePublicKey();
                var signRes = await res.Credential.RequestSignAsync(pubKey);
                _isAuthInProgress = false;
                if (signRes.Status == KeyCredentialStatus.Success)
                {
                    NavigationControl.ShowMessage("Sėkminga!", "Jūsų tapatybė patvirtinta! Dabar galite peržiūrėti slaptažodžius.", InfoBarSeverity.Success);
                    _nextCheck = DateTime.Now.AddSeconds(45);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                _isAuthInProgress = false;
                NavigationControl.ShowMessage("Nepavyko atlikti tapatybės nustatymo.", "Galimai atšaukėte tapatybės patvirtinimą arba įvyko netikėta klaida.", InfoBarSeverity.Warning);
                return false;
            }
        }


    }
}
