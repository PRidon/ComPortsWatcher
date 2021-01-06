using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComPortsWatcher
{
    class Program
    {
        static bool IsRunnig = false;

        static void Main(string[] args)
        {
            AppTrayIcon ati = new AppTrayIcon();
            ati.CloseTrayIconEvent += EndProgram;
            ati.Start();
            LogitechGSDK LogitechLcd = new LogitechGSDK(null, null, null, null);
            ComPortSearcher PortsSearcher = new ComPortSearcher();
            PortsSearcher.NewComPortEvent += LogitechLcd.LcdUpdate;

            try
            {
                uint tte = 0;
                IsRunnig = true;
                do
                {
                    LogitechLcd.Polling();
                    tte++;
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
            //LogitechGSDK.LogiLcdColorSetBackground(pixelMatrix);
        }

        private static void EndProgram()
        {
            IsRunnig = false;
        }
    }
}
