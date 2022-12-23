using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ClipTheSurface
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool hidden = false;
        public bool ffmode = false;
        System.Windows.Threading.DispatcherTimer ffm_watch = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            SystemInteraction si = new SystemInteraction();
            si.SPBToogle();

            InitializeComponent();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            ReloadPosition();
        }

        void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            ReloadPosition();
        }

        void ReloadPosition()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = (desktopWorkingArea.Bottom - this.Height) / 2;
        }

        private void shiftToogle_Click(object sender, RoutedEventArgs e)
        {
            SystemInteraction si = new SystemInteraction();
            si.genericToogle_Click(VirtualKeyCode.LSHIFT, shiftToogle);
        }

        private void ctrlToogle_Click(object sender, RoutedEventArgs e)
        {
            SystemInteraction si = new SystemInteraction();
            si.genericToogle_Click(VirtualKeyCode.LCONTROL, ctrlToogle);
        }

        private void altToogle_Click(object sender, RoutedEventArgs e)
        {
            SystemInteraction si = new SystemInteraction();
            si.genericToogle_Click(VirtualKeyCode.LMENU, altToogle);
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            //failsafe to reset all modified key presses

            SystemInteraction si = new SystemInteraction();
            si.cleanKeyboard();
            this.Close();
        }

        private void hideApp_Click(object sender, RoutedEventArgs e)
        {
            //failsafe to reset all modified key presses
            if (!hidden)
            {
                UIButtons.Visibility = Visibility.Hidden;
                hideApp.Content = "";
                hidden = true;

            }
            else
            {
                UIButtons.Visibility = Visibility.Visible;
                hideApp.Content = "";
                hidden = false;
            }
        }

        private async void tabletToogle_Click(object sender, RoutedEventArgs e)
        {
            var TTOriginal = tabletToogle.Content;
            tabletToogle.Content = "\uF16A";

            //Change to Full Focus Mode
            SystemInteraction si = new SystemInteraction();
            ffmode = si.fullFocusMode(ffmode, tabletToogle, ffmWatchLabel);
            await Task.Delay(3000);

            //Enable Watch
            if (ffmode == true)
            {
                ffm_watch.Tick += new EventHandler(ffm_watch_event);
                ffm_watch.Interval = new TimeSpan(0, 0, 1);
                ffm_watch.Start();
            }
            else
            {
                ffmWatchLabel.Content = "";
                ffm_watch.Stop();
            }


            tabletToogle.Content = TTOriginal;
        }

        private void ffm_watch_event(object sender, EventArgs e)
        {
            DateTime d;
            d = DateTime.Now;
            ffmWatchLabel.Content = d.Hour + ":" + d.Minute;
        }

        private void upPrecision_Click(object sender, RoutedEventArgs e)
        {
            SystemInteraction si = new SystemInteraction();
            si.genericPrecision_Click(VirtualKeyCode.UP);
        }

        private void leftPrecision_Click(object sender, RoutedEventArgs e)
        {
            SystemInteraction si = new SystemInteraction();
            si.genericPrecision_Click(VirtualKeyCode.LEFT);

        }

        private void rightPrecision_Click(object sender, RoutedEventArgs e)
        {
            SystemInteraction si = new SystemInteraction();
            si.genericPrecision_Click(VirtualKeyCode.RIGHT);
        }

        private void downPrecision_Click(object sender, RoutedEventArgs e)
        {
            SystemInteraction si = new SystemInteraction();
            si.genericPrecision_Click(VirtualKeyCode.DOWN);
        }



        private void hideApp_StylusEnter(object sender, System.Windows.Input.StylusEventArgs e)
        {
            MainContainer.Opacity = 1;
            ffmWatchLabel.Opacity = 1;
        }

        private void hideApp_StylusLeave(object sender, System.Windows.Input.StylusEventArgs e)
        {
            MainContainer.Opacity = 0.3;
            ffmWatchLabel.Opacity = 0;
        }
    }
}
