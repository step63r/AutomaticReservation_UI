using System.Windows;
using AutomaticReservation_UI.ViewModel;

namespace AutomaticReservation_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            TargetDatePicker.BlackoutDates.AddDatesInPast();
            Closing += (s, e) => ViewModelLocator.Cleanup();

            // Setting の値をウィンドウに反映
            Left = Properties.Settings.Default.Left;
            Top = Properties.Settings.Default.Top;
            Width = Properties.Settings.Default.Width;
            Height = Properties.Settings.Default.Height;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Normal)
                {
                    // ウィンドウの値を Settings に格納
                    Properties.Settings.Default.Left = Left;
                    Properties.Settings.Default.Top = Top;
                    Properties.Settings.Default.Width = Width;
                    Properties.Settings.Default.Height = Height;
                    // ファイルに保存
                    Properties.Settings.Default.Save();
                }
            }
            catch { }
        }
    }
}