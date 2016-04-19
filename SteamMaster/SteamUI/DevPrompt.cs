using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DummyClassSolution.Properties;
using static SteamUI.SteamTheme;

namespace SteamUI
{
    public partial class DevPrompt : Form
    {
        public const int WmNclbuttondown = 0xA1;
        public const int Htcaption = 0x2;

        public DevPrompt()
        {
            InitializeComponent();
        }

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void btnOK_Click(object sender, EventArgs e)
        {
            DevKey = devKeyTextBox.Text;
            if (saveCheckBox1.Checked)
            {
                Settings.Default.DevKey = DevKey;
                Settings.Default.Save(); // Saves settings in application configuration file
            }
            Close();
        }

        private void DevPrompt_Load(object sender, EventArgs e)
        {
            devKeyTextBox.Text = Settings.Default.DevKey;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DevPrompt_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WmNclbuttondown, Htcaption, 0);
            }
        }

        private void devKeyTextBox_Click(object sender, EventArgs e)
        {
            if (devKeyTextBox.Text == "null")
            {
                devKeyTextBox.Text = "";
            }
        }
    }
}