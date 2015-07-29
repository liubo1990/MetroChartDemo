namespace De.TorstenMandelkow.MetroChart
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Collections.ObjectModel;

#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Core;
#else
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
#endif
    
    public class DataPointGroup : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// The type of the series
        /// Bullet, Line, Both, Other
        /// </summary>
        public static readonly DependencyProperty SeriesTypeProperty =
           DependencyProperty.Register("GASeriesType",
           typeof(string),
           typeof(DataPointGroup),
           new PropertyMetadata(null));

        /// <summary>
        /// Name of style for the bullets.
        /// This should be Path styles, or leave empty to use default
        /// </summary>
        public static readonly DependencyProperty GAScatterBulletStyleProperty =
          DependencyProperty.Register("GAScatterBulletStyle", typeof(String), typeof(DataPointGroup),
          new PropertyMetadata(null));

        /// <summary>
        /// Name of style for the lines.
        /// These should be rectangle styles, or leave empty to use default
        /// </summary>
        public static readonly DependencyProperty GALineStyleProperty =
          DependencyProperty.Register("GALineStyle", typeof(String), typeof(DataPointGroup),
          new PropertyMetadata(null));

        public static readonly DependencyProperty SumOfDataPointGroupProperty =
            DependencyProperty.Register("SumOfDataPointGroup",
            typeof(double),
            typeof(DataPointGroup),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
            typeof(object),
            typeof(DataPointGroup),
            new PropertyMetadata(null));

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public double SumOfDataPointGroup
        {
            get { return (double)GetValue(SumOfDataPointGroupProperty); }
            set { SetValue(SumOfDataPointGroupProperty, value); }
        }

        public ObservableCollection<DataPoint> DataPoints
        { get; set; }

        public ChartBase ParentChart
        { get; private set; }

        // <summary>
        /// The type of the series
        /// Bullet, Line, Both, Other
        /// </summary>
        public string GASeriesType
        {
            get
            {
                return (string)GetValue(SeriesTypeProperty);
            }
            set
            {
                SetValue(SeriesTypeProperty, value);
            }
        }


        /// <summary>
        /// Name of style for the bullets.
        /// Set using the dependency property
        /// Used : Height, Width, radiusX, RadiusY, Fill
        /// The Fill is used like a boolean. If a value is given it is filled with the series value
        /// If a value isnt given it is left as a stroke only.
        /// </summary>
        public string GABulletStyle
        {
            get { return (string)GetValue(GAScatterBulletStyleProperty); }
            set { SetValue(GAScatterBulletStyleProperty, value); }
        }

        /// <summary>
        /// Name of style for the Lines.
        /// Set using the dependency property
        /// Used : StrokeThickness, Stroke
        /// </summary>
        public string GALineStyle
        {
            get { return (string)GetValue(GALineStyleProperty); }
            set { SetValue(GALineStyleProperty, value); }
        }
       
        public DataPointGroup(ChartBase parentChart, string caption, bool showcaption)
        {
            ParentChart = parentChart;
            Caption = caption;
            ShowCaption = showcaption;
            DataPoints = new ObservableCollection<DataPoint>();
            DataPoints.CollectionChanged += Items_CollectionChanged;
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in e.NewItems)
            {
                if (item is INotifyPropertyChanged)
                {
                    (item as INotifyPropertyChanged).PropertyChanged += DataPointGroup_PropertyChanged;
                }
            }
        }

        void DataPointGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                RecalcValues();
            }
        }

        private void RecalcValues()
        {
            double maxValue = 0.0;
            double sum = 0.0;
            foreach (var item in DataPoints)
            {
                item.StartValue = sum;
                sum += item.Value;
                if (item.Value > maxValue)
                {
                    maxValue = item.Value;
                }
            }
            SumOfDataPointGroup = sum;
            RaisePropertyChangeEvent("SumOfDataPointGroup");
        }

        public string Caption { get; private set; }

        public bool ShowCaption { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangeEvent(String propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
