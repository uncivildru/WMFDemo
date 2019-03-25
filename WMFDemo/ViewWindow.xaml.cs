using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WMFDemo
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        internal const int
            LbnSelchange = 0x00000001,
            WmCommand = 0x00000111,
            LbGetcursel = 0x00000188,
            LbGettextlen = 0x0000018A,
            LbAddstring = 0x00000180,
            LbGettext = 0x00000189,
            LbDeletestring = 0x00000182,
            LbGetcount = 0x0000018B;

        private Application _app;
        private int _itemCount;
        private WMVideoControl _videoControl;
        private Window _myWindow;

        public IntPtr Host { get { return _videoControl.Host; } }
        public IntPtr Player { get { return _videoControl.Player; } }

        public ViewWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _app = Application.Current;
            _myWindow = _app.MainWindow;
            _myWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _videoControl = new WMVideoControl(HostElement.ActualHeight, HostElement.ActualWidth);
            HostElement.Child = _videoControl;
            _videoControl.MessageHook += _videoControl_MessageHook;
        }

        private IntPtr _videoControl_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;

            return IntPtr.Zero;
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hwnd,
            int msg,
            int wParam,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hwnd,
            int msg,
            IntPtr wParam,
            string lParam);
    }
}
