﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MessengerPigeon
{
    public sealed class Converter: IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null
                ?Visibility.Visible 
                :Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is Visibility visibility
                && visibility == Visibility.Visible;
        }

    }
}
