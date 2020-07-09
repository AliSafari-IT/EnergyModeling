using Microsoft.Win32;
using PEM.AppWindows;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PEM
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {

        public MainWindow ()
            {
            InitializeComponent ();
            TextBox_Browse.Background = SystemColors.MenuHighlightBrush;

            }

        protected override void OnClosed (EventArgs e)
            {
            base.OnClosed (e);

            Application.Current.Shutdown ();
            }

        private void TextBox_Browse_PreviewMouseDown (object sender, MouseButtonEventArgs e)
            {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName (System.Diagnostics.Process.GetCurrentProcess ().MainModule.FileName);

            if (openFileDialog.ShowDialog () == true)
                {

                TextBox_Browse.Text = openFileDialog.FileName;
                TextBox_Browse.FlowDirection = FlowDirection.LeftToRight;
                TextBox_Browse.Height = 35;
                TextBox_Browse.Background = Brushes.Bisque;
                importCSVFile.Visibility = Visibility.Visible;
                TextBox_Browse.ToolTip = "Click again to select another file";
                }

            }

        private void importCSVFile_Click (object sender, RoutedEventArgs e)
            {
            CreateTimeSeriesWindow createTS = new CreateTimeSeriesWindow(this);
            createTS.ShowDialog ();
            }
        }
    }


// This will logically show the date combined with the time
//DateTime.ParseExact(e.CellElement.Value.ToString(), "dd.MM.yyyy HH:mm:ss", CultureInfo.CurrentCulture)

//DateTime.ParseExact(e.CellElement.Value.ToString(), "dd.MM.yyyy", CultureInfo.CurrentCulture
