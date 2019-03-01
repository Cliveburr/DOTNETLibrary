using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PasswordStore.HotKeys
{
    public class KeyControl : NativeWindow, IDisposable
    {
        private const int MOD_ALT = 0x1;
        private const int MOD_CONTROL = 0x2;
        private const int MOD_SHIFT = 0x4;
        private const int MOD_WIN = 0x8;
        private const int WM_HOTKEY = 0x0312;

        private bool _preventRecursive;

        public event Action<int> OnHotKey;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public KeyControl()
        {
            CreateHandle(new CreateParams());
        }

        public void Dispose()
        {
            DestroyHandle();
        }

        public bool RegisterHotKey(Keys key, int ID)
        {
            var mod = 0;

            if ((key & Keys.Alt) > 0) mod |= MOD_ALT;
            if ((key & Keys.Control) > 0) mod |= MOD_CONTROL;
            if ((key & Keys.Shift) > 0) mod |= MOD_SHIFT;

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;

            return RegisterHotKey(Handle, ID, (uint)mod, (uint)k);
        }

        public void UnregisterHotKey(int ID)
        {
            try
            {
                UnregisterHotKey(Handle, ID);
            }
            catch
            {
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if (_preventRecursive)
                {
                    return;
                }

                _preventRecursive = true;

                OnHotKey?.Invoke((int)m.WParam);

                _preventRecursive = false;
            }
        }
    }
}