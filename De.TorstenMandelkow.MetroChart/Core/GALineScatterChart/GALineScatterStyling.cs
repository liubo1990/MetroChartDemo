using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace De.TorstenMandelkow.MetroChart
{
    public class GALineScatterStyling
    {
        public Brush lineBrush;
        public Brush fillBrush;
        public Double strokeThickness;
        public bool scatterIsFilled;
        public Size scatterSize;
        public double scatterXRadius;
        public double scatterYRadius;

        public GALineScatterStyling(Style lineStyle, Style bulletStyle,DataPoint firstDataPoint )
        {
            var bulletHeightSetter = getSetter(bulletStyle, "Height");
            var bulletWidthSetter = getSetter(bulletStyle, "Width");
            var butlletRadiusXSetter = getSetter(bulletStyle, "RadiusX");
            var butlletRadiusYSetter = getSetter(bulletStyle, "RadiusY");
            var butlletFillSetter = getSetter(bulletStyle, "Fill");

            var lineFillSetter = getSetter(lineStyle, "Fill");
            var lineStrokeSetter = getSetter(lineStyle, "Stroke");
            var lineStrokeThicknessSetter = getSetter(lineStyle, "StrokeThickness");

            scatterSize = new Size((double)bulletWidthSetter.Value, (double)bulletHeightSetter.Value);
            scatterXRadius = (double)butlletRadiusXSetter.Value;
            scatterYRadius = (double)butlletRadiusYSetter.Value;
            scatterIsFilled = !(butlletFillSetter == null || butlletFillSetter.Value == null || butlletFillSetter.Value == "");
            strokeThickness = (double)lineStrokeThicknessSetter.Value;

            // null GASCatterBulletStyle FILL & not-null GALineStyle FILL means use the standard colour
            if (lineFillSetter == null && scatterIsFilled)
            {
                fillBrush = firstDataPoint.ItemBrush;
            }
            else if (lineFillSetter != null && scatterIsFilled)
            {
                fillBrush = (Brush)lineFillSetter.Value;
            }

            // line stroke - if not set then from the pallette, else from the style
            if (lineStrokeSetter == null)
            {
                lineBrush = firstDataPoint.ItemBrush;
            }
            else
            {
                lineBrush = (Brush)lineStrokeSetter.Value;
            }
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
