using System;
using System.Diagnostics;
using System.Threading;

namespace ComPortsWatcher
{
    class Program
    {
        static bool IsRunnig = false;

        static void Main(string[] args)
        {
            AppTrayIcon ati = new AppTrayIcon();
            Action polling = null;
            ati.CloseTrayIconEvent += EndProgram;
            ati.Start();
            ComPortSearcher PortsSearcher = new ComPortSearcher();
            //LogitechGSDK LogitechLcd = new LogitechGSDK(null, null, null, null);
            //PortsSearcher.PortUpdateEvent += LogitechLcd.LcdUpdate;
            try
            {
                LogitechGSDK LogitechLcd = new LogitechGSDK(null, null, null, null);
                if (LogitechGSDK.LcdExist())
                {
                    polling = LogitechLcd.Polling;
                    PortsSearcher.PortUpdateEvent += LogitechLcd.LcdUpdate;
                }
            }
            catch (Exception)
            { }
            PortsSearcher.PortUpdateEvent += ati.UpdateMenuByPorts;
            PortsSearcher.NewPortEvent += ati.ShowNotifier;

            try
            {
                IsRunnig = true;
                do
                {
                    polling?.Invoke();
                    Thread.Sleep(33);
                }
                while (IsRunnig);
            }
            catch (Exception e)
            {
                Trace.TraceError("Caught exception, application exited " + e.ToString());
            }
            ati.Dispose();
            PortsSearcher.Dispose();
            //LogitechGSDK.LogiLcdShutdown();
            //byte[] pixelMatrix = new byte[LogitechGSDK.LOGI_LCD_COLOR_WIDTH * LogitechGSDK.LOGI_LCD_COLOR_HEIGHT * 4];
            ////fill	this	array	with	your	image
        }

        private static void EndProgram()
        {
            IsRunnig = false;
        }
    }
}
