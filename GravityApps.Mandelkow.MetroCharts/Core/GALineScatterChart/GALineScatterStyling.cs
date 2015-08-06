using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace GravityApps.Mandelkow.MetroCharts
{
    /// <summary>
    /// gets information about the line and bullet styles
    /// </summary>
    public class GALineScatterStyling
    {
        public Brush lineBrush;
        public Brush bulletLineBrush;
        public Brush fillBrush;
        public Size scatterSize;
        

      /// <summary>
      /// get the information about the line/scatter styles
      /// set default colours if none supplied in the styles
      /// used in setting the styles and setting the Legend size in ChartBase
      /// and to help positioning elements in scatter line piece
      /// </summary>
      /// <param name="lineStyle"></param>
      /// <param name="bulletStyle"></param>
      /// <param name="firstDataPoint"></param>
        public GALineScatterStyling(Style lineStyle, Style bulletStyle,DataPoint firstDataPoint )
        {
            
            var bulletHeightSetter = getCalculatedSetter(bulletStyle, "Height");
            var bulletWidthSetter = getCalculatedSetter(bulletStyle, "Width");
            var butlletFillSetter = getCalculatedSetter(bulletStyle, "Fill",false);
            var butlletLineSetter = getCalculatedSetter(bulletStyle, "Stroke", false);
            bool bulletLineColourProvided = !(butlletLineSetter == null || butlletLineSetter.Value == null);

            var lineFillSetter = getCalculatedSetter(lineStyle, "Fill",false);
            var lineStrokeSetter = getCalculatedSetter(lineStyle, "Stroke",false);
            var lineStrokeThicknessSetter = getCalculatedSetter(lineStyle, "StrokeThickness");

            scatterSize = new Size((double)bulletWidthSetter.Value, (double)bulletHeightSetter.Value);
            bool scatterColourProvided = !(butlletFillSetter == null || butlletFillSetter.Value == null );
            bool lineColourProvided = !(lineStrokeSetter == null || lineStrokeSetter.Value == null);

            // fill brush - if none provided use the ones from the pallette
            if (!scatterColourProvided)
            {
                fillBrush = firstDataPoint.ItemBrush;
            }
            else 
            {
                fillBrush = (Brush)butlletFillSetter.Value;
            }

            // line stroke - if not set then from the pallette, else from the style
            if (!lineColourProvided)
            {
                lineBrush = firstDataPoint.ItemBrush;
            }
            else
            {
                lineBrush = (Brush)lineStrokeSetter.Value;
            }

            // bullet line stroke - if not set then from the pallette, else from the style
            if (!bulletLineColourProvided)
            {
                bulletLineBrush = firstDataPoint.ItemBrush;
            }
            else
            {
                bulletLineBrush = (Brush)butlletLineSetter.Value;
            }
        }

        /// <summary>
        /// get the setter for a property using the 'based on' styles if needed
        /// styles if needed
        /// </summary>
        /// <param name="style"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        private Setter getCalculatedSetter(Style style, string propertyname,bool throwError=true)
        {
            var propertySetter = getSetter(style, propertyname);
            if (propertySetter == null)
            {
                Style currentStyle = style;
                while (currentStyle.BasedOn != null)
                {
                    currentStyle = currentStyle.BasedOn;
                    var tempBulletWidthSetter = getSetter(currentStyle, propertyname);
                    if (tempBulletWidthSetter != null)
                    {
                        propertySetter = tempBulletWidthSetter;
                        break;
                    }

                }

                if (propertySetter == null)
                {
                    if (throwError)
                    {
                        throw new Exception("cant find [" + propertyname + "] setter for target [" + style.TargetType.FullName);
                    }
                    
                }
            }
            return propertySetter;
        }

        /// <summary>
        /// get the style setter of an object
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        private Setter getSetter(Style styleName, string propertyname)
        {
            var setter = styleName.Setters.OfType<Setter>().FirstOrDefault(s => s.Property.Name == propertyname);
            return setter;
        }
    }
}
