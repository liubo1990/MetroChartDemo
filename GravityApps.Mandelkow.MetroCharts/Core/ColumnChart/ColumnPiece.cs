﻿namespace GravityApps.Mandelkow.MetroCharts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;  
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Windows.Input;

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
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    
#endif

    public class ColumnPiece : PieceBase
    {
        #region Fields

        private Border slice = null;

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(ColumnPiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnPercentageChanged)));

        public static readonly DependencyProperty IsNegativePieceProperty =
            DependencyProperty.Register("IsNegativePiece", typeof(bool), typeof(ColumnPiece),
            new PropertyMetadata(false, new PropertyChangedCallback(OnPercentageChanged)));

        /// <summary>
        /// the negative grid area in the main chart
        /// allows access 
        /// </summary>
        public static readonly DependencyProperty MainChartNegativeAreaProperty =
          DependencyProperty.Register("MainChartNegativeArea", typeof(Grid), typeof(ColumnPiece),
          new PropertyMetadata(null, new PropertyChangedCallback(OnPercentageChanged)));

        
        public static readonly DependencyProperty ColumnHeightProperty =
            DependencyProperty.Register("ColumnHeight", typeof(double), typeof(ColumnPiece),
            new PropertyMetadata(0.0));
        
        #endregion Fields

        #region Constructors

        static ColumnPiece()        
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColumnPiece), new FrameworkPropertyMetadata(typeof(ColumnPiece)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPiece"/> class.
        /// </summary>
        public ColumnPiece()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(ColumnPiece);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(ColumnPiece);
#endif
            Loaded += ColumnPiece_Loaded;
        }

        #endregion Constructors

        #region Properties


        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
        
        /// <summary>
        /// the percentage of maximum negative value
        /// bound to PercentageFromMaxNegativeDataPointValue in datapint via the generic.xaml
        /// </summary>
        public bool IsNegativePiece
        {
            get { return (bool)GetValue(IsNegativePieceProperty); }
            set { SetValue(IsNegativePieceProperty, value); }
        }

        /// <summary>
        /// the negative grid area in the main chart
        /// allows access 
        /// </summary>
        public Grid MainChartNegativeArea
        {
            get { return (Grid)GetValue(MainChartNegativeAreaProperty); }
            set { SetValue(MainChartNegativeAreaProperty, value); }
        }

        public double ColumnHeight
        {
            get { return (double)GetValue(ColumnHeightProperty); }
            set { SetValue(ColumnHeightProperty, value); }
        }
 
        #endregion Properties

        #region Methods

        private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ColumnPiece).DrawGeometry();
        }

        protected override void InternalOnApplyTemplate()
        {
            slice = this.GetTemplateChild("Slice") as Border;
            RegisterMouseEvents(slice);
        }

        void ColumnPiece_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGeometry();
        }

        private DependencyObject getNthLevelDirectChild(int level,DependencyObject parent,string name)
        {
            if (level <= 0)
            {
                return parent;
            }
            DependencyObject returnValue = VisualTreeHelper.GetChild(parent, 0);
            if (level ==1)
            {
                if (returnValue!=null)
                {
                    var element = returnValue as FrameworkElement;
                    if (element.Name == name)
                    {
                        return returnValue;
                    }
                }
                throw new Exception("Cant find " + name);
            }
            else
            {
                return (getNthLevelDirectChild(--level, returnValue,name));
            }
        }

        protected override void DrawGeometry(bool withAnimation = true)
        {    
            try
            {
                if (this.ClientWidth <= 0.0)
                {
                    return;
                }
                if (this.ClientHeight <= 0.0)
                {
                    return;
                }

                double endHeight = ClientHeight;

                double startHeight = 0;
                if (slice.Height > 0)
                {
                    startHeight = slice.Height;
                }

                endHeight = endHeight * Percentage;

               
                DoubleAnimation scaleAnimation = new DoubleAnimation();
                scaleAnimation.From = startHeight;
                scaleAnimation.To = endHeight;

                scaleAnimation.Duration = TimeSpan.FromMilliseconds(withAnimation ? 500: 0);
                scaleAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
                Storyboard storyScaleX = new Storyboard();
                storyScaleX.Children.Add(scaleAnimation);

                Storyboard.SetTarget(storyScaleX, slice);

#if NETFX_CORE
                scaleAnimation.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(storyScaleX, "Height");
#else
                Storyboard.SetTargetProperty(storyScaleX, new PropertyPath("Height"));
#endif
                storyScaleX.Begin();
   
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Methods
    }
}