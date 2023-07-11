namespace BleTracking.ViewModel
{
    using System.ComponentModel;

    internal class DistanceViewModel : ViewModel, INotifyPropertyChanged
    {
        public enum Properties
        {
            Distance,
        }

        private string _distance = DigitAndDistanceDefault;

        public DistanceViewModel()
        {
            SetRecived();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                if (_distance != value)
                {
                    _distance = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distance"));
                }
            }
        }
    }
}
