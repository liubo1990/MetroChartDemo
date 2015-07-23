namespace GravityAppsMandelkowMetroCharts
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
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    
#endif
    /// <summary>
    /// this line piece draws an entire line chart
    /// should be bound to a 
    /// </summary>
    public class GALinePiece : PieceBase
    {
        #region Fields

        private Path _GALine = null;
        private Canvas _GAPlotCanvas = null;

        private Style _lineStyle;
        private Style _bulletStyle;
        private Size _bulletSize;
        private double _bulletRadiusX;
        private double _bulletRadiusY;
        private bool _bulletIsFilled;
        

        /// <summary>
        /// Name of style for the bullets.
        /// This should be Path styles, or leave empty to use default
        /// </summary>
        public static readonly DependencyProperty GAScatterBulletStyleProperty =
          DependencyProperty.Register("GAScatterBulletStyle", typeof(String), typeof(GALinePiece),
          new PropertyMetadata(""));

        /// <summary>
        /// Name of style for the lines.
        /// These should be rectangle styles, or leave empty to use default
        /// </summary>
        public static readonly DependencyProperty GALineStyleProperty =
          DependencyProperty.Register("GALineStyle", typeof(String), typeof(GALinePiece),
          new PropertyMetadata(""));



        public static readonly DependencyProperty DataPointsProperty =
           DependencyProperty.Register("DataPoints", typeof(ObservableCollection<DataPoint>), typeof(GALinePiece),
           new PropertyMetadata(new ObservableCollection<DataPoint>(), new PropertyChangedCallback(OnPercentageChanged)));

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(GALinePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnPercentageChanged)));
        
        public static readonly DependencyProperty ColumnHeightProperty =
            DependencyProperty.Register("ColumnHeight", typeof(double), typeof(GALinePiece),
            new PropertyMetadata(0.0));
        
        #endregion Fields

        #region Constructors

        static GALinePiece()        
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GALinePiece), new FrameworkPropertyMetadata(typeof(GALinePiece)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPiece"/> class.
        /// </summary>
        public GALinePiece()
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
        public string GAScatterBulletStyle
        {
            get { return (string)GetValue(GALineStyleProperty); }
            set { SetValue(GALineStyleProperty, value); }
        }

        public ObservableCollection<DataPoint>  DataPoints
        {
            get { return (ObservableCollection<DataPoint>)GetValue(DataPointsProperty); }
            set { SetValue(DataPointsProperty, value); }
        }

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
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
            (d as GALinePiece).DrawGeometry();
        }

        protected override void InternalOnApplyTemplate()
        {

            setUpStyles();
            

            //slice = this.GetTemplateChild("Slice") as Border;
            //bullet = this.GetTemplateChild("Bullet") as Rectangle;
            _GALine = this.GetTemplateChild("GALine") as Path;
           // GAPlotCanvas = this.GetTemplateChild("GAChartCanvas") as Canvas;
           
            //TODO: find a better way of doing this using the name!
            //go up the visual tree to get the GAChart Canvas
            // This will need to be altered if the xaml changes! - is 
            DependencyObject obj = VisualTreeHelper.GetParent(this);
            _GAPlotCanvas = VisualTreeHelper.GetParent(obj) as Canvas;
            
           // RegisterMouseEvents(bullet); -- do I want to do this on some things?
        }

        /// <summary>
        /// Get styles for the lines and bullets
        /// The Line and Scatter style properties a 1st priority
        /// Deafults are second
        /// </summary>
        private void setUpStyles()
        {
            _lineStyle = TryFindResource(GALineStyleProperty) as Style;
            _bulletStyle = TryFindResource(GAScatterBulletStyle) as Style;

            if (_lineStyle==null)
            {
                _lineStyle = TryFindResource("GALineStyle") as Style;
            }

            if (_bulletStyle == null)
            {
                _bulletStyle = TryFindResource("GAScatterBulletStyle") as Style;
            }

            var bulletHeightSetter = _bulletStyle.Setters.OfType<Setter>().FirstOrDefault(s => s.Property.Name == "Height");
            var bulletWidthSetter = _bulletStyle.Setters.OfType<Setter>().FirstOrDefault(s => s.Property.Name == "Width");
            var butlletRadiusXSetter = _bulletStyle.Setters.OfType<Setter>().FirstOrDefault(s => s.Property.Name == "RadiusX");
            var butlletRadiusYSetter = _bulletStyle.Setters.OfType<Setter>().FirstOrDefault(s => s.Property.Name == "RadiusY");
            var butlletFillSetter = _bulletStyle.Setters.OfType<Setter>().FirstOrDefault(s => s.Property.Name == "Fill");

            _bulletIsFilled = !(butlletFillSetter == null);

           _bulletSize = new Size((double)bulletWidthSetter.Value,(double)bulletHeightSetter.Value);
           _bulletRadiusX = (double)butlletRadiusXSetter.Value;
           _bulletRadiusY = (double)butlletRadiusYSetter.Value;;
        }

        void GAScatterPiece_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGeometry();
        }

        /// <summary>
        /// draw the lines and bullets
        /// </summary>
        /// <param name="withAnimation"></param>
        protected override void DrawGeometry(bool withAnimation = true)
        {    
            try
            { 
                if (_GALine == null) return;

                GeometryCollection geomCollection = new GeometryCollection();
                GeometryGroup group = new GeometryGroup();
                group.Children = geomCollection;
                _GALine.Data = group;


                _GALine.Stroke = DataPoints[0].ItemBrush;
                if (_bulletIsFilled) _GALine.Fill = DataPoints[0].ItemBrush;

                Point lineStartPoint = new Point(0, 0);
                Point lineEndPoint = new Point(0, 0);
                int count=0;
                double barWidth = (_GAPlotCanvas.ActualWidth) / DataPoints.Count;
                foreach (DataPoint p in DataPoints)
                {
                    

                    if (count==0)
                    {
                        double CenterX = (count * barWidth) + (barWidth / 2);
                        double CenterY = _GAPlotCanvas.ActualHeight - (_GAPlotCanvas.ActualHeight * p.PercentageFromMaxDataPointValue);
                        lineStartPoint = new Point(CenterX,CenterY) ;

                        RectangleGeometry bulletGeometry = getRectangleGeometry(CenterX, CenterY);

                        group.Children.Add(bulletGeometry);
                       

                    }
                    else
                    {

                        double CenterX = (count * barWidth) + (barWidth / 2);
                        double CenterY = _GAPlotCanvas.ActualHeight - (_GAPlotCanvas.ActualHeight * p.PercentageFromMaxDataPointValue);

                        lineEndPoint.X = CenterX;
                        lineEndPoint.Y = CenterY;

                        LineGeometry l = new LineGeometry();
                        l.StartPoint = lineStartPoint;
                        l.EndPoint = lineEndPoint;
                        
                        group.Children.Add(l);

                        lineStartPoint.X = CenterX;
                        lineStartPoint.Y = CenterY;

                        RectangleGeometry bulletGeometry = getRectangleGeometry(CenterX, CenterY);
                        
                        group.Children.Add(bulletGeometry);
                    }
                   


                    
                    
                    // use GALineBullet as a template to get the sizes
                    // need two paths, one for circles and one for lines? (on path and on line)
                    // or add rectangles to the canvas and position using margins?
                    count++;
                }

                // **************should draw the bullets here too
                //** add the posibility to animate too
                // copy / use the code to determine the colour of each line (maybe add a seriesColur ro each dataGroup)
                // see why I can mouse over some and not others (is the canvas of one interfeering with the other , or somethign else)
                // will need to put the mouse over code onto the points here in code
                // do I want to be able to put click or right click events on? (later if needed unless really simple)

                //** oh and need to look at the text etc and see why my graph isnt printing them - and hope like &&**^ that this
                // doesnt upset my stuff

                //** need to tidy up the xaml and write instructions?
                //** line graph only - with variable to make scatter only or line without scatter (ie hide bits!)
                //** column plus line

                // can I add rectangles at the correct place with the style and then the user can alter the style without code

                //if (this.ClientWidth <= 0.0)
                //{
                //    return;
                //}
                //if (this.ClientHeight <= 0.0)
                //{
                //    return;
                //}

                //double startHeight = 0;
                //if (bullet.Height > 0)
                //{
                //    startHeight = bullet.Height;
                //}

                ////this.bullet.Fill = this.slice.Background;
                ////Brush clear = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                ////slice.Background = clear;

                //Double bulletHeight = bullet.Height;

                //DoubleAnimation scaleAnimation = new DoubleAnimation();
                //scaleAnimation.From = startHeight;
                //scaleAnimation.To = (this.ClientHeight * Percentage)+(bulletHeight/2)+1;
                //scaleAnimation.Duration = TimeSpan.FromMilliseconds(withAnimation ? 500 : 0);
                //scaleAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };



                //Storyboard storyScaleX = new Storyboard();
                //storyScaleX.Children.Add(scaleAnimation);

                //Storyboard.SetTarget(storyScaleX, slice); // animate the slice and the bullet will move with it

#if NETFX_CORE
                scaleAnimation.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(storyScaleX, "Height");
#else
               // Storyboard.SetTargetProperty(storyScaleX, new PropertyPath("Height"));
#endif
               // storyScaleX.Begin();
   
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// get a rectangle geometry for the bullet
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <returns></returns>
        private RectangleGeometry getRectangleGeometry(double CenterX, double CenterY)
        {
            RectangleGeometry bulletGeometry = new RectangleGeometry();
            bulletGeometry.RadiusX = _bulletRadiusX;
            bulletGeometry.RadiusY = _bulletRadiusY;
            bulletGeometry.Rect = new Rect(new Point(CenterX - (_bulletSize.Width/2), CenterY - (_bulletSize.Height/2)), _bulletSize);
            return bulletGeometry;
        }

        #endregion Methods
    }
}