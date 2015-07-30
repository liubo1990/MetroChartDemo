
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
        private Canvas _sizeCanvas = null;

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
            _GAPlotCanvas = VisualTreeHelper.GetChild(this, 0) as Canvas; //the canvas the GALine is attached to
            _GALine = VisualTreeHelper.GetChild(_GAPlotCanvas, 0) as Path;
          
            //TODO: find a better way of doing this using the name!
            //go up the visual tree to get the GAChart Canvas
            // This will need to be altered if the xaml changes! - is 
           // _GAPlotCanvas = VisualTreeHelper.GetParent(_GALine) as Canvas; //the canvas the GALine is attached to
            ContentPresenter obj1 =  this.TemplatedParent as ContentPresenter;
            _sizeCanvas = VisualTreeHelper.GetParent(obj1) as Canvas;



         
           


            // try putting a canvas around the <local:GAScatterLinePiece<
            // and drawing on that or adding UI elements to it?
            // each item has a presenter - find this for each item and add to it
            // may have to add a canvas to it and make the canvas transaparent
            // start off with 1 item, 1 series, 1 datapoint and go from there

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

                _GAPlotCanvas.Width =_sizeCanvas.ActualWidth;
                _GAPlotCanvas.Height = _sizeCanvas.ActualHeight;
               // _GAPlotCanvas.Margin = sizeCanvas.Margin;

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
                double barWidth = (_GAPlotCanvas.Width) / DataPoints.Count;
                foreach (DataPoint p in DataPoints)
                {
                    

                    if (count==0)
                    {
                        double CenterX = (count * barWidth) + (barWidth / 2);
                        double CenterY = _GAPlotCanvas.Height - (_GAPlotCanvas.Height * p.PercentageFromMaxDataPointValue);
                        lineStartPoint = new Point(CenterX,CenterY) ;

                        if (GASeriesType == "Both" || GASeriesType == "Bullet")
                        {
                            //RectangleGeometry bulletGeometry = getRectangleGeometry(CenterX, CenterY);

                            //group.Children.Add(bulletGeometry);
                            _GAPlotCanvas.Children.Add(getRecatangle(CenterX, CenterY,p));
                        }
                       

                    }
                    else
                    {

                        double CenterX = (count * barWidth) + (barWidth / 2);
                      //  double CenterY = _GAPlotCanvas.ActualHeight - (_GAPlotCanvas.ActualHeight * p.PercentageFromMaxDataPointValue);
                        double CenterY = _GAPlotCanvas.Height - (_GAPlotCanvas.Height * p.PercentageFromMaxDataPointValue);

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
                            //RectangleGeometry bulletGeometry = getRectangleGeometry(CenterX, CenterY);

                            //group.Children.Add(bulletGeometry);

                            //Rectangle uiRect = new Rectangle();
                            //uiRect.Height = bulletGeometry.Rect.Height;
                            //uiRect.Width = bulletGeometry.Rect.Width;
                            //uiRect.Fill = _lineScatterStyle.fillBrush;
                            //uiRect.Margin = new Thickness(CenterX, CenterY, 0, 0);

                            _GAPlotCanvas.Children.Add(getRecatangle(CenterX, CenterY,p));
                          //  DependencyObject obj = VisualTreeHelper.GetParent(this);
                          //  _GAPlotCanvas = VisualTreeHelper.GetParent(obj) as Canvas;
                          //  ItemsPresenter obj1 = VisualTreeHelper.GetParent(_GAPlotCanvas) as ItemsPresenter;
                          // // get templated parent of obj1 then add items to the items list
                          //  FadingListView itemPanel = obj1.TemplatedParent as FadingListView;
                          //  // maybe add 1 item and add all the bullets to this

                           
                            
                          ////  itemPanel.Items.Add(uiRect);
                            
                        }
                        
                    }
                    count++;
                }
   
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


        private Rectangle getRecatangle(double CenterX, double CenterY,DataPoint p)
        {
            Rectangle rect = new Rectangle();
            rect.Width = _lineScatterStyle.scatterSize.Width;
            rect.Height = _lineScatterStyle.scatterSize.Height;
            rect.RadiusX = _lineScatterStyle.scatterXRadius;
            rect.RadiusY = _lineScatterStyle.scatterYRadius;
            rect.Margin = new Thickness(CenterX - (_lineScatterStyle.scatterSize.Width/2), CenterY - (_lineScatterStyle.scatterSize.Height/2), 0, 0);
            if (_lineScatterStyle.scatterIsFilled)
            {
                rect.Fill = _lineScatterStyle.fillBrush;
            }
            rect.Stroke = _lineScatterStyle.lineBrush;
            rect.StrokeThickness = _lineScatterStyle.strokeThickness;
            rect.ToolTip = p.FormattedValue;;
           
            // mouse over in here?
            //ToolTipService.ToolTip="{Binding Path=FormattedValue}"
            //will need to put
            return rect;
        }
            
      
        #endregion Methods
    }
}
