using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using System;
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

        public bool OriginalTBHidden;
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
            OriginalTBHidden = IsTaskBarAutoHidden();
        }

        private bool IsTaskBarAutoHidden()
        {
            return Math.Abs(SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height) > 0;
        }

        public Boolean fullFocusMode(Boolean ffmode, Button keyButton)
        {
            Boolean changeModeTo = false;
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

                    if (!OriginalTBHidden) { 
                    ProcessCallVoid("powershell.exe", "-command \"&{$p='HKCU:SOFTWARE\\Microsoft\\Windows\\" +
                        "CurrentVersion\\Explorer\\StuckRects3';$v=(Get-ItemProperty -Path $p).Settings;$v[8]=3;" +
                        "&Set-ItemProperty -Path $p -Name Settings -Value $v;&Stop-Process -f -ProcessName explorer}\"");
                    }


                    keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF536280");
                    changeModeTo = true;
                }
                else
                {
                    simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LSHIFT, VirtualKeyCode.TAB);

                    if (!OriginalTBHidden) {
                        ProcessCallVoid("powershell.exe", "-command \"&{$p='HKCU:SOFTWARE\\Microsoft\\Windows\\" +
                        "CurrentVersion\\Explorer\\StuckRects3';$v=(Get-ItemProperty -Path $p).Settings;$v[8]=2;" +
                        "&Set-ItemProperty -Path $p -Name Settings -Value $v;&Stop-Process -f -ProcessName explorer}\"");
                    }

                    keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF333333");
                    changeModeTo = false;
                }
            }
            return changeModeTo;
        }

        public void ProcessCallVoid(string FileName, string Arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = FileName;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.Start();
            }
        }

    }
}
