using PasswordManagerWINUI.BackEndLogic.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerWINUI.BackEndLogic.Database
{
    internal class LocalStorage
    {
        public static User AppUser { get; set; }
        private static string DBFilePath { get; set; }




        LocalStorage() 
        {
            try
            {
                // For packaged desktop apps (MSIX packages, also called desktop bridge) the executing assembly folder is read-only. 
                // In that case we need to use Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path + "\msalcache.bin" 
                // which is a per-app read/write folder for packaged apps.
                // See https://docs.microsoft.com/windows/msix/desktop/desktop-to-uwp-behind-the-scenes
                DBFilePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, ".msalcache.bin3");
            }
            catch (InvalidOperationException)
            {
                DBFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalcache.bin3";
            }
        }


    }
}
