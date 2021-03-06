﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using MoveFilesUpOneLevel.ServiceClasses;

namespace MoveFilesUpOneLevel.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
