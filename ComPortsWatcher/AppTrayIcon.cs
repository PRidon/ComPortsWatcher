using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ComPortsWatcher
{
    class AppTrayIcon : IDisposable
    {
        public event Action CloseTrayIconEvent;
        private NotifyIcon m_ni;
        
        public AppTrayIcon()
        {
            m_ni = new NotifyIcon();
        }

        public void Start()
        {
            var myThread = new Thread(delegate ()
            {
                Display();
                Application.ApplicationExit += (object sender, EventArgs e) => CloseTrayIconEvent?.Invoke();
                Application.Run();
            });

            myThread.SetApartmentState(ApartmentState.STA);
            myThread.Start();
        }

        public void Display()
        {
            // Put the icon in the system tray and allow it react to mouse clicks.			
            m_ni.MouseClick += new MouseEventHandler(ni_MouseClick);
            m_ni.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); ;
            m_ni.Text = "ComPortsWatcher";
            m_ni.Visible = true;

            // Attach a context menu.
            m_ni.ContextMenuStrip = new ContextMenu().Create();
        }

        public void UpdateMenuByPorts(List<string> ports)
        {
            if (m_ni.ContextMenuStrip.InvokeRequired)
                m_ni.ContextMenuStrip.Invoke(new Action<List<string>>(UpdateMenuByPorts), new object[] { ports });
            else
            {
                while (m_ni.ContextMenuStrip.Items.Count > 3)
                {
                    m_ni.ContextMenuStrip.Items.RemoveAt(0);
                }
                foreach (string port in ports)
                {

                    ToolStripMenuItem tool = new ToolStripMenuItem();
                    tool.Text = port;
                    m_ni.ContextMenuStrip.Items.Insert(0, tool);
                }
            }
        }
        public void ShowNotifier(string mess)
        {
            m_ni.BalloonTipText = mess;
            m_ni.BalloonTipTitle = "New port available";
            //m_ni.Icon = Properties.Resources.ComPortIcon;
            m_ni.BalloonTipIcon = ToolTipIcon.Info;
            m_ni.ShowBalloonTip(1000);
        }

        public void Dispose()
        {
            m_ni.Visible = false;
            m_ni.Dispose();
        }

        // TODO handle any mouse click directly on the notification icon
        void ni_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
    }
}