using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerWINUI
{

    internal class PasswordListItemModel : INotifyPropertyChanged
    {
        public ObservableCollection<PasswordItem> Passwords { get; set; }

        public PasswordListItemModel()
        {
            Passwords = new ObservableCollection<PasswordItem>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
