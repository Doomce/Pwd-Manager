using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PasswordManagerWINUI.BackEndLogic.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Navigation;
using User = Microsoft.Graph.Models.User;

namespace PasswordManagerWINUI.BackEndLogic.Microsoft
{
    internal class MsAccount
    {
        public bool isHidden { get; set; } = false;

        private string[] scopes = new string[] { "user.read" };

        private const string ClientId = "bbda2122-0dc6-4f14-87c1-c9cbfce6ff7a";

        private const string Tenant = "common"; // Alternatively "[Enter your tenant, as obtained from the Azure portal, e.g. kko365.onmicrosoft.com]"
        private const string Authority = "https://login.microsoftonline.com/" + Tenant;

        private static IPublicClientApplication PublicClientApp;

        private static string MSGraphURL = "https://graph.microsoft.com/v1.0/";
        private AuthenticationResult authResult;
        private User AccountProperties;

        public MsAccount()
        {
            if (PublicClientApp != null) return;

            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithAuthority(Authority)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")

                .WithLogging((level, message, containsPii) =>
                {
                    Debug.WriteLine($"MSAL: {level} {message} ");
                }, LogLevel.Warning, enablePiiLogging: false, enableDefaultPlatformLogging: true)

                .Build();
            TokenCacheHelper.EnableSerialization(PublicClientApp.UserTokenCache);
        }

        public async Task<string> TryLoginToAccountAsync()
        {
            if (!isHidden) NavigationControl.ShowMessage("Luktelkite", "Jungiamės prie jūsų paskyros...", InfoBarSeverity.Informational);
            try
            {
                GraphServiceClient graphClient = await SignInAndInitializeGraphServiceClient(scopes);
                AccountProperties = await graphClient.Me.GetAsync();
                return AccountProperties.Id;
            }
            catch (MsalClientException)
            {
                NavigationControl.HideMessage();
            }
            catch (MsalException msalEx)
            {
                //NavigationControl.ShowMessage("Klaida", $"Error Acquiring Token:{Environment.NewLine}{msalEx}", InfoBarSeverity.Error);
            }
            catch (Exception ex)
            {
                //NavigationControl.ShowMessage("Klaida", $"Error Acquiring Token:{Environment.NewLine}{ex}", InfoBarSeverity.Error);
            }
            return null;
        }

        private async Task<string> SignInUserAndGetTokenUsingMSAL(string[] scopes)
        {
            IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync().ConfigureAwait(false);
            IAccount firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await PublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                    .WithForceRefresh(false)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                if (isHidden) return null;
                authResult = await PublicClientApp.AcquireTokenInteractive(scopes)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            return authResult.AccessToken;
        }

        private async Task<GraphServiceClient> SignInAndInitializeGraphServiceClient(string[] scopes)
        {
            var tokenProvider = new TokenProvider(SignInUserAndGetTokenUsingMSAL, scopes);
            var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
            var graphClient = new GraphServiceClient(authProvider, MSGraphURL);

            return await Task.FromResult(graphClient);
        }

        public void DisplayLoginInfo()
        {
            if (authResult != null && AccountProperties != null)
            {
                NavigationControl.ShowMessage("Sėkminga", $"Prisijungta su microsoft paskyra: {AccountProperties.DisplayName} ({AccountProperties.UserPrincipalName})", InfoBarSeverity.Success);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>1-DisplayName, 2-MsAccId</returns>
        public (string, string) GetUserData()
        {
            if (authResult == null && AccountProperties == null) return (null, null);
            var photo = AccountProperties.Photo;
            return (AccountProperties.DisplayName, AccountProperties.Id);
        }

        public async Task<bool> SignOut()
        {
            IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync().ConfigureAwait(false);
            IAccount firstAccount = accounts.FirstOrDefault();

            try
            {
                await PublicClientApp.RemoveAsync(firstAccount).ConfigureAwait(false);
                NavigationControl.ShowMessage("Sekminga", "Jūs atsijungėte nuo savo paskyros.", InfoBarSeverity.Success);
                return true;
            }
            catch (MsalException ex)
            {
                NavigationControl.ShowMessage("Error", $"Error signing out user: {ex.Message}", InfoBarSeverity.Error);
                return false;
            }
        }
    }
}
