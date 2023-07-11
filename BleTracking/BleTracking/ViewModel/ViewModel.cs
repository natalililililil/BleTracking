namespace BleTracking.ViewModel
{
    internal class ViewModel
    {
        protected const string DigitAndDistanceDefault = "";
        protected internal bool Reciving { get; set; }
        internal void SetReciving()
        {
            Reciving = true;
        }

        internal void SetRecived()
        {
            Reciving = false;
        }
    }
}
