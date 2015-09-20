using System;

namespace Ark
{
    public enum Resolution { WVGA, WXGA, HD };

    public static class ResolutionHelper
    {
        private static bool WVGA
        {
            get { return App.Current.Host.Content.ScaleFactor == 100; }
        }

        private static bool WXGA
        {
            get { return App.Current.Host.Content.ScaleFactor == 160; }
        }

        public static bool HD
        {
            get { return App.Current.Host.Content.ScaleFactor == 150; }
        }

        public static Resolution DeviceResolution
        {
            get
            {
                if(WVGA)
                {
                    return Resolution.WVGA;
                }
                else if(WXGA)
                {
                    return Resolution.WXGA;
                }
                else if(HD)
                {
                    return Resolution.HD;
                }
                else
                {
                    throw new InvalidOperationException("Unknown resolution");
                }
            }
        }
    }
}