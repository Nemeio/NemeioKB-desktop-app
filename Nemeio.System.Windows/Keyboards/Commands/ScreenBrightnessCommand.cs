using System.Management;
using KeyboardCommand = Nemeio.Core.Models.SystemKeyboardCommand.SystemKeyboardCommand;

namespace Nemeio.Platform.Windows.Keyboards.Commands
{
    public abstract class ScreenBrightnessCommand : KeyboardCommand
    {
        public int GetCurrentBrightness()
        {
            using (var mclass = new ManagementClass("WmiMonitorBrightness") { Scope = new ManagementScope(@"\\.\root\wmi") })
            using (var instances = mclass.GetInstances())
            {
                foreach (ManagementObject instance in instances)
                {
                    return (byte)instance.GetPropertyValue("CurrentBrightness");
                }
            }
                
            return 0;
        }

        public void SetCurrentBrightness(int brightness)
        {
            var args = new object[] { 1, brightness };

            using (var mclass = new ManagementClass("WmiMonitorBrightnessMethods") { Scope = new ManagementScope(@"\\.\root\wmi") }) 
            using (var instances = mclass.GetInstances())
            {
                foreach (ManagementObject instance in instances)
                {
                    instance.InvokeMethod("WmiSetBrightness", args);
                }
            }
        }
    }
}
