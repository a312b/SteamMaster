using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DummyClassSolution
{
    public class DescriptionBox : RichTextBox
    {
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;
        private uint WM_VSCROLL = 0x115;

        public DescriptionBox()
        {
            ScrollBars = RichTextBoxScrollBars.None;
            MouseWheel += Me_MouseWheel;
        }

        //// Source of idea https://social.msdn.microsoft.com/Forums/vstudio/en-US/ba339154-95b7-4e13-a2c0-32593cadb984/richtextbox-vscrollbar?forum=vbgeneral
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);


        private void Me_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    SendMessage(Handle, WM_VSCROLL, SB_LINEDOWN, 0); // Scrolls down
                }
                else
                {
                    SendMessage(Handle, WM_VSCROLL, SB_LINEUP, 0);// Scrolls up
                }
            }
            catch
            {
            }
        }
    }
}
