using System;
using System.Collections.Generic;
using System.Text;

namespace BleTracking.Filter
{
    public static class KalmalFilter
    {
        static double Q = 0.05; // скорость реакции на изменение (подобрать вручную)
        static double P0 = 0.0;
        static double Pk = 1.0;
        static double X0 = 0.0;
        static double X = 0.0;
        static double F = 1.0;
        static double H = 1.0;

        internal static double GetFileterData(int rssi, double R)
        {
            X0 = F * X;
            P0 = F * Pk * F + Q;

            double K = H * P0 / (H * P0 * H + R);
            X = X0 + K * (rssi - H * X0);
            Pk = (1 - K * H) * P0;
            return X;
        }
    }
}
