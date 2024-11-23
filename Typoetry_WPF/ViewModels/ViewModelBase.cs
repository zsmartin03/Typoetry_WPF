﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Typoetry_WPF.ViewModels
{

    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;


        protected ViewModelBase() { }


        protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
