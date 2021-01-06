using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace ComPortsWatcher
{
    class ContextMenu
    {
        private bool m_isAboutLoaded = false;

        public ContextMenuStrip Create()
        {
            // Add the default menu options.
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem about;
            ToolStripSeparator sep;

            // About.
            about = new ToolStripMenuItem();
            about.Text = "About";
            about.Click += new EventHandler(About_Click);
            menu.Items.Add(about);

            // Separator.
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            about = new ToolStripMenuItem();
            about.Text = "Exit";
            about.Click += new System.EventHandler(Exit_Click);
            menu.Items.Add(about);

            return menu;
        }

        void About_Click(object sender, EventArgs e)
        {
            if (!m_isAboutLoaded)
            {
                m_isAboutLoaded = true;
                //new AboutBox().ShowDialog();
                m_isAboutLoaded = false;
            }
        }

        void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
