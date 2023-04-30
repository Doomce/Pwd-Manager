// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Microsoft.UI;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PasswordManagerWINUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SecondPage : Page
    {
        private PasswordListItemModel Model;
        private AppBarToggleButton VisiblePassword;

        public SecondPage()
        {
            this.InitializeComponent();
            Model = new PasswordListItemModel();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void Refresh_Btn_OnClick(object sender, RoutedEventArgs e)
        {

            Model.Passwords.Add(new PasswordItem { Title = "GOOGLE.COM", Password = "password12333" });
            var obj = ListViewPasswords.ItemTemplate;
            ListViewPasswords.ItemsSource = Model.Passwords;

        }


        private void Element_Got_Focus_On_Keyboard(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            Debug.WriteLine("GotFocus");
            
            VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
            
        }

        private void Pointer_Entered_OnElem(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                var item = sender as Control;
                VisualStateManager.GoToState(item, "PointerOver", true);
            }
        }

        private void Pointer_Exited_OnElem(object sender, PointerRoutedEventArgs e)
        {
            var item = sender as Control;
            var dataItem = item.DataContext as PasswordItem;
            if (dataItem != null && dataItem.IsPassHidden.Contains("Visible")) return;
            VisualStateManager.GoToState(item, "Normal", true);

        }

        private void PassBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (AppBarToggleButton)sender;
            var dataItem = button.DataContext as PasswordItem;

            foreach (var passwordItem in Model.Passwords.Where(item => item.IsPassHidden == "Visible" && item!=dataItem))
            {
                passwordItem.ChangePassHiddenState();
                passwordItem.OnPropertyChanged(nameof(passwordItem.IsPassHidden));
                passwordItem.OnPropertyChanged(nameof(passwordItem.IsPassButtonActive));
            }

            dataItem.ChangePassHiddenState();
            dataItem.OnPropertyChanged(nameof(dataItem.IsPassHidden));
            dataItem.OnPropertyChanged(nameof(dataItem.IsPassButtonActive));

            ChangeShowPassBtnText(button, dataItem.IsPassButtonActive);
            if (VisiblePassword != null && VisiblePassword != button) ChangeShowPassBtnText(VisiblePassword, !dataItem.IsPassButtonActive);
            if (dataItem.IsPassButtonActive) VisiblePassword = button;
        }


        private void ChangeShowPassBtnText(AppBarToggleButton button, bool isBtnActive)
        {
            if (button is null) return;
            if (isBtnActive)
            {
                button.Label = "Slëpti slaptaþodá";
                button.Icon = new FontIcon()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uED1A"
                };
                button.IsChecked = true;
            }
            else
            {
                button.Label = "Perþiûrëti slaptaþodá";
                button.Icon = new FontIcon()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uF78D"
                };
                button.IsChecked = false;
            }
        }

        private void CopyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (AppBarButton)sender;
            if (button.DataContext is not PasswordItem dataItem) return;

            var package = new DataPackage();
            package.SetText(dataItem.Password);
            Clipboard.SetContent(package);
        }
    }
}
