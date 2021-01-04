using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LogiTry1
{
    public class ComPortSearcher
    {
        private ManagementClass ComputerManagment;
        private ManagementObjectCollection Ports;

        public ComPortSearcher()
        {
            ComputerManagment = new ManagementClass("Win32_PnPEntity");
        }

        public string[] GetComPortsDescription()
        {
            Ports = ComputerManagment.GetInstances();
            List<string> comPorts = new List<string>();
            foreach (ManagementObject property in Ports)
            {
                if (property.GetPropertyValue("Name") != null)
                    if (property.GetPropertyValue("Name").ToString().Contains("(COM"))
                    {
                        //Console.WriteLine(property.GetPropertyValue("Name").ToString());
                        comPorts.Add(property.GetPropertyValue("Name").ToString());
                    }
            }
            return MoveComFirst(comPorts).ToArray();
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
    }
}
