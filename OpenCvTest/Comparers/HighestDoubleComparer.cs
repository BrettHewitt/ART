using System.Collections.Generic;

namespace AutomatedRodentTracker.Comparers
{
    public class HighestDoubleComparer : IComparer<double>
    {
        public int Compare(double x, double y)
        {
            if (x < y)
            {
                return 1;
            }

            if (x > y)
            {
                return -1;
            }

            return 0;
        }
    }
}
