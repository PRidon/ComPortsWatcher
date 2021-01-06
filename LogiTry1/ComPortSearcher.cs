using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace ComPortsWatcher
{
    public class ComPortSearcher : IDisposable
    {
        public event Action<List<string>> NewComPortEvent;

        private Thread PortSearchThread;
        private ManagementClass ComputerManagment;
        private ManagementObjectCollection Ports;
        private List<string> ComPorts;
        private bool disposedValue;

        public ComPortSearcher()
        {
            ComputerManagment = new ManagementClass("Win32_PnPEntity");
            ComPorts = new List<string>();
            PortSearchThread = new Thread(NewPortSearcher);
            PortSearchThread.Start();
        }

        private int FindNewComPorts()
        {
            Ports = ComputerManagment.GetInstances();
            foreach (ManagementObject property in Ports)
            {
                if (property.GetPropertyValue("Name") != null)
                    if (property.GetPropertyValue("Name").ToString().Contains("(COM"))
                    {
                        string port = property.GetPropertyValue("Name").ToString();
                        //Console.WriteLine(property.GetPropertyValue("Name").ToString());
                        if (ComPorts.IndexOf(port) < 0) ComPorts.Add(port);
                    }
            }
            ComPorts = MoveComFirst(ComPorts);
            return ComPorts.Count;
        }

        private List<string> MoveComFirst(List<string> strs)
        {
            List<string> ret = new List<string>();
            foreach (string str in strs)
            {
                int start = str.LastIndexOf('(');
                int end = str.LastIndexOf(')');
                if ((start < 0) || (end < 0)) continue;
                string com = str.Substring(start + 1, end - start - 1);
                ret.Add(str.Remove(start).Insert(0, com + " "));
            }
            return ret;
        }

        private void NewPortSearcher()
        {
            int portCnt = 0;
            while(true)
            {
                if (portCnt != FindNewComPorts())
                {
                    portCnt = ComPorts.Count;
                    NewComPortEvent?.Invoke(ComPorts);
                }
                Thread.Sleep(3000);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PortSearchThread.Abort();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
