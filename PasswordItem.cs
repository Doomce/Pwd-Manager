using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerWINUI
{
    internal enum PasswordItemState
    {
        Visible,
        Hidden,
        IsInVerificationProcess    
    }

    internal class PasswordItem : INotifyPropertyChanged
    {
        public PasswordItem() { ChangeState(PasswordItemState.Hidden); }

        public string Title { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string PassHiddenString { get; set; }
        public bool IsNotPassHidden { get; set; }

        public PasswordItemState GetState()
        {
            return IsNotPassHidden ? PasswordItemState.Visible : PasswordItemState.Hidden;
        }

        public void ChangeState([Optional] PasswordItemState state)
        {   
            switch (state)
            {
                case PasswordItemState.Hidden:
                    {
                        PassHiddenString = "Hidden";
                        IsNotPassHidden = false;
                        break;
                    }
                case PasswordItemState.Visible:
                    {
                        PassHiddenString = "Visible";
                        IsNotPassHidden = true;
                        break;
                    }
            }
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
