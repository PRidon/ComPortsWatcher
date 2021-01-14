using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace ComPortsWatcher
{
    public class ComPortSearcher : IDisposable
    {
        public event Action<List<string>> PortUpdateEvent;
        public event Action<string> NewPortEvent;

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

        private bool CheckNewComPorts()
        {
            bool isNewPortsExist = false;
            List<string> comPorts = new List<string>();
            Ports = ComputerManagment.GetInstances();
            foreach (ManagementObject property in Ports)
            {
                if (property.GetPropertyValue("Name") != null)
                    if (property.GetPropertyValue("Name").ToString().Contains("(COM"))
                    {
                        string port = property.GetPropertyValue("Name").ToString();
                        //Console.WriteLine(property.GetPropertyValue("Name").ToString());
                        comPorts.Add(port);
                    }
            }
            // check com ports removing
            List<string> diff = ComPorts.Except(comPorts).ToList();
            if (diff.Count > 0) isNewPortsExist = true;
            // check com ports adding
            diff = comPorts.Except(ComPorts).ToList();
            if (diff.Count > 0) isNewPortsExist = true;
            foreach (string port in diff)
            {
                NewPortEvent?.Invoke(port);
            }
            ComPorts = comPorts;
            return isNewPortsExist;
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
                try
                {
                    if (CheckNewComPorts())
                    {
                        portCnt = ComPorts.Count;
                        PortUpdateEvent?.Invoke(MoveComFirst(ComPorts));
                    }
                }
                catch (Exception)
                { }
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
