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
        }
    }
}