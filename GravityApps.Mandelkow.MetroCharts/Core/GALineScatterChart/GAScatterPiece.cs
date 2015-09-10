namespace GravityApps.Mandelkow.MetroCharts
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

    public class GAScatterPiece : PieceBase
    {
        #region Fields

        private Rectangle slice = null;
        private Rectangle selectedSlice = null;
        GALineScatterStyling _adjustedStyle;

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(GAScatterPiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnPercentageChanged)));

        public static readonly DependencyProperty IsNegativePieceProperty =
            DependencyProperty.Register("IsNegativePiece", typeof(bool), typeof(GAScatterPiece),
            new PropertyMetadata(false, new PropertyChangedCallback(OnPercentageChanged)));

        
        public static readonly DependencyProperty ColumnHeightProperty =
            DependencyProperty.Register("ColumnHeight", typeof(double), typeof(GAScatterPiece),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty GAScatterBulletStyleProperty =
         DependencyProperty.Register("GAScatterBulletStyle", typeof(Style), typeof(GAScatterPiece),
         new PropertyMetadata(null));

        public static readonly DependencyProperty GASelectedScatterBulletStyleProperty =
        DependencyProperty.Register("GASelectedScatterBulletStyle", typeof(Style), typeof(GAScatterPiece),
        new PropertyMetadata(null));

        
        #endregion Fields

        #region Constructors

        static GAScatterPiece()        
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GAScatterPiece), new FrameworkPropertyMetadata(typeof(GAScatterPiece)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GAScatterPiece"/> class.
        /// </summary>
        public GAScatterPiece()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(GAScatterPiece);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(GAScatterPiece);
#endif
            Loaded += GAScatterPiece_Loaded;
        }

        #endregion Constructors

        #region Properties

        public Style GAScatterBulletStyle
        {
            get
            {
                return (Style)GetValue(GAScatterBulletStyleProperty);
            }
            set
            {
                SetValue(GAScatterBulletStyleProperty, value);
            }
        }

        public Style GASelectedScatterBulletStyle
        {
            get
            {
                return (Style)GetValue(GASelectedScatterBulletStyleProperty);
            }
            set
            {
                SetValue(GASelectedScatterBulletStyleProperty, value);
            }
        }

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
        
        /// <summary>
        /// is thew area the piece is drawn in a negative or positive one
        /// </summary>
        public bool IsNegativePiece
        {
            get { return (bool)GetValue(IsNegativePieceProperty); }
            set { SetValue(IsNegativePieceProperty, value); }
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
            (d as GAScatterPiece).DrawGeometry();
        }

        protected override void InternalOnApplyTemplate()
        {
            slice = this.GetTemplateChild("Slice") as Rectangle;
            selectedSlice = this.GetTemplateChild("SelectionHighlight") as Rectangle;
            RegisterMouseEvents(slice);
        }

        void GAScatterPiece_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGeometry();
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

                if (endHeight <= 0.0)
                {
                    endHeight=0;
                }
                double percentToUse = Percentage;

                if (GAScatterBulletStyle!=null)
                {
                    this.Style = GAScatterBulletStyle;
                }
                double startHeight = 0;
                TranslateTransform t = new TranslateTransform(0, 0);

                if (slice.RenderTransform!=null && slice.RenderTransform.GetType()== typeof(TranslateTransform))
                {
                    t=(slice.RenderTransform as TranslateTransform);
                    startHeight = t.Y;
                    
                }
                else
                {
                    t = new TranslateTransform(0, 0);
                    slice.RenderTransform = t;

                }

                this.Visibility = Visibility.Visible;
                if ( (!IsNegativePiece && percentToUse<0))
                {
                    this.Visibility = Visibility.Hidden;
                }

                if ((IsNegativePiece && percentToUse <= 0))
                {
                    this.Visibility = Visibility.Hidden;
                }

                if (percentToUse < 0) percentToUse = 0;
                if (!IsNegativePiece)
                {
                    endHeight = (endHeight * percentToUse) - getHalfBulletHeight();
                    
                }
                else
                {  
                    endHeight = (endHeight * (1 - percentToUse))- getHalfBulletHeight()+1.5;

                }

                DoubleAnimation moveAnimation = new DoubleAnimation(-endHeight, TimeSpan.FromMilliseconds(500));
                moveAnimation.From = startHeight;
                moveAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
                t.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
                selectedSlice.RenderTransform = new TranslateTransform(0, -endHeight);
                
            }
            catch (Exception ex)
            {
            }
        }

        private double getHalfBulletHeight()
        {
            
             GALineScatterStyling lineScatterStyle = new GALineScatterStyling(null, GAScatterBulletStyle, this.DataContext as DataPoint);
             return lineScatterStyle.scatterSize.Height / 2;
        }

        #endregion Methods
    }
}