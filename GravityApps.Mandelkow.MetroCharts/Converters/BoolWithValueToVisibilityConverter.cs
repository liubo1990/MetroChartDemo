namespace GravityApps.Mandelkow.MetroCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

#if NETFX_CORE
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml;
#else
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Data;
#endif

    /// <summary>
    /// Used to show and hide the data tags on the column chart
    /// information[0] should be the value
    //  information[1] should be isHEightExceeded
    //  information[2] should be "positive" or "negative" depending on which series it is ??
    /// </summary>
    public sealed class BoolWithValueToVisibilityConverter : IMultiValueConverter
    {

#if NETFX_CORE

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return InternalConvert(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return InternalConvertBack(value, targetType, parameter);
        }

#else
        public object Convert(object [] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return InternalConvert(value, targetType, parameter);
        }

        public object[] ConvertBack(object value, Type [] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return InternalConvertBack(value, targetType, parameter);
        }

#endif

        private object InternalConvert(object [] information, Type targetType, object dataPointValue)
        {
            //information[0] should be the value
            // information[1] should be isHEightExceeded
            //information[2] should be "positive" or "negative" depending on which series it is ??
            try
            {
                var flag = true;

                if ((string)information[2] == "Negative")
                {
                    if ((double)information[0] >= 0) flag = false;
                    if ((double)information[0] < 0 && (bool)information[1] == false) flag = false;
                }
                else if ((string)information[2] == "Positive")
                {
                    if ((double)information[0] < 0) flag = false;
                    if ((double)information[0] > 0 && (bool)information[1] == false) flag = false;
                }
                     
                if (flag)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
            }
            return Visibility.Visible;

        }

        public object[] InternalConvertBack(object value, Type[] targetType, object parameter)
        {
            //var back = ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
            //if (parameter != null)
            //{
            //    if ((bool)parameter)
            //    {
            //        back = !back;
            //    }
            //}
            throw new NotSupportedException();
        }
    }
}
