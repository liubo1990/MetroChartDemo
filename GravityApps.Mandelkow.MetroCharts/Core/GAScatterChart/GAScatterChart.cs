﻿
namespace De.TorstenMandelkow.MetroChart
{
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
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
#endif

    /// <summary>
    /// Represents an Instance of the bar-chart
    /// </summary>
    public class GAScatterChart : ChartBase
    {
        #region Fields

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="GAScatterChart"/> class.
        /// </summary>
        static GAScatterChart()
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GAScatterChart), new FrameworkPropertyMetadata(typeof(GAScatterChart)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GAScatterChart"/> class.
        /// </summary>
        public GAScatterChart()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(ClusteredColumnChart);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(ClusteredColumnChart);
#endif
        }

        #endregion Constructors

        protected override double GridLinesMaxValue
        {
            get
            {
                return MaxDataPointValue;
            }
        }

        protected override void OnMaxDataPointValueChanged(double p)
        {
            UpdateGridLines();
        }

        public override bool IsUseNextBiggestMaxValue
        {
            get
            {
                return true;
            }
        }
    }
}
