using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordStore.Helpers
{
    public class ObjectFocus : NativeWindow, IDisposable
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        internal static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

        private static ObjectFocus _instance;
        private IntPtr _focused;

        private ObjectFocus()
        {
            this.CreateHandle(new CreateParams());
        }

        public static ObjectFocus Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ObjectFocus();
                return _instance;
            }
        }

        public void Dispose()
        {
            this.DestroyHandle();
        }

        public void Get()
        {
            //Focused = GetFocus();
            //if (Focused == IntPtr.Zero)
            //    Focused = GetForegroundWindow();

            //var activeWindowHandle = GetForegroundWindow();

            //IntPtr activeWindowThread = GetWindowThreadProcessId(activeWindowHandle, IntPtr.Zero);
            //IntPtr thisWindowThread = GetWindowThreadProcessId((IntPtr)Handle, IntPtr.Zero);

            //AttachThreadInput(activeWindowThread, thisWindowThread, true);
            //IntPtr focusedControlHandle = GetFocus();
            //AttachThreadInput(activeWindowThread, thisWindowThread, false);

            //this._focused = focusedControlHandle;

            this._focused = GetForegroundWindow();
        }

        public void Set()
        {
            //var old = this._focused;

            //this.Get();
            //var get1 = this._focused;

            //var activeWindowHandle = GetForegroundWindow();

            //IntPtr activeWindowThread = GetWindowThreadProcessId(activeWindowHandle, IntPtr.Zero);
            //IntPtr thisWindowThread = GetWindowThreadProcessId((IntPtr)Handle, IntPtr.Zero);

            //AttachThreadInput(activeWindowThread, thisWindowThread, true);
            //var focused = SetFocus(this._focused);
            //AttachThreadInput(activeWindowThread, thisWindowThread, false);

            //this.Get();
            //var get2 = this._focused;

            //var a = 1;

            SetForegroundWindow(this._focused);
        }

        public IntPtr GetWindow()
        {
            return GetForegroundWindow();
        }
    }
}