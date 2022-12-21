using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Windows;

namespace ClipTheSurface
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean hidden = false;

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
            this.Top = desktopWorkingArea.Bottom - this.Height;
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
            if (!hidden) {
            hideApp.HorizontalAlignment = HorizontalAlignment.Left;
                hideApp.Content = "";

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width + 140;
                hidden = true;

            }
            else
            {

                
                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                this.Left = desktopWorkingArea.Right - this.Width;

                hideApp.HorizontalAlignment = HorizontalAlignment.Right;
                hideApp.Content = "";

                hidden = false;
            }
        }


    }
}
