using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SP2013Access.Controls.Converters
{
    public class VisibilityToBoolConverter : IValueConverter
    {
        #region Inverted Property

        public bool Inverted { get; set; }

        #endregion Inverted Property

        #region Not Property

        public bool Not { get; set; }

        #endregion Not Property

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Inverted ? this.BoolToVisibility(value) : this.VisibilityToBool(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Inverted ? this.VisibilityToBool(value) : this.BoolToVisibility(value);
        }

        private object VisibilityToBool(object value)
        {
            //if( !( value is Visibility ) )
            //  throw new InvalidOperationException( ErrorMessages.GetMessage( "SuppliedValueWasNotVisibility" ) );

            return (((Visibility)value) == Visibility.Visible) ^ Not;
        }

        private object BoolToVisibility(object value)
        {
            //if( !( value is bool ) )
            //  throw new InvalidOperationException( ErrorMessages.GetMessage( "SuppliedValueWasNotBool" ) );

            return ((bool)value ^ Not) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}