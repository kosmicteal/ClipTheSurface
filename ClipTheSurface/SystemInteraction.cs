using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClipTheSurface
{
    class SystemInteraction
    {
        public void genericToogle_Click(VirtualKeyCode key, Button keyButton)
        {
            var simulator = new InputSimulator();
            var isKeyDown = simulator.InputDeviceState.IsKeyDown(key);

            if (isKeyDown)
            {
                simulator.Keyboard.KeyUp(key);
                keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#7F333333");
            }
            else
            {
                simulator.Keyboard.KeyDown(key);
                keyButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#BF536280");
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
            VirtualKeyCode[] keys = { VirtualKeyCode.SHIFT, VirtualKeyCode.LCONTROL, VirtualKeyCode.LMENU };
            foreach (VirtualKeyCode vky in keys)
            {
                simulator.Keyboard.KeyUp(vky);
            }
        }
    }
}
