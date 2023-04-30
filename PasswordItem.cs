using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerWINUI
{
    internal class PasswordItem : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Password { get; set; }
        public string IsPassHidden { get; set; } = "Hidden";
        public bool IsPassButtonActive { get; set; } = false;


        public void ChangePassHiddenState()
        {
            if (IsPassHidden == "Hidden")
            {
                IsPassHidden = "Visible";
                IsPassButtonActive = true;
            }
            else
            {
                IsPassHidden = "Hidden";
                IsPassButtonActive = false;
            }
            OnPropertyChanged();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
