using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordStore.Helpers
{
    public static class KeySender
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        public static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MapVirtualKey(int uCode, int uMapType);

        private enum KeyCode : uint
        {
            KeyDown = 0x00,
            KeyUp = 0x02,
            Shift = 0xA0,
            Control = 11
        }

        public static void SendCtrlC()
        {
            uint KEYEVENTF_KEYUP = 2;
            uint KEYEVENTF_KEYDOWN = 0;
            byte VK_CONTROL = 0x11;
            byte VK_SHIFT = 0x10;
            byte VK_C = 0x43;

            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(0x72, 0, KEYEVENTF_KEYUP, 0);
            System.Windows.Forms.Application.DoEvents();

            /*keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_C, 0, KEYEVENTF_KEYDOWN, 0);
            Application.DoEvents();

            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
            Application.DoEvents();*/

            keybd_event(VK_C, 0, KEYEVENTF_KEYDOWN, 0);
            System.Windows.Forms.Application.DoEvents();

            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, 0);
            System.Windows.Forms.Application.DoEvents();
        }

        public static void SendKeys(string text)
        {
            IntPtr keyboardLayout = GetKeyboardLayout(0);

            char[] chars = text.ToCharArray();

            foreach (char t in chars)
            {
                short vKey = VkKeyScanEx(t, keyboardLayout);

                byte m_HIBYTE = (Byte)(vKey >> 8);
                byte m_LOWBYTE = (Byte)(vKey & 0xFF);

                byte sScan = (byte)MapVirtualKey(m_LOWBYTE, 0);

                if ((m_HIBYTE == 1))
                    keybd_event((byte)KeyCode.Shift, 0x2A, (uint)KeyCode.KeyDown, 0);
                else if ((m_HIBYTE == 2))
                    keybd_event((byte)KeyCode.Control, 0, (uint)KeyCode.KeyDown, 0);

                keybd_event(m_LOWBYTE, sScan, (uint)KeyCode.KeyDown, 0);
                keybd_event(m_LOWBYTE, sScan, (uint)KeyCode.KeyUp, 0);

                if ((m_HIBYTE == 1))
                    keybd_event((byte)KeyCode.Shift, 0x2A, (uint)KeyCode.KeyUp, 0);
                else if ((m_HIBYTE == 2))
                    keybd_event((byte)KeyCode.Control, 0, (uint)KeyCode.KeyUp, 0);
            }

            System.Windows.Forms.Application.DoEvents();
        }
    }
}