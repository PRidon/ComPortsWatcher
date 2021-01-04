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

namespace LogiTry1
{
    class Program
    {
        static void Main(string[] args)
        {
            LogitechGSDK LogitechLcd = new LogitechGSDK(null, null, null, null);
            ComPortSearcher PortsSearcher = new ComPortSearcher();
            try
            {
                uint tte = 0;
                do
                {
                    if (tte % 100 == 0)
                    {
                        LogitechLcd.LcdUpdate(PortsSearcher.GetComPortsDescription());
                    }
                    LogitechLcd.Polling();
                    tte++;
                    Thread.Sleep(33);
                }
                while (true);
            }
            catch (Exception e)
            {
                Trace.TraceError("Caught exception, application exited " + e.ToString());
            }
            //LogitechGSDK.LogiLcdShutdown();
            //byte[] pixelMatrix = new byte[LogitechGSDK.LOGI_LCD_COLOR_WIDTH * LogitechGSDK.LOGI_LCD_COLOR_HEIGHT * 4];
            ////fill	this	array	with	your	image
            //LogitechGSDK.LogiLcdColorSetBackground(pixelMatrix);
        }
    }
}
