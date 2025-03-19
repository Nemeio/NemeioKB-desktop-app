using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;
using ApplicationException = Nemeio.Tools.Core.ApplicationException<Nemeio.Cli.Application.ApplicationExitCode>;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetProvisionning
{
    internal sealed class SetProvisionningInput
    {
        public string Serial { get; private set; }
        public PhysicalAddress Mac { get; private set; }
        public SetProvisionningInput(string serial, string mac)
        {
            Serial = serial;
            try
            {
                Mac = PhysicalAddress.Parse(mac.ToUpper().Replace(":", "").Replace("-", ""));
            }
            catch (FormatException)
            {
                throw new SetProvisionningFailedException("Invalid Mac value");
            }
            
        }
    }
}