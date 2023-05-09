using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerWINUI.Appearance
{
    public static class GlobalInfoBar
    {
        public static InfoBar MessageBar { get; set; } = new InfoBar() { Message = "INIT" };
    }
}
