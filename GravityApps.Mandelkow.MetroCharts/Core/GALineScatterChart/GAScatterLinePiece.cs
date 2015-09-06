
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
        private int _animationTime = 500;

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
          DependencyProperty.Register("GAScatterBulletStyle", typeof(Style), typeof(GAScatterLinePiece),
          new PropertyMetadata(null));

        /// <summary>
        /// Name of style for the lines.
        /// These should be rectangle styles, or leave empty to use default
        /// </summary>
        public static readonly DependencyProperty GALineStyleProperty =
          DependencyProperty.Register("GALineStyle", typeof(Style), typeof(GAScatterLinePiece),
          new PropertyMetadata(null));

        public static readonly DependencyProperty IsNegativePieceProperty =
          DependencyProperty.Register("IsNegativePiece", typeof(bool), typeof(GAScatterLinePiece),
          new PropertyMetadata(false, new PropertyChangedCallback(OnPercentageChanged)));


        public static readonly DependencyProperty DataPointsProperty =
           DependencyProperty.Register("DataPoints", typeof(ObservableCollection<DataPoint>), typeof(GAScatterLinePiece),
           new PropertyMetadata(new ObservableCollection<DataPoint>(), new PropertyChangedCallback(OnPercentageChanged)));
        
       
        public static readonly DependencyProperty ColumnHeightProperty =
            DependencyProperty.Register("ColumnHeight", typeof(double), typeof(GAScatterLinePiece),
            new PropertyMetadata(0.0));


        List<DependencyObject> hitTestResults = new List<DependencyObject>();

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
        
        /// <summary>
        /// Name of style for the Lines.
        /// Set using the dependency property
        /// Used : StrokeThickness, Stroke
        /// </summary>
        public Style GALineStyle
        {
            get { return (Style)GetValue(GALineStyleProperty); }
            set { SetValue(GALineStyleProperty, value); }
        }

        /// <summary>
        /// is the area the piece is drawn in a negaitive or positive one?
        /// </summary>
        public bool IsNegativePiece
        {
            get { return (bool)GetValue(IsNegativePieceProperty); }
            set { SetValue(IsNegativePieceProperty, value); }
        }

        public ObservableCollection<DataPoint>  DataPoints
        {
            get { return (ObservableCollection<DataPoint>)GetValue(DataPointsProperty); }
            set { SetValue(DataPointsProperty, value); }
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

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GAScatterLinePiece).DrawGeometry();
        }

        /// <summary>
        /// get parent of spefified type and name if one exists
        /// </summary>
        /// <param name="current"></param>
        /// <param name="matchinmgName"></param>
        /// <param name="typeToTestFor"></param>
        /// <returns></returns>
        private DependencyObject getSpecifiedParent(DependencyObject current, string matchinmgName,Type typeToTestFor)
        {
            DependencyObject returnValue = null;

            while (VisualTreeHelper.GetParent(current) != null)
            {
                current = VisualTreeHelper.GetParent(current);
                if (current.GetType() == typeToTestFor)
                {
                    if (string.IsNullOrEmpty(matchinmgName))
                    {
                        returnValue = (DependencyObject)current;
                    }
                    else
                    {
                        var element = current as FrameworkElement;
                        if (element.Name == matchinmgName)
                        {
                            returnValue = (DependencyObject)current;
                            break;
                        }
                    }
                   

                }
            }
            return returnValue;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //this gets fired only in the postive part, and only if a bullet is not found
            // check to see if a column piece has been touched

            // get common ancestor
            Grid plotterAreaGrid = null;
            object a = e.MouseDevice.DirectlyOver;
            if (a.GetType() == typeof(Canvas))
            {
                plotterAreaGrid = (Grid) getSpecifiedParent((DependencyObject)a, "plotterAreaGrid", typeof(Grid));

                if (plotterAreaGrid==null)
                {
                    e.Handled = false;
                    return;
                }

                hitTestResults.Clear();
                Point pt = e.GetPosition((UIElement)plotterAreaGrid);
                VisualTreeHelper.HitTest(plotterAreaGrid, null,
         new HitTestResultCallback(MyHitTestResult),
         new PointHitTestParameters(pt));

                bool finishedHandling = false;
                foreach (DependencyObject o in hitTestResults)
                {
                    var fe = o as FrameworkElement;
                    if (fe.Name=="SelectionHighlight")
                    {
                       DependencyObject columnPiece= getSpecifiedParent(o, null, typeof(ColumnPiece));
                       if (columnPiece!=null)
                       {
                           ((ColumnPiece)columnPiece).IsClickedByUser = true;
                          // ((ColumnPiece)columnPiece).IsSelected = true;
                           finishedHandling = true;
                       }
                        
                    }
                }
                e.Handled = finishedHandling;
            }
        }

        private HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            if (result.VisualHit.GetType() == typeof(Border))
            {
                hitTestResults.Add(result.VisualHit);
            }
            
            return HitTestResultBehavior.Continue;
        }

        protected override void InternalOnApplyTemplate()
        {
            

            setUpStyles();
            _GAPlotCanvas = VisualTreeHelper.GetChild(this, 0) as Canvas; //the canvas the GALine is attached to
            _GALine = VisualTreeHelper.GetChild(_GAPlotCanvas, 0) as Path;
 
            ContentPresenter obj1 =  this.TemplatedParent as ContentPresenter;
            _sizeCanvas = VisualTreeHelper.GetParent(obj1) as Canvas; // used to size the plot canvas

         
           // RegisterMouseEvents(bullet); -- do I want to do this on some things?
        }

       
        void DataPointGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                //DrawGeometry(true,true,(DataPoint)sender);
                moveDataPoint((DataPoint)sender);
            }
        }
        /// <summary>
        /// Get styles for the lines and bullets
        /// The Line and Scatter style properties a 1st priority
        /// Deafults are second
        /// </summary>
        private void setUpStyles()
        {
            
            if (GALineStyle==null) 
            {
                GALineStyle = TryFindResource("GALineStyle") as Style;
            }

            if (GAScatterBulletStyle==null)
            {
                GAScatterBulletStyle = TryFindResource("GAScatterBulletStyle") as Style;
            }

            _lineScatterStyle = new GALineScatterStyling(GALineStyle, GAScatterBulletStyle, DataPoints[0]);

            
        }

       

        void GAScatterPiece_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGeometry();
        }

        protected override void DrawGeometry(bool withAnimation = true)
        {
           DrawGeometry();
        }

        /// <summary>
        /// move a data point whose value has been changed
        /// </summary>
        /// <param name="newPoint"></param>
        protected void moveDataPoint(DataPoint newPoint)
        {
            try
            {
                newPoint.PropertyChanged -= DataPointGroup_PropertyChanged;
                double barWidth = (_GAPlotCanvas.Width) / DataPoints.Count;
                double CenterX = (newPoint.locationInDataset * barWidth) + (barWidth / 2);
                double CenterY = _GAPlotCanvas.Height - (_GAPlotCanvas.Height * newPoint.PercentageFromMaxDataPointValue);
                Rectangle newRect = getRecatangle(CenterX, CenterY, newPoint);
                newRect.Style = this.GAScatterBulletStyle;

                int rectangleCounter = 0;
                int index = 0;
                foreach (UIElement ell in _GAPlotCanvas.Children)
                {
                    if (ell.GetType() == typeof(Path))
                    {
                        Path lines = (Path)ell;
                        GeometryGroup linesGeom = (GeometryGroup) lines.Data;

                        moveLinePoints(newPoint, CenterX, CenterY, linesGeom);

                    }
                    if (ell.GetType() == typeof(Rectangle))
                    {
                        Rectangle rect = (Rectangle)ell;
                        if (rectangleCounter == newPoint.locationInDataset)
                        {
                            Thickness newMargin = new Thickness(newRect.Margin.Left, newRect.Margin.Top, 0, 0);

                            ThicknessAnimation moveAnimation = new ThicknessAnimation(newMargin, TimeSpan.FromMilliseconds(_animationTime));
                            moveAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
                            rect.ToolTip = newPoint.FormattedValue;
                            rect.BeginAnimation(Rectangle.MarginProperty,moveAnimation);
                            break;
                        }
                        rectangleCounter++;
                    }
                    index++;
                }
                newPoint.PropertyChanged += DataPointGroup_PropertyChanged;
                newPoint.OldValue = newPoint.Value;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Move the existing line points
        /// This may include moving multiple or single lines
        /// </summary>
        /// <param name="newPoint"></param>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="linesGeom"></param>
        private void moveLinePoints(DataPoint newPoint, double CenterX, double CenterY, GeometryGroup linesGeom)
        {

            if (newPoint.locationInDataset == 0)
            {
                animateLine(CenterX, CenterY, linesGeom, LineGeometry.StartPointProperty, 0);
                return;
            }
            if (newPoint.locationInDataset == linesGeom.Children.Count)
            {
                animateLine(CenterX, CenterY, linesGeom, LineGeometry.EndPointProperty, linesGeom.Children.Count - 1);
            }
            else
            {
                animateLine(CenterX, CenterY, linesGeom, LineGeometry.EndPointProperty, newPoint.locationInDataset - 1);
                animateLine(CenterX, CenterY, linesGeom, LineGeometry.StartPointProperty, newPoint.locationInDataset);
            }
        }

        /// <summary>
        /// move and animate a single line in the group of datapoints
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="linesGeom"></param>
        /// <param name="dp"></param>
        /// <param name="location"></param>
        private void animateLine(double CenterX, double CenterY, GeometryGroup linesGeom,DependencyProperty dp,int location)
        {
            LineGeometry thisLine = (LineGeometry)linesGeom.Children[location];
            PointAnimation movePointAnimation = new PointAnimation(new Point(CenterX, CenterY), TimeSpan.FromMilliseconds(_animationTime));
            movePointAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
            thisLine.BeginAnimation(dp, movePointAnimation);
        }

        /// <summary>
        /// draw the lines and bullets
        /// </summary>
        /// <param name="withAnimation"></param>
        protected  void DrawGeometry()
        {    
            try
            { 
                if (_GALine == null) return;

                _GALine.Style = this.GALineStyle; 
               

                _GAPlotCanvas.Width =_sizeCanvas.ActualWidth;
                _GAPlotCanvas.Height = _sizeCanvas.ActualHeight;

               

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
                    p.locationInDataset = count;
                    p.PropertyChanged -= DataPointGroup_PropertyChanged; // ensure that the event wont fire while we are changing things!
                    

                    if (count==0) // first point doesnt get a line
                    {
                        double CenterX = (count * barWidth) + (barWidth / 2);
                        double CenterY = _GAPlotCanvas.Height - (_GAPlotCanvas.Height * p.PercentageFromMaxDataPointValue);


                        lineStartPoint = new Point(CenterX,CenterY) ;

                        if (GASeriesType == "Both" || GASeriesType == "Bullet")
                        {
                            
                            Rectangle rect = getRecatangle(CenterX, CenterY, p);
                            rect.Style = this.GAScatterBulletStyle;
                             _GAPlotCanvas.Children.Add(rect);
                            
                        }

                    }
                    else
                    {

                        double CenterX = (count * barWidth) + (barWidth / 2);
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
                            Rectangle rect = getRecatangle(CenterX, CenterY, p);
                            rect.Style = this.GAScatterBulletStyle;
                            _GAPlotCanvas.Children.Add(rect);
                           
                        }
                        
                    }

                    p.OldValue = p.Value;
                   // p.locationInDataset = count;
                    p.PropertyChanged += DataPointGroup_PropertyChanged; // and now let thigs be changed again
                    count++;
                }
   
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// get the UI rectangle to be  drawn
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private Rectangle getRecatangle(double CenterX, double CenterY,DataPoint p)
        {
            
            Rectangle rect = new Rectangle();
            rect.Margin = new Thickness(CenterX - (_lineScatterStyle.scatterSize.Width/2), CenterY - (_lineScatterStyle.scatterSize.Height/2), 0, 0);
            rect.ToolTip = p.FormattedValue;
            rect.Tag = p.locationInDataset;
            rect.MouseDown += rect_MouseDown;
            
            return rect;
        }

        void rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            int locationInDataset = (int)rect.Tag;
            this.IsClickedByUser = true;
            ParentChart.SelectedItem = DataPoints[locationInDataset];
            DataPoints[locationInDataset].IsSelected = true;
            // this will handle the rectangle hits find a rectangle and then highlight it etc
            // OR do I add a preview mouse down to the rectangle in the positive values only?
            // if I do it that way will it get the hit of the rectangle (bullet) or the rectangle?
            
           // throw new NotImplementedException();

            // ensure any column pieces dont get selected as well
            e.Handled = true;
        }
            
      
        #endregion Methods
    }
}
