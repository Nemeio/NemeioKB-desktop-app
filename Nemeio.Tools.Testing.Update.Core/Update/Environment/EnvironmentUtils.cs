using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Tools.Testing.Update.Core.Update.Environment
{
    public static class EnvironmentUtils
    {
        private const string Develop = "develop";
        private const string Testing = "testing";
        private const string Master = "master";

        public static NemeioEnvironment FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            switch (value.ToLower())
            {
                case Develop:
                    return NemeioEnvironment.Develop;
                case Testing:
                    return NemeioEnvironment.Testing;
                case Master:
                    return NemeioEnvironment.Master;
                default:
                    throw new NotSupportedEnvironment();
            }
        }
    }
}
