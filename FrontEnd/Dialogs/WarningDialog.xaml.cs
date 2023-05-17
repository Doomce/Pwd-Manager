// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Application = Microsoft.UI.Xaml.Application;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PasswordManagerWINUI.FrontEnd.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WarningDialog : Page
    {
        private ContentDialog dialog = new ContentDialog();
        private XamlRoot Root;

        public WarningDialog(XamlRoot root)
        {
            this.InitializeComponent();

            dialog.XamlRoot = root;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.CloseButtonText = "Supratau";
            this.Background = dialog.Background;
            dialog.Content = this;
        }


        public async void WithNoAccMessage()
        {
            MsgTitle.Text = "Dëmesio!";
            MsgText.Text = "Norëdami saugoti slaptaþodþius, pirmiausiai prisijunkite prie Microsoft paskyros.";
            await dialog.ShowAsync();
        }

    }
}
