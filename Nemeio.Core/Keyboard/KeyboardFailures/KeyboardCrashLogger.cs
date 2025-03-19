using System;
using System.Collections.Generic;
using System.IO;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Keyboard.KeyboardFailures
{
    public abstract class KeyboardCrashLogger : IKeyboardCrashLogger
    {
        private static int _assertFailLabelSizeAlignment = 10;
        private static int _faultExceptionLabelSizeAlignment = 13;

        public string Path { get; private set; }

        public KeyboardCrashLogger(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }

        public void WriteKeyboardCrashLog(FirmwareVersions keyboardVersion, IList<KeyboardFailure> keyboardFailures)
        {
            //  Sanity: prevent empty log creation
            if (keyboardFailures.Count == 0)
            {
                return;
            }

            //  Formatting string to better align values in log file
            var fileName = $"{NemeioConstants.KeyboardCrashFileName}{DateTime.Now.ToString("yyyyMMdd-hhmmss")}{NemeioConstants.LogExtension}";
            var filePath = System.IO.Path.Combine(Path, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //  FIXME: Keyboard must send this information to be sure it's true
                writer.WriteLine($"Versions:");
                writer.WriteLine($"Cpu: {keyboardVersion.Stm32}");
                writer.WriteLine($"Nrf: {keyboardVersion.Nrf}");
                writer.WriteLine($"Ite: {keyboardVersion.Ite}");
                writer.WriteLine($"Waveform: {keyboardVersion.Waveform}");

                foreach (KeyboardFailure keyboardFailure in keyboardFailures)
                {
                    int labelSize = _assertFailLabelSizeAlignment;
                    if (keyboardFailure.Id == KeyboardEventId.FaultExceptionEvent)
                    {
                        labelSize = _faultExceptionLabelSizeAlignment;
                    }

                    if (keyboardFailure.Id == KeyboardEventId.TestEvent)
                    {
                        var testHeader = "TEST";
                        writer.Write(FormatLabel(testHeader, testHeader.Length));
                        bool isHeaderRaw = true;
                        keyboardFailure.Description.ForEach(x =>
                        {
                            writer.WriteLine((isHeaderRaw ? string.Empty : AlignBodyRaw(new string(' ', testHeader.Length), testHeader.Length)) + x.ToString().Trim());
                            isHeaderRaw = false;    
                        });
                        continue;
                    }

                    writer.WriteLine(FormatLabel("EventId", labelSize) + keyboardFailure.Id.ToString());


                    for (int i = 0; i < KeyboardFailure.NumberOfRegistries; i++)
                    {
                        writer.WriteLine(FormatLabel(string.Format("R{0}", i), labelSize) + FormatUInt32(keyboardFailure.Registries[i]));
                    }
                    writer.WriteLine(FormatLabel("SP", labelSize) + FormatUInt32(keyboardFailure.SP));
                    writer.WriteLine(FormatLabel("LR", labelSize) + FormatUInt32(keyboardFailure.LR));
                    writer.WriteLine(FormatLabel("PC", labelSize) + FormatUInt32(keyboardFailure.PC));
                    writer.WriteLine(FormatLabel("xPSR", labelSize) + FormatUInt32(keyboardFailure.xPSR));

                    if (keyboardFailure.Id == KeyboardEventId.FaultExceptionEvent)
                    {
                        writer.WriteLine(FormatLabel("exceptType", labelSize) + keyboardFailure.ExceptionType.ToString());
                    }


                    writer.WriteLine("===");
                }
            }
        }

        private string FormatLabel(string label, int size) => string.Format(@"{0} = ", label).PadLeft(size);
        private string AlignBodyRaw(string label, int size) => string.Format(@"{0}   ", label).PadLeft(size);

        private string FormatUInt32(UInt32 value) => string.Format("0x{0:X8}", value);
    }
}
