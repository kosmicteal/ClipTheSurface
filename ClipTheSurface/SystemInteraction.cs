using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace ClipTheSurface
{

    class SystemInteraction
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        public void genericToogle_Click(VirtualKeyCode key, Button keyButton)
        {
            var simulator = new InputSimulator();
            var isKeyDown = simulator.InputDeviceState.IsKeyDown(key);

            if (isKeyDown)
            {
                simulator.Keyboard.KeyUp(key);
                keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF333333");
            }
            else
            {
                simulator.Keyboard.KeyDown(key);
                keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF536280");
            }
        }

        public void genericPrecision_Click(VirtualKeyCode key)
        {
            Process p = Process.GetProcessesByName("CLIPStudioPaint").FirstOrDefault();
            var simulator = new InputSimulator();

            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
                simulator.Keyboard.KeyPress(key);
            }
        }

        public void genericKeystrokes_Click(IEnumerable<VirtualKeyCode> modKeys, VirtualKeyCode key)
        {
            //InputSimulator wrapper to avoid reiterative calls
            var simulator = new InputSimulator();
                simulator.Keyboard.ModifiedKeyStroke(modKeys, key);

        }

        public void SPBToogle()
        {
            //This function kills all processes with this name if it's already running.
            //To be used as a "Toogle" with Surface Pen's button.

            cleanKeyboard(); //failsafe in case of unexpected termination

            var current = Process.GetCurrentProcess();
            var processlist = Process.GetProcessesByName(current.ProcessName);
            if (processlist.Length > 1)
            {
                processlist.Where(t => t.Id != current.Id).ToList().ForEach(t => t.Kill());
                current.Kill();
            }
        }

        public void cleanKeyboard()
        {
            //This function cleans any modifier key status.
            var simulator = new InputSimulator();
            VirtualKeyCode[] keys = { VirtualKeyCode.LSHIFT, VirtualKeyCode.LCONTROL, VirtualKeyCode.LMENU };
            foreach (VirtualKeyCode vky in keys)
            {
                simulator.Keyboard.KeyUp(vky);
            }

            int currentTBValue = IsTaskBarAutoHidden();
            //Get initial value of system if it's not saved
            if (Properties.Settings.Default.InitialTBHide == 0)
            {

                Properties.Settings.Default.InitialTBHide = currentTBValue;

            }
            else if (Properties.Settings.Default.InitialTBHide == 2 && currentTBValue == 1)
            {
                //reset AutoHide value if saved one and current are different
                AutoHideTaskBar(false);
            }
        }

        private int IsTaskBarAutoHidden()
        {
            if (Math.Abs(SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height) > 0)
            {
                return 2; //false
            }
            else
            {
                return 1; //true
            }

        }

        public bool fullFocusMode(bool ffmode, Button keyButton, Label ffmWatchLabel)
        {
            bool changeModeTo = false;
            Process p = Process.GetProcessesByName("CLIPStudioPaint").FirstOrDefault();
            var simulator = new InputSimulator();

            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);

                if (!ffmode)
                {
                    simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LSHIFT, VirtualKeyCode.TAB);
                    simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LSHIFT, VirtualKeyCode.TAB);

                    if (Properties.Settings.Default.InitialTBHide == 2)
                    {
                        AutoHideTaskBar(true);
                    }

                    ffmWatchLabel.Visibility = Visibility.Visible;
                    keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF536280");
                    changeModeTo = true;
                }
                else
                {
                    simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LSHIFT, VirtualKeyCode.TAB);

                    if (Properties.Settings.Default.InitialTBHide == 2)
                    {
                        AutoHideTaskBar(false);
                    }

                    ffmWatchLabel.Visibility = Visibility.Hidden;
                    keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF333333");
                    changeModeTo = false;
                }
            }
            return changeModeTo;
        }

        public void AutoHideTaskBar(bool type)
        {
            int value;

            if (type)
            {
                value = 3;
            }
            else
            {
                value = 2;
            }

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = "-command \"&{$p='HKCU:SOFTWARE\\Microsoft\\Windows\\" +
                        "CurrentVersion\\Explorer\\StuckRects3';$v=(Get-ItemProperty -Path $p).Settings;$v[8]=" + value.ToString() +
                        ";&Set-ItemProperty -Path $p -Name Settings -Value $v;&Stop-Process -f -ProcessName explorer}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.Start();
            }
        }

    }
}
