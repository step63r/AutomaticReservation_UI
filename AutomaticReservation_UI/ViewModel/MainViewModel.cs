using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using AutomaticReservation_UI.Model;
using System;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using AutomaticReservation_UI.UserControls;

namespace AutomaticReservation_UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region コマンド・プロパティ
        private readonly IDataService _dataService;
        private DateTime _targetDate;
        public DateTime TargetDate
        {
            get { return _targetDate; }
            set
            {
                _targetDate = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand _btnExecute;
        public RelayCommand BtnExecute
        {
            get
            {
                if (_btnExecute == null)
                {
                    _btnExecute = new RelayCommand(Execute, CanExecute);
                }
                return _btnExecute;
            }
        }

        public ObservableCollection<ReservationControlViewModel> ReservationList { get; set; } = new ObservableCollection<ReservationControlViewModel>();
        private ReservationControlViewModel _reservationViewModel;
        public ReservationControlViewModel ReservationViewModel
        {
            get
            {
                return _reservationViewModel;
            }
            set
            {
                _reservationViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                });
            TargetDate = DateTime.Now;
        }

        /// <summary>
        /// コマンドを実行
        /// </summary>
        public void Execute()
        {
            var item = new ReservationControlViewModel(DateTime.Now, "This is Test");
            ReservationList.Add(item);
        }
        /// <summary>
        /// コマンドが実行可能かどうかを判定
        /// </summary>
        /// <returns></returns>
        public bool CanExecute()
        {
            return true;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}