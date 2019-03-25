using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WMFDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _capturing;
        private bool _playback;
        private ViewWindow _window;
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();
            _playback = false;
            _capturing = false;
        }

        private void StartCapture_Click(object sender, RoutedEventArgs e)
        {
            if (_window.Player == IntPtr.Zero)
            {
                Console.WriteLine("No valid player");
                return;
            }
            
            StartCapture(_window.Host, playFilename.Text, 0);

        }

        private void StartPlayback_Click(object sender, RoutedEventArgs e)
        {

            if (_window.Player == IntPtr.Zero)
            {
                Console.WriteLine("No valid player");
                return;
            }

            try
            {
                if (!File.Exists(playFilename.Text))
                {
                    Console.WriteLine("Couldn't find a valid file!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load file {0}", ex.Message);
                return;
            }
            IntPtr host = _window.Host;
            IntPtr player = _window.Player;
            BeginPlayback(playFilename.Text, host, player);
            if (_timer != null) { _timer.Start(); }
        }

        private void SetOffset_Click(object sender, RoutedEventArgs e)
        {
            if (_window.Player == IntPtr.Zero)
            {
                Console.WriteLine("No valid player");
                return;
            }
            long position;
            if (!long.TryParse(TimeOffset.Text, out position))
            {
                return;
            }
            SetVideoPosition(position);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _window = new ViewWindow();
            _window.Show();
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(50000);
            
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            long offset = GetVideoPosition();
            Action update = (() =>
            {
                videoOffset.Content = offset.ToString();
            });
            Application.Current.Dispatcher.Invoke(update);
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_window != null)
            {
                _window.Close();
            }
            EndCapture();
            EndPlayback();
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
            }
        }

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void BeginPlayback(string filanem, IntPtr hwnd, IntPtr hwndVideo);

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RepaintVideo();

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Pause();

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Play();


        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVideoPosition(long position);

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern long GetVideoPosition();

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void EndPlayback();

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool StartCapture(IntPtr hwnd, string filename, uint deviceIndex);

        [DllImport(@"WMF.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool EndCapture();

        private void StopPlaybackBtn_Click(object sender, RoutedEventArgs e)
        {
            EndPlayback();
        }

        private void StopCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
           EndCapture();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SetOffset_Click(this, null);
        }
    }
}
