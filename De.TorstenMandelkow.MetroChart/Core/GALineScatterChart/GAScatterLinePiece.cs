
namespace De.TorstenMandelkow.MetroChart
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

    public class GAScatterLinePiece : PieceBase
    {
                #region Fields

        private Path _GALine = null;
        private Canvas _GAPlotCanvas = null;

        private Style _lineStyle;
        private Style _bulletStyle;
        GALineScatterStyling _lineScatterStyle;


        /// <summary>
        /// The type of the series
        /// Bullet, Line, Both, Other
        /// </summary>
        public static readonly DependencyProperty GASeriesTypeProperty =
           DependencyProperty.Register("GASeriesType",
           typeof(string),
           typeof(GAScatterLinePiece),
           new PropertyMetadata(null));

        /// <summary>
        /// Name of style for the bullets.
        /// This should be Path styles, or leave empty to use default
        /// </summary>
        public static readonly DependencyProperty GAScatterBulletStyleProperty =
          DependencyProperty.Register("GAScatterBulletStyle", typeof(String), typeof(GAScatterLinePiece),
          new PropertyMetadata(null));

        /// <summary>
        /// Name of style for the lines.
        /// These should be rectangle styles, or leave empty to use default
        /// </summary>
        public static readonly DependencyProperty GALineStyleProperty =
          DependencyProperty.Register("GALineStyle", typeof(String), typeof(GAScatterLinePiece),
          new PropertyMetadata(null));



        public static readonly DependencyProperty DataPointsProperty =
           DependencyProperty.Register("DataPoints", typeof(ObservableCollection<DataPoint>), typeof(GAScatterLinePiece),
           new PropertyMetadata(new ObservableCollection<DataPoint>(), new PropertyChangedCallback(OnPercentageChanged)));

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(GAScatterLinePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnPercentageChanged)));
        
        public static readonly DependencyProperty ColumnHeightProperty =
            DependencyProperty.Register("ColumnHeight", typeof(double), typeof(GAScatterLinePiece),
            new PropertyMetadata(0.0));
        
        #endregion Fields

        #region Constructors

        static GAScatterLinePiece()        
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GAScatterLinePiece), new FrameworkPropertyMetadata(typeof(GAScatterLinePiece)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPiece"/> class.
        /// </summary>
        public GAScatterLinePiece()
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

        // <summary>
        /// The type of the series
        /// Bullet, Line, Both, Other
        /// </summary>
        public string GASeriesType
        {
            get
            {
                return (string)GetValue(GASeriesTypeProperty);
            }
            set
            {
                SetValue(GASeriesTypeProperty, value);
            }
        }


        /// <summary>
        /// Name of style for the bullets.
        /// Set using the dependency property
        /// Used : Height, Width, radiusX, RadiusY, Fill
        /// The Fill is used like a boolean. If a value is given it is filled with the series value
        /// If a value isnt given it is left as a stroke only.
        /// </summary>
        /// 
        public string GAScatterBulletStyle
        {
            get
            {
                return (string)GetValue(GAScatterBulletStyleProperty);
            }
            set
            {
                SetValue(GAScatterBulletStyleProperty, value);
            }
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
            (d as GAScatterLinePiece).DrawGeometry();
        }

        protected override void InternalOnApplyTemplate()
        {

            setUpStyles();

            _GALine = this.GetTemplateChild("GALine") as Path;
          
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
            if (!string.IsNullOrEmpty(GALineStyle)) 
            {
                _lineStyle = TryFindResource(GALineStyle) as Style;
            }
            else
            {
                _lineStyle = TryFindResource("GALineStyle") as Style;
            }

            if (!string.IsNullOrEmpty(GAScatterBulletStyle))
            {
                _bulletStyle = TryFindResource(GAScatterBulletStyle) as Style;
            }
            else
            {
                _bulletStyle = TryFindResource("GAScatterBulletStyle") as Style;
            }
            
            if (_lineStyle==null)
            {
                _lineStyle = TryFindResource("GALineStyle") as Style;
            }
            if (_bulletStyle==null)
            {
                _bulletStyle = TryFindResource("GAScatterBulletStyle") as Style;
            }

            _lineScatterStyle = new GALineScatterStyling(_lineStyle, _bulletStyle, DataPoints[0]);

            
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

                _GALine.Fill = _lineScatterStyle.fillBrush;
                _GALine.StrokeThickness = _lineScatterStyle.strokeThickness;
                _GALine.Stroke = _lineScatterStyle.lineBrush;
                

                GeometryCollection geomCollection = new GeometryCollection();
                GeometryGroup group = new GeometryGroup();
                group.Children = geomCollection;
                _GALine.Data = group;

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

                        if (GASeriesType == "Both" || GASeriesType == "Bullet")
                        {
                            RectangleGeometry bulletGeometry = getRectangleGeometry(CenterX, CenterY);

                            group.Children.Add(bulletGeometry);
                        }
                       

                    }
                    else
                    {

                        double CenterX = (count * barWidth) + (barWidth / 2);
                        double CenterY = _GAPlotCanvas.ActualHeight - (_GAPlotCanvas.ActualHeight * p.PercentageFromMaxDataPointValue);

                        lineEndPoint.X = CenterX;
                        lineEndPoint.Y = CenterY;

                        if (GASeriesType=="Both" || GASeriesType=="Line")
                        {
                            LineGeometry l = new LineGeometry();
                            l.StartPoint = lineStartPoint;
                            l.EndPoint = lineEndPoint;

                            group.Children.Add(l);
                        }
                        

                        lineStartPoint.X = CenterX;
                        lineStartPoint.Y = CenterY;

                        if (GASeriesType=="Both" || GASeriesType=="Bullet")
                        {
                            RectangleGeometry bulletGeometry = getRectangleGeometry(CenterX, CenterY);

                            group.Children.Add(bulletGeometry);
                        }
                        
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
            bulletGeometry.RadiusX = _lineScatterStyle.scatterXRadius;
            bulletGeometry.RadiusY = _lineScatterStyle.scatterYRadius;
            bulletGeometry.Rect = new Rect(new Point(CenterX - (_lineScatterStyle.scatterSize.Width / 2), CenterY - (_lineScatterStyle.scatterSize.Height / 2)), _lineScatterStyle.scatterSize);
            return bulletGeometry;
        }

        #endregion Methods
    }
}
