using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DummyClassSolution
{
    public class DescriptionBox : RichTextBox
    {

        //// see http://social.msdn.microsoft.com/Forums/vstudio/en-US/ba339154-95b7-4e13-a2c0-32593cadb984/descriptionBox-vscrollbar?forum=vbgeneral
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private uint WM_VSCROLL = 0x115;
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;

        public DescriptionBox()
        {
            this.ScrollBars = RichTextBoxScrollBars.None;
            this.MouseWheel += Me_MouseWheel;
        }


        private void Me_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    SendMessage(this.Handle, WM_VSCROLL, SB_LINEDOWN, 0); // Scrolls down
                }
                else
                {
                    SendMessage(this.Handle, WM_VSCROLL, SB_LINEUP, 0);// Scrolls up
                }
            }
            catch
            {
            }
        }
    }
}
