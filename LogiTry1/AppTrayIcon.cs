using System;
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
            m_ni.Text = "SpotifyStatusApplet";
            m_ni.Visible = true;

            // Attach a context menu.
            m_ni.ContextMenuStrip = new ContextMenu().Create();
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