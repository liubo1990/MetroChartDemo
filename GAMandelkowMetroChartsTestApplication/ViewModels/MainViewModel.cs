using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;
using GAMandelkowMetroChartsTestApplication.Models;

namespace GAMandelkowMetroChartsTestApplication.ViewModels
{
    /// <summary>
    /// *****************************
    /// NOTE : Debug / enable the VS hosting process can be turned off to maintain the config file
    /// This is in the project properties / debig tab
    /// so that when testing you dont have to keep recreating the DB
    /// ALSO see the TransactionContext : DbContext to ensure the createAlways or only if required is set!
    /// ******************************
    /// 
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        private GraphSeriesInformation _scatterData;
        public GraphSeriesInformation scatterData
        {
            get
            {
                return _scatterData;
            }
            set
            {
                if (_scatterData != value)
                {
                    _scatterData = value;
                    RaisePropertyChanged("scatterData");
                }
            }
        }

        private GraphSeriesInformation _scatterData1;
        public GraphSeriesInformation scatterData1
        {
            get
            {
                return _scatterData1;
            }
            set
            {
                if (_scatterData1 != value)
                {
                    _scatterData1 = value;
                    RaisePropertyChanged("scatterData1");
                }
            }
        }
       
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {

            }
            scatterData = new GraphSeriesInformation();
            scatterData.seriesDisplayName = "Series 1";
            scatterData.Items.Add(new GraphSeriesDataPoint("mon", 7));
            scatterData.Items.Add(new GraphSeriesDataPoint("tue", 3.3));
            scatterData.Items.Add(new GraphSeriesDataPoint("wed", 4.5));
            scatterData.Items.Add(new GraphSeriesDataPoint("thur", 5.2));
            scatterData.Items.Add(new GraphSeriesDataPoint("fri", 3.3));
            scatterData.Items.Add(new GraphSeriesDataPoint("sat", 1));
            scatterData.Items.Add(new GraphSeriesDataPoint("sun", 1));

            scatterData1 = new GraphSeriesInformation();
            scatterData1.seriesDisplayName = "Series 2";
            scatterData1.Items.Add(new GraphSeriesDataPoint("mon", 1));
            scatterData1.Items.Add(new GraphSeriesDataPoint("tue", 2));
            scatterData1.Items.Add(new GraphSeriesDataPoint("wed", 4.0));
            scatterData1.Items.Add(new GraphSeriesDataPoint("thur", 9));
            scatterData1.Items.Add(new GraphSeriesDataPoint("fri", 2.8));
            scatterData1.Items.Add(new GraphSeriesDataPoint("sat", 3));
            scatterData1.Items.Add(new GraphSeriesDataPoint("sun", 0));
        }
   
    }
}