﻿using System;using System.Collections.Generic;using System.Globalization;using System.IO;using System.Linq;using System.Text;using System.Threading.Tasks;using System.Windows;using System.Windows.Controls;using System.Windows.Data;using System.Windows.Documents;using System.Windows.Input;using System.Windows.Media;using System.Windows.Media.Imaging;using System.Windows.Shapes;using Path = System.IO.Path;using PEM.Model;using PEM.AppWindows;namespace PEM.AppWindows    {    /// <summary>    /// Interaction logic for AnalysisTimeSeriesWindow.xaml    /// </summary>    public partial class AnalysisTimeSeriesWindow : Window        {        private CreateTimeSeriesWindow createTimeSeriesWindow;        private string selctedComboItem;        private List<Double> ts_dataValues;        public int arima_d_order;        public int arima_p_order;        public int arima_q_order;        public double alpha;        private bool data_Log_Transformation;        private bool boxcox_power_trans;        private double predictionNumber;        public AnalysisTimeSeriesWindow ()            {            InitializeComponent ();            }        public AnalysisTimeSeriesWindow (CreateTimeSeriesWindow createTimeSeriesWindow)            {            this.createTimeSeriesWindow = createTimeSeriesWindow;            InitializeComponent ();            FillUp_TS_Combo ();            }        private void FillUp_TS_Combo ()            {            selctedComboItem = Path.GetFileName (createTimeSeriesWindow.outFile_PTS);            string path = Path.GetDirectoryName(createTimeSeriesWindow.filename);            object[] tsFiles = new DirectoryInfo (path).GetFiles ("*.pts").ToArray ();            foreach (object o in tsFiles)                {                Combo_TS_FileList.Items.Add (o.ToString ());                if (selctedComboItem == o.ToString ())                    {                    Combo_TS_FileList.SelectedItem = o.ToString ();                    }                }            }        private void Window_Loaded (object sender, RoutedEventArgs e)            {            if (Double.TryParse (numPredic.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out predictionNumber))                {                if (predictionNumber < 1)                    numPredicExplain.Text = "If number of prediction np&lt;1, then number of predictions is calculated as a ratio of the total data number.";                }            else                {                numPredicExplain.Text = "Number of prediction np&gt;=1.";                }            // Get time series values from the selected PTS file            ts_dataValues = get_TS_data (createTimeSeriesWindow.outFile_PTS, ',');  //Load data list            //  Calculate mean value            meanValue.Text = getAverage ().ToString ();            // median of elements            medianValue.Text = GetMedian ().ToString ();            //calculate the standard deviation            sdValue.Text = getStandardDeviation ().ToString ();            // Get max and min            maxVal.Text = getMax ().ToString ();            minVal.Text = getMin ().ToString ();            // Count the list elements             tsLength.Text = getLength ().ToString ();            updateArimaOrders ();            }        private void FillInArimaModelButton ()            {            modelARIMA.Content = "ARIMA(" + pOrder.Text + "," + diffOrder.Text + "," + qOrder.Text + ")";            }        private int getLength ()            {            return ts_dataValues.Count;            }        private object getMin ()            {            return ts_dataValues.Min ();            }        private double getMax ()            {            return ts_dataValues.Max ();            }        private double getAverage ()            {            return ts_dataValues.Average ();    //  Calculate mean value            }        private double getStandardDeviation ()            {            double sumOfSquaresOfDifferences = ts_dataValues.Select(val => (val - getAverage()) * (val - getAverage())).Sum();            double sd = Math.Sqrt(sumOfSquaresOfDifferences / ts_dataValues.Count);            return sd;            }        private double GetMedian ()            {            var ary = new double[ts_dataValues.Count];            for (var ii = 0; ii < ts_dataValues.Count; ii++)                {                ary[ii] = Convert.ToDouble (ts_dataValues[ii]);                }            Array.Sort (ary);            if (ary.Length % 2 != 0)                {                return ary[ary.Length / 2];                }            else                {                return (ary[ary.Length / 2] + ary[(ary.Length / 2) + 1]) / 2;                }            }        private double ts_sum (List<double> ts_dataValues)            {            double sum=0;            foreach (double v in ts_dataValues)                {                sum += v;                }            return sum;            }        private List<Double> get_TS_data (string ptsFile, char delimiter)            {            List<Double> ts = new List<Double>();            StreamReader fileReader = null;            string[] headers=createTimeSeriesWindow.getCSV_Header (ptsFile, ',');            int fieldsNr = headers.Count();            try                {                fileReader = new StreamReader (ptsFile);                }            catch (FileNotFoundException)                {                string msg = "Error# File Not Found! "+ptsFile;                MessageBox.Show (msg);                }            string line = fileReader.ReadLine(); // skip header            int lineNr=1;            while ((line = fileReader.ReadLine ()) != null)                {                lineNr++;                line = line.Replace ('"', ' ');                string[] values = line.Split(delimiter).Select(s => s.Trim()).Where(s => s != String.Empty).ToArray();                if (Double.TryParse (values[values.Count () - 1], NumberStyles.Any, CultureInfo.CurrentCulture, out double number))                    {                    Console.WriteLine (string.Format ("Local culture results for the parsing of {0} is {1}", values[values.Count () - 1], number));                    ts.Add (number);                    }                else                    {                    Console.WriteLine ("Unable to parse '{0}'.", values[values.Count () - 1].ToString ());                    MessageBox.Show ("Unable to parse: " + values[values.Count () - 1].ToString () + " at line number " + lineNr);                    }                }            return ts;            }        private void Combo_TS_FileList_SelectionChanged (object sender, SelectionChangedEventArgs e)            {            String ptsFileSelected = e.AddedItems[0].ToString ();            // Get time series values from the selected PTS file            ts_dataValues = get_TS_data (ptsFileSelected, ',');  //Load data list            //  Calculate mean value            meanValue.Text = getAverage ().ToString ();            // median of elements            medianValue.Text = GetMedian ().ToString ();            //calculate the standard deviation            sdValue.Text = getStandardDeviation ().ToString ();            // Get max and min            maxVal.Text = getMax ().ToString ();            minVal.Text = getMin ().ToString ();            // Count the list elements             tsLength.Text = getLength ().ToString ();            }        private void Combo_Modeling_List_SelectionChanged (object sender, SelectionChangedEventArgs e)            {            }        private void boxCox_Click (object sender, RoutedEventArgs e)            {            lambdaPanel.Visibility = Visibility.Visible;            }        private void logTrans_Click (object sender, RoutedEventArgs e)            {            lambdaPanel.Visibility = Visibility.Collapsed;            }        private void sigLevel_TextChanged (object sender, TextChangedEventArgs e)            {            }        private void sigLevel_LostFocus (object sender, RoutedEventArgs e)            {            if (!double.TryParse (sigLevel.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out alpha))                {                string str= "Try again! The level of statistical significance (α) for 95% confidence intervals is 5% and for 99% confidence is 1%.";                MessageBox.Show (str, "Invalid α level", MessageBoxButton.OK);                }            else if (alpha > 100 || alpha < 0)                {                MessageBox.Show ("α level beyound the range : " + alpha);                }            }        private void updateArimaOrders ()            {            arima_d_order = getParsed (diffOrder.Text, "Err# ARIMA differencing is an integer value. ");            arima_p_order = getParsed (pOrder.Text, "Err# p is an integer value. ");            arima_q_order = getParsed (qOrder.Text, "Err# q is an integer value. ");            }        private int getParsed (string textNr, string v)            {            if (!int.TryParse (textNr, NumberStyles.Any, CultureInfo.CurrentCulture, out int number))                {                v += "Try again!";                MessageBox.Show (v, "Wrong input!", MessageBoxButton.OK);                }            else                {                FillInArimaModelButton ();                }            return number;            }        private void Order_PreviewKeyDown (object sender, KeyEventArgs e)            {            if (e.Key == Key.Return)                {                updateArimaOrders ();                }            }        //*****************************************************************************************************************************************************************************************//        private void modelARIMA_Click (object sender, RoutedEventArgs e)            {            updateArimaOrders ();            TimeSeriesAnalyzer seriesAnalyzer = new TimeSeriesAnalyzer(this);

            string inputFile = null;
           // seriesAnalyzer.getPrediction (inputFile);            // public void forecasttest(string file, string outfile, string xattr, char delimiter, bool hastime, bool change, double inmodel, double numpredict, bool reverse, double siglevel, bool log)         //   forecasttest (files[0], output + xattrs[4] + ".csv", xattrs[4], delimiters[0], false, true, 1000, 10, false, 0.05, false);            }        //*****************************************************************************************************************************************************************************************//        protected override void OnClosed (EventArgs e)            {            base.OnClosed (e);            App.Current.Shutdown ();            }        private void dateListSeparator_LostFocus (object sender, RoutedEventArgs e)            {            }        private void timeListSeparator_LostFocus (object sender, RoutedEventArgs e)            {            }        private void numPredic_LostFocus (object sender, RoutedEventArgs e)            {            }        }    }