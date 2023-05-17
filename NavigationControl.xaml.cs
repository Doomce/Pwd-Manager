// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PasswordManagerWINUI.Appearance;
using PasswordManagerWINUI.BackEndLogic.Database;
using PasswordManagerWINUI.FrontEnd;
using PasswordManagerWINUI.FrontEnd.Dialogs;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PasswordManagerWINUI
{
    public partial class NavigationControl : UserControl
    {
        private FrameNavigationOptions navOptions = new FrameNavigationOptions();

        public static void ShowMessage(string title, string message, InfoBarSeverity severity)
        {
            GlobalInfoBar.MessageBar.IsOpen = false;
            GlobalInfoBar.MessageBar.Title = title;
            GlobalInfoBar.MessageBar.Message = message;
            GlobalInfoBar.MessageBar.Severity = severity;
            GlobalInfoBar.MessageBar.IsOpen = true;
        }

        public NavigationControl()
        {
            InitializeComponent();
            ContentFrame.NavigateToType(typeof(FirstPage), null , navOptions);
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact)
            {
                navOptions.IsNavigationStackEnabled = false;
            }
            Type pageType;
            switch (args.InvokedItem)
            {
                case "Vartotojo paskyra":
                    {
                        pageType = typeof(FirstPage);
                        break;
                    }
                case "Slaptaþodþiai":
                    {
                        if (SqlMethods.PwMngAccount == null)
                        {
                            new WarningDialog(this.XamlRoot).WithNoAccMessage();
                            return;
                        }
                        pageType = typeof(SecondPage);
                        break;
                    }
                case "Parametrai":
                    {
                        pageType = typeof(SecondPage);
                        break;
                    }
                default:
                    {
                        pageType = typeof(SecondPage);
                        break;
                    }
            }
            ContentFrame.NavigateToType(pageType, null, navOptions);

        }
    }
}
