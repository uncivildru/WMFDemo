using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace WMFDemo
{
    public class WMVideoControl : HwndHost
    {
        internal const int
            WsChild = 0x40000000,
            WsVisible = 0x10000000,
            LbsNotify = 0x00000001,
            HostId = 0x00000002,
            ListboxId = 0x00000001,
            WsVscroll = 0x00200000,
            WsBorder = 0x00800000,
            SSBitmap = 0x0000000E;

        private readonly int _hostHeight;
        private readonly int _hostWidth;
        private IntPtr _hwndHost;
        private IntPtr _player;
        public IntPtr Host { get { return _hwndHost; } }
        public IntPtr Player { get { return _player; } }
        public WMVideoControl(double height, double width)
        {
            _hostHeight= (int) height;
            _hostWidth = (int)width;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _hwndHost = _player =IntPtr.Zero;

            _hwndHost = CreateWindowEx(0, "static", "",
                WsChild | WsVisible,
                0, 0,
                _hostHeight, _hostWidth,
                hwndParent.Handle,
                (IntPtr)HostId,
                IntPtr.Zero,
                0);

            _player = CreateWindowEx(0, "static", "",
                WsChild | WsVisible| SSBitmap,
                0, 0,
                _hostHeight, _hostWidth,
                _hwndHost,
                (IntPtr)HostId,
                IntPtr.Zero,
                0);


            return new HandleRef(this, _hwndHost);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }

        //PInvoke declarations
        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
            string lpszClassName,
            string lpszWindowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInst,
            [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);
    }
}
