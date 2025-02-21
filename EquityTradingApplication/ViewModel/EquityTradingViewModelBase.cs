﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EquityTradingApplication.ViewModel;

internal class EquityTradingViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}