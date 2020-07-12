using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using CSharpMath;
using System.Diagnostics;

namespace PEM.AppWindows
    {
    /// <summary>
    /// Interaction logic for CreateTimeSeriesWindow.xaml
    /// </summary>
    public partial class CreateTimeSeriesWindow : Window
        {
        private MainWindow mainWindow;
        private string[] headers;
        private string selectedTimeSeries;
        private int selectedTimeSeriesIndex;
        private DataTable timeseriesTable;
        private DataTable dateseriesTable;
        private bool timeseries_has_time;
        public string filename;
        public string outFile_PTS_filePathWithoutExt;
        public string outFile_PTS;
        private char delimiter;
        private int dateIndex;
        private int timeIndex;
        private string date_Title;
        private string time_Title;
        public ImageSource Source { get; set; }
        public new ControlTemplate Template { get; set; }

        public CreateTimeSeriesWindow ()
            {
            InitializeComponent ();

            }
        public CreateTimeSeriesWindow (MainWindow mainWindow)
            {

            this.mainWindow = mainWindow;
            InitializeComponent ();

            timeseries_has_time = false;

            filename = mainWindow.TextBox_Browse.Text;

            dateIndex = 0;
            timeIndex = 1;
            selectedTimeSeriesIndex = -1;   //Assuming the selected time series column doesn't exist until we find it.

            date_Title = "date";
            time_Title = "time";

            //initializing DataTables

            timeseriesTable = new DataTable ();
            dateseriesTable = new DataTable ();

            timeseriesTable.Columns.Add ("Date", typeof (string));
            timeseriesTable.Columns.Add ("Time", typeof (string));
            timeseriesTable.Columns.Add ("Variable", typeof (string));

            dateseriesTable.Columns.Add ("Date", typeof (string));
            dateseriesTable.Columns.Add ("Variable", typeof (string));

            }

        private void loadHeaderButton_Click (object sender, RoutedEventArgs e)
            {
            stackPanel_TS_Import.IsEnabled = true;
            stackPanel_ImportBtn.IsEnabled = true;

            loadComboBoxItems ();

            if (dateFormat_rdButton.IsChecked == true)
                setDateColumnTitle ();
            if (dateTimeFormat_rdButton.IsChecked == true)
                setTimeColumnTitle ();
            }


        private void loadComboBoxItems ()
            {

            string[] headers=getCSV_Header (filename, GetDelimiter ());

            Console.WriteLine ("Number of columns: {0}.", headers.Length);
            //MessageBox.Show ("number of columns " + headers.Length, "Verifying the choice of list separator ", (MessageBoxButton) MessageBoxButtons.OKCancel);

            //Fill in the Combo Box for choosing the time series to extract from the source file
            foreach (string colName in headers)
                {
                headersComboList.Items.Add (colName);
                }
            headersComboList.SelectedItem = headers[headers.Length - 1];
            }

        private void setTimeColumnTitle ()
            {
            setDateColumnTitle ();
            string stringToCheck = "time";
            int stringToCheckIndex = -1;
            string elementInArray = "Not Defined or Not Found";
            timeColTitle.Text = elementInArray;
            if (Array.Exists<string> (headers, (Predicate<string>) delegate (string s)
                {
                    stringToCheckIndex = s.IndexOf (stringToCheck, StringComparison.OrdinalIgnoreCase);
                    elementInArray = s;
                    return stringToCheckIndex > -1;
                    }))
                {
                timeColTitle.Text = elementInArray;
                }
            }

        private void setDateColumnTitle ()
            {

            dateColTitle.Text = "Not Defined or Not Found";

            var match = headers.FirstOrDefault(c => c.IndexOf("date", StringComparison.OrdinalIgnoreCase) > 0);

            if (match!=null)                
                dateColTitle.Text = match;
            timeColTitle.Text = "The chosen time seies have only date but no time to display.";
            }

        private char GetDelimiter ()
            {
            if (semicolonDelimiter.IsChecked == true)
                {
                return ';';
                }
            if (tabDelimiter.IsChecked == true)
                {
                return '\t';
                }
            if (spaceDelimiter.IsChecked == true)
                {
                return ' ';
                }
            if (otherDelimiter.Text.Length > 0)
                {
                return otherDelimiter.Text[0];
                }
            return ','; // default delimiter
            }


        // returns an array of strings representing the different fields of the csv file
        public string[] getCSV_Header (string filename, char delimiter)
            {
            StreamReader fileReader = null;
            try
                {
                fileReader = new StreamReader (@filename);
                }
            catch (FileNotFoundException)
                {
                return null;
                }
            string header = fileReader.ReadLine();
            header = header.Replace ('"', ' ');
            headers = header.Split (delimiter).Select (s => s.Trim ()).Where (s => s != String.Empty).ToArray ();

            headers.ToList ().ForEach (i => Console.WriteLine (i.ToString ()));

            Console.WriteLine ("[{0}]", string.Join (", ", headers));

            fileReader.Close ();
            return headers;
            }

        private void TextBox_CreateCSV_PreviewMouseDown (object sender, MouseButtonEventArgs e)
            {

            }

        private void previousButton_Click (object sender, RoutedEventArgs e)
            {
            this.Close ();
            mainWindow.Show ();
            }

        private void specificDelimiter_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e)
            {

            }

        private void importCSVFile_Click (object sender, RoutedEventArgs e)
            {

            try
                {
                date_Title = dateColTitle.Text;
                }
            catch (NullReferenceException err)
                {
                Console.WriteLine ("Error in Dta/Time title setting: " + err.ToString ());
                dateColTitle.Text = date_Title;
                timeColTitle.Text = time_Title;
                }
            finally
                {
                if (String.Compare ("date", dateColTitle.Text) != 0)
                    {
                    date_Title = dateColTitle.Text;
                    }

                if (String.Compare ("time", timeColTitle.Text) != 0)
                    {
                    time_Title = timeColTitle.Text;
                    }
                }

            Console.WriteLine ("Date title is: {0}, and Time title is {1}.", date_Title, time_Title);

            for (int j = 0; j < headers.Length; j++)
                {
                if (String.Compare (headers[j], selectedTimeSeries) == 0)
                    {
                    selectedTimeSeriesIndex = j;
                    }
                if (String.Compare (headers[j], date_Title) == 0)
                    dateIndex = j;
                if (String.Compare (headers[j], time_Title) == 0)
                    timeIndex = j;

                }

            Console.WriteLine ("Selected: {0} and with index: {1}.", selectedTimeSeries, selectedTimeSeriesIndex);
            Console.WriteLine ("dateIndex: {0}", dateIndex);
            Console.WriteLine ("timeIndex: {0}", timeIndex);
            Console.WriteLine ("selectedTimeSeriesIndex {0}.", selectedTimeSeriesIndex);
            Console.WriteLine ("TimeSeries has time:     {0}!.", timeseries_has_time.ToString ());

            get_TS_Data (); // fill the data table

            if (saveTimeSeriesTo_CSV_File ())
                {
                if (MessageBox.Show ("The Selected time series " + selectedTimeSeries + " has been saved!", "Saved Successfully ", (MessageBoxButton) MessageBoxButtons.OKCancel) == MessageBoxResult.OK)
                    {
                    AnalysisTimeSeriesWindow analysis = new AnalysisTimeSeriesWindow (this);
                    analysis.ShowDialog ();
                    }
                }

            }

        private bool saveTimeSeriesTo_CSV_File ()
            {

            string extension = Path.GetExtension(filename);
            outFile_PTS_filePathWithoutExt = filename.Substring (0, filename.Length - extension.Length);
            outFile_PTS = outFile_PTS_filePathWithoutExt + "_" + selectedTimeSeries + ".pts";

            using (StreamWriter file = new StreamWriter (@outFile_PTS))
                {
                if (timeseries_has_time)
                    {
                    file.WriteLine ("date, time, variable");
                    foreach (DataRow dr in timeseriesTable.Rows)
                        file.WriteLine (dr["Date"] + ", " + dr["Time"] + ", " + dr["Variable"]);
                    }
                else
                    {
                    file.WriteLine ("date, variable");
                    foreach (DataRow dr in dateseriesTable.Rows)
                        file.WriteLine (dr["Date"] + ", " + dr["Variable"]);
                    }
                file.Close ();
                return true;
                }
            }

        //********************************************************************************************************//
        private void get_TS_Data ()
            {
            StreamReader fileReader = null;
            try
                {
                fileReader = new StreamReader (filename);
                }
            catch (FileNotFoundException)
                {
                MessageBox.Show ("Error# Check the input csv file name!");
                }

            string line = fileReader.ReadLine();

            while ((line = fileReader.ReadLine ()) != null)
                {

                line = line.Replace ('"', ' ');
                string[] values = line.Split(delimiter).Select(s => s.Trim()).Where(s => s != String.Empty).ToArray();


                values.ToList ().ForEach (i => Console.Write (" " + i.ToString ())); Console.WriteLine ();

                if (timeseries_has_time)
                    {
                    timeseriesTable.Rows.Add (new object[] { values[dateIndex], values[timeIndex], values[selectedTimeSeriesIndex] });
                    }
                else
                    {
                    dateseriesTable.Rows.Add (new object[] { values[dateIndex], values[selectedTimeSeriesIndex] });
                    }
                }
            }

        private void headersComboList_SelectionChanged (object sender, SelectionChangedEventArgs e)
            {
            selectedTimeSeries = e.AddedItems[0].ToString ();
            }

        private void dateFormat_rdButton_Click (object sender, RoutedEventArgs e)
            {
            timeseries_has_time = false;
            }

        private void dateTimeFormat_rdButton_Click (object sender, RoutedEventArgs e)
            {
            timeseries_has_time = true;
            setTimeColumnTitle ();
            }

        private void otherDelimiter_LostFocus (object sender, RoutedEventArgs e)
            {
            if (!otherDelimiter.Text.Equals (""))
                {
                commaDelimiter.IsChecked = false;
                semicolonDelimiter.IsChecked = false;
                tabDelimiter.IsChecked = false;
                spaceDelimiter.IsChecked = false;
                delimiter = otherDelimiter.Text[0];
                }

            }

        private void questionMark_MouseEnter (object sender, System.Windows.Input.MouseEventArgs e)
            {
            Image img = ((Image)sender);
            img.ToolTip = "Check if what shown below is the same as what you have in your source file as Date Column title.";

            }

        private void questionMark_MouseLeave (object sender, System.Windows.Input.MouseEventArgs e)
            {
            Image img = ((Image)sender);            
            }

        private void dateFormat_rdButton_Checked (object sender, RoutedEventArgs e)
            {
            setDateColumnTitle ();
            Create_CSV_btn_Panel.IsEnabled = true;
            }

        private void dateTimeFormat_rdButton_Checked (object sender, RoutedEventArgs e)
            {
            setTimeColumnTitle ();
            Create_CSV_btn_Panel.IsEnabled = true;
            }
        }
    }